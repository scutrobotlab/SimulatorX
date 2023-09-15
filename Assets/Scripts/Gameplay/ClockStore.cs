using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Gameplay
{
    /// <summary>
    /// 倒计时管理器。
    /// 用于记录时间并触发时序事件。
    /// </summary>
    public class ClockStore : StoreBase
    {
        private static ClockStore _instance;

        // 网络单例
        public static ClockStore Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ClockStore>();
            }

            return _instance;
        }

        [HideInInspector][SyncVar] public bool playing;
        [HideInInspector][SyncVar] public bool finished;
        [SyncVar] public int countDown;
        [SyncVar] private MapType _currentMap;
        public bool powerAvailable = false;

        private double _startTime;
        private readonly int[] _oreDropOrder = {0, 0};
        // private double _gameTime = 7 * 60 + 26;
        private double _gameTime = 426;

        private readonly HashSet<int> _happened = new HashSet<int>();

        private bool _disconnectStarted;

        /// <summary>
        /// 声明输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.GameOver
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
                case ActionID.Stage.GameOver:
                    playing = false;
                    finished = true;
                    break;
            }
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!isServer) return;
            // 五个矿石掉落顺序随机
            _oreDropOrder[0] = Random.Range(0, 2) == 1 ? 1 : 3;
            _oreDropOrder[1] = _oreDropOrder[0] == 1 ? 3 : 1;
        }

        /// <summary>
        /// 如果是 RMUL，更改总比赛时间。
        /// </summary>
        /// <param name="mapType"></param>
        protected override void InitializeByMapType(MapType mapType)
        {
            base.InitializeByMapType(mapType);
            _currentMap = mapType;
            if (!isServer) return;
            _gameTime = mapType switch
            {
                MapType.RMUL2022 => 5 * 60 + 11,
                MapType.DeDust2 => 115 + 16,
                _ => _gameTime
            };
        }

        /// <summary>
        /// 获取当前倒计时值。
        /// </summary>
        /// <returns></returns>
        public int Countdown()
        {
            var countdown = _currentMap switch
            {
                MapType.None => 0,
                MapType.RMUC2022 => countDown - 7 * 60,//十秒的调整站位时间
                MapType.RMUL2022 => countDown - 5 * 60,
                MapType.DeDust2 => countDown - 115,
                _ => 0
            };
            if (countdown <= 0) return 0;
            return countdown;
        }

        /// <summary>
        /// 更新游戏倒计时
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (Dispatcher.Instance().replay || EntityManager.Instance().initialized)
            {
                if (isServer)
                {
                    if (_startTime == 0)
                    {
                        _startTime = NetworkTime.time;
                    }

                    // 倒计时
                    countDown = (int) (_gameTime - (NetworkTime.time - _startTime)+10);//增加十秒提供站位时间
                }

                TimeSensor(countDown);

                if (isClient)
                {
                    if (finished && !_disconnectStarted)
                    {
                        StartCoroutine(DelayDisconnect());
                        _disconnectStarted = true;
                    }
                }
            }
        }

        private IEnumerator DelayDisconnect()
        {
            yield return new WaitForSeconds(30);
            if (NetworkClient.active)
                NetworkManager.singleton.StopClient();
            if (NetworkServer.active)
                NetworkManager.singleton.StopServer();
            SceneManager.LoadScene("Offline");
        }


        /// <summary>
        /// 判断游戏剩余时间
        /// </summary>
        /// <param name="time">倒计时</param>
        private void TimeSensor(int time)
        {
            if (finished) return;
            switch (time)
            {
                // TODO:调整站位
                case 440:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                    }

                    

                    break;

                // TODO:十秒钟倒计时
                case 430:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                       
                        // 事件
                        Dispatcher.Instance().Send(new StartChecking());
                    }

                    break;

                // TODO:倒计时UI
                case 425:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new StartCountdown());
                            Dispatcher.Instance().Send(new StartChecking());
                        }
                    }

                    break;

                //不断检查抢跑
                case 423:
                case 421:
                    Dispatcher.Instance().Send(new StartChecking());
                    break;
                
                //开始 Coin+400
                case 420:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        playing = true;
                        // 初始金币见 CoinStore.cs
                        // 不在此处设置初始金币
                        Dispatcher.Instance().Send(new CoinNaturalGrowth
                        {
                            MinuteCoin = 400,
                            NotNature = true
                        });
                        Dispatcher.Instance().Send(new PowerRuneAvailable
                        {
                            Available = false,
                            IsLarge = false
                        });
                        Dispatcher.Instance().Send(new AgreeDroneSupport
                        {
                            Agree = false,
                            Timestart = true,
                            StopTime = NetworkTime.time
                        });
                        // 前哨站中部装甲开始旋转，旋转 5 秒内达到 0.4r/s 的速度，方向随机。 
                        Dispatcher.Instance().Send(new ActivateOutpost
                        {
                            Activate = true
                        });
                        Dispatcher.Instance().Send(new EngineerBuff()
                        {
                            canGet =true
                        });
                        Dispatcher.Instance().Send(new StartDroneCount()
                        {
                        });
                    }

                    break;
                
                // 第一波矿石释放前的提示
                case 415:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new DroneWarning
                            {
                                warningType=DroneWarning.WarningType.OreFall
                            });
                        }
                    }

                    break;

                //15s：第一次落矿，提前两秒（应该1.5s的），3号
                case 407:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        Dispatcher.Instance().Send(new DropOre
                        {
                            Index = 2
                        });
                    }

                    break;

                // 能量机关前的提示
                case 380:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new DroneWarning
                            {
                                warningType=DroneWarning.WarningType.PowerRune
                            });
                        }
                    }
                    
                    break;
                
                //1min提前2s，第二批矿石掉落，1号和5号
                case 362:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        Dispatcher.Instance().Send(new DropOre
                        {
                            Index = 0
                        });
                        Dispatcher.Instance().Send(new DropOre
                        {
                            Index = 4
                        });
                    }

                    break;

                // 1 min：Coin+100,小能量机关开始
                case 360:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        Dispatcher.Instance().Send(new CoinNaturalGrowth
                        {
                            MinuteCoin = 50
                        });
                        Dispatcher.Instance().Send(new PowerRuneAvailable
                        {
                            Available = true,
                            IsLarge = false
                        });
                        powerAvailable = true;
                    }

                    break;
                
                // RMUL 倒计时
                case 305:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUL2022)
                        {
                            Dispatcher.Instance().Send(new StartCountdown());
                        }
                    }
                    break;

                // 2min：
                // RMUC: Coin+100
                // RMUL: 初始金币 200, 60秒后激活中心增益点
                case 300:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        switch (_currentMap)
                        {
                            case MapType.RMUC2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 50
                                });
                                break;
                            case MapType.RMUL2022:
                                playing = true;
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 200
                                });
                                Dispatcher.Instance().Send(new CentralBuffActivated());
                                break;
                        }
                    }

                    break;
                
                //2分30秒 小能量机关
                case 270:
                    Dispatcher.Instance().Send(new PowerRuneAvailable
                    {
                        Available = true,
                        IsLarge = false
                    });
                    break;
                
                // 第二波矿石释放前的提示
                case 250:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new DroneWarning
                            {
                                warningType=DroneWarning.WarningType.OreFall
                            });
                        }
                    }

                    break;

                // 第三波矿石释放，2号和4号
                case 242:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new DropOre
                            {
                                Index = 1
                            });
                            Dispatcher.Instance().Send(new DropOre
                            {
                                Index = 3
                            });
                        }
                    }

                    break;

                // 3min：
                // RMUC: Coin+100 小能量机关不可激活
                // RMUL: Coin+200
                // RMUC: 前哨站未被击毁也停止转动
                case 240:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        switch (_currentMap)
                        {
                            case MapType.RMUC2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 50
                                });
                                powerAvailable = false;
                                Dispatcher.Instance().Send(new PowerRuneAvailable
                                {
                                    Available = false,
                                    IsLarge = false
                                });
                                Dispatcher.Instance().Send(new ActivateOutpost
                                {
                                    Activate = false
                                });
                                Dispatcher.Instance().Send(new EngineerBuff()
                                {
                                    canGet = false
                                });
                                break;
                            case MapType.RMUL2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 200
                                });
                                break;
                        }
                    }

                    break;

                
                // 能量机关的提示
                case 200:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new DroneWarning
                            {
                                warningType=DroneWarning.WarningType.PowerRune
                            });
                        }
                    }
                    
                    break;

                // 4min：Coin+100 大能量机关开始
                // RMUL: Coin+200
                case 180:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.RMUC2022)
                        {
                            Dispatcher.Instance().Send(new CoinNaturalGrowth
                            {
                                MinuteCoin = 50
                            });
                            Dispatcher.Instance().Send(new PowerRuneAvailable
                            {
                                Available = true,
                                IsLarge = true
                            });
                            Dispatcher.Instance().Send(new PartyTime());
                            powerAvailable = true;
                        }

                        if (_currentMap == MapType.RMUL2022)
                        {
                            Dispatcher.Instance().Send(new CoinNaturalGrowth
                            {
                                MinuteCoin = 200
                            });
                        }
                    }

                    break;

                // 5min:
                //RMUC:Coin+50
                // RMUL: Coin+300
                case 120:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        switch (_currentMap)
                        {
                            case MapType.RMUC2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                            {
                                MinuteCoin = 50
                            });
                                break;
                            case MapType.RMUL2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 300
                                });
                                break;
                            case MapType.DeDust2:
                                Dispatcher.Instance().Send(new StartCountdown());
                                break;
                        }
                    }

                    break;
                
                // De_Dust2: 开始
                case 115:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        if (_currentMap == MapType.DeDust2)
                        {
                            playing = true;
                            Dispatcher.Instance().Send(new CoinNaturalGrowth
                            {
                                MinuteCoin = 4750
                            });
                        }
                    }

                    break;

                //5min15
                case 105:
                    Dispatcher.Instance().Send(new PowerRuneAvailable
                    {
                        Available = true,
                        IsLarge = true
                    });
                    break;
                
                // 6min：
                // RMUC: Coin+200
                // RMUL: COin+300
                case 60:
                    if (!_happened.Contains(time))
                    {
                        _happened.Add(time);
                        switch (_currentMap)
                        {
                            case MapType.RMUC2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 150
                                });
                                break;
                            case MapType.RMUL2022:
                                Dispatcher.Instance().Send(new CoinNaturalGrowth
                                {
                                    MinuteCoin = 300
                                });
                                break;
                        }
                    }

                    break;

                //6min30
                case 30:
                    Dispatcher.Instance().Send(new PowerRuneAvailable
                    {
                        Available = true,
                        IsLarge = true
                    });
                    break;

                // 比赛结束
                case 0:
                    if (playing)
                    {
                        if (!_happened.Contains(time))
                        {
                            _happened.Add(time);
                            if (finished) break;
                            playing = false;
                            finished = true;
                        }
                    }

                    break;
            }
        }
    }
}