using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay.Attribute;
using Gameplay.Effects;
using Gameplay.Events;
using Honeti;
using Infrastructure;
using Mirror;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ejected = Gameplay.Events.Ejected;

namespace Gameplay
{
    public class RobotReference
    {
        public NetworkConnection Conn;
        public RobotStoreBase Robot;
        public NetworkConnection SecondaryConn;
    }

    [Serializable]
    public class KillRecord
    {
        public Identity killer;
        public Identity victim;
    }

    /// <summary>
    /// 持有所有赛制中实体的特殊单例。
    /// 获取场上所有机器人、建筑物控制器实例。
    /// 记录每个客户端所控制的机器人。
    /// 确认赛场初始化完毕（每个客户端都有自己的机器人）。
    /// </summary>
    public class EntityManager : StoreBase
    {
        private static EntityManager _instance;

        // 初始化情况
        [SyncVar] public bool initialized;
        private readonly List<StoreBase> _blueEnter = new List<StoreBase>();

        // 已完成确认的 ID 集合
        private readonly HashSet<Identity> _confirmSet = new HashSet<Identity>();

        // 已进入服务器的 ID 集合
        private readonly HashSet<Identity> _enterSet = new HashSet<Identity>();

        //中央增益区双方队列
        private readonly List<StoreBase> _redEnter = new List<StoreBase>();

        // 服务端客户端共用
        // 实体引用表
        /// <summary>
        /// 一个存储所有机器人和建筑id的字典。
        /// </summary>
        private readonly Dictionary<Identity, StoreBase> _references = new Dictionary<Identity, StoreBase>();

        // 各机器人对应 NetworkConnection
        private readonly Dictionary<Identity, RobotReference> _robots = new Dictionary<Identity, RobotReference>();

        // 会进入服务器的 ID 集合
        private readonly HashSet<Identity> _waitSet = new HashSet<Identity>();

        // 伤害记录
        public readonly SyncDictionary<Identity.Camps, float> DamageRecords =
            new SyncDictionary<Identity.Camps, float>();

        // 击杀记录
        public readonly SyncList<KillRecord> KillRecords = new SyncList<KillRecord>();

        // 受击记录    机器人受攻击时间点的记录，用于判断工程\哨兵的自动回血
        public readonly SyncDictionary<Identity, float> lastAttackTime =
            new SyncDictionary<Identity, float>();

        private int _confirmGetReferences;

        // 服务端
        private int _confirmSpawned;

        private MapType _currentMap = MapType.None;

        //增益区是否有被占领
        private bool _isOccupied;

        // 客户端
        private PlayerStore _localPlayer;

        private int _rewardTimes = 1;
        private bool _startGetReferences;

        private bool _waitingForReplaySpawn = true;

        //小能量机关转化经验
        private float exp;
        private bool large2BaseCoolTime = false;

        /*大能量机关先激活的一方时间
        private double firstTime;
        大能量机关先激活的一方的增益
        private EffectBase _firstBuff;
        private EffectBase _secondBuff;*/

        /// <summary>
        /// 初始化记录值。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!isServer) return;
            DamageRecords[Identity.Camps.Blue] = 0;
            DamageRecords[Identity.Camps.Red] = 0;
            if (Dispatcher.Instance().replay)
                StartCoroutine(WaitForReplaySpawn());
        }

        /// <summary>
        /// 检查是否已生成所有机器人。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;
            if (!initialized)
            {
                // Replay 则仍需等待
                if (_currentMap == MapType.None) return;
                if (!Dispatcher.Instance().replay)
                    if (_waitSet.Count == 0 || !_waitSet.SetEquals(_enterSet))
                        return;
                // 所有机器人已在服务端生成
                if (_confirmSpawned < NetworkServer.connections.Count) return;
                if (Dispatcher.Instance().replay && _waitingForReplaySpawn) return;
                // 所有客户端已确认本地机器人
                if (!_startGetReferences)
                {
                    UpdateLoading();
                    if (!isClient) EntityReferences();
                    if (!Dispatcher.Instance().replay)
                    {
                        EntityReferencesRpc(_currentMap.ToString(), _waitSet.Count);
                    }
                    else
                    {
                        var count = FindObjectsOfType<RobotStoreBase>().Length - 2;
                        EntityReferencesRpc(_currentMap.ToString(), count);
                    }

                    _startGetReferences = true;
                }
                else
                {
                    // 等待所有客户端完成引用获取
                    if (_confirmGetReferences < NetworkServer.connections.Count) return;
                    // 所有客户端已完成引用获取
                    Debug.Log("Initialized");
                    initialized = true;
                    // TODO: 超时，同下
                }
            }
            else
            {
                // TODO: UC结算
                if (_currentMap == MapType.RMUL2022)
                {
                    if (ClockStore.Instance().finished && ClockStore.Instance().countDown == 0)
                    {
                        var redBase = (BaseStore)Ref(new Identity(Identity.Camps.Red, Identity.Roles.Base));
                        var blueBase = (BaseStore)Ref(new Identity(Identity.Camps.Blue, Identity.Roles.Base));
                        if (Mathf.Abs(redBase.health - blueBase.health) > 1e-2)
                        {
                            Dispatcher.Instance().Send(new GameOver
                            {
                                WinningCamp = redBase.health > blueBase.health
                                    ? Identity.Camps.Red
                                    : Identity.Camps.Blue,
                                Description = I18N.instance.getValue("^judged_by_base")
                            });
                        }
                        else
                        {
                            var redSentinel =
                                (RobotStoreBase)Ref(
                                    new Identity(Identity.Camps.Red, Identity.Roles.Sentinel, order: 7));
                            var blueSentinel =
                                (RobotStoreBase)Ref(
                                    new Identity(Identity.Camps.Blue, Identity.Roles.Sentinel, order: 7));
                            if (Mathf.Abs(redSentinel.health - blueSentinel.health) > 1e-2)
                            {
                                Dispatcher.Instance().Send(new GameOver
                                {
                                    WinningCamp = redSentinel.health > blueSentinel.health
                                        ? Identity.Camps.Red
                                        : Identity.Camps.Blue,
                                    Description = I18N.instance.getValue("^judged_by_sentry")
                                });
                            }
                            else
                            {
                                if (Mathf.Abs(DamageRecords[Identity.Camps.Blue] -
                                              DamageRecords[Identity.Camps.Red]) > 1e-2)
                                {
                                    Dispatcher.Instance().Send(new GameOver
                                    {
                                        WinningCamp =
                                            DamageRecords[Identity.Camps.Red] > DamageRecords[Identity.Camps.Blue]
                                                ? Identity.Camps.Red
                                                : Identity.Camps.Blue,
                                        Description = I18N.instance.getValue("^judged_by_total_damage")
                                    });
                                }
                                else
                                {
                                    Dispatcher.Instance().Send(new GameOver
                                    {
                                        WinningCamp = Identity.Camps.Other,
                                        Description = I18N.instance.getValue("^tie")
                                    });
                                }
                            }
                        }
                    }
                }

                if (_currentMap == MapType.RMUC2022)
                {
                    if (ClockStore.Instance().finished && ClockStore.Instance().countDown == 0)
                    {
                        var redBase = (BaseStore)Ref(new Identity(Identity.Camps.Red, Identity.Roles.Base));
                        var blueBase = (BaseStore)Ref(new Identity(Identity.Camps.Blue, Identity.Roles.Base));
                        if (Mathf.Abs(redBase.health - blueBase.health) > 1e-2)
                        {
                            Dispatcher.Instance().Send(new GameOver
                            {
                                WinningCamp = redBase.health > blueBase.health
                                    ? Identity.Camps.Red
                                    : Identity.Camps.Blue,
                                Description = I18N.instance.getValue("^judged_by_base")
                            });
                        }
                        else
                        {
                            var redOutpost =
                                (OutpostStore)Ref(
                                    new Identity(Identity.Camps.Red, Identity.Roles.Outpost));
                            var blueOutpost =
                                (OutpostStore)Ref(
                                    new Identity(Identity.Camps.Blue, Identity.Roles.Outpost));
                            if (Mathf.Abs(redOutpost.health - blueOutpost.health) > 1e-2)
                            {
                                Dispatcher.Instance().Send(new GameOver
                                {
                                    WinningCamp = redOutpost.health > blueOutpost.health
                                        ? Identity.Camps.Red
                                        : Identity.Camps.Blue,
                                    Description = I18N.instance.getValue("^judged_by_outpost")
                                });
                            }
                            else
                            {
                                RobotStoreBase redSentinel = null;
                                RobotStoreBase blueSentinel = null;
                                foreach (var robot in RobotRef().Where(robot =>
                                             robot.id.role == Identity.Roles.AutoSentinel))
                                {
                                    switch (robot.id.camp)
                                    {
                                        case Identity.Camps.Red:
                                            redSentinel = (RobotStoreBase)robot;
                                            break;
                                        case Identity.Camps.Blue:
                                            blueSentinel = (RobotStoreBase)robot;
                                            break;
                                    }
                                }

                                if (redSentinel != null && blueSentinel != null
                                                        && Mathf.Abs(redSentinel.health - blueSentinel.health) >
                                                        1e-2)
                                {
                                    Dispatcher.Instance().Send(new GameOver
                                    {
                                        WinningCamp = redSentinel.health > blueSentinel.health
                                            ? Identity.Camps.Red
                                            : Identity.Camps.Blue,
                                        Description = I18N.instance.getValue("^judged_by_sentry")
                                    });
                                }
                                else if (redSentinel == null && blueSentinel != null
                                         || blueSentinel == null && redSentinel != null)
                                {
                                    Dispatcher.Instance().Send(new GameOver
                                    {
                                        WinningCamp = blueSentinel == null
                                            ? Identity.Camps.Red
                                            : Identity.Camps.Blue,
                                        Description = I18N.instance.getValue("^judged_by_sentry")
                                    });
                                }

                                else
                                {
                                    if (Mathf.Abs(DamageRecords[Identity.Camps.Blue] -
                                                  DamageRecords[Identity.Camps.Red]) > 1e-2)
                                    {
                                        Dispatcher.Instance().Send(new GameOver
                                        {
                                            WinningCamp =
                                                DamageRecords[Identity.Camps.Red] >
                                                DamageRecords[Identity.Camps.Blue]
                                                    ? Identity.Camps.Red
                                                    : Identity.Camps.Blue,
                                            Description = I18N.instance.getValue("^judged_by_total_damage")
                                        });
                                    }
                                    else
                                    {
                                        Dispatcher.Instance().Send(new GameOver
                                        {
                                            WinningCamp = Identity.Camps.Other,
                                            Description = I18N.instance.getValue("^tie")
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 网络单例
        public static EntityManager Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EntityManager>();
            }

            return _instance;
        }

        public float GetLastAttackTime(Identity id)
        {
            if (lastAttackTime.ContainsKey(id))
                return lastAttackTime[id];
            else
            {
                lastAttackTime[id] = (float)NetworkTime.time;
            }

            return lastAttackTime[id];
        }

        private IEnumerator WaitForReplaySpawn()
        {
            yield return new WaitForSeconds(1);
            _waitingForReplaySpawn = false;
        }

        /// <summary>
        /// 确认地图类型和将会生成的机器人列表。
        /// 由 NetworkRoomManagerExt 调用。
        /// </summary>
        /// <param name="currentMap">当前比赛地图类型</param>
        /// <param name="waitList">ID 列表</param>
        public void Configure(MapType currentMap, List<Identity> waitList)
        {
            _currentMap = currentMap;
            foreach (var identity in waitList)
            {
                _waitSet.Add(identity);
            }
        }

        public MapType CurrentMap() => _currentMap;

        /// <summary>
        /// 用于确认本地操控的机器人。
        /// 由 <c>PlayerStore</c> 获得机器人引用后调用。
        /// </summary>
        /// <param name="player">本地玩家</param>
        /// <param name="robot">本地机器人</param>
        [Client]
        public void RegisterLocal(PlayerStore player, RobotStoreBase robot)
        {
            _localPlayer = player;
            CmdConfirmSpawned();

            if (player.id.role == Identity.Roles.Drone)
            {
                CmdConfirmSpawned();
            }
        }

        /// <summary>
        /// 用于获得本地操控的机器人 ID。
        /// 用于 UI 等。
        /// </summary>
        /// <returns>本地控制机器人 ID</returns>
        [Client]
        public Identity LocalRobot()
        {
            return _localPlayer.localRobot == null ? new Identity() : _localPlayer.localRobot.id;
        }

        [Client]
        public PlayerStore LocalPlayer()
        {
            return _localPlayer;
        }

        // TODO: 似乎可以写外挂
        // 只用于在比赛时确认全部同步生成
        // 不能用于获得引用，因为重放也需要能正常获得
        [Command(requiresAuthority = false)]
        private void CmdConfirmSpawned()
        {
            _confirmSpawned++;
        }

        /// <summary>
        /// 生成机器人实体时进行确认。
        /// 由 NetworkRoomManagerExt 调用。
        /// </summary>
        /// <param name="robot">机器人</param>
        /// <param name="conn">对应网络连接</param>
        /// <exception cref="Exception"></exception>
        [Server]
        public void RegisterRobotSpawn(RobotStoreBase robot, NetworkConnection conn)
        {
            _enterSet.Add(robot.id);
            if (_robots.ContainsKey(robot.id))
            {
                if (robot.id.role != Identity.Roles.Drone)
                {
                    // TODO: 服务端 Exception，炸服挂
                    throw new Exception("Registering more than one connection for a robot other than drone.");
                }

                _robots[robot.id].SecondaryConn = conn;
            }
            else
            {
                _robots[robot.id] = new RobotReference
                {
                    Robot = robot,
                    Conn = conn
                };
            }
        }

        [Server]
        public bool RobotSpawned(Identity robot) => _enterSet.Contains(robot);

        /// <summary>
        /// 根据机器人 ID 获得网络连接。
        /// 用于 TargetRpc。
        /// </summary>
        /// <param name="identity">ID</param>
        /// <param name="index">网络连接序号，默认为 0，无人机飞手为 1</param>
        /// <returns></returns>
        [Server]
        public NetworkConnection ConnectionByIdentity(Identity identity, int index = 0)
        {
            if (!_robots.ContainsKey(identity)) return null;
            return index switch
            {
                0 when _robots[identity].Conn != null => _robots[identity].Conn,
                1 when _robots[identity].SecondaryConn != null => _robots[identity].SecondaryConn,
                _ => null
            };
        }

        [Server]
        private Identity IdentityByConnection(NetworkConnection conn)
        {
            return (from robot in _robots
                where robot.Value.Conn == conn || robot.Value.SecondaryConn == conn
                select robot.Key).FirstOrDefault();
        }

        [Server]
        public RobotStoreBase RobotByConnection(NetworkConnection conn)
        {
            return (RobotStoreBase)Ref(IdentityByConnection(conn));
        }

        /// <summary>
        /// 通知客户端获得实体引用。
        /// </summary>
        /// <param name="currentMap">当前地图类型</param>
        /// <param name="robotAmount">新创建机器人数量</param>
        [ClientRpc]
        private void EntityReferencesRpc(string currentMap, int robotAmount)
        {
            _currentMap = (MapType)Enum.Parse(typeof(MapType), currentMap);
            StartCoroutine(TryGetEntityReferences(robotAmount));
        }

        ///延时时间改变变量
        private IEnumerator BoolDelayChange(float time)
        {
            yield return new WaitForSeconds(time);
            large2BaseCoolTime = !large2BaseCoolTime;
        }


        /// <summary>
        /// 尝试获得所有实体引用。
        /// </summary>
        /// <param name="robotAmount">新创建机器人数量</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IEnumerator TryGetEntityReferences(int robotAmount)
        {
            const int maxTries = 20;
            var tries = 0;

            while (_currentMap == MapType.None)
            {
                yield return new WaitForSeconds(0.2f);
                tries++;
                if (tries == maxTries)
                {
                    throw new Exception("Try to synchronize entities without map type.");
                }
            }

            if (_currentMap == MapType.RMUC2022 || _currentMap == MapType.RMUL2022)
            {
                // 去掉哨兵
                // 等待机器人全部生成
                while (FindObjectsOfType<RobotStoreBase>().Length != robotAmount)
                {
                    yield return new WaitForSeconds(1);
                    tries++;
                    if (tries != maxTries) continue;
                    if (NetworkClient.active)
                        NetworkManager.singleton.StopClient();
                    if (NetworkServer.active)
                        NetworkManager.singleton.StopServer();
                    Debug.LogWarning("由于过久未同步，你已被移出房间。");
                    SceneManager.LoadScene("Offline");
                }

                EntityReferences();
            }

            if (_currentMap == MapType.DeDust2)
            {
                // 去掉哨兵
                // 等待机器人全部生成
                while (FindObjectsOfType<RobotStoreBase>().Length != robotAmount)
                {
                    yield return new WaitForSeconds(1);
                    tries++;
                    if (tries != maxTries) continue;
                    if (NetworkClient.active)
                        NetworkManager.singleton.StopClient();
                    if (NetworkServer.active)
                        NetworkManager.singleton.StopServer();
                    Debug.LogWarning("由于过久未同步，你已被移出房间。");
                    SceneManager.LoadScene("Offline");
                }

                EntityReferences();
            }
        }

        /// <summary>
        /// 获取并遍历场景中的StoreBase和RobotStoreBase，将双方阵营的机器人实体与建筑实体全部存储到_references字典中。
        /// </summary>
        /// <exception cref="Exception">机器人生成重复或缺失。</exception>
        private void EntityReferences()
        {
            if (_currentMap == MapType.RMUC2022)
            {
                // 搜索哨兵，前哨站，基地引用
                var stores = FindObjectsOfType<StoreBase>().ToList();
                foreach (var camp in new List<Identity.Camps>
                         {
                             Identity.Camps.Blue,
                             Identity.Camps.Red
                         })
                {
                    foreach (var role in new List<Identity.Roles>
                             {
                                 // Identity.Roles.Sentinel,
                                 Identity.Roles.Outpost,
                                 Identity.Roles.Base,
                                 Identity.Roles.LaunchingStation
                             })
                    {
                        var identity = new Identity(camp, role);
                        if (_references.ContainsKey(identity)) continue;
                        if (stores.Count(s => s.id == identity) == 1)
                        {
                            _references[identity] = stores.First(s => s.id == identity);
                        }
                        else
                        {
                            throw new Exception("Scene lack of permanent entity: " + identity.Data());
                        }
                    }
                }

                // 添加其他机器人
                var robotStores = FindObjectsOfType<RobotStoreBase>();
                foreach (var robotStore in robotStores)
                {
                    if (!_references.ContainsKey(robotStore.id))
                    {
                        _references[robotStore.id] = robotStore;
                    }
                }

                if (isClient)
                    CmdDoneGetReferences();
            }

            // RMUL2022,实体包括双方哨兵、基地、其他地面机器人。
            if (_currentMap == MapType.RMUL2022)
            {
                var stores = FindObjectsOfType<StoreBase>().ToList();
                foreach (var camp in new List<Identity.Camps>
                         {
                             Identity.Camps.Blue,
                             Identity.Camps.Red
                         })
                {
                    foreach (var role in new List<Identity.Roles>
                             {
                                 //Identity.Roles.Sentinel,
                                 Identity.Roles.Base
                             })
                    {
                        var identity = new Identity(camp, role);
                        if (_references.ContainsKey(identity)) continue;
                        if (stores.Count(s => s.id == identity) == 1)
                        {
                            _references[identity] = stores.First(s => s.id == identity);
                        }
                        else
                        {
                            throw new Exception("Scene lack of permanent entity: " + identity.Data());
                        }
                    }
                }

                // 添加其他机器人
                var robotStores = FindObjectsOfType<RobotStoreBase>();
                foreach (var robotStore in robotStores)
                {
                    if (!_references.ContainsKey(robotStore.id))
                    {
                        _references[robotStore.id] = robotStore;
                    }
                }

                if (isClient)
                    CmdDoneGetReferences();
            }

            // De_Dust2，实体只有机器人。
            if (_currentMap == MapType.DeDust2)
            {
                // 添加其他机器人
                var robotStores = FindObjectsOfType<RobotStoreBase>();
                foreach (var robotStore in robotStores)
                {
                    if (!_references.ContainsKey(robotStore.id))
                    {
                        _references[robotStore.id] = robotStore;
                    }
                }

                if (isClient)
                    CmdDoneGetReferences();
            }
        }

        /// <summary>
        /// 获取完成后通知服务端。
        /// </summary>
        [Command(requiresAuthority = false)]
        private void CmdDoneGetReferences(NetworkConnectionToClient sender = null)
        {
            _confirmGetReferences++;
            var robot = IdentityByConnection(sender);
            if (robot != null)
            {
                _confirmSet.Add(robot);
            }
            else
            {
                Debug.Log("未确认");
                sender?.Disconnect();
            }

            UpdateLoading();
        }

        private void UpdateLoading()
        {
            var notReady = _enterSet.Except(_confirmSet);
            var readyText = "";
            var readyArray = _confirmSet.ToArray();
            for (var i = 0; i < readyArray.Length; i++)
            {
                if (i > 0 && i % 2 == 0) readyText += "\n";
                readyText += readyArray[i].Describe() + "   ";
            }

            var notReadyText = "";
            var notReadyArray = notReady.ToArray();
            for (var i = 0; i < notReadyArray.Length; i++)
            {
                if (i > 0 && i % 2 == 0) notReadyText += "\n";
                notReadyText += notReadyArray[i].Describe() + "   ";
            }

            var panel = FindObjectOfType<LoadingPanel>();
            if (panel != null)
            {
                panel.UpdateListRpc(readyText, notReadyText);
            }
        }

        // 处理全局事件

        /// <summary>
        /// 声明关注事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Armor.ArmorHit,
                ActionID.Stage.Ejected,
                ActionID.Stage.PowerRuneActivated,
                ActionID.Stage.Kill,
                ActionID.ControlAction.SearchControlEffect,
                ActionID.Clock.StartChecking,
                ActionID.Stage.Penalty
            }).ToList();
        }

        /// <summary>
        /// 获取与identity相对应的实体的StoreBase。
        /// </summary>
        /// <param name="identity">待获取实体的identity（包括阵营camp与角色role）</param>
        /// <returns>与identity对应的实体对象</returns>
        /// <exception cref="Exception"></exception>
        public StoreBase Ref(Identity identity)
        {
            if (_references.ContainsKey(identity))
            {
                return _references[identity];
            }

            // TODO: 处理
            throw new Exception("Getting non-exist reference.");
        }

        /// <summary>
        /// 获取与阵营camp相对应的实体StoreBase列表。
        /// </summary>
        /// <param name="camp">阵营</param>
        /// <returns>实体StoreBase列表</returns>
        public List<StoreBase> CampRef(Identity.Camps camp)
        {
            return (
                from reference in _references
                where reference.Key.camp == camp
                select reference.Value).ToList();
        }

        /// <summary>
        /// 获得场上所有机器人
        /// </summary>
        /// <returns></returns>
        public List<StoreBase> RobotRef()
        {
            return (from reference in _references
                where reference.Key.IsRobot()
                select reference.Value).ToList();
        }

        /// <summary>
        /// 处理机器人扣血、建筑扣血、buff增益等事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Armor.ArmorHit:
                    if (!ClockStore.Instance().playing || ClockStore.Instance().finished) return;

                    var hitAction = (ArmorHit)action;

                    lastAttackTime[hitAction.Receiver] = (float)NetworkTime.time;

                    if (hitAction.Receiver.role == Identity.Roles.Energy) break;

                    // 扣血逻辑
                    if (hitAction.Receiver.IsRobot())
                    {
                        // 击中机器人
                        var hitter = Ref(hitAction.Hitter);
                        var receiver = (RobotStoreBase)Ref(hitAction.Receiver);
                        var bulletDamage = hitAction.Caliber switch
                        {
                            MechanicType.CaliberType.Dart => 750,
                            MechanicType.CaliberType.Large => 100,
                            MechanicType.CaliberType.Small => 10,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        var damage = bulletDamage * hitter.CurrentBuff().damage;
                        damage *= 1 - receiver.CurrentBuff().shield;

                        // 前哨站击毁前，哨兵无敌
                        if (_currentMap == MapType.RMUC2022)
                        {
                            if (receiver.id.role == Identity.Roles.AutoSentinel)
                            {
                                var outpostRef =
                                    (OutpostStore)Ref(
                                        new Identity(receiver.id.camp, Identity.Roles.Outpost));
                                if (outpostRef.health > 0) break;
                            }
                        }

                        DamageRecords[hitter.id.camp] += damage;

                        if (receiver.health > 0)
                        {
                            receiver.health -= damage;
                            if (receiver.health <= 0)
                            {
                                receiver.health = 0;
                                // TODO：击毁

                                KillRecords.Add(new KillRecord
                                {
                                    killer = hitter.id,
                                    victim = receiver.id
                                });

                                Dispatcher.Instance().Send(new Kill
                                {
                                    killer = hitter.id,
                                    victim = receiver.id,
                                    method = I18N.instance.getValue("^destroy")
                                });

                                if (_currentMap == MapType.RMUL2022)
                                {
                                    // 基地无敌解除
                                    var baseStore =
                                        (BaseStore)Ref(new Identity(receiver.id.camp, Identity.Roles.Base));
                                    baseStore.invincible = false;
                                    // 发射机构锁定
                                    receiver.gunLocked = true;
                                }

                                if (receiver.id.role == Identity.Roles.AutoSentinel)
                                {
                                    Dispatcher.Instance().Send(new SentinelFall
                                    {
                                        Camp = receiver.id.camp
                                    });
                                }

                                if (hitter is RobotStoreBase hitterRobot)
                                {
                                    if (_currentMap == MapType.RMUC2022)
                                    {
                                        if (!hitterRobot.HasEffect(EffectID.Buffs.SmallPowerRune) || exp >= 100)
                                        {
                                            if (receiver.id.role == Identity.Roles.Hero
                                                || receiver.id.role == Identity.Roles.Infantry ||
                                                receiver.id.role == Identity.Roles.BalanceInfantry)
                                            {
                                                hitterRobot.experience += AttributeManager.Instance()
                                                    .RobotAttributes(receiver).ExperienceValue;
                                            }

                                            //经验均分
                                            else
                                            {
                                                var gainCamp = receiver.id.camp switch
                                                {
                                                    Identity.Camps.Blue => Identity.Camps.Red,
                                                    Identity.Camps.Red => Identity.Camps.Blue,
                                                    _ => Identity.Camps.Other
                                                };
                                                if (gainCamp != Identity.Camps.Other)
                                                {
                                                    var robots = CampRef(gainCamp).FindAll(r =>
                                                        r is RobotStoreBase robotStore &&
                                                        robotStore.id.IsGroundRobot() &&
                                                        robotStore.health > 0);
                                                    var totalExp = AttributeManager.Instance().RobotAttributes(receiver)
                                                        .ExperienceValue;
                                                    var avgExp = totalExp / robots.Count;
                                                    foreach (var robot in robots)
                                                    {
                                                        ((RobotStoreBase)robot).experience += avgExp;
                                                    }
                                                }
                                            }

                                            exp = 0;
                                        }

                                        else
                                        {
                                            var gainCamp = hitter.id.camp switch
                                            {
                                                Identity.Camps.Blue => Identity.Camps.Blue,
                                                Identity.Camps.Red => Identity.Camps.Red,
                                                _ => Identity.Camps.Other
                                            };
                                            if (gainCamp != Identity.Camps.Other)
                                            {
                                                var robots = CampRef(gainCamp).FindAll(r =>
                                                    r is RobotStoreBase robotStore &&
                                                    robotStore.id.IsGroundRobot() &&
                                                    robotStore.health > 0);
                                                exp += damage / 10 / robots.Count;

                                                if (exp > 100)
                                                    exp = 100;

                                                foreach (var robot in robots)
                                                {
                                                    ((RobotStoreBase)robot).experience += exp;
                                                }
                                            }
                                        }
                                    }

                                    // 经验均分
                                    if (_currentMap == MapType.RMUL2022)
                                    {
                                        var gainCamp = receiver.id.camp switch
                                        {
                                            Identity.Camps.Blue => Identity.Camps.Red,
                                            Identity.Camps.Red => Identity.Camps.Blue,
                                            _ => Identity.Camps.Other
                                        };
                                        if (gainCamp != Identity.Camps.Other)
                                        {
                                            var robots = CampRef(gainCamp).FindAll(r =>
                                                r is RobotStoreBase robotStore && robotStore.id.IsGroundRobot() &&
                                                robotStore.health > 0);
                                            var totalExp = AttributeManager.Instance().RobotAttributes(receiver)
                                                .ExperienceValue;
                                            totalExp += KillRecords.Count == 1 ? 5 : 0;
                                            var avgExp = totalExp / robots.Count;
                                            foreach (var robot in robots)
                                            {
                                                ((RobotStoreBase)robot).experience += avgExp;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 击中建筑物
                        int bulletDamage = 0;
                        if (hitAction.Caliber == MechanicType.CaliberType.Large)
                        {
                            // 大弹丸
                            if (hitAction.Receiver.role == Identity.Roles.Base)
                            {
                                // 击中基地
                                if (hitAction.Armor.serial == 0 || hitAction.Armor.serial == 1)
                                {
                                    bulletDamage = _currentMap == MapType.RMUL2022 ? 0 : 200;
                                }
                                // 大弹丸打到基地顶部三角装甲板
                                else if (hitAction.Armor.serial == 10)
                                {
                                    bulletDamage = 300;
                                }
                                else
                                {
                                    bulletDamage = 200;
                                }

                                // 梯形高地狙击伤害
                                // TODO: 10秒防御期
                                if (Ref(hitAction.Hitter).HasEffect(EffectID.Buffs.HighlandCoin) && !large2BaseCoolTime)
                                {
                                    bulletDamage = (int)(2.5 * bulletDamage);
                                    large2BaseCoolTime = true;
                                    StartCoroutine(BoolDelayChange(10.0f));
                                }
                                else if (large2BaseCoolTime)
                                {
                                    bulletDamage = 0;
                                }
                            }
                            else if (hitAction.Receiver.role == Identity.Roles.Outpost)
                            {
                                // 击中前哨站
                                // 顶部三角装甲板为300，其它装甲板为200
                                bulletDamage = hitAction.Armor.serial == 0 ? 300 : 200;
                            }
                        }
                        else if (hitAction.Caliber == MechanicType.CaliberType.Dart)
                        {
                            if (hitAction.Armor.serial == 1 && hitAction.Receiver.role == Identity.Roles.Outpost)
                            {
                                // 前哨站的飞镖检测模块
                                bulletDamage = _currentMap == MapType.RMUL2022 ? 0 : 750;
                            }
                            else if (hitAction.Armor.serial == 3 && hitAction.Receiver.role == Identity.Roles.Base)
                            {
                                // 基地的飞镖检测模块
                                bulletDamage = _currentMap == MapType.RMUL2022 ? 0 : 1000;
                            }
                            else
                            {
                                // 识别成大弹丸
                                bulletDamage = 300;
                            }
                        }
                        else
                        {
                            if (hitAction.Armor.serial == 1 && hitAction.Receiver.role == Identity.Roles.Outpost)
                            {
                                // 前哨站的飞镖检测模块
                                bulletDamage = 0;
                            }
                            else if (hitAction.Armor.serial == 3 && hitAction.Receiver.role == Identity.Roles.Base)
                            {
                                // 基地的飞镖检测模块
                                bulletDamage = 0;
                            }
                            else
                            {
                                // 正常识别成小弹丸
                                bulletDamage = 5;
                            }
                        }

                        var hitter = Ref(hitAction.Hitter);
                        var receiver = Ref(hitAction.Receiver);
                        var damage = bulletDamage * hitter.CurrentBuff().damage;
                        damage *= 1 - receiver.CurrentBuff().shield;

                        DamageRecords[hitter.id.camp] += damage;

                        switch (receiver.id.role)
                        {
                            case Identity.Roles.Outpost:
                                var outpostStore = (OutpostStore)receiver;
                                if (outpostStore.invincible) break;
                                if (outpostStore.health > 0)
                                {
                                    if (outpostStore.ReturnAct() && hitAction.Caliber == MechanicType.CaliberType.Small)
                                        damage = 10;
                                    outpostStore.health -= damage;

                                    //前哨站每500血量金币返还
                                    if (NetworkTime.time < 180f)
                                    {
                                        if (outpostStore.health <= 1000 && _rewardTimes == 1)
                                        {
                                            Dispatcher.Instance().Send(new AskRewardCoin()
                                            {
                                                Camp = outpostStore.id.camp == Identity.Camps.Red
                                                    ? Identity.Camps.Blue
                                                    : Identity.Camps.Red,
                                                Coin = 100
                                            });
                                            _rewardTimes++;
                                        }

                                        if (outpostStore.health <= 500 && _rewardTimes == 2)
                                        {
                                            Dispatcher.Instance().Send(new AskRewardCoin()
                                            {
                                                Camp = outpostStore.id.camp == Identity.Camps.Red
                                                    ? Identity.Camps.Blue
                                                    : Identity.Camps.Red,
                                                Coin = 100
                                            });
                                            _rewardTimes++;
                                        }

                                        if (outpostStore.health <= 0 && _rewardTimes == 3)
                                        {
                                            Dispatcher.Instance().Send(new AskRewardCoin()
                                            {
                                                Camp = outpostStore.id.camp == Identity.Camps.Red
                                                    ? Identity.Camps.Blue
                                                    : Identity.Camps.Red,
                                                Coin = 100
                                            });
                                            _rewardTimes++;
                                        }
                                    }

                                    //飞镖击中前哨站，发送白屏负面buff
                                    if (hitAction.Caliber == MechanicType.CaliberType.Dart)
                                    {
                                        if (receiver.id.camp == Identity.Camps.Blue)
                                        {
                                            //给步兵以及平衡步
                                            for (var i = 0; i < 3; i++)
                                            {
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Blue,
                                                        role = Identity.Roles.Infantry,
                                                        serial = i
                                                    }
                                                });
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Blue,
                                                        role = Identity.Roles.BalanceInfantry,
                                                        serial = i
                                                    }
                                                });
                                            }

                                            //给英雄
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Blue,
                                                    role = Identity.Roles.Hero
                                                }
                                            });
                                            //给工程
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Blue,
                                                    role = Identity.Roles.Engineer
                                                }
                                            });
                                        }
                                        else if (receiver.id.camp == Identity.Camps.Red)
                                        {
                                            //给步兵
                                            for (var i = 0; i < 3; i++)
                                            {
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Red,
                                                        role = Identity.Roles.Infantry,
                                                        serial = i
                                                    }
                                                });
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Red,
                                                        role = Identity.Roles.BalanceInfantry,
                                                        serial = i
                                                    }
                                                });
                                            }

                                            //给英雄
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Red,
                                                    role = Identity.Roles.Hero
                                                }
                                            });
                                            //给工程
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Red,
                                                    role = Identity.Roles.Engineer
                                                }
                                            });
                                        }
                                    }

                                    if (outpostStore.health <= 0)
                                    {
                                        outpostStore.health = 0;
                                        // 击毁前哨站

                                        KillRecords.Add(new KillRecord
                                        {
                                            killer = hitter.id,
                                            victim = receiver.id
                                        });

                                        Dispatcher.Instance().Send(new Kill
                                        {
                                            killer = hitter.id,
                                            method = "摧毁",
                                            victim = receiver.id
                                        });

                                        Dispatcher.Instance().Send(new OutpostFall
                                        {
                                            Camp = receiver.id.camp
                                        });
                                        if (hitter is RobotStoreBase hitterRobot)
                                        {
                                            hitterRobot.experience += 5;
                                        }
                                    }
                                }

                                break;
                            case Identity.Roles.Base:

                                var baseStore = (BaseStore)receiver;
                                if (baseStore.invincible) break;
                                if (baseStore.health > 0)
                                {
                                    if (baseStore.shield > damage)
                                    {
                                        baseStore.shield -= damage;
                                    }
                                    else
                                    {
                                        baseStore.health -= damage - baseStore.shield;
                                        baseStore.shield = 0;
                                    }

                                    //飞镖击中基地，发送白屏负面buff
                                    if (hitAction.Caliber == MechanicType.CaliberType.Dart)
                                    {
                                        if (receiver.id.camp == Identity.Camps.Blue)
                                        {
                                            //给步兵以及平衡步
                                            for (var i = 0; i < 3; i++)
                                            {
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Blue,
                                                        role = Identity.Roles.Infantry,
                                                        serial = i
                                                    }
                                                });
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Blue,
                                                        role = Identity.Roles.BalanceInfantry,
                                                        serial = i,
                                                    }
                                                });
                                            }

                                            //给英雄
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Blue,
                                                    role = Identity.Roles.Hero
                                                }
                                            });
                                            //给工程
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Blue,
                                                    role = Identity.Roles.Engineer
                                                }
                                            });
                                        }
                                        else if (receiver.id.camp == Identity.Camps.Red)
                                        {
                                            //给步兵
                                            for (var i = 0; i < 3; i++)
                                            {
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Red,
                                                        role = Identity.Roles.Infantry,
                                                        serial = i
                                                    }
                                                });
                                                Dispatcher.Instance().Send(new AddEffect()
                                                {
                                                    Effect = new DartAttack(),
                                                    Receiver = new Identity()
                                                    {
                                                        camp = Identity.Camps.Red,
                                                        role = Identity.Roles.BalanceInfantry,
                                                        serial = i
                                                    }
                                                });
                                            }

                                            //给英雄
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Red,
                                                    role = Identity.Roles.Hero
                                                }
                                            });
                                            //给工程
                                            Dispatcher.Instance().Send(new AddEffect()
                                            {
                                                Effect = new DartAttack(),
                                                Receiver = new Identity()
                                                {
                                                    camp = Identity.Camps.Red,
                                                    role = Identity.Roles.Engineer
                                                }
                                            });
                                        }
                                    }

                                    if (baseStore.health <= 0)
                                    {
                                        baseStore.health = 0;
                                        // 基地被击毁，比赛结束
                                        if (!ClockStore.Instance().finished)
                                        {
                                            Dispatcher.Instance().Send(new GameOver
                                            {
                                                WinningCamp = baseStore.id.camp switch
                                                {
                                                    Identity.Camps.Red => Identity.Camps.Blue,
                                                    Identity.Camps.Blue => Identity.Camps.Red,
                                                    _ => Identity.Camps.Other
                                                },
                                                Description = "击毁基地"
                                            });
                                        }
                                    }
                                }

                                break;
                        }
                    }

                    break;

                case ActionID.Stage.Ejected:
                    var ejectedAction = (Ejected)action;

                    // 一血无敌状态解除
                    if (_currentMap == MapType.RMUL2022)
                    {
                        var baseStore = (BaseStore)Ref(
                            new Identity(
                                ejectedAction.target.camp, Identity.Roles.Base));
                        baseStore.invincible = false;
                    }

                    /*
                     // 哨兵死亡基地虚拟护盾解除
                     if (ejectedAction.target.role == Identity.Roles.Infantry && ejectedAction.target.serial == 4)
                     {
                         var baseStore = (BaseStore) Ref(
                             new Identity(
                                 ejectedAction.target.camp, Identity.Roles.Base));
                         baseStore.shield = 0;
                     }
                     */

                    break;

                case ActionID.Stage.PowerRuneActivated:
                    var activatedAction = (PowerRuneActivated)action;
                    EffectBase buff;
                    if (activatedAction.IsLarge)
                    {
                        var score = activatedAction.Score;
                        if (score >= 5 && score <= 15)
                            buff = new OneLargePowerRuneBuff();
                        else if (score > 15 && score <= 25)
                            buff = new TwoLargePowerRuneBuff();
                        else if (score > 25 && score <= 35)
                            buff = new ThreeLargePowerRuneBuff();
                        else if (score > 35 && score <= 40)
                            buff = new FourLargePowerRuneBuff();
                        else if (score > 40 && score <= 45)
                            buff = new FiveLargePowerRuneBuff();
                        else
                            buff = score switch
                            {
                                46 => new SixLargePowerRuneBuff(),
                                47 => new SevenLargePowerRuneBuff(),
                                48 => new EightLargePowerRuneBuff(),
                                49 => new NineLargePowerRuneBuff(),
                                _ => new TenLargePowerRuneBuff()
                            };
                        foreach (var reference in
                                 CampRef(activatedAction.Camp).Where(reference => reference.id.IsRobot()))
                        {
                            Dispatcher.Instance().Send(new AddEffect
                            {
                                Receiver = reference.id,
                                Effect = new LargePowerRuneBuff()
                            });
                            Dispatcher.Instance().Send(new AddEffect
                            {
                                Receiver = reference.id,
                                Effect = buff
                            });
                        }
                    }
                    else
                    {
                        buff = new SmallPowerRuneBuff();

                        foreach (var reference in
                                 CampRef(activatedAction.Camp).Where(reference => reference.id.IsRobot()))
                        {
                            Dispatcher.Instance().Send(new AddEffect
                            {
                                Receiver = reference.id,
                                Effect = buff
                            });
                        }
                    }


                    break;

                case ActionID.Stage.Kill:
                    var killAction = (Kill)action;
                    if (killAction.killer == killAction.victim)
                    {
                        Debug.Log("OK");
                        KillRecords.Add(new KillRecord
                        {
                            killer = killAction.killer,
                            victim = killAction.victim
                        });

                        // 经验均分
                        if (_currentMap == MapType.RMUL2022)
                        {
                            var gainCamp = killAction.victim.camp switch
                            {
                                Identity.Camps.Blue => Identity.Camps.Red,
                                Identity.Camps.Red => Identity.Camps.Blue,
                                _ => Identity.Camps.Other
                            };
                            if (gainCamp != Identity.Camps.Other)
                            {
                                var robots = CampRef(gainCamp).FindAll(r =>
                                    r is RobotStoreBase robotStore && robotStore.id.IsGroundRobot() &&
                                    robotStore.health > 0);
                                var totalExp = AttributeManager.Instance().RobotAttributes(
                                        (RobotStoreBase)Ref(killAction.victim))
                                    .ExperienceValue;
                                totalExp += KillRecords.Count == 1 ? 5 : 0;
                                var avgExp = totalExp / robots.Count;
                                foreach (var robot in robots)
                                {
                                    ((RobotStoreBase)robot).experience += avgExp;
                                }
                            }
                        }
                    }

                    break;

                case ActionID.ControlAction.SearchControlEffect:
                    foreach (var robot in RobotRef())
                    {
                        if (robot.HasEffect(EffectID.Status.ControlBuff))
                        {
                            if (robot.id.camp == Identity.Camps.Red)
                                _redEnter.Add(robot);
                            else _blueEnter.Add(robot);
                        }
                        else
                        {
                            if (_redEnter.Contains(robot))
                                _redEnter.Remove(robot);
                            else if (_blueEnter.Contains(robot))
                                _blueEnter.Remove(robot);
                        }
                    }

                    if (_redEnter.Count == 0 && _blueEnter.Count != 0 && !_isOccupied)
                    {
                        Dispatcher.Instance().Send(new SendOccupiedMessage()
                        {
                            CanCount = true,
                            Camp = Identity.Camps.Blue,
                            Time = (float)NetworkTime.time
                        });
                        //对方没有占领
                        Dispatcher.Instance().Send(new SendLeftMessage()
                        {
                            Camp = Identity.Camps.Red
                        });
                        _isOccupied = true;
                    }
                    else if (_blueEnter.Count == 0 && _redEnter.Count != 0 && !_isOccupied)
                    {
                        Dispatcher.Instance().Send(new SendOccupiedMessage()
                        {
                            CanCount = true,
                            Camp = Identity.Camps.Red,
                            Time = (float)NetworkTime.time
                        });
                        //对方没有占领
                        Dispatcher.Instance().Send(new SendLeftMessage()
                        {
                            Camp = Identity.Camps.Blue
                        });
                        _isOccupied = true;
                    }
                    else if (_redEnter.Count != 0 && _blueEnter.Count != 0)
                        Dispatcher.Instance().Send(new SendOccupiedMessage()
                        {
                            CanCount = false
                        });
                    else if (_redEnter.Count == 0 && _blueEnter.Count == 0)
                    {
                        if (_isOccupied)
                        {
                            Dispatcher.Instance().Send(new SendOccupiedMessage()
                            {
                                CanCount = false
                            });
                            Dispatcher.Instance().Send(new SendLeftMessage()
                            {
                                Camp = Identity.Camps.Red
                            });
                            Dispatcher.Instance().Send(new SendLeftMessage()
                            {
                                Camp = Identity.Camps.Blue
                            });
                            _isOccupied = false;
                        }
                    }

                    break;

                case ActionID.Clock.StartChecking:
                    foreach (var robot in RobotRef().Where(robot =>
                                 !robot.HasEffect(EffectID.Buffs.Base) && !robot.HasEffect(EffectID.Buffs.DroneBase)))
                    {
                        var robotStore = (RobotStoreBase)robot;
                        if (robotStore.health == 0 || robot.id.role == Identity.Roles.AutoSentinel)
                            continue;
                        Dispatcher.Instance().Send(new Ejected
                        {
                            target = robot.id
                        });
                        robotStore.health = 0;
                    }

                    break;

                case ActionID.Stage.Penalty:
                    var PenaltyAction = (Penalty)action;
                    var camp = PenaltyAction.target.camp;
                    var target = (RobotStoreBase)Ref(PenaltyAction.target);
                    target.health -= target.health * 0.15f;
                    foreach (var AllRobots in CampRef(camp).Where(robot => robot is RobotStoreBase robotStore
                                                                           && robotStore.id.IsGroundRobot()
                                                                           && robotStore.id.role !=
                                                                           Identity.Roles.AutoSentinel
                                                                           && robotStore.id != PenaltyAction.target
                                                                           && robotStore.health > 0))
                    {
                        var robot = (RobotStoreBase)AllRobots;
                        robot.health -= robot.health * 0.05f;
                    }

                    break;
            }
        }
    }
}