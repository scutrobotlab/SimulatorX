using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gameplay.Attribute;
using Infrastructure;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;
// ReSharper disable once RedundantUsingDirective
using IgnoranceTransport;
using Misc;
using UI;
#if UNITY_EDITOR
using ParrelSync;
#endif
using GlobalConfig = Config.GlobalConfig;
using Version = Config.Version;

namespace Gameplay.Networking
{
    /// <summary>
    /// <c>NetworkManager</c> 实例，
    /// 用于管理场景切换、机器人和玩家生成等。
    /// </summary>
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once ConvertToConstant.Local

        public MapType currentMap = MapType.RMUC2022;

        // 将待选择的预制件先拖入，根据用户的选择等待被选取
        public GameObject engineerPrefab;
        public GameObject[] infantryPrefabs;
        public GameObject heroPrefab;
        public GameObject dronePrefab;
        public GameObject judgePrefab;
        public GameObject spectatorPrefab;
        public GameObject droneManipulatorPrefab;
        public GameObject autoSentinelPrefab;
        public GameObject[] balanceInfantryPrefabs;

        // 本地存储用户名
        [HideInInspector] public string nickname;

        // 服务器信息
        public string serverName;
        public string serverToken;

        // 队伍信息
        public string redTeam;
        public string blueTeam;

        // 是否在选择角色
        private bool _atLobby = true;

        // 客户端信息
        private bool _isFullscreen;

        // 是否回放模式
        private bool _isReplay;
        private string _replayFile;
        private string _replayStoragePath;
        private bool _replayWaitingForUpload;
        private string _serverEntry;
        private int _serverPort;
        private int _width;

        public override void Awake()
        {
            if (Environment.GetCommandLineArgs().Skip(1).FirstOrDefault() == "-v")
            {
                Console.WriteLine(Version.VersionName);
                Application.Quit();
            }

            base.Awake();
        }

        /// <summary>
        /// 解析命令行参数。
        /// 服务端以特定端口、密码、名称启动。
        /// 客户端选择是否全屏。
        /// </summary>
        public override void Start()
        {
#if UNITY_SERVER
            var args = Environment.GetCommandLineArgs();
#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                args = ("SimulatorX.exe " + ClonesManager.GetArgument()).Split(' ');
            }
            else
            {
                args = "SimulatorX.exe -n LocalHost -e 127.0.0.1 -p 5335 -P 111".Split(' ');
            }
#endif
            // 以不同端口启动
            var scanner = 1;
            var arg = "";
            while (scanner < args.Length)
            {
                if (scanner % 2 == 0)
                {
                    var value = args[scanner];
                    //port 为端口号
                    if (arg == "-p" || arg == "--port")
                    {
                        if (int.TryParse(value, out var port))
                        {
                            GetComponent<Ignorance>().port = port;
                        }
                    }

                    if (arg == "-n" || arg == "--name")
                    {
                        serverName = value;
                    }

                    // entry 是ip地址或者域名
                    if (arg == "-e" || arg == "--entry")
                    {
                        _serverEntry = value;
                    }

                    if (arg == "-m" || arg == "--map")
                    {
                        var mapType = value switch
                        {
                            "RMUC2022" => MapType.RMUC2022,
                            "RMUL2022" => MapType.RMUL2022,
                            _ => MapType.RMUC2022
                        };
                        SetCurrentMap(mapType);
                    }
                }
                else arg = args[scanner];

                scanner++;
            }

            _serverPort = GetComponent<Ignorance>().port;
#else
            var clientArgs = Environment.GetCommandLineArgs();
            var clientScanner = 1;
            var clientArg = "";
            while (clientScanner < clientArgs.Length)
            {
                if (clientScanner % 2 == 0)
                {
                    var value = clientArgs[clientScanner];

                    if (clientArg == "-f" || clientArg == "--fullscreen")
                    {
                        _isFullscreen = value == "1";
                    }

                    if (clientArg == "-w" || clientArg == "--width")
                    {
                        int.TryParse(value, out _width);
                    }

                    if (clientArg == "-n" || clientArg == "--nickname")
                    {
                        nickname = value;
                    }
                }
                else clientArg = clientArgs[clientScanner];

                clientScanner++;
            }

            UpdateFullScreenMode();
#endif
            serverToken = GenerateToken();

            base.Start();
        }

        /// <summary>
        /// 更新离线、在线场景选择。
        /// </summary>
        private void FixedUpdate()
        {
            if (currentMap == MapType.None) return;
            var mapInfo = MapInfoManager.Instance().MapInfo(currentMap);
            onlineScene = mapInfo.roomScene;
            RoomScene = mapInfo.roomScene;
            GameplayScene = mapInfo.arenaScene;
        }

        /// <summary>
        /// 防止绘制默认GUI。
        /// </summary>
        public override void OnGUI()
        {
        }

        private void UpdateFullScreenMode()
        {
            if (_isFullscreen)
                Screen.SetResolution(
                    Screen.currentResolution.width,
                    Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
            else
            {
                if (_width == 0)
                {
                    _width = (int)(Screen.currentResolution.width * 0.83f);
                }

                var height = _width / 16 * 9;
                Screen.SetResolution(_width, height, FullScreenMode.Windowed);
            }
        }

        public void OnFullscreen()
        {
            _isFullscreen = true;
            UpdateFullScreenMode();
        }

        public void OnExitFullscreen()
        {
            _isFullscreen = false;
            UpdateFullScreenMode();
        }

        public void LoadRecord(string file)
        {
            MatchRecord matchRecord;

            try
            {
                var compressedRecord = File.ReadAllBytes(file);
                var recordJson = GZipCompress.DecompressToString(compressedRecord);
                matchRecord = JsonUtility.FromJson<MatchRecord>(recordJson);
            }
            catch (Exception e)
            {
                Debug.Log("Fail to load record file: " + e.Message);
                return;
            }

            SetCurrentMap(matchRecord.mapType);
            redTeam = matchRecord.redTeamName;
            blueTeam = matchRecord.blueTeamName;
            _isReplay = true;
            _replayFile = file;
            StartHost();
        }

        public void SetCurrentMap(MapType map)
        {
            currentMap = map;
            if (currentMap == MapType.None) return;
            var mapInfo = MapInfoManager.Instance().MapInfo(currentMap);
            onlineScene = mapInfo.roomScene;

            RoomScene = mapInfo.roomScene;
            GameplayScene = mapInfo.arenaScene;
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
        {
            if (_isReplay) StartCoroutine(DelayStartReplay());
            return base.OnRoomServerCreateRoomPlayer(conn);
        }

        private IEnumerator DelayStartReplay()
        {
            yield return new WaitForSeconds(0.5f);
            var localPlayer = FindObjectOfType<NetworkRoomPlayerExt>();
            if (localPlayer != null)
            {
                localPlayer.chosenRobot = new Identity(Identity.Camps.Other, Identity.Roles.Spectator);
                localPlayer.CmdChangeReadyState(true);
                StartGame();
            }
            else
            {
                // TODO: 回主界面
                throw new Exception("Fail to start replay");
            }
        }

        /// <summary>
        /// 有客户端断开时进行相关处理。
        /// 如果客户端全部断开则重置服务器。
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (_atLobby)
            {
                ClientManager.Instance().OnServerDisconnect(conn);
            }

            if (NetworkServer.connections.Count == 0)
            {
                OnServerReset();
            }

            base.OnServerDisconnect(conn);
        }

        /// <summary>
        /// 重置服务器。
        /// </summary>
        private void OnServerReset()
        {
            nickname = "";
            serverToken = GenerateToken();
            _isReplay = false;
        }

        /// <summary>
        /// 通知 <c>EntityManager</c> 即将生成的机器人列表。
        /// </summary>
        /// <param name="sceneName"></param>
        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);
            var mapInfo = MapInfoManager.Instance().MapInfo(currentMap);

            _atLobby = sceneName == mapInfo.roomScene;
            if (_atLobby)
            {
                if (!_isReplay)
                {
                    _replayStoragePath = "v" + Version.VersionName;
                }
            }

            if (sceneName == mapInfo.arenaScene)
            {
                var roomPlayers = FindObjectsOfType<NetworkRoomPlayerExt>();
                var waitList = roomPlayers.Select(roomPlayer => roomPlayer.chosenRobot).ToList();
                // TODO: 都生成无人机
                /*foreach (var chosenRobot in waitList.FindAll(r=>r.role == Identity.Roles.DroneManipulator))
                {
                    waitList.Add(new Identity(chosenRobot.camp, Identity.Roles.Drone));
                }*/

                //添加哨兵

                foreach (var player in roomPlayers)
                {
                    if (player.chosenRobot.role == Identity.Roles.Drone)
                    {
                        waitList.Add(new Identity(player.chosenRobot.camp, Identity.Roles.AutoSentinel, 0, 7));
                        ;
                    }
                }

                EntityManager.Instance().Configure(currentMap, waitList);
                if (_isReplay)
                {
                    Dispatcher.Instance().replay = true;
                    Dispatcher.Instance().replayFile = _replayFile;
                }
                else
                {
                    Debug.Log(serverName + serverName.Length);
                    _replayStoragePath += "_" + (serverName.Length == 0 ? "Server" : serverName);
                    _replayStoragePath += "_" + (_serverPort == 0 ? 5333 : _serverPort);
                    _replayStoragePath += "_" + DateTime.Now.ToString("yy-MM-dd");
                    _replayStoragePath += "_" + DateTime.Now.ToString("HH-mm-ss");
                    _replayStoragePath += ".rec";
                    if (Directory.Exists("../record"))
                    {
                        _replayStoragePath = "../record/" + _replayStoragePath;
                    }

                    Dispatcher.Instance().replayFile = _replayStoragePath;
                }
            }
        }

        /// <summary>
        /// 生成玩家选择的机器人，在机器人生成后再生成相应的playerStore进行控制。
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="roomPlayer"></param>
        /// <returns>return的对象将会替代room-player。</returns>
        [Server]
        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
        {
            var robot = roomPlayer.GetComponent<NetworkRoomPlayerExt>().chosenRobot;

            if (robot.IsRobot())
            {
                Dispatcher.Instance().Send(new Events.Child.RobotSpawn
                {
                    Robot = robot
                });
            }

            var robotInstance = SpawnRobot(robot, conn);
            // 当机器人成功生成后，再生成相应的playerStore进行控制
            var playerInstance = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            playerInstance.GetComponent<PlayerStore>().SetControlledRobot(robotInstance);

            return playerInstance;
        }

        [Server]
        public GameObject SpawnRobot(Identity robot, NetworkConnection conn)
        {
            if (robot.role == Identity.Roles.Drone)
            {
                SpawnRobot(new Identity(robot.camp, Identity.Roles.AutoSentinel, 0, 7), new LocalConnectionToServer());
            }

            //首先生成机器人
            var robotPrefab = robot.role switch
            {
                Identity.Roles.AutoSentinel => autoSentinelPrefab,
                Identity.Roles.Hero => heroPrefab,
                Identity.Roles.Engineer => engineerPrefab,
                Identity.Roles.Judge => judgePrefab,
                Identity.Roles.Spectator => spectatorPrefab,
                Identity.Roles.Drone => dronePrefab,
                Identity.Roles.DroneManipulator => droneManipulatorPrefab,
                Identity.Roles.Infantry => infantryPrefabs[robot.serial % infantryPrefabs.Length],
                Identity.Roles.BalanceInfantry => balanceInfantryPrefabs[robot.serial % balanceInfantryPrefabs.Length],
                _ => null
            };

            // 生成机器人实例
            var spawnSpot = SpawnManager.Instance().LoadLocation(robot);
            /*if (robot.role == Identity.Roles.DroneManipulator)
            {
                var drone = new Identity{camp = robot.camp,role = Identity.Roles.Drone};
                var droneSpot = SpawnManager.Instance().LoadLocation(drone);
                var droneInstance = Instantiate(dronePrefab, droneSpot.position, droneSpot.rotation);
                var droneStore = droneInstance.GetComponent<RobotStoreBase>();
                NetworkServer.Spawn(droneInstance);
                droneStore.Identify(drone);

            }*/
            var robotInstance = Instantiate(
                robotPrefab,
                spawnSpot.position,
                spawnSpot.rotation);
            NetworkServer.Spawn(robotInstance);
            // 初始化 ID
            var robotStore = robotInstance.GetComponent<RobotStoreBase>();
            robotStore.Identify(robot);

            // 初始化装甲板
            char armorText;
            try
            {
                armorText = char.Parse(robot.order.ToString());
            }
            catch (Exception e)
            {
                armorText = ' ';
            }
            robotStore.SetVisual(armorText);
            // 初始化基本信息
            robotStore.level = 1;
            robotStore.health = AttributeManager.Instance().RobotAttributes(robotStore).MaxHealth;
            robotStore.revivalProcessRequired = robot.role switch
            {
                Identity.Roles.Infantry => 10,
                Identity.Roles.Hero => 20,
                Identity.Roles.Engineer => 20,
                Identity.Roles.BalanceInfantry => 10,
                _ => 100000
            };
            // 确认已生成
            EntityManager.Instance().RegisterRobotSpawn(robotStore, conn);
            return robotInstance;
        }

        /// <summary>
        /// 启动游戏。
        /// </summary>
        public void StartGame()
        {
            ServerChangeScene(GameplayScene);
        }

        /// <summary>
        /// 当所有玩家准备就绪。
        /// </summary>
        public override void OnRoomServerPlayersReady()
        {
        }

        /// <summary>
        /// 随机生成口令。
        /// </summary>
        /// <returns></returns>
        private static string GenerateToken()
        {
            var newToken = "";
            var random = new System.Random();
            for (var i = 0; i < 6; i++)
            {
                newToken += Convert.ToChar('A' + random.Next(0, 25)).ToString();
            }

            return newToken;
        }
    }
}