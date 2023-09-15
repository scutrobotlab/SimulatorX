using System;
using System.Collections.Generic;
using AdditionalAssets.RobotArm.Scripts;
using Gameplay.Events;
using Infrastructure;
using JetBrains.Annotations;
using Mirror;

namespace Gameplay.Customize
{
    /// <summary>
    /// 管理自定义信息。
    /// 根据机器人 ID，返回相应 Key 所对应的数据。
    /// 此单例要严格控制执行位置。
    /// </summary>
    public class CustomizeManager : NetworkBehaviour
    {
        private static CustomizeManager s_Instance;

        /// <summary>
        /// 创建一个嵌套字典，一个机器人 对应其相应的用另一个字典储存的信息（key:信息的名称，value:该信息对应的数值）。
        /// </summary>
        private readonly Dictionary<Identity, Dictionary<string, float>> _customizeData =
            new Dictionary<Identity, Dictionary<string, float>>();

        // 网络单例
        public static CustomizeManager Instance()
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<CustomizeManager>();
            }

            if (s_Instance == null)
            {
                throw new Exception("Getting CustomizeManager before initialization.");
            }

            return s_Instance;
        }

        // TODO：开始比赛前网络初始化

        /// <summary>
        /// 根据车辆类型获取自定义信息。
        /// </summary>
        /// <param name="id">车辆 ID</param>
        /// <param name="property">信息名</param>
        /// <returns></returns>
        /// TODO:是否要抛异常？ 我觉得得抛，给个1好奇怪 
        [Server]
        public float Data(Identity id, string property)
        {
            if (!_customizeData.ContainsKey(id)) return 1;
            if (!_customizeData[id].ContainsKey(property)) return 1;
            return _customizeData[id][property];
        }

        /// <summary>
        /// 查询是否有指定内容
        /// </summary>
        /// <param name="id">车辆 ID</param>
        /// <param name="property">信息键</param>
        /// <returns></returns>
        public bool Has([CanBeNull] Identity id, string property)
        {
            if (id == null || property == null) return false;
            return _customizeData.TryGetValue(id, out var value) && value.ContainsKey(property);
        }

        /// <summary>
        /// 设置自定义信息。
        /// </summary>
        /// <param name="id">车辆 ID</param>
        /// <param name="property">信息名</param>
        /// <param name="value">信息值</param>
        [Command(requiresAuthority = false)]
        public void CmdSetData(Identity id, string property, float value)
        {
            if (!_customizeData.ContainsKey(id))
            {
                _customizeData[id] = new Dictionary<string, float>();
            }

            _customizeData[id][property] = value;
        }

        [Command(requiresAuthority = false)]
        public void CmdSendExchange(Identity id, int money)
        {
            Dispatcher.Instance().Send(new ExchangeHealth()
            {
                Id = id,
                Money = -1 * money
            });
        }

        [Command(requiresAuthority = false)]
        public void CmdChangeGrade(Identity id, Grade grade)
        {
            Dispatcher.Instance().Send(new ChangeGrade()
            {
                Id = id,
                Gr = grade
            });
        }

        public bool IsServer()
        {
            return isServerOnly;
        }

        public static bool IsInit()
        {
            if (s_Instance == null) return false;
            else return true;
        }
    }
}