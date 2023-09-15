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
    /// 前哨站控制器。
    /// </summary>
    public class OutpostStore : StoreBase
    {
        [SyncVar] public float health;
        [SyncVar] public float initialHealth;
        public Identity.Camps camp;
        public float speed=1.0f;

        public GameObject rotator;
        public bool invincible = true;
        private bool _activated;
        private bool _clockwise;

        /// <summary>
        /// 编辑器中设置
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 声明兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Clock.ActivateOutpost,
                ActionID.Stage.OccupyControlArea,
                ActionID.Stage.LeaveControlArea
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
                case ActionID.Clock.ActivateOutpost:
                    var activateAction = (ActivateOutpost) action;
                    if (health > 0)
                    {
                        _activated = activateAction.Activate;
                        if (_activated)
                        {
                            // 随机旋转方向
                            _clockwise = Random.Range(0, 2) == 1;
                            invincible = false;
                        }
                    }

                    break;
                
                case ActionID.Stage.OccupyControlArea:
                    var occupyAction = (OccupyControlArea)action;
                    if (health > 0 && occupyAction.Camp != camp)
                    {
                        speed = 0.5f;
                    }
                    else 
                        speed = 1.0f;
                    break;
                
                case ActionID.Stage.LeaveControlArea:
                    var leaveAction = (LeaveControlArea)action;
                    if (health > 0 && leaveAction.Camp != camp)
                        speed = 1.0f;
                    break;
            }
        }
        
        /// <summary>
        /// 初始化血量和灯条状态。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!isServer) return;
            initialHealth = health;
            Dispatcher.Instance().SendChild(new SetLightState
            {
                Receiver = new ChildIdentity(ChildType.Light),
                State = LightState.Percentage
            }, id);
        }

        /// <summary>
        /// 旋转装甲板，更新灯光。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (health == 0)
            {
                _activated = false;
                for (var i = 0; i < 5; i++)
                {
                    Dispatcher.Instance().SendChild(new TurnArmor
                    {
                        ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                        IsOn = false
                    }, id);
                }
            }

            if (_activated)
            {
                rotator.transform.Rotate(
                    rotator.transform.up,
                    360 / Mathf.PI * Time.fixedDeltaTime * (_clockwise ? 1 : -1) * speed);
            }

            Dispatcher.Instance().SendChild(new SetPercentage
            {
                Receiver = new ChildIdentity(ChildType.Light),
                Percentage = health / initialHealth
            }, id);
        }

        public bool ReturnAct()
        {
            return _activated;
        }
        
    }
}