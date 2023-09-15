using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Honeti;
using Infrastructure;
using Mirror;

#if UNITY_SERVER
using Misc;
#endif

using UI;
using UnityEngine;
using Version = Config.Version;

namespace Gameplay.Networking
{
    /// <summary>
    /// 在组队界面管理连入的客户端、服务器版本等。
    /// </summary>
    public class ClientManager : NetworkBehaviour
    {
        private static ClientManager _instance;

        [Obsolete("V0.10.0 采用新的联机策略", true)] public string version;

        // 用户名到网络连接ID的映射
        private readonly Dictionary<string, int> _nicknameToConnId = new Dictionary<string, int>();

        private bool _ownerConfirmed;

        /// <summary>
        /// 连接服务器后申请登录验证。
        /// </summary>
        private void Start()
        {
            if (isClient)
                CmdClientAuthentication(((NetworkRoomManagerExt)NetworkManager.singleton).nickname,
                    Version.CurrentVersionCode);
        }

        // 网络单例
        public static ClientManager Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ClientManager>();
            }

            if (_instance == null)
            {
                throw new Exception("Getting ClientManager before initialization.");
            }

            return _instance;
        }

        /// <summary>
        /// 根据连接 ID 获取用户昵称。
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        [Server]
        public string Nickname(int connId)
        {
            if (_nicknameToConnId.ContainsValue(connId))
            {
                return _nicknameToConnId.First(n => n.Value == connId).Key;
            }

            return "";
        }

        /// <summary>
        /// 检查客户端与服务器版本是否一致，检查是否有重名用户。
        /// </summary>
        /// <param name="clientNickname">用户昵称</param>
        /// <param name="localVersion">客户端版本</param>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        private void CmdClientAuthentication(string clientNickname, int localVersion,
            NetworkConnectionToClient sender = null)
        {
            Debug.Log($"local = {localVersion}, remote = {GlobalConfig.Version.MinVersionCode}");
            if (sender == null) return;
            if (localVersion < GlobalConfig.Version.MinVersionCode)
            {
                // 本地版本小于服务器最低支持版本
                OutdatedVersion(sender);
                StartCoroutine(DelayDisconnect(sender));
                return;
            }

            if (_nicknameToConnId.ContainsKey(clientNickname))
            {
                // 用户名重复
                ReplicatedNickname(sender);
                StartCoroutine(DelayDisconnect(sender));
                return;
            }

            _nicknameToConnId[clientNickname] = sender.connectionId;
            var roomManager = (NetworkRoomManagerExt)NetworkManager.singleton;
            ClientStartRpc(roomManager.serverName, roomManager.serverToken, _ownerConfirmed);

            if (!_ownerConfirmed) _ownerConfirmed = true;
        }

        /// <summary>
        /// 等待上一任务（TargetRpc）完成后再断开连接。
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        [Server]
        private IEnumerator DelayDisconnect(NetworkConnection conn)
        {
            yield return new WaitForSeconds(0.5f);
            conn.Disconnect();
        }

        /// <summary>
        /// 服务端验证完成之后，将相关信息同步到客户端。
        /// </summary>
        /// <param name="serverName">服务器名称</param>
        /// <param name="serverToken">服务器口令</param>
        /// <param name="ownerConfirmed">是否是房主</param>
        [ClientRpc]
        private void ClientStartRpc(string serverName, string serverToken, bool ownerConfirmed)
        {
            var roleSelectionPanel = FindObjectOfType<RoleSelectionPanel>();
            if (roleSelectionPanel != null)
            {
                roleSelectionPanel.serverNameText.text =
                    WrapServerName(serverName, 24, I18N.instance.getValue("^exercise_room"));
                roleSelectionPanel.tokenText.text = serverToken;
                // TODO: 双端验证
                if (!ownerConfirmed) roleSelectionPanel.isOwner = true;

                roleSelectionPanel.redTeam.readOnly = !roleSelectionPanel.isOwner;
                roleSelectionPanel.redTeam.interactable = roleSelectionPanel.isOwner;
                roleSelectionPanel.blueTeam.readOnly = !roleSelectionPanel.isOwner;
                roleSelectionPanel.blueTeam.interactable = roleSelectionPanel.isOwner;
            }
        }

        private static string WrapServerName(string serverName, int maxLen, string fallback)
        {
            if (serverName.Length == 0)
                return fallback;

            if (serverName.Length > maxLen)
            {
                serverName = serverName.Substring(0, maxLen) + "...";
            }

            return serverName;
        }

        /// <summary>
        /// 通知客户端版本已过期。
        /// </summary>
        /// <param name="target"></param>
        [TargetRpc]
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void OutdatedVersion(NetworkConnection target)
        {
            PlayerPrefs.SetString(PrefKeys.Authentication.KickReason,
                I18N.instance.getValue("^need_to_update_warning"));
        }

        /// <summary>
        /// 通知客户端用户名重复。
        /// </summary>
        /// <param name="target"></param>
        [TargetRpc]
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void ReplicatedNickname(NetworkConnection target)
        {
            PlayerPrefs.SetString(PrefKeys.Authentication.KickReason, I18N.instance.getValue("^same_name_warning"));
        }

        /// <summary>
        /// 当有客户端断开连接时，清理相关数据。
        /// </summary>
        /// <param name="conn"></param>
        public void OnServerDisconnect(NetworkConnection conn)
        {
            if (_nicknameToConnId.ContainsValue(conn.connectionId))
            {
                var mapping =
                    _nicknameToConnId.First(
                        m => m.Value == conn.connectionId);
                var roleSelectionPanel = FindObjectOfType<RoleSelectionPanel>();
                if (roleSelectionPanel != null)
                {
                    roleSelectionPanel.PlayerLeave(mapping.Key);
                }

                _nicknameToConnId.Remove(mapping.Key);
            }
        }
    }
}