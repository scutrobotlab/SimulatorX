using System.Collections.Generic;
using Controllers.Components;
using Gameplay.Events;
using Gameplay;
using Infrastructure;
using Mirror;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// 实现无人机飞行控制事件转发和飞手视角控制。
    /// </summary>
    public class DroneManipulatorStore : RobotStoreBase
    {
        // 云台组件
        public Transform yaw;
        public Transform pitch;
        private Ptz _ptz;

        private Vector2 _primaryAxisInput;
        private Vector2 _secondaryAxisInput;

        /// <summary>
        /// 声明接收事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return new List<string>
            {
                ActionID.Input.PrimaryAxis,
                ActionID.Input.ViewControl,
                ActionID.Input.StateControl,
                ActionID.Input.SecondaryAxis
            };
        }

        /// <summary>
        /// 组件初始化。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (isServer)
            {
                _ptz = new Ptz(yaw, pitch, GetComponent<Rigidbody>());
            }
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action">事件</param>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Input.PrimaryAxis:
                    var primaryAxisAction = (PrimaryAxis) action;
                    _primaryAxisInput = new Vector2(primaryAxisAction.X, primaryAxisAction.Y);
                    /*Dispatcher.Instance().Send(new UpdateDrone
                    {
                        Camp = id.camp,
                        primaryAxisInput = _primaryAxisInput
                    });*/
                    break;

                case ActionID.Input.SecondaryAxis:
                    var secondaryAxisAction = (SecondaryAxis) action;
                     _secondaryAxisInput = new Vector2(secondaryAxisAction.X, secondaryAxisAction.Y);
                    /*Dispatcher.Instance().Send(new UpdateDrone
                    {
                        Camp = id.camp,
                        secondAxisInput = _secondaryAxisInput
                    });*/
                    break;
                
            }
        }

        /// <summary>
        /// 更新组件。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;
            _ptz.Update();
        }
    }
}
