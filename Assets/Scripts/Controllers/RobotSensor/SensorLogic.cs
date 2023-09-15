using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using UnityEngine;

namespace Controllers.RobotSensor
{
    /// <summary>
    /// <c>SensorLogic</c> 实现传感器核心逻辑。
    /// <br/>根据配置和机器人进出事件，统计触发传感器的机器人列表。
    /// </summary>
    public class SensorLogic
    {
        private readonly SensorConfig.Accessor _accessor;
        private readonly SensorConfig.Quantity _quantity;
        private readonly float _warmUpTime;
        private readonly float _coolDownTime;
        /// <summary>
        /// 用于储存进入传感器的机器人的队列
        /// </summary>
        private readonly List<StoreBase> _enterQueue = new List<StoreBase>();
        /// <summary>
        /// 用于储存离开传感器的机器人的队列
        /// </summary>
        private readonly List<StoreBase> _exitQueue = new List<StoreBase>();

        public readonly List<StoreBase> Targets = new List<StoreBase>();

        /// <summary>
        /// 初始化核心逻辑。
        /// </summary>
        /// <param name="accessor">哪个阵营可触发传感器</param>
        /// <param name="quantity">有多少机器人可触发传感器</param>
        /// <param name="warmUpTime">触发前摇</param>
        /// <param name="coolDownTime">离开后摇</param>
        public SensorLogic(SensorConfig.Accessor accessor, SensorConfig.Quantity quantity, float warmUpTime,
            float coolDownTime)
        {
            _accessor = accessor;
            _quantity = quantity;
            _warmUpTime = warmUpTime;
            _coolDownTime = coolDownTime;
        }

        /// <summary>
        /// 处理机器人进入触发区域事件。
        /// </summary>
        /// <param name="robot">进入的机器人</param>
        /// <returns></returns>
        public IEnumerator Enter(StoreBase robot)
        {
            //如果队列中已存在该机器人则不进行再次添加进入队列
            if (_enterQueue.Contains(robot)) yield break;
            
            //添加机器人进入队列并将出队队列中相应的机器人删除
            _enterQueue.Add(robot);
            if (_exitQueue.Contains(robot)) _exitQueue.Remove(robot);
            
            yield return new WaitForSeconds(_warmUpTime);
            if (!_enterQueue.Contains(robot)) yield break;
            _enterQueue.Remove(robot);
            Activate(robot);
        }

        /// <summary>
        /// 处理机器人离开触发区域事件。
        /// </summary>
        /// <param name="robot">离开的机器人</param>
        /// <returns></returns>
        public IEnumerator Exit(StoreBase robot)
        {
            if (_exitQueue.Contains(robot)) yield break;
            _exitQueue.Add(robot);
            if (_enterQueue.Contains(robot)) _enterQueue.Remove(robot);
            yield return new WaitForSeconds(_coolDownTime);
            if (!_exitQueue.Contains(robot)) yield break;
            _exitQueue.Remove(robot);
            Deactivate(robot);
        }

        /// <summary>
        /// 前摇结束后，进行机器人触发判定。
        /// </summary>
        /// <param name="robot">机器人</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void Activate(StoreBase robot)
        {
            if (_accessor == SensorConfig.Accessor.None) return;
            if (_accessor != SensorConfig.Accessor.Both)
            {
                // 只有某一方能激活
                if (!(_accessor == SensorConfig.Accessor.Red && robot.id.camp == Identity.Camps.Red) &&
                    !(_accessor == SensorConfig.Accessor.Blue && robot.id.camp == Identity.Camps.Blue)) return;
                if (_quantity == SensorConfig.Quantity.First)
                {
                    if (Targets.Count == 0)
                    {
                        Targets.Add(robot);
                    }
                }
                else
                {
                    Targets.Add(robot);
                }
            }
            else
            {
                // 双方都能激活
                switch (_quantity)
                {
                    case SensorConfig.Quantity.First:
                        if (Targets.Count == 0)
                        {
                            Targets.Add(robot);
                        }

                        break;
                    case SensorConfig.Quantity.Camp:
                        if (Targets.Count == 0)
                        {
                            Targets.Add(robot);
                        }
                        else
                        {
                            if (Targets[0].id.camp == robot.id.camp)
                            {
                                Targets.Add(robot);
                            }
                        }

                        break;
                    case SensorConfig.Quantity.All:
                        Targets.Add(robot);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 后摇结束后，停止机器人触发。
        /// </summary>
        /// <param name="robot">机器人</param>
        private void Deactivate(StoreBase robot)
        {
            if (Targets.Contains(robot))
                Targets.Remove(robot);
        }
    }
}