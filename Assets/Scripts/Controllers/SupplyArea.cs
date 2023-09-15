using System.Collections.Generic;
using System.Linq;
using Controllers.RobotSensor;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Effects;
using Gameplay.Events;
using Infrastructure;
using Misc;

namespace Controllers
{
    public class SupplyArea : StoreBase
    {
        public Sensor sensor;
        private SensorUtility _sensorUtility;

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 标识输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Supply.DoSupply
            }).ToList();
        }

        /// <summary>
        /// 处理输入事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Supply.DoSupply:
                    var doSupplyAction = (DoSupply) action;
                    if (doSupplyAction.Target.camp == id.camp)
                    {
                        var robotStore = (RobotStoreBase) EntityManager.Instance().Ref(doSupplyAction.Target);
                        
                        switch (doSupplyAction.Type)
                        {
                            case MechanicType.CaliberType.Small:
                                robotStore.totalMagSmall += doSupplyAction.Amount;
                                break;
                            case MechanicType.CaliberType.Large:
                                robotStore.totalMagLarge += doSupplyAction.Amount;
                                break;
                        }

                        Dispatcher.Instance().Send(new AddBullet
                        {
                            Amount = doSupplyAction.Amount,
                            Receiver = doSupplyAction.Target,
                            Type = doSupplyAction.Type
                        });
                    }
                    break;
            }
        }

        /// <summary>
        /// 机器人进入补给区。
        /// </summary>
        /// <param name="robot"></param>
        private static void RobotEnter(StoreBase robot)
        {
            if (!(robot is RobotStoreBase robotStore)) return;
            Dispatcher.Instance().Send(new AddEffect
            {
                Receiver = robotStore.id,
                Effect = new AtSupply()
            });
        }

        /// <summary>
        /// 机器人退出补给区。
        /// </summary>
        /// <param name="robot"></param>
        private static void RobotExit(StoreBase robot)
        {
            if (!(robot is RobotStoreBase robotStore)) return;
            Dispatcher.Instance().Send(new RemoveEffect
            {
                Receiver = robotStore.id,
                Effect = new AtSupply()
            });
        }

        /// <summary>
        /// 初始化组件。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _sensorUtility = new SensorUtility(RobotEnter, RobotExit);
        }

        /// <summary>
        /// 更新传感器状态。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            _sensorUtility.Update(sensor);
        }
    }
}