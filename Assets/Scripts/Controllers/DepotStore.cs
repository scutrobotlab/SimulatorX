using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.RobotSensor;
using Gameplay.Effects;
using Gameplay.Events;
using Gameplay;
using Gameplay.Attribute;
using Infrastructure;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    /// <summary>
    /// <c>DepotStore</c> 控制补给站。
    /// <br/>根据传感器给予机器人 <c>AtSupply</c> 效果。
    /// <br/>接收来自 <c>CoinStore</c> 的补给指令，发放弹丸。
    /// </summary>
    public class DepotStore : StoreBase
    {
        [Header("Identity")] public Identity.Camps camp;
        [Header("Hero Sensor")] public Sensor heroSensor;
        [Header("Left Sensor")] public Sensor leftSensor;
        public Transform leftDropSpot;
        [Header("Right Sensor")] public Sensor rightSensor;
        public Transform rightDropSpot;
        [Header("Bullet Prefab")] public GameObject smallBullet;
        

        private RobotStoreBase _heroRobot;
        private RobotStoreBase _leftRobot;
        private RobotStoreBase _rightRobot;
        
        private bool _isSameTarget;

        /// <summary>
        /// 确认身份。
        /// </summary>
        protected override void Identify()
        {
            id = new Identity(camp, Identity.Roles.Depot);
        }

        /// <summary>
        /// 标识兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Supply.DoSupply
            }).ToList();
        }

        // 初始化传感器
        protected override void Start()
        {
            base.Start();

            // 只感应英雄（改版需要，全部感应）
            heroSensor.AddCustomTriggerFilter(store => store.id.role == Identity.Roles.Hero
                                                       ||store.id.role == Identity.Roles.Infantry||store.id.role == Identity.Roles.BalanceInfantry);
            // 只感应步兵以及平衡步兵
            leftSensor.AddCustomTriggerFilter(store => store.id.role == Identity.Roles.Infantry||store.id.role == Identity.Roles.BalanceInfantry);
            rightSensor.AddCustomTriggerFilter(store => store.id.role == Identity.Roles.Infantry||store.id.role == Identity.Roles.BalanceInfantry);

            _isSameTarget = false;
        }

        /// <summary>
        /// 监视补给舱内机器人状态。
        /// </summary>
        /// <exception cref="Exception">机器人异常切换</exception>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (heroSensor.targets.Count > 0)
            {
                if (_heroRobot) _isSameTarget = (heroSensor.targets[0].id != _heroRobot.id);
                if ((!_heroRobot) || _isSameTarget)
                {
                    if (!_heroRobot)
                    {
                        _heroRobot = (RobotStoreBase)heroSensor.targets[0];
                        if(_heroRobot.id.camp==id.camp)
                            Dispatcher.Instance().Send(new AddEffect 
                            {
                            Receiver = _heroRobot.id,
                            Effect = new AtSupply() 
                            });
                        
                        else
                            Dispatcher.Instance().Send(new Penalty()
                            {
                                target = _heroRobot.id,
                                Description = "进入禁区"
                            });
                    }
                    else throw new Exception("Sudden switch of robot in original hero sensor.");
                }
               
            }
            else
            {
                if (_heroRobot)
                {
                    Dispatcher.Instance().Send(new RemoveEffect
                    {
                        Receiver = _heroRobot.id,
                        Effect = new AtSupply()
                    });
                    _heroRobot = null;
                }
            }

            if (leftSensor.targets.Count > 0)
            {
                if (!_leftRobot || leftSensor.targets[0].id != _leftRobot.id)
                {
                    if (!_leftRobot)
                    {
                        _leftRobot = (RobotStoreBase) leftSensor.targets[0];
                        
                        if(_leftRobot.id.camp==id.camp) 
                            Dispatcher.Instance().Send(new AddEffect 
                            {
                            Receiver = _leftRobot.id,
                            Effect = new AtSupply() 
                            });
                        
                        else
                            Dispatcher.Instance().Send(new Penalty()
                            {
                                target = _leftRobot.id,
                                Description = "进入禁区"
                            });
                    }
                    else throw new Exception("Sudden switch of robot in left sensor.");
                }
            }
            else
            {
                if (_leftRobot != null)
                {
                    Dispatcher.Instance().Send(new RemoveEffect
                    {
                        Receiver = _leftRobot.id,
                        Effect = new AtSupply()
                    });
                    _leftRobot = null;
                }
            }

            if (rightSensor.targets.Count > 0)
            {
                if (!_rightRobot || rightSensor.targets[0].id != _rightRobot.id)
                {
                    if (!_rightRobot)
                    {
                        _rightRobot = (RobotStoreBase) rightSensor.targets[0];
                       
                        if(_rightRobot.id.camp==id.camp)
                            Dispatcher.Instance().Send(new AddEffect
                            {
                                Receiver = _rightRobot.id,
                                Effect = new AtSupply()
                            });
                       
                        else
                            Dispatcher.Instance().Send(new Penalty()
                            {
                                target = _rightRobot.id,
                                Description = "进入禁区"
                            });
                    }
                    else throw new Exception("Sudden switch of robot in right sensor.");
                }
            }
            else
            {
                if (_rightRobot != null)
                {
                    Dispatcher.Instance().Send(new RemoveEffect
                    {
                        Receiver = _rightRobot.id,
                        Effect = new AtSupply()
                    });
                    _rightRobot = null;
                }
            }
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                // 发放弹丸
                case ActionID.Supply.DoSupply:
                    var supplyAction = (DoSupply) action;
                    if (supplyAction.Target.camp == id.camp)
                    {
                        if (supplyAction.Target.role == Identity.Roles.Infantry)
                        {
                            var infantry = (InfantryStore) EntityManager.Instance().Ref(supplyAction.Target);
                            infantry.totalMagSmall += supplyAction.Amount;
                            Dispatcher.Instance().Send(new AddBullet
                            {
                                Receiver = supplyAction.Target,
                                Type = MechanicType.CaliberType.Small,
                                Amount = supplyAction.Amount
                            });
                            
                            //发弹动画
                            if (infantry.magazine)
                            {
                                if (leftSensor.targets.Count == 1 && leftSensor.targets[0].id == supplyAction.Target)
                                {
                                    SyncDropBullets(leftDropSpot.position, supplyAction.Amount);
                                }
                                else if (rightSensor.targets.Count == 1 &&
                                         rightSensor.targets[0].id == supplyAction.Target)
                                {
                                    SyncDropBullets(rightDropSpot.position, supplyAction.Amount);
                                }
                            }
                        }
                        else if (supplyAction.Target.role == Identity.Roles.BalanceInfantry)
                        {
                            var balance = (BalancedInfantryStore) EntityManager.Instance().Ref(supplyAction.Target);
                            balance.totalMagSmall += supplyAction.Amount;
                            Dispatcher.Instance().Send(new AddBullet
                            {
                                Receiver = supplyAction.Target,
                                Type = MechanicType.CaliberType.Small,
                                Amount = supplyAction.Amount
                            });
                            
                            //发弹动画
                            if (balance.magazine)
                            {
                                if (leftSensor.targets.Count == 1 && leftSensor.targets[0].id == supplyAction.Target)
                                {
                                    SyncDropBullets(leftDropSpot.position, supplyAction.Amount);
                                }
                                else if (rightSensor.targets.Count == 1 &&
                                         rightSensor.targets[0].id == supplyAction.Target)
                                {
                                    SyncDropBullets(rightDropSpot.position, supplyAction.Amount);
                                }
                            }
                        }
                        else
                        {
                            var hero = (HeroStore) EntityManager.Instance().Ref(supplyAction.Target);
                            hero.totalMagLarge += supplyAction.Amount;
                            Dispatcher.Instance().Send(new AddBullet
                            {
                                Receiver = supplyAction.Target,
                                Type = MechanicType.CaliberType.Large,
                                Amount = supplyAction.Amount
                            });
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// 在服务端和客户端同时播放发放小弹丸动画。
        /// </summary>
        /// <param name="dropSpot">弹丸发射点</param>
        /// <param name="amount">弹丸发射量</param>
        [Server]
        private void SyncDropBullets(Vector3 dropSpot, int amount)
        {
            if (isServerOnly)
            {
                StartCoroutine(DropBullets(dropSpot, amount));
            }

            // 带有客户端的 Host 未考虑
            DropBulletsRpc(dropSpot, amount);
        }

        [ClientRpc]
        private void DropBulletsRpc(Vector3 dropSpot, int amount)
        {
            StartCoroutine(DropBullets(dropSpot, amount));
        }

        /// <summary>
        /// 执行发放小弹丸动作。
        /// </summary>
        /// <param name="dropSpot">弹丸发射点</param>
        /// <param name="amount">弹丸发射量</param>
        /// <returns></returns>
        private IEnumerator DropBullets(Vector3 dropSpot, int amount)
        {
            for (var i = 0; i < amount/5; i++)
            {
                var newBullet = Instantiate(smallBullet, dropSpot, Quaternion.identity);
                newBullet.GetComponent<Rigidbody>().velocity = Vector3.down * 2 +
                                                               Random.Range(-0.3f, 0.3f) * Vector3.right +
                                                               Random.Range(-0.3f, 0.3f) * Vector3.forward;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}