using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.Components
{
    /// <summary>
    /// <c>Chassis</c> 实现了全向运动底盘组件。
    /// <br/>其功能包括全向移动、云台跟随、小陀螺等。
    /// </summary>
    public class GroundChassis
    {
        private readonly List<string> _collideWith = new List<string>();
        private readonly Transform _ptz;
        private readonly Transform _transform;
        private readonly List<WheelCollider> _wheels;
        public readonly Rigidbody Rigidbody;
        private float _integral, _lastError;

        /// <summary>
        /// 初始化全向运动底盘。
        /// </summary>
        /// <param name="chassis">车辆底盘</param>
        /// <param name="wheels">多个（从动）轮</param>
        /// <param name="ptzYaw">（可选）云台Yaw轴</param>
        /// <exception cref="Exception">初始化问题</exception>
        public GroundChassis(GameObject chassis, List<WheelCollider> wheels, Transform ptzYaw = null)
        {
            Rigidbody = chassis.GetComponent<Rigidbody>();
            if (Rigidbody == null)
            {
                throw new Exception("Initiating chassis without a Rigidbody component.");
            }

            _transform = chassis.GetComponent<Transform>();
            if (_transform == null)
            {
                throw new Exception("Initiating chassis without a Transform component.");
            }

            if (_transform.GetComponent<Collider>() == null)
            {
                throw new Exception("Initiating chassis without a Collider component.");
            }

            _wheels = wheels;
            if (_wheels.Count == 0)
            {
                throw new Exception("Initiating chassis without any wheel.");
            }

            _ptz = ptzYaw == null ? chassis.transform : ptzYaw;
        }

        /// <summary>
        /// 接收运动控制输入，驱动底盘。
        /// </summary>
        /// <param name="axis">控制输入</param>
        /// <param name="multiplier">驱动功率加成</param>
        /// <param name="velocityLimit">底盘限速</param>
        /// <param name="superBattery">超级电容</param>
        /// <param name="isBalance">平衡步</param>
        /// <param name="turn">平衡步侧身</param>
        /// <param name="batteryMultiplier">超级电容效率</param>
        /// ///
        public void Move(Vector2 axis, float multiplier = 1, float velocityLimit = 4, bool superBattery = false,
            bool isBalance = false, bool turn = false,
            float batteryMultiplier = 1)
        {
            // 解锁轮胎并计算着地轮胎比例
            var wheelsGrounded = 0;
            foreach (var wheel in _wheels)
            {
                wheel.motorTorque = axis.magnitude != 0 ? 1e-7f : 0;
                wheel.brakeTorque = axis.magnitude != 0 ? 0 : 1e7f;
                if (wheel.isGrounded) wheelsGrounded++;
            }

            var wheelsGroundedMultiplier = (float)wheelsGrounded / _wheels.Count;

            // 施加驱动力
            if (wheelsGrounded != 0 && axis.magnitude != 0)
            {
                var force = superBattery
                    ? Rigidbody.mass * 25 * multiplier * wheelsGroundedMultiplier * batteryMultiplier
                    : Rigidbody.mass * 12 * multiplier * wheelsGroundedMultiplier;
                velocityLimit = superBattery ? velocityLimit * 2 : velocityLimit;
                if (Rigidbody.velocity.magnitude < velocityLimit)
                {
                    if (!isBalance)
                    {
                        Rigidbody.AddForce(
                            // 横轴（较慢）
                            _ptz.right * (-axis.x * 0.7f * force)
                            // 纵轴
                            + _ptz.forward * (-axis.y * force));
                    }
                    else
                    {
                        if (!turn)
                        {
                            Rigidbody.AddForce(
                                // 纵轴
                                _ptz.forward * (-axis.y * force * 0.7f));
                        }

                        if (turn)
                        {
                            // 横轴（较慢）
                            Rigidbody.AddForce(
                                // 纵轴
                                _ptz.right * (-axis.x * 0.6f * force));
                        }
                    }
                }
            }

            // 响应曲线
            if (Rigidbody.velocity.magnitude > 0)
            {
                var velocity = Rigidbody.velocity;
                var inputMagnitude = axis.magnitude;
                if (inputMagnitude < 2.8e-2f) Rigidbody.velocity = new Vector3(0, velocity.y, 0);
                else
                    Rigidbody.AddForce(
                        -velocity * (1 / inputMagnitude * 3200 * velocity.magnitude * wheelsGroundedMultiplier));
            }

            // 停止自旋
            if (axis.magnitude == 0 && wheelsGrounded == _wheels.Count)
            {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
            }
        }


        /// <summary>
        /// 执行小陀螺或云台跟随（需要每帧调用）。
        /// </summary>
        /// <param name="delta">帧时间</param>
        /// <param name="spin">是否处于小陀螺状态</param>
        /// <param name="multiplier">旋转速度加成</param>
        /// <param name="sensorDrift">传感器温漂（度每分钟）</param>
        /// <param name="stickWithYaw">云台是否没有 Yaw 轴</param>
        /// <param name="superBattery">超级电容</param>
        /// <param name="batteryMultiplier">超级电容效率</param>
        /// <param name="turn">平衡步侧身</param>
        /// <returns>云台作为子物体需要旋转抵消影响的角度</returns>
        public float Update(float delta, bool spin = false, float multiplier = 1, float sensorDrift = 0,
            bool stickWithYaw = false, bool superBattery = false, float batteryMultiplier = 1, bool isbalance = false,
            bool turn = false)
        {
            // 小陀螺
            if (spin)
            {
                var speed = superBattery
                    ? 3e2f * delta * multiplier * 2 * batteryMultiplier
                    : 3e2f * delta * multiplier;
                _transform.Rotate(Vector3.up * speed);
                if (isbalance)
                    Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
                return -speed;
            }
            else
            {
                Rigidbody.constraints = RigidbodyConstraints.None;
            }

            //todo


            // 如果有云台则进行底盘跟随
            if (_ptz == _transform) return 0;

            var chassisY = _transform.eulerAngles.y % 360;
            var yawY = (chassisY + _ptz.localEulerAngles.y) % 360;
            if (chassisY < 0) chassisY += 360;
            if (yawY < 0) chassisY += 360;
            _transform.Rotate(sensorDrift / 60 * delta * Vector3.up);

            //侧身（？
            if (isbalance)
                if (turn)
                {
                    var present = yawY - chassisY + 90;
                    if (Mathf.Abs(present) > 180)
                    {
                        present = present > 0 ? present - 360 : present + 360;
                    }

                    _integral += present * delta;
                    var diff = (present - _lastError) / delta;
                    _lastError = present;
                    var speed = present * .1f + _integral * .2f + diff * .01f;

                    _transform.Rotate(Vector3.up * speed);
                    return -speed;
                }
                else
                {
                    var present = yawY - chassisY;
                    if (Mathf.Abs(present) > 180)
                    {
                        present = present > 0 ? present - 360 : present + 360;
                    }

                    _integral += present * delta;
                    var diff = (present - _lastError) / delta;
                    _lastError = present;
                    var speed = present * .1f + _integral * .2f + diff * .01f;

                    _transform.Rotate(Vector3.up * speed);
                    return -speed;
                }

            if (!stickWithYaw)
            {
                var present = yawY - chassisY;
                if (Mathf.Abs(present) > 180)
                {
                    present = present > 0 ? present - 360 : present + 360;
                }

                _integral += present * delta;
                var diff = (present - _lastError) / delta;
                _lastError = present;
                var speed = present * .1f + _integral * .2f + diff * .01f;

                _transform.Rotate(Vector3.up * speed);
                return -speed;
            }

            // TODO: 工程防抖
            _transform.Rotate(Vector3.up * (yawY - chassisY));
            return chassisY - yawY;
        }

        /// <summary>
        /// 阻止车辆撞击场地后打滑旋转。
        /// </summary>
        /// <param name="collision"></param>
        public void OnCollisionEnter(Collision collision)
        {
            var other = collision.gameObject;
            _collideWith.Add(other.name);
            // TODO: 防止飞天
            // rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionY;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
        }

        /// <summary>
        /// 车辆不再与场地碰撞后，解锁旋转。
        /// </summary>
        /// <param name="collision"></param>
        public void OnCollisionExit(Collision collision)
        {
            var other = collision.gameObject;
            _collideWith.Remove(other.name);
            if (_collideWith.Count == 0) Rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
}