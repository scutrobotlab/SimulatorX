using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Events;
using Gameplay.Attribute;
using Infrastructure;
using Mirror;
using UI;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// <c>CoinStore</c> 对场上金币情况进行管理。
    /// <br/>存储双方金币数量、进行金币扣除相关授权等。
    /// </summary>
    public class CoinStore : StoreBase
    {
        private static CoinStore _instance;
        //人为增加金币次数
        private int times = 0;

        // 网络单例
        public static CoinStore Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CoinStore>();
            }

            if (_instance == null)
            {
                throw new Exception("Getting CoinStore before initialization.");
            }

            return _instance;
        }

        private readonly SyncDictionary<Identity.Camps, int> _coins = new SyncDictionary<Identity.Camps, int>
        {
            {Identity.Camps.Red, 0},
            {Identity.Camps.Blue, 0}
        };

        private readonly SyncDictionary<Identity.Camps, int> _maxCoins = new SyncDictionary<Identity.Camps, int>
        {
            {Identity.Camps.Red, 0},
            {Identity.Camps.Blue, 0}
        };
        
        private readonly SyncDictionary<Identity.Camps, bool> _startCoins = new SyncDictionary<Identity.Camps, bool>
        {
            {Identity.Camps.Red, false},
            {Identity.Camps.Blue, false}
        };
        
        private readonly SyncDictionary<Identity.Camps, int> _droneTime = new SyncDictionary<Identity.Camps, int>
        {
            {Identity.Camps.Red, 0},
            {Identity.Camps.Blue, 0}
        };

        //return maxCoins 返回最大金币数
        public int MaxCoinReturn(Identity.Camps ca)
        {
            return _maxCoins[ca];
        }

        /// <summary>
        /// 获取阵营经济描述。
        /// </summary>
        /// <param name="camp"></param>
        /// <returns></returns>
        public string CampCoinDescription(Identity.Camps camp)
        {
            return _coins[camp] + " / " + _maxCoins[camp];
        }

        private SupplyPanel _supplyUI;

        /// <summary>
        /// 声明关注事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            // 在父类中原方法的作用是返回一个空字符串，而在robot中需要覆盖这个方法来返回“兴趣字符串”
            // 这个函数将会把robot感兴趣的字符串存到Dispatcher中去
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Supply.AskSupply,
                ActionID.Exchange.DoExchange,
                ActionID.Exchange.ExchangeHealth,
                ActionID.Clock.CoinNaturalGrowth,
                ActionID.Supply.DoSupply,
                ActionID.DroneControl.RequestDroneSupport,
                ActionID.Supply.AskReturnCoin,
                ActionID.Supply.AskRewardCoin,
                
            }).ToList();
        }

        /// <summary>
        /// 准备补给站相关UI。
        /// </summary>
        protected override void LoadUI()
        {
            base.LoadUI();
            if (!isClient) return;
            // _supplyUI = UIManager.GetInstance().GetUI().GetComponentInChildren<SupplyPanel>();
            _supplyUI = UIManager.Instance().UI<SupplyPanel>();
        }

        // 正在进行的补给
        private readonly List<AskSupply> _supplyInProcess = new List<AskSupply>();

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                // 机器人请求复活时,扣除相应金币
                case ActionID.Exchange.ExchangeHealth:
                    var exchangeHeal = (ExchangeHealth) action;
                    _coins[exchangeHeal.Id.camp] += exchangeHeal.Money;
                    break;
                
                // 机器人请求补给时，启动相应客户端的补给站UI。
                case ActionID.Supply.AskSupply:
                    var supplyAction = (AskSupply) action;
                    //先判断补给发起者的阵营（id）
                    if (_supplyInProcess.Count(i => i.Target == supplyAction.Target) == 0)
                    {
                        _supplyInProcess.Add(supplyAction);
                    }

                    //把具体的判断金币是否满足放到UI功能中去实现
                    var conn = EntityManager.Instance().ConnectionByIdentity(supplyAction.Target);
                    if (conn == null)
                    {
                        // Is Replay
                        // throw new Exception("Supplying for not connected client.");
                    }
                    else
                    {
                        if(!supplyAction.isFar)
                            StartSupplyUISessionTarget(
                                conn,
                                supplyAction.Target.camp,
                                supplyAction.Type == MechanicType.CaliberType.Large ? 15 : 1,
                                supplyAction.Target);
                        else 
                            StartSupplyUISessionTarget(
                                conn,
                                supplyAction.Target.camp,
                                supplyAction.Type == MechanicType.CaliberType.Large ? 30 : 2,
                                supplyAction.Target);
                    }

                    break;
                
                //英雄梯形高地吊射的金币返还
                case ActionID.Supply.AskReturnCoin:
                    var returnCoinAction = (AskReturnCoin) action;
                    _coins[returnCoinAction.Camp] += returnCoinAction.Coin;
                    _maxCoins[returnCoinAction.Camp] += returnCoinAction.Coin;
                    break;
                
                //前哨站被打击奖励金币
                case ActionID.Supply.AskRewardCoin:
                    var rewardCoinAction = (AskRewardCoin) action;
                    _coins[rewardCoinAction.Camp] += rewardCoinAction.Coin;
                    _maxCoins[rewardCoinAction.Camp] += rewardCoinAction.Coin;
                    break;

                //判断矿石兑换阵营进行增加金币
                case ActionID.Exchange.DoExchange:
                    var exchangeAction = (DoExchange) action;
                    StartCoroutine(ChangeDelay(exchangeAction.Camp,exchangeAction.OreSinglePrice));
                    break;

                //经济的自然增长
                case ActionID.Clock.CoinNaturalGrowth:
                    var coinGrowAction = (CoinNaturalGrowth)action;
                    if (coinGrowAction.NotNature && times == 0)
                    {
                        var redAddCoin = PlayerPrefs.GetInt("red_init_add_coin");
                        var blueAddCoin = PlayerPrefs.GetInt("blue_init_add_coin");
                        PlayerPrefs.DeleteKey("red_init_add_coin");
                        PlayerPrefs.DeleteKey("blue_init_add_coin");
                        redAddCoin += coinGrowAction.MinuteCoin;
                        blueAddCoin += coinGrowAction.MinuteCoin;
                        _coins[Identity.Camps.Red] += redAddCoin;
                        _maxCoins[Identity.Camps.Red] += redAddCoin;
                        _coins[Identity.Camps.Blue] += blueAddCoin;
                        _maxCoins[Identity.Camps.Blue] += blueAddCoin;
                        ++times;
                        break;
                    }

                    _coins[Identity.Camps.Blue] += coinGrowAction.MinuteCoin;
                    _maxCoins[Identity.Camps.Blue] += coinGrowAction.MinuteCoin;
                    _coins[Identity.Camps.Red] += coinGrowAction.MinuteCoin;
                    _maxCoins[Identity.Camps.Red] += coinGrowAction.MinuteCoin;
                    break;
                //弹丸补给
                case ActionID.Supply.DoSupply:
                    if (!Dispatcher.Instance().replay) break;
                    var doSupplyAction = (DoSupply) action;
                    var price = doSupplyAction.Amount * (doSupplyAction.Type == MechanicType.CaliberType.Large ? 15 : 1);
                    var localRobotRef = (RobotStoreBase) EntityManager.Instance().Ref(doSupplyAction.Target);
                    var campInfantry = EntityManager.Instance().CampRef(doSupplyAction.Target.camp).FindAll(r =>
                        r is RobotStoreBase robotStore && (robotStore.id.role == Identity.Roles.Infantry || robotStore.id.role==Identity.Roles.BalanceInfantry	));
                    int totalMagSmall = 0;
                    foreach (var robot in campInfantry)
                    {
                        totalMagSmall+=((RobotStoreBase) robot).totalMagSmall ;
                    }
                    if ((doSupplyAction.Target.role == Identity.Roles.Infantry|| doSupplyAction.Target.role	==Identity.Roles.BalanceInfantry )&& totalMagSmall > 1500) return;
                    if (doSupplyAction.Target.role == Identity.Roles.Hero && localRobotRef.totalMagLarge > 100) return; 
                    if (_coins[doSupplyAction.Target.camp] < price) return;
                    _coins[doSupplyAction.Target.camp] -= price;
                    break;
                
                //空中支援
                case ActionID.DroneControl.RequestDroneSupport:
                    var requestDroneSupportAction = (RequestDroneSupport) action;
                    int coinNeed = 0;
                    if (requestDroneSupportAction.RequestTime > 175)
                    {
                        Dispatcher.Instance().Send(new AgreeDroneSupport
                        {
                            Camp = requestDroneSupportAction.Camp,
                            Agree = true,
                            Timestart = false,
                            StopTime = NetworkTime.time + 30
                        });  
                    }
                    else
                    {
                        coinNeed = 175 - (int)Math.Ceiling(requestDroneSupportAction.RequestTime);
                        if (_coins[requestDroneSupportAction.Camp] > coinNeed-1)
                        {
                            Dispatcher.Instance().Send(new AgreeDroneSupport
                            {
                                Camp = requestDroneSupportAction.Camp,
                                Agree = true,
                                Timestart = false,
                                StopTime = NetworkTime.time + 30
                            });
                            _coins[requestDroneSupportAction.Camp] -= coinNeed;  
                            _droneTime[requestDroneSupportAction.Camp]++;
                        }
                        else
                        {
                            Dispatcher.Instance().Send(new AgreeDroneSupport
                            {
                                Camp = requestDroneSupportAction.Camp,
                                Agree = false,
                                Timestart = false
                            });  
                        } 
                    }
                    break;
            }
        }

        #region SupplyUISession

        // 客户端变量
        private Identity.Camps _supplyUICamp = Identity.Camps.Other;

        /// <summary>
        /// 在特定客户端上打开补给UI。
        /// </summary>
        /// <param name="t">Rpc目标</param>
        /// <param name="camp">请求补给的单位所属阵营</param>
        /// <param name="singlePrice">请求补给弹丸单价</param>
        /// <param name="identity">id信息</param>   
        [TargetRpc]
        // ReSharper disable once UnusedParameter.Local
        private void StartSupplyUISessionTarget(NetworkConnection t, Identity.Camps camp, int singlePrice,Identity identity)
        {
            if (_supplyUICamp == Identity.Camps.Other)
            {
                _supplyUI.CurrentCoin = _coins[camp];
                _supplyUI.SinglePrice = singlePrice;
                var localRobotRef = (RobotStoreBase) EntityManager.Instance().Ref(identity);
                var campInfantry = EntityManager.Instance().CampRef(identity.camp).FindAll(r =>
                    r is RobotStoreBase robotStore && (robotStore.id.role == Identity.Roles.Infantry|| robotStore.id.role==Identity.Roles.BalanceInfantry	));
                int totalMagSmall = 0;
                foreach (var robot in campInfantry)
                {
                    totalMagSmall+=((RobotStoreBase) robot).totalMagSmall ;
                }
                _supplyUI.MaxcanSupply = identity.role == Identity.Roles.Hero ? 100 - localRobotRef.totalMagLarge : 1500-totalMagSmall;
                _supplyUICamp = camp;
                
                _supplyUI.StartSession(ReplySupplyUISession, identity);
                UIManager.SetCursorLock(false);
                StartCoroutine(UpdateSupplyUI());
            }
            else EndSupplyUISession();
        }

        /// <summary>
        /// 关闭补给UI。
        /// </summary>
        private void EndSupplyUISession()
        {
            _supplyUICamp = Identity.Camps.Other;
            UIManager.SetCursorLock(true);
            _supplyUI.EndSession();
            CmdEndSupplyUISession(EntityManager.Instance().LocalRobot());
        }

        [Command(requiresAuthority = false)]
        private void CmdEndSupplyUISession(Identity identity)
        {
            if (_supplyInProcess.Count(i => i.Target == identity) == 0)
            {
                throw new Exception("Ending non-exist supply session.");
            }

            var supplyAction = _supplyInProcess.First(i => i.Target == identity);
            _supplyInProcess.Remove(supplyAction);
        }

        /// <summary>
        /// 在打开补给UI时，同步现有金币数。
        /// </summary>
        /// <returns></returns>
        [Client]
        private IEnumerator UpdateSupplyUI()
        {
            while (_supplyUICamp != Identity.Camps.Other)
            {
                _supplyUI.CurrentCoin = _coins[_supplyUICamp];
                yield return new WaitForSeconds(Time.fixedDeltaTime * 4);
            }
        }

        [Client]
        private void ReplySupplyUISession(int number)
        {
            CmdReplySupplyUISession(number, EntityManager.Instance().LocalRobot());
        }

        /// <summary>
        /// 操作手确认补给选择时的回调。
        /// 判断金币是否足够，足够则通知补给站发放弹丸。
        /// </summary>
        /// <param name="number">选择补给的弹丸数量</param>
        /// <param name="identity">客户端所控制机器人</param>
        [Command(requiresAuthority = false)]
        private void CmdReplySupplyUISession(int number, Identity identity)
        {
            if (_supplyInProcess.Count(i => i.Target == identity) == 0)
            {
                throw new Exception("Supplying non-exist session.");
            }

            var supplyAction = _supplyInProcess.First(i => i.Target == identity);
            var price = number * (supplyAction.Type == MechanicType.CaliberType.Large ? 15 : 1);
            
            //远程供弹双倍价格
            if (supplyAction.isFar)
                price *= 2;
            
            var localRobotRef = (RobotStoreBase) EntityManager.Instance().Ref(identity);
            
            if (number < 0 && !_startCoins[supplyAction.Target.camp])
            {
                _maxCoins[supplyAction.Target.camp] -= number;
                _coins[supplyAction.Target.camp] -= number;
                _startCoins[supplyAction.Target.camp] = true;
                return;
            }
            
            if (identity.role == Identity.Roles.Hero && localRobotRef.totalMagLarge + number > 100) return;
            //获取本队步兵总兑换量
            var campInfantry = EntityManager.Instance().CampRef(identity.camp).FindAll(r =>
                r is RobotStoreBase robotStore && (robotStore.id.role == Identity.Roles.Infantry || robotStore.id.role==Identity.Roles.BalanceInfantry ));
            int totalMagSmall = 0;
            foreach (var robot in campInfantry)
            {
                totalMagSmall+=((RobotStoreBase) robot).totalMagSmall ;
            }
            if ((identity.role == Identity.Roles.Infantry || identity.role==Identity.Roles.BalanceInfantry) && totalMagSmall + number > 1500) return;
            if (_coins[supplyAction.Target.camp] < price) return;
            _coins[supplyAction.Target.camp] -= price;

            Dispatcher.Instance().Send(new DoSupply
            {
                Amount = number,
                Target = supplyAction.Target,
                Type = supplyAction.Type
            });
        }

        #endregion
        
        /// <summary>
        /// 延迟兑矿实现。
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangeDelay(Identity.Camps camp,int orePrice)
        {
            yield return new WaitForSeconds(1f);
            _coins[camp] += orePrice;
            _maxCoins[camp] +=orePrice;
        }

        public int CoinLeft(Identity.Camps camp)
        {
            return _coins[camp];
        }
    }
}