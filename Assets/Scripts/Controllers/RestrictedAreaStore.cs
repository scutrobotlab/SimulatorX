using System;
using Controllers.RobotSensor;
using Gameplay;
using Gameplay.Events;
using Infrastructure;
using Misc;

namespace Controllers
{
    public class RestrictedAreaStore : StoreBase
    {
        private ISensor _sensor;
        private SensorUtility _sensorUtility;
        
        /// <summary>
        /// 寻找区域传感器。
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected override void Start()
        {
            base.Start();
            if (GetComponent<Sensor>()) _sensor = GetComponent<Sensor>();
            if (GetComponent<SensorGroup>()) _sensor = GetComponent<SensorGroup>();
            if (_sensor == null)
            {
                throw new Exception("Initializing restricted area without sensor.");
            }

            _sensorUtility = new SensorUtility(
                enter =>
                {
                    if (!(enter is RobotStoreBase robot)) return;
                    RobotEnter(robot);
                },
                exit =>
                {
                    if (!(exit is RobotStoreBase robot)) return;
                    RobotExit(robot);
                });
        }

        /// <summary>
        /// 更新传感数据。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;
            if (_sensor != null)
            {
                _sensorUtility.Update(_sensor);
            }
        }

        /// <summary>
        /// 自动判罚。
        /// </summary>
        /// <param name="robot"></param>
        private void RobotEnter(RobotStoreBase robot)
        {
           Dispatcher.Instance().Send(new Penalty
           {
               target = robot.id,
               Description = "进入禁区"
           }); 
        }

        private void RobotExit(RobotStoreBase robot)
        {
            // 暂无动作
        }
    }
}