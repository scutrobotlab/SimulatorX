using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Events;
using Gameplay.Events.Child;
using Gameplay.Networking;
using Honeti;
using Infrastructure;
using Mirror;
using Misc;
using UI.UC;
using UI.UL;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// <c>UIManager</c> 用于管理UI组件。
    /// 自动加载UI场景，并提供获取指定UI组件控制器的方法。
    /// </summary>
    public class UIManager : StoreBase
    {
        private static UIManager _instance;
        private readonly ToggleHelper _escapeHelper = new ToggleHelper();
        private readonly ToggleHelper _escHelper = new ToggleHelper();
        private readonly ToggleHelper _hideUIHelper = new ToggleHelper();
        private readonly ToggleHelper _lcHelper = new ToggleHelper();
        private readonly ToggleHelper _lmbHelper = new ToggleHelper();

        // 解锁鼠标
        private readonly ToggleHelper _mouseReleaseHelper = new ToggleHelper();
        private readonly ToggleHelper _overheatHelper = new ToggleHelper();
        private readonly ToggleHelper _pHelper = new ToggleHelper();

        // 异常状态页面
        private AbnormalPanel _abnormalPanel;

        // 通知
        private AnnouncementPanel _announcementPanel;

        // 阵亡
        private DeadPanel _deadPanel;

        //云台手遮挡画面
        private DroneCoverPanel _droneCoverPanel;

        //云台手提醒界面
        private DroneWarningPanel _droneWarningPanel;
        private bool _hideUI;

        // 受伤
        private HurtPanel _hurtPanel;
        private JudgePanelUc _judgePanelUc;

        private JudgePanelUl _judgePanelUl;
        private AsyncOperation _loadScene;

        // 机构设置界面
        private MechanicSelectionPanel _mechanicSelectionPanel;
        private bool _mechanicTypeChosen;

        // HUD 界面
        private OperatorPanelUc _operatorPanelUc;
        private OperatorPanelUl _operatorPanelUl;

        // 过热
        private OverheatPanel _overheatPanel;

        // HUD 叠加层
        private OverlayPanel _overlayPanel;

        // 暂停
        private CustomizePanel _pausePanel;

        // 判罚页面
        private PenaltyPanel _penaltyPanel;

        // 结果
        private ResultPanel _resultPanel;

        //UC结果
        private UCResultPanel _ucResultPanel;

        // 聊天室
        private Scene _uiScene;

        // 警告
        private WarningPanel _warningPanel;
        private double droneCoverEnd;
        private Identity.Camps RecieveCamps;

        // 增量载入
        public bool Loaded => _loadScene.isDone;

        // 控制机器人
        private static RobotStoreBase LocalRobot => EntityManager.Instance() == null
            ? null
            : !EntityManager.Instance().initialized
                ? null
                : EntityManager.Instance().LocalPlayer().localRobot;

        private static RobotStoreBase OriginalLocalRobot => EntityManager.Instance() == null
            ? null
            : !EntityManager.Instance().initialized
                ? null
                : EntityManager.Instance().LocalPlayer().originalLocalRobot;

        private static PlayerStore LocalPlayer => EntityManager.Instance() == null
            ? null
            : !EntityManager.Instance().initialized
                ? null
                : EntityManager.Instance().LocalPlayer();

        /// <summary>
        /// 是否已开启暂停面板的标志。
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Paused { get; private set; }

        /// <summary>
        /// 自动加载UI场景。
        /// </summary>
        protected void Awake()
        {
            _loadScene = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            _uiScene = SceneManager.GetSceneByName("UI");
        }

        protected override void Start()
        {
            base.Start();
            if (FindObjectOfType<NetworkRoomPlayerExt>() != null)
                SetCursorLock(true);
            droneCoverEnd = 0;
        }


        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isClient)
            {
                if (LocalRobot == null) return;
                if (!Loaded) return;

                // 自定义面板，RMUL 不显示。
                if (EntityManager.Instance().CurrentMap() != MapType.RMUL2022)
                {
                    if (Loaded && _pausePanel == null)
                    {
                        // TODO: 真正的 PauseView
                        _pausePanel = UI<CustomizePanel>();
                    }

                    //检测到按下Q键正处于上升沿，则执行
                    if (_escHelper.Toggle(Keyboard.current.qKey.isPressed) == ToggleHelper.State.Re)
                    {
                        SetCursorLock(!GetCursorLocked());
                        if (!GetCursorLocked())
                        {
                            _pausePanel.StartSession();
                            Paused = true;
                        }
                        else
                        {
                            _pausePanel.EndSession();
                            Paused = false;
                        }
                    }
                }

                // 切换角色主面板。
                switch (EntityManager.Instance().CurrentMap())
                {
                    case MapType.RMUL2022:
                        _operatorPanelUl.gameObject.SetActive(LocalRobot.id.IsRobot());
                        _judgePanelUl.gameObject.SetActive(!LocalRobot.id.IsRobot() && !_hideUI &&
                                                           LocalRobot.id.role != Identity.Roles.DroneManipulator);
                        break;
                    case MapType.RMUC2022:
                        _operatorPanelUc.gameObject.SetActive(LocalRobot.id.IsRobot());
                        _judgePanelUc.gameObject.SetActive(!LocalRobot.id.IsRobot() && !_hideUI &&
                                                           LocalRobot.id.role != Identity.Roles.DroneManipulator);
                        break;
                }

                // 阵亡面板。    
                if (LocalRobot.health == 0)
                {
                    var map = EntityManager.Instance().CurrentMap();
                    if (!LocalRobot.HasEffect(EffectID.Status.Ejected))
                    {
                        //不同的地图，读秒的操作不一样
                        switch (map)
                        {
                            //UL场景中自动读秒复活
                            case MapType.RMUL2022:
                                _deadPanel.Countdown(
                                    (LocalRobot.revivalProcessRequired + LocalRobot.requiredAdded -
                                     LocalRobot.revivalProcess) / 2);
                                _deadPanel.Show(map);
                                break;

                            //UC中，等待工程救援后读秒
                            case MapType.RMUC2022:

                                //在复活区内的复活状态
                                if (LocalRobot.HasEffect(EffectID.Buffs.Revive))
                                {
                                    _deadPanel.Show(map, StatusID.Revive);
                                    _deadPanel.Countdown(
                                        (LocalRobot.revivalProcessRequired + LocalRobot.requiredAdded -
                                         LocalRobot.revivalProcess) / 2);
                                }

                                //工程救援的复活状态
                                else if (LocalRobot.HasEffect(EffectID.Buffs.CardRevive))
                                {
                                    _deadPanel.Show(map, StatusID.CardRevive);
                                    _deadPanel.Countdown(LocalRobot.revivalProcessRequired + LocalRobot.requiredAdded -
                                                         LocalRobot.revivalProcess);
                                }
                                else
                                {
                                    _deadPanel.Show(map, StatusID.Dead);
                                }

                                break;
                        }
                    }
                    else
                    {
                        _deadPanel.Countdown(-1);
                    }
                }
                else _deadPanel.Hide();

                //无人机遮挡
                if (LocalRobot.id.role == Identity.Roles.Drone)
                {
                    if (droneCoverEnd != 0)
                    {
                        if (RecieveCamps == LocalRobot.id.camp)
                        {
                            _droneCoverPanel.EndSession();
                            //_droneCoverPanel.CountDown(Convert.ToInt32(droneCoverEnd));
                            droneCoverEnd -= Time.fixedDeltaTime;
                        }
                        else
                        {
                            //_droneCoverPanel.StartSession();
                            droneCoverEnd = 0;
                        }
                    }
                    else
                    {
                        droneCoverEnd = 0;
                        //_droneCoverPanel.StartSession();
                    }
                }
                else
                {
                    _droneCoverPanel.EndSession();
                }

                // 警告面板
                if (LocalRobot.id.IsRobot())
                {
                    if (LocalRobot.id.role == Identity.Roles.Engineer || LocalRobot.id.role == Identity.Roles.Drone)
                    {
                        _warningPanel.EndSession();
                    }
                    else
                    {
                        if (!_mechanicTypeChosen || LocalRobot.gunLocked)
                        {
                            if (!_mechanicTypeChosen && LocalRobot.id == OriginalLocalRobot.id)
                            {
                                _warningPanel.StartSession(I18N.instance.getValue("^not_choose_mechanism_types_title"),
                                    I18N.instance.getValue("^not_choose_mechanism_types_desc"));
                            }

                            if (LocalRobot.gunLocked)
                            {
                                _warningPanel.StartSession(I18N.instance.getValue("^transmitter_lose_power_title"),
                                    I18N.instance.getValue("^transmitter_lose_power_desc"));
                            }
                        }
                        else
                        {
                            _warningPanel.EndSession();
                        }
                    }
                }
                else
                {
                    _warningPanel.EndSession();
                }


                // 机构类型选择面板
                if (_pHelper.Toggle(Keyboard.current.pKey.isPressed) == ToggleHelper.State.Re)
                {
                    if (!_mechanicSelectionPanel.selecting)
                    {
                        if (GetCursorLocked())
                        {
                            SetCursorLock(false);
                        }

                        _mechanicSelectionPanel.StartSession(OnMechanicTypeChange, OnSensitivityChange,
                            (LocalRobot.id.role == Identity.Roles.Infantry ||
                             LocalRobot.id.role == Identity.Roles.BalanceInfantry));
                    }
                    else
                    {
                        if (!Paused && !GetCursorLocked())
                        {
                            SetCursorLock(true);
                        }

                        _mechanicSelectionPanel.EndSession();
                    }
                }

                // 过热面板
                switch (_overheatHelper.Toggle(LocalRobot.HasEffect(EffectID.Status.OverHeat)))
                {
                    case ToggleHelper.State.Re:
                        _overheatPanel.StartSession();
                        break;
                    case ToggleHelper.State.De:
                        _overheatPanel.EndSession();
                        break;
                }


                // 判罚或FPV操作
                if (LocalRobot.id.role == Identity.Roles.Judge
                    || LocalRobot.id.role == Identity.Roles.Spectator
                    || OriginalLocalRobot.id.role == Identity.Roles.Spectator)
                {
                    // 启停判罚选择
                    switch (_lcHelper.Toggle(Keyboard.current.leftCtrlKey.isPressed))
                    {
                        case ToggleHelper.State.Re:
                            if (GetCursorLocked())
                            {
                                SetCursorLock(false);
                            }

                            break;
                        case ToggleHelper.State.De:
                            if (!GetCursorLocked())
                            {
                                _penaltyPanel.EndSession();
                                EntityManager.Instance().LocalPlayer().ChangeLocalRobot(OriginalLocalRobot);
                                SetCursorLock(true);
                            }

                            break;
                        case ToggleHelper.State.Hold:
                            if (!GetCursorLocked())
                            {
                                // 选择机器人
                                if (_lmbHelper.Toggle(Mouse.current.leftButton.isPressed) == ToggleHelper.State.De)
                                {
                                    var ray = LocalRobot.fpvCamera.ScreenPointToRay(Mouse.current.position
                                        .ReadValue());
                                    if (Physics.Raycast(ray, out var hit, 20))
                                    {
                                        var store = hit.transform.GetComponent<StoreBase>();
                                        if (store == null) store = hit.transform.GetComponentInParent<StoreBase>();
                                        if (store != null && store.id.IsRobot())
                                        {
                                            // 选中机器人
                                            var chosenRobot = (RobotStoreBase)store;
                                            if (LocalRobot.id.role == Identity.Roles.Judge)
                                            {
                                                _penaltyPanel.StartSession(
                                                    chosenRobot.id, OnRobotEjected, OnPenaltyGiven, OnGameOver);
                                            }

                                            if (LocalRobot.id.role == Identity.Roles.Spectator)
                                            {
                                                EntityManager.Instance().LocalPlayer().ChangeLocalRobot(chosenRobot);
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }

                    // 隐藏 UI 
                    if (LocalRobot.id.role == Identity.Roles.Spectator)
                    {
                        if (_hideUIHelper.Toggle(
                                Keyboard.current.ctrlKey.isPressed
                                && Keyboard.current.leftShiftKey.isPressed) == ToggleHelper.State.Re)
                        {
                            _hideUI = !_hideUI;
                        }
                    }
                }
                else
                {
                    // 解锁鼠标指针
                    SetCursorLock(
                        _mouseReleaseHelper.Toggle(Keyboard.current.ctrlKey.isPressed &&
                                                   Keyboard.current.leftAltKey.isPressed) switch
                        {
                            ToggleHelper.State.Re => false,
                            ToggleHelper.State.De => true,
                            _ => GetCursorLocked()
                        });
                }
            }
        }

        private void OnDestroy()
        {
            SetCursorLock(false);
        }

        // 网络单例
        public static UIManager Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
            }

            return _instance;
        }

        public OperatorPanelUc GetPanelUc() => _operatorPanelUc;

        /// <summary>
        /// 声明感兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.GameOver,
                ActionID.Stage.Penalty,
                ActionID.Stage.Kill,
                ActionID.Stage.Ejected,
                ActionID.Armor.ArmorHit,
                ActionID.Stage.DroneWarning,
                ActionID.Input.StateControl,
                ActionID.DroneControl.AgreeDroneSupport,
                ActionID.Clock.PowerRuneAvailable,
                ActionID.Stage.PowerRuneActivated,
                ActionID.Stage.OccupyControlArea,
                ActionID.Stage.LeaveControlArea,
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
                    var gameOverAction = (GameOver)action;
                    GameOverRpc(gameOverAction.WinningCamp.ToString(), gameOverAction.Description);

                    break;

                case ActionID.Stage.Penalty:
                    var penaltyAction = (Penalty)action;
                    PenaltyRpc(PolymorphicSerializer.Serialize(penaltyAction),
                        I18N.instance.getValue("^penalty"),
                        penaltyAction.Description);

                    break;

                case ActionID.Stage.Kill:
                    var killAction = (Kill)action;
                    KillRpc(killAction.killer, killAction.victim, killAction.method);
                    break;

                case ActionID.Stage.Ejected:
                    var ejectedAction = (Ejected)action;
                    KillRpc(new Identity(Identity.Camps.Other, Identity.Roles.Judge),
                        ejectedAction.target,
                        ejectedAction.Description + I18N.instance.getValue("^send_off"));
                    break;

                case ActionID.Armor.ArmorHit:
                    var hitEvent = (ArmorHit)action;
                    HurtRpc(hitEvent.Receiver, hitEvent.Armor.serial);
                    BaseUnderAttackRpc(hitEvent.Receiver);

                    break;

                case ActionID.Stage.DroneWarning:
                    ReceiveDroneWarningRpc(PolymorphicSerializer.Serialize(action));
                    break;

                case ActionID.Input.StateControl:
                    var stateControlAction = (StateControl)action;
                    if (_escapeHelper.Toggle(stateControlAction.FunctionI) != ToggleHelper.State.Hold)
                    {
                        EndDroneWarningRpc();
                    }

                    break;

                case ActionID.DroneControl.AgreeDroneSupport:
                    var agreeDroneSupport = (AgreeDroneSupport)action;
                    DroneCoverRpc(agreeDroneSupport);
                    if (agreeDroneSupport.Agree)
                    {
                        RecieveCamps = agreeDroneSupport.Camp;
                        droneCoverEnd = agreeDroneSupport.StopTime - NetworkTime.time;
                        Debug.Log("支援开始");
                    }
                    else
                    {
                        droneCoverEnd = 0;
                        Debug.Log("支援结束");
                    }

                    break;
                case ActionID.Stage.PowerRuneActivated:
                    var actionPowerRune = (PowerRuneActivated)action;
                    PowerRuneActivatedRpc(actionPowerRune.Camp, actionPowerRune.IsLarge);
                    break;
                case ActionID.Clock.PowerRuneAvailable:
                    var act = (PowerRuneAvailable)action;
                    PowerRuneAvailableRpc(act.Available, act.IsLarge);
                    break;
                case ActionID.Stage.OccupyControlArea:
                    var occupyControlArea = (OccupyControlArea)action;
                    OccupyControlAreaRpc(occupyControlArea.Camp);
                    break;
                case ActionID.Stage.LeaveControlArea:
                    var leaveControlArea = (LeaveControlArea)action;
                    LeaveControlAreaRpc(leaveControlArea.Camp);
                    break;
            }
        }

        /// <summary>
        /// 查询鼠标解锁状态。
        /// </summary>
        /// <returns>鼠标的解锁状态，若处于锁定状态则返回Ture，反之</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool GetCursorLocked() => Cursor.lockState == CursorLockMode.Locked;


        /// <summary>
        /// 设置鼠标的解锁状态
        /// </summary>
        /// <param name="isLock">需要设置的鼠标解锁状态</param>
        public static void SetCursorLock(bool isLock)
        {
            Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None;
        }

        /// <summary>
        /// 获取指定类型UI组件控制器。
        /// </summary>
        /// <typeparam name="T">UI组件控制器类型</typeparam>
        /// <returns>UI组件控制器</returns>
        public T UI<T>()
        {
            if (!Loaded)
            {
                throw new Exception("Try to get UI element before ui scene loaded.");
            }

            //在场景中找到第一个具有符合要求的GameObject后，获取到他相应的面板组件
            return (_uiScene.GetRootGameObjects().First(o => o.GetComponent<T>() != null)).GetComponent<T>();
        }

        [ClientRpc]
        private void BaseUnderAttackRpc(Identity receiver)
        {
            var baseID = new Identity(LocalRobot.id.camp, Identity.Roles.Base);
            if (receiver == baseID)
            {
                var baseStore = (BaseStore)EntityManager.Instance().Ref(baseID);
                if (!baseStore.invincible)
                {
                    if (LocalPlayer.localRobot != LocalRobot) return;
                    if (!_overlayPanel) return;
                    _overlayPanel.BaseUnderAttack();
                }
            }
        }

        [ClientRpc]
        private void HurtRpc(Identity receiver, int armorIndex)
        {
            if (LocalRobot.id != receiver) return;
            if (LocalPlayer.localRobot != LocalRobot) return;
            if (!_hurtPanel) return;
            _hurtPanel.StartSession(armorIndex);
        }

        /// <summary>
        /// 展示击杀提示面板。
        /// </summary>
        /// <param name="killer">击杀者</param>
        /// <param name="victim">受害者</param>
        /// <param name="method">击杀方式</param>
        [ClientRpc]
        private void KillRpc(Identity killer, Identity victim, string method)
        {
            _announcementPanel.AddBannerNotice(
                $"{killer.Describe()} {method} {victim.Describe()}", killer.camp.GetColor());
        }

        [ClientRpc]
        private void ReceiveDroneWarningRpc(string action)
        {
            var droneWarningEvent = (DroneWarning)PolymorphicSerializer.Deserialize(action);
            if (LocalRobot.id.role == Identity.Roles.Drone)
            {
                switch (droneWarningEvent.warningType)
                {
                    case Gameplay.Events.DroneWarning.WarningType.OreFall:
                        DroneWarning(I18N.instance.getValue("^pay_attention_to_the_mining_title"),
                            I18N.instance.getValue("^press_esc_to_exit"));
                        break;
                    case Gameplay.Events.DroneWarning.WarningType.PowerRune:
                        DroneWarning(I18N.instance.getValue("^pay_attention_to_the_ power_rune"),
                            I18N.instance.getValue("^press_esc_to_exit"));
                        break;
                    default:
                        EndDroneWarning();
                        break;
                }
            }
            else
            {
                EndDroneWarning();
            }
        }

        [ClientRpc]
        private void DroneCoverRpc(AgreeDroneSupport action)
        {
            if (LocalRobot.id.role == Identity.Roles.Drone)
            {
                if (action.Camp == LocalRobot.id.camp)
                {
                    if (action.Agree)
                    {
                        RecieveCamps = action.Camp;
                        droneCoverEnd = action.StopTime - NetworkTime.time;
                        //Debug.Log("支援开始");
                    }
                    else
                    {
                        droneCoverEnd = 0;
                        //Debug.Log("支援结束");
                    }
                }
            }
        }

        private void DroneWarning(string title, string method)
        {
            if (!_droneWarningPanel) return;
            _droneWarningPanel.StartSession(title, method);
        }

        private void EndDroneWarning()
        {
            if (!_droneWarningPanel) return;
            _droneWarningPanel.EndSession();
        }

        [ClientRpc]
        private void EndDroneWarningRpc()
        {
            EndDroneWarning();
        }


        /// <summary>
        /// 展示异常状态面板。
        /// </summary>
        /// <param name="action">PenaltyAction</param>
        /// <param name="title">状态标题</param>
        /// <param name="desc">状态描述</param>
        [ClientRpc]
        private void PenaltyRpc(string action, string title, string desc)
        {
            var penaltyAction = (Penalty)PolymorphicSerializer.Deserialize(action);
            if (LocalRobot.id.camp == penaltyAction.target.camp)
            {
                if (LocalPlayer.localRobot == LocalRobot)
                {
                    _abnormalPanel.StartSession(title, desc, LocalRobot.id == penaltyAction.target ? 5 : 2);
                }
            }
        }

        /// <summary>
        /// 展示结算面板。
        /// </summary>
        /// <param name="winningCamp">获胜阵营</param>
        /// <param name="description">描述</param>
        [ClientRpc]
        private void GameOverRpc(string winningCamp, string description)
        {
            StartCoroutine(DelayGameOver(winningCamp, description));
        }

        [ClientRpc]
        private void PowerRuneActivatedRpc(Identity.Camps camp, bool isLarge)
        {
            _announcementPanel.AddBannerNotice(I18N.instance.getValue("^power_rune_activated",
                    new[]
                    {
                        I18NIdentity.T(camp),
                        isLarge switch
                        {
                            true => I18N.instance.getValue("^large_power_rune"),
                            false => I18N.instance.getValue("^small_power_rune")
                        }
                    }),
                camp.GetColor(),
                NoticeIcon.Tag.PowerRune, 
                camp.GetColor());
        }

        [ClientRpc]
        private void PowerRuneAvailableRpc(bool available, bool isLarge)
        {
            _announcementPanel.AddBannerNotice(I18N.instance.getValue(
                    available ? "^power_rune_available" : "^power_rune_unavailable",
                    new []
                    {
                        isLarge switch
                        {
                            true => I18N.instance.getValue("^large_power_rune"),
                            false => I18N.instance.getValue("^small_power_rune")
                        }
                    }),
                Color.green,
                available ? NoticeIcon.Tag.PowerRune : NoticeIcon.Tag.None,
                available ? Color.green : default);
        }
        
        [ClientRpc]
        private void OccupyControlAreaRpc(Identity.Camps camp)
        {
            _announcementPanel.AddBannerNotice(I18N.instance.getValue("^occupy_control_area",
                    new[]
                    {
                        camp.Translate(),
                        camp.Opposite().Translate()
                    }),
                camp.GetColor());
        }
        
        [ClientRpc]
        private void LeaveControlAreaRpc(Identity.Camps camp)
        {
            _announcementPanel.AddBannerNotice(I18N.instance.getValue("^leave_control_area",
                    new[]
                    {
                        camp.Translate()
                    }),
                camp.GetColor());
        }

        /// <summary>
        /// 设置机构类型回调。
        /// </summary>
        /// <param name="chassis">底盘类型</param>
        /// <param name="gun">发射机构类型</param>
        private void OnMechanicTypeChange(MechanicType.Chassis chassis, MechanicType.GunType gun)
        {
            if (LocalRobot == null) return;
            if (LocalRobot.id.IsGroundRobot() && LocalRobot.id.role != Identity.Roles.Engineer)
            {
                _mechanicTypeChosen = true;
                CmdMechanicTypeChange(chassis.ToString(), gun.ToString());
                LocalRobot.chassisType = chassis;
                LocalRobot.Guns[0].type = gun;
            }
        }

        /// <summary>
        /// 修改鼠标灵敏度回调。
        /// </summary>
        /// <param name="sensitivity">灵敏度</param>
        private void OnSensitivityChange(float sensitivity)
        {
            if (isClient)
            {
                LocalPlayer.sensitivity = sensitivity;
            }
        }

        /// <summary>
        /// 在服务端更新类型设置。
        /// </summary>
        /// <param name="chassis"></param>
        /// <param name="gun"></param>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        private void CmdMechanicTypeChange(string chassis, string gun, NetworkConnectionToClient sender = null)
        {
            var localRobot = EntityManager.Instance().RobotByConnection(sender);
            if (localRobot == null) return;
            var maxHealth = AttributeManager.Instance().RobotAttributes(localRobot).MaxHealth;
            var chassisType = (MechanicType.Chassis)Enum.Parse(typeof(MechanicType.Chassis), chassis);
            var gunType = (MechanicType.GunType)Enum.Parse(typeof(MechanicType.GunType), gun);
            localRobot.chassisType = chassisType;
            localRobot.Guns[0].type = gunType;
            var newMaxHealth = AttributeManager.Instance().RobotAttributes(localRobot).MaxHealth;
            localRobot.health = localRobot.health / maxHealth * newMaxHealth;
            Dispatcher.Instance().Send(new MechanicSelect
            {
                Chassis = chassis,
                Gun = gun,
                Receiver = localRobot.id
            });
        }

        // TODO: 可以写外挂？
        private void OnRobotEjected(Identity robot, string description)
        {
            _penaltyPanel.EndSession();
            CmdOnRobotEjected(robot, description);
        }

        [Command(requiresAuthority = false)]
        private void CmdOnRobotEjected(Identity robot, string description, NetworkConnectionToClient sender = null)
        {
            var localRobot = EntityManager.Instance().RobotByConnection(sender);
            if (localRobot.id.role != Identity.Roles.Judge) return;
            Dispatcher.Instance().Send(new Ejected
            {
                target = robot,
                Description = description
            });
        }

        /// <summary>
        /// 裁判给予判罚。
        /// </summary>
        /// <param name="robot">目标机器人</param>
        /// <param name="description">判罚描述</param>
        private void OnPenaltyGiven(Identity robot, string description)
        {
            _penaltyPanel.EndSession();
            CmdOnPenaltyGiven(robot, description);
        }

        /// <summary>
        /// 发送判罚事件。
        /// </summary>
        /// <param name="robot">目标机器人</param>
        /// <param name="description">判罚描述</param>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        private void CmdOnPenaltyGiven(Identity robot, string description, NetworkConnectionToClient sender = null)
        {
            var localRobot = EntityManager.Instance().RobotByConnection(sender);
            if (localRobot.id.role != Identity.Roles.Judge) return;
            Dispatcher.Instance().Send(new Penalty
            {
                target = robot,
                Description = description
            });
        }

        /// <summary>
        /// 裁判给予判负。
        /// </summary>
        /// <param name="winner">胜利阵营</param>
        /// <param name="description">判负描述</param>
        private void OnGameOver(Identity.Camps winner, string description)
        {
            _penaltyPanel.EndSession();
            CmdOnGameOver(winner, description);
        }

        /// <summary>
        /// 发送判负事件。
        /// </summary>
        /// <param name="winner">胜利阵营</param>
        /// <param name="description">判负描述</param>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        private void CmdOnGameOver(Identity.Camps winner, string description, NetworkConnectionToClient sender = null)
        {
            var localRobot = EntityManager.Instance().RobotByConnection(sender);
            if (localRobot.id.role != Identity.Roles.Judge) return;
            Dispatcher.Instance().Send(new GameOver
            {
                WinningCamp = winner,
                Description = description
            });
        }

        protected override void LoadUI()
        {
            base.LoadUI();
            _mechanicSelectionPanel = UI<MechanicSelectionPanel>();
            _penaltyPanel = UI<PenaltyPanel>();
            _deadPanel = UI<DeadPanel>();
            _droneCoverPanel = UI<DroneCoverPanel>();
            _announcementPanel = UI<AnnouncementPanel>();
            _resultPanel = UI<ResultPanel>();
            _abnormalPanel = UI<AbnormalPanel>();
            _overheatPanel = UI<OverheatPanel>();
            _hurtPanel = UI<HurtPanel>();
            _warningPanel = UI<WarningPanel>();
            _overlayPanel = UI<OverlayPanel>();
            _pausePanel = UI<CustomizePanel>();
            _droneWarningPanel = UI<DroneWarningPanel>();
            _operatorPanelUc = UI<OperatorPanelUc>();
            _judgePanelUc = UI<JudgePanelUc>();
            _operatorPanelUl = UI<OperatorPanelUl>();
            _judgePanelUl = UI<JudgePanelUl>();
            _ucResultPanel = UI<UCResultPanel>();
        }

        private IEnumerator DelayGameOver(string winningCamp, string description)
        {
            yield return new WaitForSeconds(0.5f);
            if (EntityManager.Instance().LocalPlayer().localRobot == LocalRobot)
            {
                var winner = (Identity.Camps)Enum.Parse(typeof(Identity.Camps), winningCamp);
                if (LocalRobot != null)
                {
                    if (EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
                    {
                        if (winner == Identity.Camps.Other)
                            _resultPanel.StartSession(I18N.instance.getValue("^tie"), description, winner);
                        else
                        {
                            if (LocalRobot.id.IsRobot())
                            {
                                _resultPanel.StartSession(
                                    I18N.instance.getValue(LocalRobot.id.camp == winner ? "^victory" : "^failure"),
                                    description,
                                    winner);
                            }
                            else
                            {
                                _resultPanel.StartSession(
                                    I18N.instance.getValue(winner == Identity.Camps.Blue ? "^blue_win" : "^red_win"),
                                    description,
                                    winner);
                            }
                        }
                    }
                    else if (EntityManager.Instance().CurrentMap() == MapType.RMUC2022)
                    {
                        if (winner == Identity.Camps.Other)
                            _ucResultPanel.StartSession(I18N.instance.getValue("^tie"), description, winner);
                        else
                        {
                            if (LocalRobot.id.IsRobot())
                            {
                                _ucResultPanel.StartSession(
                                    I18N.instance.getValue(LocalRobot.id.camp == winner ? "^victory" : "^failure"),
                                    description,
                                    winner);
                            }
                            else
                            {
                                _ucResultPanel.StartSession(
                                    I18N.instance.getValue(winner == Identity.Camps.Blue ? "^blue_win" : "^red_win"),
                                    description,
                                    winner);
                            }
                        }
                    }
                }
            }
        }
    }
}