using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Gameplay.Networking;
using Honeti;
using Infrastructure;
using Michsky.UI.Shift;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 组队页面面板。
    /// </summary>
    public class RoleSelectionPanel : NetworkBehaviour
    {
        public TMP_Text serverNameText;
        public TMP_Text tokenText;
        public TMP_Text versionWatermark;
        public Button copyServerTokenButton;

        public MainButton startGame;
        public MainButton leaveBtn;
        [HideInInspector] public bool isOwner;

        public TMP_InputField redTeam;
        public TMP_InputField blueTeam;

        public GameObject loadingPanel;

        public TMP_Dropdown redInitCoin;
        public TMP_Dropdown blueInitCoin;

        // 客户端
        [HideInInspector] public NetworkRoomPlayerExt localRoomPlayer;

        // 服务端
        private readonly SyncDictionary<string, Identity> _roleSelection = new SyncDictionary<string, Identity>();
        private List<RoleChoice> _choices = new List<RoleChoice>();
        [SyncVar] private int _redCoin, _blueCoin;

        private void Start()
        {
            if (!isServerOnly) loadingPanel.SetActive(true);
            StartCoroutine(DelayHideLoading());

            startGame.buttonText = I18N.instance.getValue("^start");
            leaveBtn.buttonText = I18N.instance.getValue("^leave");
            versionWatermark.text = GlobalConfig.Version.GetVersionWatermark();

            _choices = FindObjectsOfType<RoleChoice>().ToList();
            Debug.Log($"Found {_choices.Count} choices.");

#if !UNITY_SERVER
            GameObject.Find("LobbyMusic").GetComponent<AudioSource>().volume =
                PlayerPrefs.GetFloat("MasterVolumeSliderValue", 1.0f);
#endif
        }

        /// <summary>
        /// 更新面板显示和准备状态。
        /// </summary>
        private void FixedUpdate()
        {
            if (!isClient) return;
            // 房主才有
            startGame.gameObject.SetActive(isOwner);
            redTeam.gameObject.SetActive(isOwner);
            blueTeam.gameObject.SetActive(isOwner);

            // 房主才能设置
            redInitCoin.interactable = isOwner;
            blueInitCoin.interactable = isOwner;

            tokenText.gameObject.SetActive(isClientOnly);
            copyServerTokenButton.gameObject.SetActive(isClientOnly);

            foreach (var roleChoice in _choices)
            {
                if (_roleSelection.Any(r => r.Value == roleChoice.role))
                {
                    var selector = _roleSelection
                        .First(r => r.Value == roleChoice.role).Key;

                    if (roleChoice.role == localRoomPlayer.chosenRobot)
                    {
                        roleChoice.GetComponent<Animator>().Play("Highlighted");
                    }

                    var nicknameClamp = selector;
                    if (nicknameClamp.Length > 10)
                    {
                        nicknameClamp = nicknameClamp.Substring(0, 11);
                    }

                    roleChoice.SetNickname(nicknameClamp, true);
                }
                else
                {
                    roleChoice.SetNickname(I18N.instance.getValue("^waiting"));
                    if (roleChoice.gameObject.activeSelf)
                    {
                        roleChoice.GetComponent<Animator>().Play("Normal");
                    }
                }
            }

            redInitCoin.value = _redCoin;
            blueInitCoin.value = _blueCoin;
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetInt("red_init_add_coin", Convert.ToInt32(redInitCoin.options[redInitCoin.value].text));
            PlayerPrefs.SetInt("blue_init_add_coin", Convert.ToInt32(blueInitCoin.options[blueInitCoin.value].text));
        }

        private IEnumerator DelayHideLoading()
        {
            yield return new WaitForSeconds(1);
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }

        /// <summary>
        /// 点击按钮选择角色。
        /// </summary>
        /// <param name="role">选择的角色</param>
        public void ChooseRole(Identity role)
        {
            var localPlayer = FindObjectsOfType<NetworkRoomPlayerExt>()
                .FirstOrDefault(r => r.isLocalPlayer);
            if (localPlayer == null || localPlayer.readyToBegin) return;
            if (!_roleSelection.Any(rs =>
                    rs.Value == role && rs.Key != ((NetworkRoomManagerExt)NetworkManager.singleton).nickname))
            {
                localPlayer.CmdChooseRobot(role);
                CmdChooseRole(role);
            }
        }

        /// <summary>
        /// 用户离开时清理字典。
        /// </summary>
        /// <param name="nickname"></param>
        [Server]
        public void PlayerLeave(string nickname)
        {
            if (_roleSelection.ContainsKey(nickname))
            {
                _roleSelection.Remove(nickname);
            }
        }

        /// <summary>
        /// 该客户端是否已经选择角色
        /// </summary>
        /// <returns></returns>
        public bool HasChosenRole()
        {
            return _roleSelection.Any(r => r.Key == ((NetworkRoomManagerExt)NetworkManager.singleton).nickname);
        }

        /// <summary>
        /// 获取本人的标识
        /// </summary>
        /// <returns></returns>
        public Identity MyIdentity()
        {
            return _roleSelection.First(r => r.Key == ((NetworkRoomManagerExt)NetworkManager.singleton).nickname).Value;
        }

        /// <summary>
        /// 尝试选择角色。
        /// </summary>
        /// <param name="role">选择的角色</param>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        private void CmdChooseRole(Identity role, NetworkConnectionToClient sender = null)
        {
            if (sender == null) return;
            var nickname = ClientManager.Instance().Nickname(sender.connectionId);
            if (nickname == "" && isServerOnly) return;

            var localPlayer = FindObjectsOfType<NetworkRoomPlayerExt>()
                .FirstOrDefault(r => r.isLocalPlayer);

            // 重复或取消选择
            if (_roleSelection.Any(r => r.Value == role))
            {
                if (_roleSelection.First(r => r.Value == role).Key == nickname)
                {
                    _roleSelection.Remove(nickname);
                    if (localPlayer != null)
                    {
                        localPlayer.CmdChangeReadyState(false);
                    }
                }
            }
            else
            {
                // 选择
                _roleSelection[nickname] = role;
                if (localPlayer != null)
                {
                    localPlayer.CmdChangeReadyState(false);
                }
            }
        }

        /// <summary>
        /// 断开连接。
        /// </summary>
        public void OnLeaveButtonClicked()
        {
            if (isOwner && _roleSelection.Count > 0) return;
            if (NetworkClient.active)
                NetworkManager.singleton.StopClient();
            if (NetworkServer.active)
                NetworkManager.singleton.StopServer();
            SceneManager.LoadScene("Offline");
        }

        /// <summary>
        /// 更新队名回调。
        /// </summary>
        public void OnTeamNamesChanged()
        {
            CmdTeamNamesChanged(redTeam.text, blueTeam.text);
        }

        /// <summary>
        /// 更新队名。
        /// </summary>
        /// <param name="red">红方队名</param>
        /// <param name="blue">蓝方队名</param>
        [Command(requiresAuthority = false)]
        private void CmdTeamNamesChanged(string red, string blue)
        {
            var roomManager = (NetworkRoomManagerExt)NetworkManager.singleton;
            roomManager.redTeam = red;
            roomManager.blueTeam = blue;
            TeamNamesChangedRpc(red, blue);
        }

        /// <summary>
        /// 在每个客户端同步队名。
        /// </summary>
        /// <param name="red">红方队名</param>
        /// <param name="blue">蓝方队名</param>
        [ClientRpc]
        private void TeamNamesChangedRpc(string red, string blue)
        {
            var roomManager = (NetworkRoomManagerExt)NetworkManager.singleton;
            roomManager.redTeam = red;
            roomManager.blueTeam = blue;
        }

        /// <summary>
        /// 金币数量变化时回调
        /// </summary>
        public void OnInitCoinChanged()
        {
            CmdInitCoinChanged(redInitCoin.value, blueInitCoin.value);
        }

        /// <summary>
        /// 更新红蓝金币数
        /// </summary>
        /// <param name="red"></param>
        /// <param name="blue"></param>
        [Command(requiresAuthority = false)]
        private void CmdInitCoinChanged(int red, int blue)
        {
            _redCoin = red;
            _blueCoin = blue;
            redInitCoin.value = red;
            blueInitCoin.value = blue;
        }


        /// <summary>
        /// 房主强制启动游戏。
        /// </summary>
        public void OnForceStart()
        {
            if (!isClient) return;
            if (!isOwner) return;
            if (localRoomPlayer.chosenRobot == new Identity()) return;
            CmdForceStart();
        }

        /// <summary>
        /// 复制服务器口令到剪贴板
        /// </summary>
        public void CopyServerToken()
        {
            GUIUtility.systemCopyBuffer = tokenText.text;
        }

        /// <summary>
        /// 在客户端强制启动。
        /// </summary>
        [Command(requiresAuthority = false)]
        private void CmdForceStart()
        {
            foreach (var roomPlayer in FindObjectsOfType<NetworkRoomPlayerExt>())
            {
                if (roomPlayer.chosenRobot == new Identity())
                {
                    // 踢出
                    KickRpc(roomPlayer.connectionToClient);
                    StartCoroutine(DelayDisconnect(roomPlayer.connectionToClient));
                }
                else
                {
                    roomPlayer.CmdChangeReadyState(true);
                }
            }

            StartCoroutine(DelayStart());
        }

        /// <summary>
        /// 通知未选择角色的玩家。
        /// </summary>
        /// <param name="target"></param>
        [TargetRpc]
        // ReSharper disable once UnusedParameter.Local
        private void KickRpc(NetworkConnection target)
        {
            Debug.LogError("比赛已被强制开始，你未选择角色。");
        }

        /// <summary>
        /// 延迟断开玩家连接。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        [Server]
        private IEnumerator DelayDisconnect(NetworkConnection target)
        {
            yield return new WaitForSeconds(0.5f);
            target.Disconnect();
        }

        /// <summary>
        /// 等待所有未选择角色玩家被踢出后启动游戏。
        /// </summary>
        /// <returns></returns>
        [Server]
        private IEnumerator DelayStart()
        {
            var tires = 0;
            while (FindObjectsOfType<NetworkRoomPlayerExt>()
                   .Any(rp => rp.chosenRobot == new Identity()))
            {
                tires++;
                if (tires > 10) yield break;
                yield return new WaitForSeconds(0.5f);
            }

            var roomManager = FindObjectOfType<NetworkRoomManagerExt>();
            roomManager.StartGame();
        }
        
        public void OnInfantryTypeChange(uint order, Identity.Camps camp, string selection)
        {
            CmdInfantryTypeChange(order, camp, selection);
        }

        [Command(requiresAuthority = false)]
        private void CmdInfantryTypeChange(uint order, Identity.Camps camp, string selection)
        {
            var card = _choices.First(c => c is LobbyInfantryCard s && c.role.order == order && c.role.camp == camp);
            ((LobbyInfantryCard)card).UpdateSelection(selection);
            InfantryTypeChangeRpc(order, camp, selection);
        }
        
        [ClientRpc]
        private void InfantryTypeChangeRpc(uint order, Identity.Camps camp, string selection)
        {
            var card = _choices.First(c => c is LobbyInfantryCard s && c.role.order == order && c.role.camp == camp);
            ((LobbyInfantryCard)card).UpdateSelection(selection);
        }
    }
}