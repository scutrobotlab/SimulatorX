using System.Collections.Generic;
using Gameplay;
using Infrastructure;
using UnityEngine;

namespace Controllers.RobotSensor
{
    /// <summary>
    /// <c>Sensor</c> 控制机器人传感器。
    /// <br/>根据特定筛选条件，传感进入范围的机器人。
    /// <br/>也可以作为传感器组中的单个传感器。
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Sensor : MonoBehaviour, ISensor
    {
        public SensorConfig.Accessor accessor;
        public SensorConfig.Quantity quantity;
        public float warmUpTime;
        public float coolDownTime;
        public bool maintainOnDeath;

        private SensorLogic _logic;
        public List<StoreBase> targets => _logic.Targets;

        // 用于自定义过滤函数的代理
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
        /// 初始化底层逻辑。
        /// </summary>
        private void Awake()
        {
            _logic = new SensorLogic(accessor, quantity, warmUpTime, coolDownTime);
        }

        /// <summary>
        /// 将传感器碰撞组件设置为触发器模式。
        /// </summary>
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        /// <summary>
        /// 当有物体进入触发器时，代理该事件。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            var otherStore = other.GetComponent<StoreBase>();
            if (otherStore == null) return;
            // 不接受死亡机器人
            if (otherStore is RobotStoreBase {health: 0} && !maintainOnDeath) return;
            if (_decideFunction != null && !_decideFunction(otherStore)) return;
            StartCoroutine(_logic.Enter(otherStore));
        }

        // TODO：实时检查触发状态

        /// <summary>
        /// 当有物体离开触发器时，代理该事件。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            var otherStore = other.GetComponent<StoreBase>();
            if (otherStore == null) return;
            if (_decideFunction != null && !_decideFunction(otherStore)) return;
            StartCoroutine(_logic.Exit(otherStore));
        }
        
        //手动移除矿石
        public void TargetExit(StoreBase ore)
        {
            if(ore == null)
                return;
            if (_decideFunction != null && !_decideFunction(ore)) return;
            StartCoroutine(_logic.Exit(ore));

        }

        /// <summary>
        /// 若死亡则将其移除。
        /// </summary>
        private void FixedUpdate()
        {
            foreach (var target in targets)
            {
                if (target is RobotStoreBase robotStore)
                {
                    if (robotStore.health == 0 && !maintainOnDeath)
                    {
                        StartCoroutine(_logic.Exit(robotStore));
                    }
                }
            }
        }
    }
}