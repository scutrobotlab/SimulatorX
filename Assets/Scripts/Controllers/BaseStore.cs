using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Events;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Child;
using Mirror;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// 基地控制器。
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class BaseStore : StoreBase
    {
        [SyncVar] public bool invincible = true;
        [SyncVar] public float health;
        [SyncVar] public float shield;
        [SyncVar] public float initialHealth;
        private Animator _baseController;
        //哨兵是否离开巡逻区
        private bool _isLeave;
        //护盾掉落倒计时
        private float time;
        private static readonly int Unfold = Animator.StringToHash("Unfold");
        //是否展开护甲
        public bool isUnFold = false;

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 声明感兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.OutpostFall,
                ActionID.Stage.SentinelFall,
                ActionID.Stage.AtPatrol
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Stage.OutpostFall:
                    // 前哨站被击毁，基地无敌状态解除。
                    if (EntityManager.Instance().CurrentMap() == MapType.RMUC2022)
                    {
                        var outpostFallAction = (OutpostFall) action;
                        if (outpostFallAction.Camp == id.camp)
                        {
                            invincible = false;
                            time = 30;
                            RobotStoreBase robot = null;
                            foreach (var r in EntityManager.Instance().RobotRef().Where(robotTemp =>
                                         robotTemp.id.role == Identity.Roles.AutoSentinel && robotTemp.id.camp==outpostFallAction.Camp))
                            {
                                robot = (RobotStoreBase)r;
                            }
                            if (robot == null || robot.health==0)
                            {
                                shield = 0;
                                _baseController.SetTrigger(Unfold);
                                if (isServerOnly) BaseUnfoldRpc();
                            }
                                
                        }
                    }
                    break;
                
                case ActionID.Stage.SentinelFall:
                    // 哨兵死亡则基地护盾展开，虚拟护盾失效。
                    var mapType = EntityManager.Instance().CurrentMap();
                    if (mapType == MapType.RMUC2022 || mapType == MapType.RMUL2022)
                    {
                        var sentinelFallAction = (SentinelFall) action;
                        if (sentinelFallAction.Camp == id.camp)
                        {
                            shield = 0;
                            _baseController.SetTrigger(Unfold);
                            if (isServerOnly) BaseUnfoldRpc();
                        }
                    }

                    break;
                
                                
                case ActionID.Stage.AtPatrol:
                    var checkAction = (AtPatrol)action;
                    if (checkAction.Id.camp == id.camp)
                    {
                        _isLeave = !checkAction.IsIn;
                        if (_isLeave)
                            time = 30;
                    }

                    break;
            }
        }

        [ClientRpc]
        private void BaseUnfoldRpc()
        {
            _baseController.SetTrigger(Unfold);
            isUnFold = true;
        }

        /// <summary>
        /// 初始化状态，记录初始血量等。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _baseController = GetComponent<Animator>();
            if (!isServer) return;
            initialHealth = health + shield;
            Dispatcher.Instance().SendChild(new SetLightState
            {
                Receiver = new ChildIdentity(ChildType.Light),
                State = LightState.Percentage
            }, id);
        }

        /// <summary>
        /// 更新装甲板和灯条状态。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;

            if (health == 0)
            {
                for (var i = 0; i < 7; i++)
                {
                    Dispatcher.Instance().SendChild(new TurnArmor
                    {
                        ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                        IsOn = false
                    }, id);
                }
            }

            Dispatcher.Instance().SendChild(new SetPercentage
            {
                Receiver = new ChildIdentity(ChildType.Light),
                Percentage = (health+shield) / initialHealth
            }, id);
            
            //哨兵巡逻判断
            if ( _isLeave && !invincible && !isUnFold)
            {
                time=time - Time.fixedDeltaTime > 0 ? time - Time.fixedDeltaTime : 0;
                if (time != 0) return;
                if (shield >= 25 * Time.fixedDeltaTime)
                    shield -= 25 * Time.fixedDeltaTime;
                else
                {
                    shield = 0;
                    _baseController.SetTrigger(Unfold);
                    if (isServerOnly)
                        BaseUnfoldRpc();
                    isUnFold = true;
                }
            }
        }
    }
}