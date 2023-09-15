using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gameplay;
using Gameplay.Events;
using Gameplay.Events.Child;
using Gameplay.Networking;
using Infrastructure.Child;
using Mirror;
using Misc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Infrastructure
{
    /// <summary>
    /// 帧内发送缓存，用于游戏进行时通过Dispatcher发送指令。
    /// </summary>
    public class SendCache
    {
        public bool IsSendChild;
        public IAction Action;
        public Identity Owner;
        public int frame;
    }

    /// <summary>
    /// 全场发送事件记录，构成回放时的json文件。
    /// </summary>
    [Serializable]
    public class SendRecord
    {
        public int frame;
        public bool isSendChild;
        public string action;
        public string owner;
    }

    /// <summary>
    /// 记录包装器。
    /// </summary>
    [Serializable]
    public class MatchRecord
    {
        public MapType mapType;
        public string redTeamName;
        public string blueTeamName;
        public SendRecord[] sendRecords;
    }

    /// <summary>
    /// <c>Dispatcher</c> 负责转发系统中的所有事件。
    /// <br/>每个 <c>StoreBase</c> 都会注册到此处。
    /// <br/>这个单例只！能！在！服！务！端！使！用！
    /// </summary>
    public class Dispatcher : Singleton<Dispatcher>
    {
        public bool replay;
        public string replayFile;

        private readonly Dictionary<string, List<StoreBase>> _stores = new Dictionary<string, List<StoreBase>>();

        /// <summary>
        /// 声明队列在Dispatcher中发送指令。
        /// </summary>
        private readonly Queue<SendCache> _sendCache = new Queue<SendCache>();

        /// <summary>
        /// 延迟重放缓存。
        /// </summary>
        private readonly Queue<SendCache> _delayCache = new Queue<SendCache>();

        /// <summary>
        /// 声明列表用于记录动作，生成json回放文件。
        /// </summary>
        private List<SendRecord> _records = new List<SendRecord>();

        /// <summary>
        /// 比赛记录信息。
        /// </summary>
        private readonly MatchRecord _matchRecord = new MatchRecord();

        private Identity.Camps _winner = Identity.Camps.Unknown;

        private int _currentFrame;
        private int _replayCursor;
        private bool _storeCacheInitialized;

        private int _counter = 0;

        /// <summary>
        /// 声明字典 记录全场每个ID对应的一个Store。
        /// </summary>
        private readonly Dictionary<Identity, StoreBase> _storeCache = new Dictionary<Identity, StoreBase>();

        private readonly Dictionary<string, string> _redundancyChildCache = new Dictionary<string, string>();

        /// <summary>
        /// 加载json记录文件。
        /// </summary>
        /// <param name="file"></param>
        private void LoadMatchRecord(string file)
        {
            var compressedRecord = File.ReadAllBytes(file);
            var recordJson = GZipCompress.DecompressToString(compressedRecord);
            var matchRecord = JsonUtility.FromJson<MatchRecord>(recordJson);
            _records = new List<SendRecord>(matchRecord.sendRecords);
        }

        /// <summary>
        /// Dispatcher 初始化意味着网络服务器的启动。
        /// </summary>
        private void Start()
        {
            Random.InitState(825172);
            _currentFrame = 0;
            StartCoroutine(DelayStart());
        }

        private IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(0.1f);
            if (replay)
            {
                _replayCursor = 0;
                LoadMatchRecord(replayFile);
            }
            else
            {
                var roomManager = (NetworkRoomManagerExt)NetworkManager.singleton;
                _matchRecord.mapType = roomManager.currentMap;
                _matchRecord.redTeamName = roomManager.redTeam;
                _matchRecord.blueTeamName = roomManager.blueTeam;
            }
        }

        public void UpdateStoreCache()
        {
            foreach (var storeBase in FindObjectsOfType<StoreBase>())
            {
                if (!_storeCache.ContainsKey(storeBase.id))
                {
                    _storeCache[storeBase.id] = storeBase;
                }
            }
        }

        /// <summary>
        /// 分批发送，子组件缓存。
        /// </summary>
        private void FixedUpdate()
        {
            // 如果在重放模式，则由 Dispatcher 驱动机器人生成
            if (!replay && (EntityManager.Instance() == null || !EntityManager.Instance().initialized)) return;

            //进行全场store储存的初始化
            if (!_storeCacheInitialized)
            {
                UpdateStoreCache();
                _storeCacheInitialized = true;
            }

            //当队列中还存在未发送的指令时，继续使队列_sendCache进行出队发送指令
            while (_sendCache.Count > 0)
            {
                var send = _sendCache.Dequeue();
                switch (send.IsSendChild)
                {
                    //若动作不是发给子组件时
                    case false:

                        if (_stores.ContainsKey(send.Action.ActionName()))
                        {
                            //先进行json记录
                            if (!replay)
                            {
                                if (!IgnoreAction(send.Action))
                                {
                                    //在列表中写入当前的指令内容用于形成json文件
                                    _records.Add(new SendRecord
                                    {
                                        frame = _currentFrame,
                                        isSendChild = false,
                                        //将Action多态序列化（转换为字符串）进行存储
                                        action = PolymorphicSerializer.Serialize(send.Action),
                                        owner = null
                                    });
                                }
                            }

                            //json记录完成后，再分发action
                            foreach (var store in _stores[send.Action.ActionName()])
                            {
                                // 分发action;
                                store.Receive(send.Action);
                                //Debug.Log(_counter+" "+store.id.Data()+"接收"+send.Action.ActionName());
                                _counter++;
                            }

                            if (send.Action.ActionName() == ActionID.Stage.GameOver)
                            {
                                var gameOverAction = (GameOver)send.Action;
                                _winner = gameOverAction.WinningCamp;
                            }
                        }
                        else
                        {
                            _delayCache.Enqueue(send);
                        }

                        break;

                    case true:
                        var typeName = send.Action.GetType().FullName;
                        if (typeName != null)
                        {
                            var serializedAction = PolymorphicSerializer.Serialize(send.Action);

                            if (!replay)
                            {
                                var childAction = (IChildAction)send.Action;
                                var key = send.Owner.Data() + childAction.ReceiverChildType().Data() + typeName;
                                if (_redundancyChildCache.ContainsKey(key))
                                {
                                    if (_redundancyChildCache[key] == serializedAction)
                                    {
                                        // 冗余事件
                                        break;
                                    }

                                    _redundancyChildCache[key] = serializedAction;
                                }
                                else
                                {
                                    _redundancyChildCache[key] = serializedAction;
                                }
                            }


                            // 若在全场找到子组件的持有者，也向他发送指令
                            // 通过这种方法绕开一个prefab上有两个NetworkIdentity的情况
                            if (_storeCache.ContainsKey(send.Owner))
                            {
                                // 给子组件发送事件
                                if (!replay)
                                {
                                    _records.Add(new SendRecord
                                    {
                                        frame = _currentFrame,
                                        isSendChild = true,
                                        action = serializedAction,
                                        owner = send.Owner.Data(),
                                    });
                                }

                                if (replay || !IgnoreAction(send.Action))
                                {
                                    _storeCache[send.Owner].Receive(send.Action);
                                    //Debug.Log(_counter+" "+send.Owner.Data()+"子组件接收"+send.Action.ActionName());
                                    _counter++;

                                }
                            }
                            else
                            {
                                _delayCache.Enqueue(send);
                            }
                        }

                        break;
                }
            }

            // 保存延迟一秒以内的记录
            while (_delayCache.Count > 0)
            {
                var delaySend = _delayCache.Dequeue();
                if (_currentFrame - delaySend.frame < 1 / Time.fixedDeltaTime)
                {
                    _sendCache.Enqueue(delaySend);
                }
            }

            //进行回放时执行的代码:
            if (replay && _replayCursor < _records.Count)
            {
                var record = _records[_replayCursor];
                while (record.frame <= _currentFrame)
                {
                    if (record.isSendChild)
                    {
                        SendChild(
                            (IChildAction)PolymorphicSerializer.Deserialize(record.action),
                            new Identity(record.owner),
                            true);
                    }
                    else
                    {
                        Send(
                            (IAction)PolymorphicSerializer.Deserialize(record.action),
                            true);
                    }

                    _replayCursor++;
                    if (_replayCursor == _records.Count) break;
                    record = _records[_replayCursor];
                }
            }

            // 最后
            _currentFrame++;
        }

        private static bool IgnoreAction(IAction action)
        {
            if (action is IChildAction)
            {
                if (action is Rectify)
                {
                    return true;
                }
            }
            else
            {
                switch (action)
                {
                    case PrimaryAxis primaryAxis:
                        return !primaryAxis.Receiver.IsRobot();
                    case SecondaryAxis secondaryAxis:
                        return !secondaryAxis.Receiver.IsRobot();
                    case ViewControl viewControl:
                        return !viewControl.Receiver.IsRobot();
                    case StateControl stateControl:
                        return !stateControl.Receiver.IsRobot();
                }
            }

            return false;
        }

        /// <summary>
        /// 将 <c>StoreBase</c> 注册到此 <c>Dispatcher</c>。
        /// </summary>
        /// <param name="store"><c>StoreBase</c></param>
        public void Register(StoreBase store)
        {
            // 将每个注册对象以及注册对象感兴趣的接收命令存到一个字典中，用于之后分发action的筛选
            foreach (var action in store.InputActions())
            {
                // 每一个字符串对应着对他“感兴趣”的一些对象
                if (_stores.ContainsKey(action))
                {
                    _stores[action].Add(store);
                }
                else
                {
                    _stores[action] = new List<StoreBase> { store };
                }
            }
        }

        /// <summary>
        /// 通过此 <c>Dispatcher</c> 发送事件。
        /// </summary>
        /// <param name="action">要发送的事件</param>
        /// <param name="isReplay">是否用于重播</param>
        public void Send(IAction action, bool isReplay = false) //先进行筛选，在分发
        {
            if (replay && !isReplay)
            {
                // 保留未被记录的事件
                if (!IgnoreAction(action)) return;
            }

            // 筛选掉对action不感兴趣的数据流向，进行下一步分发action时，将数据（函数调用指令）分发到对action感兴趣的对象上
            // if (!_stores.ContainsKey(action.ActionName()))
            //     return;
            _sendCache.Enqueue(new SendCache
            {
                Action = action,
                IsSendChild = false,
                Owner = null,
                frame = _currentFrame
            });
        }

        /// <summary>
        /// 发送事件给子组件。
        /// </summary>
        /// <param name="action">事件</param>
        /// <param name="owner">根组件 ID</param>
        /// <param name="isReplay">是否用于重播</param>
        /// <exception cref="Exception"></exception>
        public void SendChild(IChildAction action, Identity owner, bool isReplay = false)
        {
            if (replay && !isReplay) return;
            // 向子组件发送事件
            _sendCache.Enqueue(new SendCache
            {
                Action = action,
                IsSendChild = true,
                Owner = owner,
                frame = _currentFrame
            });
        }

        /// <summary>
        /// 写入记录文件。
        /// </summary>
        private void OnDestroy()
        {
            if (replay) return;
            if (_records.Count == 0) return;
            _matchRecord.sendRecords = _records.ToArray();
            var recordJson = JsonUtility.ToJson(_matchRecord);
            var compressedRecord = GZipCompress.Compress(recordJson);
            // ReSharper disable once RedundantAssignment
            var isEditor = false;
            var isServer = false;
#if UNITY_EDITOR
            isEditor = true;
#endif
#if UNITY_SERVER
            isServer = true;
#endif
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (isEditor || isServer)
            {
                File.WriteAllBytes(replayFile, compressedRecord);
                PlayerPrefs.SetString(PrefKeys.Statistics.ReplayToUpload, replayFile);
                PlayerPrefs.SetString(PrefKeys.Statistics.Winner, _winner switch
                {
                    Identity.Camps.Red => "Red",
                    Identity.Camps.Blue => "Blue",
                    Identity.Camps.Other => "Tie",
                    _ => "Unknown"
                });
            }
        }
    }
}