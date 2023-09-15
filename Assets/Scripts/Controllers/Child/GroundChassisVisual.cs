using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Child;
using UnityEngine;

namespace Controllers.Child
{
    /// <summary>
    /// 同步车辆底盘视觉效果。
    /// </summary>
    public class GroundChassisVisual : StoreChildBase
    {
        public List<Transform> wheels = new List<Transform>();
        public Vector3 rotateAround = Vector3.left;
        private Vector3 _speed;
        private bool _isSpinning;
        public Light groundLight;

        /// <summary>
        /// 声明关注事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.GroundChassisVisual.UpdateStatus
            }).ToList();
        }

        protected override void StartWithRoot()
        {
            base.StartWithRoot();
            groundLight.color = Root.id.camp == Identity.Camps.Red ? Color.red : Color.blue;
        }

        /// <summary>
        /// 标识身份。
        /// </summary>
        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.GroundChassisVisual);
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                // 根据事件设置车轮旋转视觉效果
                case ChildActionID.GroundChassisVisual.UpdateStatus:
                    var updateStatusAction = (UpdateStatus) action;
                    _speed = updateStatusAction.Speed;
                    _isSpinning = updateStatusAction.Spinning;
                    break;
            }
        }

        /// <summary>
        /// 根据运动状态旋转车轮。
        /// </summary>
        private void Update()
        {
            if (_isSpinning)
            {
                RotateWheels(50);
            }
            else
            {
                RotateWheels(_speed.magnitude * 50);
            }
        }

        /// <summary>
        /// 旋转车轮。
        /// </summary>
        /// <param name="speed"></param>
        private void RotateWheels(float speed)
        {
            foreach (var wheel in wheels)
            {
                wheel.Rotate(rotateAround, speed);
            }
        }
    }
}