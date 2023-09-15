using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers.Components
{
    /// <summary>
    /// <c>Ptz</c> 实现了两轴云台组件。
    /// <br/>其功能包括控制云台朝向、配合底盘反转等。
    /// </summary>
    public class Ptz
    {
        // 云台组件
        private readonly Transform _yaw;
        private readonly Transform _pitch;
        private readonly Rigidbody _chassis;

        // Pitch 轴限位
        private readonly float _pitchMin;
        private readonly float _pitchMax;

        // 旋转角度
        private float _yawEuler;
        private float _pitchEuler;

        // 初始状态
        private readonly Vector3 _yawInitEuler;
        private readonly Vector3 _pitchInitEuler;

        // PID 控制量
        private float _yawNeed;
        private float _yawIntegral;
        private float _yawLastError;
        private float _pitchNeed;
        private float _pitchIntegral;
        private float _pitchLastError;

        private float _additionalYawRotate;

        private const float Kp = 0.4f;
        private const float Ki = 0.0f;
        private const float Kd = 0.01f;

        /// <summary>
        /// 初始化云台组件。
        /// </summary>
        /// <param name="yaw">云台偏航部分</param>
        /// <param name="pitch">云台俯仰部分（yaw的子物体）</param>
        /// <param name="chassis">机器人底盘</param>
        /// <param name="pitchMin">俯仰角下限</param>
        /// <param name="pitchMax">俯仰角上限</param>
        /// <exception cref="Exception">初始化问题</exception>
        public Ptz(
            Transform yaw,
            Transform pitch,
            Rigidbody chassis = null,
            float pitchMin = -10,
            float pitchMax = 40)
        {
            _yaw = yaw;
            if (pitch.parent != _yaw)
            {
                throw new Exception("Pitch is not a child of Yaw.");
            }

            _pitch = pitch;
            _chassis = chassis;
            _pitchMin = pitchMin;
            _pitchMax = pitchMax;
            _yawInitEuler = yaw.localRotation.eulerAngles;
            _pitchInitEuler = pitch.localRotation.eulerAngles;
        }

        /// <summary>
        /// 接收视角控制输入，驱动云台。
        /// </summary>
        /// <param name="delta">帧时间</param>
        /// <param name="axis">控制输入</param>
        /// <param name="multiplier">驱动速度加成（默认设为 Vector2.one）</param>
        public void View(float delta, Vector2 axis, Vector2 multiplier)
        {
            // 转向减速
            if (_chassis != null)
            {
                if (_chassis.velocity.magnitude > 2.8e-2f)
                {
                    var axisX = axis.x;
                    if (Mathf.Abs(axisX) > 200) axisX *= 200 / Mathf.Abs(axisX);
                    var velocity = _chassis.velocity;
                    _chassis.AddForce(
                        velocity * (-10 * axisX * (2 - velocity.magnitude)));
                }
            }

            // 云台旋转
            _yawNeed += axis.x * delta * multiplier.x;
            _pitchNeed += axis.y * delta * multiplier.y;

            var yawPresent = _yawEuler % 360;
            if (yawPresent < 0) yawPresent += 360;
            if (yawPresent > 360) yawPresent -= 360;
            var yawPresentNeed = _yawNeed % 360;
            if (yawPresentNeed < 0) yawPresentNeed += 360;
            if (yawPresentNeed > 360) yawPresentNeed -= 360;
            var yawPresentError = yawPresentNeed - yawPresent;
            if (Mathf.Abs(yawPresentError) > 180)
            {
                yawPresentError = yawPresentError > 0 ? yawPresentError - 360 : yawPresentError + 360;
            }

            var yawDiff = (yawPresentError - _yawLastError) / delta;
            _yawLastError = yawPresentError;
            _yawIntegral += yawPresentError * delta;
            _yawEuler += yawPresentError * Kp + _yawIntegral * Ki + yawDiff * Kd;
            _pitchNeed = ClampValue(_pitchNeed, _pitchMin, _pitchMax);
            var pitchPresent = _pitchEuler % 360;
            if (pitchPresent < 0) pitchPresent += 360;
            if (pitchPresent > 360) pitchPresent -= 360;
            var pitchPresentNeed = _pitchNeed % 360;
            if (pitchPresentNeed < 0) pitchPresentNeed += 360;
            if (pitchPresentNeed > 360) pitchPresentNeed -= 360;
            var pitchPresentError = pitchPresentNeed - pitchPresent;
            if (Mathf.Abs(pitchPresentError) > 180)
            {
                pitchPresentError = pitchPresentError > 0 ? pitchPresentError - 360 : pitchPresentError + 360;
            }
            var pitchDiff = (pitchPresentError - _pitchLastError) / delta;
            _pitchLastError = pitchPresentError;
            _pitchIntegral += pitchPresentError * delta;
            _pitchEuler += pitchPresentError * Kp + _pitchIntegral * Ki + pitchDiff * Kd;
        }

        /// <summary>
        /// 旋转Yaw轴（可配合底盘跟随等）。
        /// </summary>
        /// <param name="angle">旋转角度</param>
        public void YawRotate(float angle)
        {
            _additionalYawRotate += angle;
        }

        /// <summary>
        /// 执行云台姿态更新（需要每帧调用）。
        /// </summary>
        public void Update()
        {
            _yaw.localRotation = Quaternion.Euler(new Vector3(0, _yawEuler + _additionalYawRotate, 0) + _yawInitEuler);
            _pitch.localRotation = Quaternion.Euler(new Vector3(_pitchEuler, 0, 0) + _pitchInitEuler);
        }

        public void SetRotation(Vector2 input)
        {
            _yaw.localRotation = Quaternion.Euler(new Vector3(0, input.y, 0));
            _pitch.localRotation = Quaternion.Euler(new Vector3(input.x, 0, 0));
        }

        /// <summary>
        /// 限制旋转角度。
        /// </summary>
        /// <param name="value">初始角度</param>
        /// <param name="min">最小角度</param>
        /// <param name="max">最大角度</param>
        /// <returns>符合限制要求的角度</returns>
        private static float ClampValue(float value, float min, float max)
        {
            if (value < -360)
                value += 360;
            if (value > 360)
                value -= 360;
            //限制value的值在min和max之间， 如果value小于min，返回min。 如果value大于max，返回max，否则返回value
            return Mathf.Clamp(value, min, max);
        }
    }
}