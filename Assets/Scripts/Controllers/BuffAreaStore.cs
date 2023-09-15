using System;
using Controllers.RobotSensor;
using Gameplay;
using Gameplay.Effects;
using Gameplay.Events;
using Infrastructure;
using Misc;

namespace Controllers
{
    /// <summary>
    /// 增益区类型。
    /// </summary>
    public enum BuffAreaID
    {
        Revive,
        Base,
        DroneBase,
        Snipe,

        /// <summary>
        /// 环形高低高亮增益区
        /// </summary>
        RingHighland,
        TrapezoidalHighland3,
        TrapezoidalHighland4,
        LaunchRampStart,
        LaunchRampEnd,
        Central,
        Outpost,
        CentralRMUL,
        Stabilize,

        /// <summary>
        /// 哨兵巡逻区
        /// </summary>
        PatrolArea
    }


    /// <summary>
    /// 增益区控制器。
    /// </summary>
    public class BuffAreaStore : StoreBase
    {
        public BuffAreaID type;
        private protected ISensor _sensor;
        private protected SensorUtility _sensorUtility;

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
                throw new Exception("Initializing buff area without sensor.");
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
        protected override void AsyncUpdate()
        {
            if (!isServer) return;
            if (_sensor != null)
            {
                _sensorUtility.Update(_sensor);
            }
        }

        protected override void Identify()
        {
        }

        /// <summary>
        /// 为机器人添加增益。
        /// </summary>
        /// <param name="robot">目标机器人引用</param>
        /// <param name="buff">增益</param>
        /// <exception cref="Exception"></exception>
        private static void AddBuff(StoreBase robot, EffectBase buff)
        {
            // if (!(buff is IBuff))
            // {
            //     throw new Exception("Adding non-buff effect as buff.");
            // }

            if (!robot.HasEffect(buff.type))
            {
                Dispatcher.Instance().Send(new AddEffect
                {
                    Receiver = robot.id,
                    Effect = buff
                });
            }
        }

        /// <summary>
        /// 为机器人去除增益。
        /// </summary>
        /// <param name="robot">目标机器人引用</param>
        /// <param name="buff">增益</param>
        /// <exception cref="Exception"></exception>
        private static void RemoveBuff(StoreBase robot, EffectBase buff)
        {
            // if (!(buff is IBuff))
            // {
            //     throw new Exception("Removing non-buff effect as buff.");
            // }

            if (robot.HasEffect(buff.type))
            {
                Dispatcher.Instance().Send(new RemoveEffect
                {
                    Receiver = robot.id,
                    Effect = buff
                });
            }
        }

        // TODO: RobotStay 用于实现增益区消失

        /// <summary>
        /// 机器人进入增益区触发。
        /// </summary>
        /// <param name="robot"></param>
        private void RobotEnter(RobotStoreBase robot)
        {
            switch (type)
            {
                case BuffAreaID.Revive:
                    if (robot.id.role != Identity.Roles.AutoSentinel)
                        AddBuff(robot, new ReviveBuff());
                    break;

                case BuffAreaID.Base:
                    if (robot.id.role != Identity.Roles.AutoSentinel)
                        AddBuff(robot, new BaseBuff());
                    break;

                case BuffAreaID.DroneBase:
                    if (robot.id.role != Identity.Roles.AutoSentinel)
                        AddBuff(robot, new DroneBaseBuff());
                    //Debug.Log("加了dronestore");
                    break;

                case BuffAreaID.Snipe:
                    AddBuff(robot, new SnipeBuff());
                    Dispatcher.Instance().Send(new PowerRuneActivating
                    {
                        Activating = true,
                        Camp = robot.id.camp
                    });
                    break;

                case BuffAreaID.RingHighland:
                    AddBuff(robot, new HighlandCoolBuff());
                    Dispatcher.Instance().Send(new TheCommandingPoint()
                    {
                        //传递传感器的阵营
                        Camp = id.camp
                    });
                    break;

                case BuffAreaID.TrapezoidalHighland3:
                    if (robot.id.role == Identity.Roles.Hero && robot.id.camp == id.camp)
                        AddBuff(robot, new HighlandCoinBuff());

                    AddBuff(robot, new HighlandCoolBuff());
                    break;

                case BuffAreaID.TrapezoidalHighland4:
                    AddBuff(robot, new HighlandCoolBuff());
                    break;

                case BuffAreaID.LaunchRampStart:
                    Dispatcher.Instance().Send(new AddEffect
                    {
                        Receiver = robot.id,
                        Effect = new LaunchRampActivating()
                    });
                    break;

                case BuffAreaID.LaunchRampEnd:
                    if (robot.HasEffect(EffectID.Status.LaunchRampActivating))
                    {
                        // 成功激活飞坡
                        AddBuff(robot, new LaunchRampBuff());
                    }

                    break;

                case BuffAreaID.Central:
                    // TODO: 禁区
                    if (robot.id.role == Identity.Roles.Engineer)
                        AddBuff(robot, new ResourceIsland());
                    break;

                case BuffAreaID.Outpost:
                    AddBuff(robot, new OutpostBuff());
                    break;

                case BuffAreaID.CentralRMUL:
                    AddBuff(robot, new CentralBuff());
                    break;

                case BuffAreaID.Stabilize:
                    AddBuff(robot, new Stabilize());
                    break;

                case BuffAreaID.PatrolArea:
                    if (robot.id.role == Identity.Roles.AutoSentinel)
                    {
                        Dispatcher.Instance().Send(new AtPatrol()
                        {
                            Id = robot.id,
                            IsIn = true
                        });


                        //给云台手
                        var dronePlayer = new Identity(robot.id.camp, Identity.Roles.Drone, robot.id.serial,
                            robot.id.order - 1);
                        Dispatcher.Instance().Send(new RemoveEffect()
                        {
                            Receiver = dronePlayer,
                            Effect = new LeavePatrol()
                        });
                    }

                    break;
            }
        }

        /// <summary>
        /// 机器人离开增益区触发。
        /// </summary>
        /// <param name="robot"></param>
        private void RobotExit(RobotStoreBase robot)
        {
            switch (type)
            {
                case BuffAreaID.Revive:
                    if (robot.id.role != Identity.Roles.AutoSentinel)
                        RemoveBuff(robot, new ReviveBuff());
                    break;
                case BuffAreaID.Base:
                    if (robot.id.role != Identity.Roles.AutoSentinel)
                        RemoveBuff(robot, new BaseBuff());
                    break;
                case BuffAreaID.Snipe:
                    RemoveBuff(robot, new SnipeBuff());
                    Dispatcher.Instance().Send(new PowerRuneActivating
                    {
                        Activating = false,
                        Camp = robot.id.camp
                    });
                    break;
                case BuffAreaID.RingHighland:
                    RemoveBuff(robot, new HighlandCoolBuff());
                    Dispatcher.Instance().Send(new LeaveHighLand()
                    {
                        Camp = id.camp
                    });
                    break;
                case BuffAreaID.TrapezoidalHighland3:
                    RemoveBuff(robot, new HighlandCoinBuff());
                    RemoveBuff(robot, new HighlandCoolBuff());
                    break;
                case BuffAreaID.TrapezoidalHighland4:
                    RemoveBuff(robot, new HighlandCoolBuff());
                    break;
                case BuffAreaID.Central:
                    // TODO: 禁区
                    RemoveBuff(robot, new ResourceIsland());
                    break;
                case BuffAreaID.Outpost:
                    RemoveBuff(robot, new OutpostBuff());
                    break;
                case BuffAreaID.CentralRMUL:
                    RemoveBuff(robot, new CentralBuff());
                    break;
                case BuffAreaID.Stabilize:
                    RemoveBuff(robot, new Stabilize());
                    break;
                case BuffAreaID.PatrolArea:
                    if (robot.id.role == Identity.Roles.AutoSentinel)
                    {
                        Dispatcher.Instance().Send(new AtPatrol()
                        {
                            Id = robot.id,
                            IsIn = false
                        });

                        //给云台手
                        var dronePlayer = new Identity(robot.id.camp, Identity.Roles.Drone, robot.id.serial,
                            robot.id.order - 1);
                        Dispatcher.Instance().Send(new AddEffect()
                        {
                            Receiver = dronePlayer,
                            Effect = new LeavePatrol()
                        });
                    }

                    break;
            }
        }
    }
}