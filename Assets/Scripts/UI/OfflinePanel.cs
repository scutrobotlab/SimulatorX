using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gameplay;
using Gameplay.Embedded;
using Gameplay.Networking;
using Honeti;
using IgnoranceTransport;
using Infrastructure;
using Michsky.UI.Shift;
using Mirror;
using Misc;
using SimpleFileBrowser;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GlobalConfig = Config.GlobalConfig;

namespace UI
{
    public enum RoomMode
    {
        JoinExistRoom = 0,
        CreateNewRoom = 1,
    }

    /// <summary>
    /// 主页 UI。
    /// </summary>
    public class OfflinePanel : MonoBehaviour
    {
        public TMP_InputField nickname;
        public TMP_InputField serverAddr;
        public MainButton startButton;
        public MainButton localButton;

        public HorizontalSelector roomModeSelector;

        public TMP_InputField serverToken;
        // public TMP_Text status;

        public TMP_InputField serverPassword;

        public RawImage anniversaryImage;

        public SwitchManager insiderAlpha;
        public ModalWindowManager modalWindow;

        public TMP_Text versionWatermark;
        public TMP_Text insiderWatermark;

        private readonly ToggleHelper _enterHelper = new ToggleHelper();

        private readonly ToggleHelper _tabHelper = new ToggleHelper();

        private readonly Dictionary<TMP_InputField, TMP_InputField> _tabNavigation =
            new Dictionary<TMP_InputField, TMP_InputField>();

        private RoomMode _roomMode = RoomMode.JoinExistRoom;
        private string[] _roomModeTitleKeys = { "^join_exist_room", "^create_new_room" };

        private void Start()
        {
            StartCoroutine(DelayStart());
            _tabNavigation[nickname] = serverToken;
            _tabNavigation[serverToken] = serverPassword;
            _tabNavigation[serverPassword] = nickname;

            EmbeddedSimulate.Instance();

            OnLanguageChanged(I18N.instance.gameLang);
            I18N.OnLanguageChanged += OnLanguageChanged;

            var nowTime = DateTime.Now.ToFileTime();
            Debug.Log($"DateTime = {DateTime.Now:yyyy-MM-dd hh:mm:ss}");
            // Simulator 发布一周年纪念日
            if (nowTime >= DateTime.Parse("2022-08-17 00:00:00").ToFileTime() &&
                nowTime < DateTime.Parse("2022-08-18 00:00:00").ToFileTime())
            {
                anniversaryImage.gameObject.SetActive(true);
            }
            else
            {
                anniversaryImage.gameObject.SetActive(false);
            }

            nickname.text = PlayerPrefs.GetString("nickname");

#if RELEASE_VERSION
            insiderWatermark.enabled = false;
#else
            insiderWatermark.enabled = true;
#endif
        }

        private void Update()
        {
#if UNITY_SERVER
#else
            versionWatermark.text = GlobalConfig.Version.GetVersionWatermark();

            if (_tabHelper.Toggle(Keyboard.current.tabKey.isPressed) == ToggleHelper.State.Re)
            {
                foreach (var inputField in _tabNavigation)
                {
                    if (inputField.Key.isFocused)
                    {
                        inputField.Key.GetComponent<CustomInputField>().FieldTrigger();
                        inputField.Value.GetComponent<CustomInputField>().Animate();
                        inputField.Value.Select();
                        break;
                    }

                    if (inputField.Key == serverPassword)
                    {
                        // 一个都没有
                        nickname.GetComponent<CustomInputField>().Animate();
                        nickname.Select();
                    }
                }
            }

            if (_enterHelper.Toggle(Keyboard.current.enterKey.isPressed) == ToggleHelper.State.Re)
                OnButtonClicked();
#endif
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _roomModeTitleKeys.Length; i++)
            {
                roomModeSelector.itemList[i].itemTitle = I18N.instance.getValue(_roomModeTitleKeys[i]);
            }

            roomModeSelector.UpdateUI();

            if (NetworkClient.active)
            {
                startButton.buttonText = I18N.instance.getValue("^connecting");
            }
            else
            {
                // 本地操作
                if (nickname.text == "")
                {
                    startButton.buttonText = I18N.instance.getValue("^input_nickname");
                }
                else
                {
                    startButton.buttonText = _roomMode switch
                    {
                        RoomMode.JoinExistRoom =>
                            serverToken.text.Length > 0
                                ? I18N.instance.getValue("^connect_server")
                                : I18N.instance.getValue("^input_token"),
                        RoomMode.CreateNewRoom => I18N.instance.getValue("^create_a_room"),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }
        }

        private void OnApplicationQuit()
        {
            EmbeddedSimulate.Instance().Close();
        }

        private void OnLanguageChanged(LanguageCode newLang)
        {
            localButton.buttonText = I18N.instance.getValue("^play_locally");
        }

        private IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(0.2f);

            if (PlayerPrefs.HasKey(PrefKeys.Authentication.KickReason))
            {
                var reason = PlayerPrefs.GetString(PrefKeys.Authentication.KickReason);
                PlayerPrefs.DeleteKey(PrefKeys.Authentication.KickReason);
                modalWindow.titleText = I18N.instance.getValue("^join_room_failed");
                modalWindow.descriptionText = reason;
                modalWindow.ModalWindowIn();
            }
        }

        public void OnIntroClicked()
        {
            Application.OpenURL("https://intro.sim.scutbot.cn");
        }

        public void OnReplayClicked()
        {
            FileBrowser.SetFilters(true,
                new FileBrowser.Filter(I18N.instance.getValue("^competition_record"), ".rec"));
            FileBrowser.SetDefaultFilter(".rec");
            FileBrowser.ShowLoadDialog(
                ReplayFileSelected, null,
                FileBrowser.PickMode.Files, false,
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                null, I18N.instance.getValue("^select_a_competition_record_file"));
        }

        private static void ReplayFileSelected(string[] paths)
        {
            if (paths.Length > 0)
            {
                ((NetworkRoomManagerExt)NetworkManager.singleton).LoadRecord(paths[0]);
            }
        }

        public void OnExitClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }

        public void OnTokenChanged()
        {
            serverToken.text = serverToken.text.Trim().ToUpper();
        }

        public void CheckEnableInsider()
        {
            if (SceneUtility.GetBuildIndexByScenePath(
                    MapInfoManager.Instance().MapInfo(MapType.RMUC2022).roomScene) == -1)
            {
                modalWindow.titleText = I18N.instance.getValue("^insider_is_disabled");
                modalWindow.descriptionText = I18N.instance.getValue("^please_use_insider");
                modalWindow.ModalWindowIn();
                StartCoroutine(DelayAnimateSwitch());
            }
        }

        public void OnFullscreen() => FindObjectOfType<NetworkRoomManagerExt>().OnFullscreen();
        public void OnExitFullscreen() => FindObjectOfType<NetworkRoomManagerExt>().OnExitFullscreen();

        private IEnumerator DelayAnimateSwitch()
        {
            yield return new WaitForSeconds(0.1f);
            insiderAlpha.AnimateSwitch();
        }

        public void OnButtonClicked()
        {
            PlayerPrefs.SetString("nickname", nickname.text);
            if (NetworkClient.active)
            {
                NetworkManager.singleton.StopClient();
            }
            else
            {
                if (insiderAlpha.isOn)
                {
                    ((NetworkRoomManagerExt)NetworkManager.singleton).SetCurrentMap(MapType.RMUC2022);
                }

                var addr = serverAddr.text.Trim().Split(':');
                var port = 5333;
                if (addr.Length > 2 
                    || !System.Net.IPAddress.TryParse(addr[0], out var ip) 
                    || (addr.Length == 2 && !int.TryParse(addr[1], out port)))
                {
                    modalWindow.titleText = I18N.instance.getValue("^failed_to_join_a_room");
                    modalWindow.descriptionText = I18N.instance.getValue("^server_addr_invalid");
                    modalWindow.ModalWindowIn();
                    return;
                }
                
                Debug.Log($"Connect to {addr[0]}:{port}");
                NetworkManager.singleton.GetComponent<Ignorance>().port = port;
                NetworkManager.singleton.networkAddress = addr[0];
                ((NetworkRoomManagerExt)NetworkManager.singleton).nickname = nickname.text;
                NetworkManager.singleton.StartClient();
            }
        }

        public void OnLocalButtonClicked()
        {
            ((NetworkRoomManagerExt)NetworkManager.singleton).nickname =
                I18N.instance.getValue("^local_player");
            NetworkManager.singleton.StartHost();
        }

        public void OnCreateRoomModeSelected()
        {
            _roomMode = RoomMode.CreateNewRoom;
            serverToken.gameObject.SetActive(false);
            serverToken.text = "";
            serverPassword.gameObject.SetActive(true);
        }

        public void OnJoinRoomModeSelected()
        {
            _roomMode = RoomMode.JoinExistRoom;
            serverToken.gameObject.SetActive(true);
            serverPassword.gameObject.SetActive(false);
            serverPassword.text = "";
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.LogWarning("PlayerPrefs has been reset.");
        }
    }
}