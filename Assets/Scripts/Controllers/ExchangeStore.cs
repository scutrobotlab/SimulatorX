using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdditionalAssets.RobotArm.Scripts;
using Controllers.RobotSensor;
using Controllers.Ore;
using Gameplay;
using Gameplay.Effects;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    /// <summary>
    /// <c>ExchangeStore</c> 控制兑换站。
    /// 进行矿石检测并向 <c>CoinStore</c> 发送增加金币的请求。
    /// </summary>
    public class ExchangeStore : StoreBase
    {
        public Sensor exchangeSensor;
        public ArmController currentState;
        [SerializeField] private OreStoreBase currentOre;
        [SerializeField] private float countTime, requiredTime;
        [SerializeField] private bool startCount = false;
        public int accumulatedCoin = 0;

        // 金银矿石可兑换金币数
        private readonly int[] _sliverOreCoins = { 75, 100, 150, 225, 375 };
        private readonly int[] _goldOreCoins = { 100, 125, 175, 250, 400 };

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
            // id = new Identity(Identity.Camps.Red, Identity.Roles.Exchange);
        }

        public override List<string> InputActions()
        {
            //在父类中原方法的作用是返回一个空字符串，而在robot中需要覆盖这个方法来返回“兴趣字符串”
            //这个函数将会把robot感兴趣的字符串存到Dispatcher中去
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Exchange.ChangeGrade
            }).ToList();
        }

        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Exchange.ChangeGrade:
                    var changeGrade = (ChangeGrade)action;
                    if (changeGrade.Id == id)
                    {
                        currentState.currentState = changeGrade.Gr;
                    }

                    break;
            }
        }

        /// <summary>
        /// 初始化传感器。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            currentState = GetComponent<ArmController>();
            exchangeSensor.AddCustomTriggerFilter(store => store.id.IsOre());
        }

        /// <summary>
        /// 传感器检测是否超时，发放相应effect状态。
        /// </summary>
        protected override void FixedUpdate()
        {
            if (!isServer) return;
            base.FixedUpdate();

            if (exchangeSensor.targets.Count > 0)
            {
                //不绕一下会出错,如有两个金矿应该加200,但加了400
                if (currentOre == null || startCount == false)
                {
                    currentOre = (OreStoreBase)exchangeSensor.targets.First();
                    Dispatcher.Instance().Send(new AddEffect
                    {
                        Receiver = currentOre.id,
                        Effect = new TimeOut()
                    });

                    countTime = 0.0f;
                    startCount = true;

                    requiredTime = currentState.currentState switch
                    {
                        Grade.Zero => 2.0f,
                        Grade.One => 3.0f,
                        Grade.Two => 4.0f,
                        Grade.Three => 5.0f,
                        Grade.Four => 6.0f,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
                else if (startCount && countTime < requiredTime && currentOre != null)
                {
                    countTime += Time.deltaTime;
                }
                else if (startCount && currentOre.HasEffect(EffectID.Status.TimeOut) && currentOre != null)
                {
                    var grade = (int)currentState.currentState;
                    //兑换限制
                    if (accumulatedCoin > 1625 && grade <= 3)
                        currentState.currentState = Grade.Four;
                    else if (accumulatedCoin > 1100 && grade <= 2)
                        currentState.currentState = Grade.Three;
                    else if (accumulatedCoin > 750 && grade <= 1)
                        currentState.currentState = Grade.Two;
                    else if (accumulatedCoin > 575 && grade == 0)
                        currentState.currentState = Grade.One;

                    SendExchange(id.camp, currentState.currentState, currentOre.id.role);

                    // exchangeSensor.targets.RemoveAt(0);
                    exchangeSensor.TargetExit(currentOre);
                    Destroy(currentOre.gameObject);

                    currentOre = null;
                    startCount = false;
                }
            }
        }

        /// <summary>
        /// 兑换站检测矿石类别，向 <c>CoinStore</c> 发送金币补充请求。
        /// </summary>
        /// <param name="other">矿石</param>
        /// <exception cref="Exception">矿石的 roles 标识有误。</exception>
        public void OnTriggerEnter(Collider other)
        {
            if (!isServer) return;
            var incomingOre = other.gameObject.GetComponent<OreStoreBase>();
            if (!incomingOre) return;
            if (!incomingOre.HasEffect(EffectID.Status.TimeOut))
            {
                SendExchange(id.camp, currentState.currentState, incomingOre.id.role);
            }

            Destroy(incomingOre.gameObject);
        }

        /// <summary>
        /// 发送兑换事件
        /// </summary>
        /// <param name="camp">阵营</param>
        /// <param name="grade">兑换等级</param>
        /// <param name="ore">矿石种类</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void SendExchange(Identity.Camps camp, Grade grade, Identity.Roles ore)
        {
            float times = accumulatedCoin > 1625 ? 2f :
                accumulatedCoin > 1100 ? 1.3f : 1f;
            int coins = (int)(times * ore switch
            {
                Identity.Roles.SilverOre => _sliverOreCoins[(int)grade], // 银矿
                Identity.Roles.GoldOre => _goldOreCoins[(int)grade], // 金矿
                _ => throw new ArgumentOutOfRangeException()
            });
            accumulatedCoin += coins;
            Dispatcher.Instance().Send(new DoExchange
            {
                Camp = camp,
                OreSinglePrice = (int) (times * ore switch
                {
                    Identity.Roles.SilverOre => _sliverOreCoins[(int)grade], // 银矿
                    Identity.Roles.GoldOre => _goldOreCoins[(int)grade], // 金矿
                    _ => throw new ArgumentOutOfRangeException()
                })
            });
        }
    }
}