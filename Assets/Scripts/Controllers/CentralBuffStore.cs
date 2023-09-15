using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.RobotSensor;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Effects;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// RMUL中心增益区。
    /// </summary>
    public class CentralBuffStore : StoreBase
    {
        public readonly SyncDictionary<Identity.Camps, float> Energy = new SyncDictionary<Identity.Camps, float>();
        public readonly SyncDictionary<Identity.Camps, int> MedicalKit = new SyncDictionary<Identity.Camps, int>();

        private bool _activated;
        [SyncVar] private float _unavailableTill;

        private static CentralBuffStore _instance;

        // 网络单例
        public static CentralBuffStore Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CentralBuffStore>();
            }

            if (_instance == null)
            {
                throw new Exception("Getting CentralBuffStore before initialization.");
            }

            return _instance;
        }

        /// <summary>
        /// 初始化能量条和血包数值。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!isServer) return;
            Energy[Identity.Camps.Red] = 0;
            Energy[Identity.Camps.Blue] = 0;
            MedicalKit[Identity.Camps.Red] = 3;
            MedicalKit[Identity.Camps.Blue] = 3;
        }

        /// <summary>
        /// 声明输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.CentralBuffActivated,
                ActionID.Armor.ArmorHit,
                ActionID.Supply.AskMedical
            }).ToList();
        }

        /// <summary>
        /// 处理输入事件。
        /// </summary>
        /// <param name="action">事件。</param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Stage.CentralBuffActivated:
                    _activated = true;
                    _unavailableTill = (float) NetworkTime.time + 60;
                    break;

                case ActionID.Armor.ArmorHit:
                    var armorHitAction = (ArmorHit) action;
                    if (!_activated) return;
                    var victim = EntityManager.Instance().Ref(armorHitAction.Receiver);
                    if (victim != null)
                    {
                        if (victim.HasEffect(EffectID.Status.CentralBuff))
                        {
                            var receiverCamp = victim.id.camp;
                            Energy[receiverCamp] -= armorHitAction.Caliber switch
                            {
                                MechanicType.CaliberType.Small => 2,
                                MechanicType.CaliberType.Large => 20,
                                _ => 0
                            };
                            if (Energy[receiverCamp] < 0) Energy[receiverCamp] = 0;
                        }
                    }

                    break;

                case ActionID.Supply.AskMedical:
                    var askMedicalAction = (AskMedical) action;
                    var healCamp = askMedicalAction.Target.camp;
                    if (MedicalKit[healCamp] > 0)
                    {
                        Dispatcher.Instance().Send(new AddEffect
                        {
                            Receiver = askMedicalAction.Target,
                            Effect = new MedicalBuff()
                        });
                        MedicalKit[healCamp]--;
                    }

                    break;
            }
        }

        /// <summary>
        /// 机器人占领、激活增益点。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;
            if (_activated && NetworkTime.time >= _unavailableTill)
            {
                var gained = new Dictionary<Identity.Camps, bool>
                {
                    {Identity.Camps.Blue, false},
                    {Identity.Camps.Red, false}
                };
                foreach (var robot in EntityManager.Instance().RobotRef())
                {
                    var robotStore = (RobotStoreBase) robot;
                    if (!robotStore.HasEffect(EffectID.Status.CentralBuff)) continue;
                    if (!(robotStore.health > 0)) continue;

                    var camp = robotStore.id.camp;
                    if (gained[camp]) continue;
                    gained[camp] = true;
                    Energy[camp] += 10 * Time.fixedDeltaTime;
                    if (!(Energy[camp] >= 100)) continue;

                    // 激活
                    MedicalKit[camp] += 2;
                    Energy[Identity.Camps.Red] = 0;
                    Energy[Identity.Camps.Blue] = 0;
                    _unavailableTill = (float) NetworkTime.time + 90;
                }
            }
        }

        /// <summary>
        /// 获取当前增益点激活倒计时。
        /// </summary>
        /// <returns></returns>
        public int Countdown()
        {
            var countdown = (int) (_unavailableTill - (float) NetworkTime.time);
            return countdown <= 0 ? 0 : countdown;
        }

        private void OnTriggerStay(Collider other)
        {
            var robotStore = other.GetComponent<RobotStoreBase>();
            if (robotStore == null) robotStore = other.GetComponentInParent<RobotStoreBase>();
            if (robotStore != null)
            {
                Dispatcher.Instance().Send(new AddEffect
                {
                    Receiver = robotStore.id,
                    Effect = new CentralBuff()
                });
            }
        }
    }
}