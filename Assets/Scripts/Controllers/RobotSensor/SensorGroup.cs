using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using UnityEngine;

namespace Controllers.RobotSensor
{
    /// <summary>
    /// <c>SensorGroup</c> 实现将多个传感器合成一组。
    /// <br/>代理所有传感器的事件，可作为单个传感器使用。
    /// </summary>
    public class SensorGroup : MonoBehaviour, ISensor
    {
        public SensorConfig.Accessor accessor;
        public SensorConfig.Quantity quantity;
        public float warmUpTime;
        public float coolDownTime;

        private SensorLogic _logic;
        public List<StoreBase> targets => _logic.Targets;
        public List<Sensor> sensors = new List<Sensor>();

        private HashSet<StoreBase> _lastTargets = new HashSet<StoreBase>();

        /// <summary>
        /// 创建一个委托方法使传感器的检测对象拓展
        /// </summary>
        public delegate bool CanTrigger(StoreBase incomingStore);

        private CanTrigger _decideFunction;

        /// <summary>
        /// 用于接收不同store传递过来的相应判断函数，用于传感器的检测
        /// </summary>
        /// <param name="decideFunction">不同store传递来的判断函数</param>
        public void AddCustomTriggerFilter(CanTrigger decideFunction)
        {
            _decideFunction = decideFunction;
        }

        /// <summary>
        /// 初始化内部逻辑。
        /// </summary>
        private void Awake()
        {
            _logic = new SensorLogic(accessor, quantity, warmUpTime, coolDownTime);
        }

        /// <summary>
        /// 收集并整合子传感器结果。
        /// </summary>
        /// TODO: 使用 SensorUtility
        private void FixedUpdate()
        {
            var childTargets = new HashSet<StoreBase>();
            foreach (var target in sensors.SelectMany(sensor => sensor.targets))
            {
                childTargets.Add(target);
            }

            foreach (var enter in childTargets.Except(_lastTargets))
            {
                if (_decideFunction == null) Enter(enter);
                else
                {
                    if (_decideFunction(enter)) Enter(enter);
                }
            }

            foreach (var exit in _lastTargets.Except(childTargets))
            {
                if (_decideFunction == null) Exit(exit);
                else
                {
                    if (_decideFunction(exit)) Exit(exit);
                }
            }

            _lastTargets = childTargets;
        }

        /// <summary>
        /// 当子传感器报告时，代理进入事件。
        /// </summary>
        /// <param name="robot">进入的机器人</param>
        private void Enter(StoreBase robot)
        {
            StartCoroutine(_logic.Enter(robot));
        }

        /// <summary>
        /// 当子传感器报告时，代理离开事件。
        /// </summary>
        /// <param name="robot">离开的机器人</param>
        private void Exit(StoreBase robot)
        {
            StartCoroutine(_logic.Exit(robot));
        }
    }
}