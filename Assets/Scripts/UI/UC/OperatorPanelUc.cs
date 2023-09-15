using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Networking;
using Honeti;
using Infrastructure;
using Infrastructure.Input;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UC
{
    /// <summary>
    /// 一个 ID 与一个血条对象绑定。
    /// </summary>
    [Serializable]
    public class HealthBarBinding
    {
        public Identity id;
        public Image healthBar;
    }

    [Serializable]
    public class TextBinding
    {
        public enum ContentType
        {
            Health,
            LargeAmmo,
            SmallAmmo,
            Time
        }

        public Identity id;
        public TMP_Text text;
        public ContentType type;
    }

    [Serializable]
    public class LevelBinding
    {
        public Identity id;
        public Image level;
    }

    //小地图
    [Serializable]
    public class MapRobot
    {
        public Identity id;
        public RawImage image;
        public TMP_Text serial;

        public void SetColor(Color col)
        {
            image.color = col;
            image.gameObject.SetActive(true);
        }

        public void SetAct(bool bo)
        {
            image.enabled = bo;
            serial.enabled = bo;
        }
    }

    /// <summary>
    /// 用于控制操作手界面上的血条、数值显示。
    /// </summary>
    public class OperatorPanelUc : MonoBehaviour
    {
        [Header("其他属性")] public TMP_Text countDown;
        public TMP_Text redCoins;
        public TMP_Text blueCoins;
        public TMP_Text velocity;
        public TMP_Text bullets;
        public Image localHealth;
        public TMP_Text localHealthText;
        public Image localLevel;
        public Image localExperience;
        public TMP_Text localSerial;

        [Header("双方队名")] public TMP_Text redTeam;
        public TMP_Text blueTeam;

        [Header("血条和文字绑定")] public List<HealthBarBinding> healthBarBindings = new List<HealthBarBinding>();
        public List<TextBinding> textBindings = new List<TextBinding>();

        [Header("等级绑定")] public List<LevelBinding> levelBindings = new List<LevelBinding>();

        [Header("动态替换")] public Image heroAvatar;
        public Image infantryAvatar;
        public Image engineerAvatar;
        public Image droneAvatar;
        public List<Sprite> levelImages = new List<Sprite>();

        [Header("能量组件")] public Image energyBar;
        public TMP_Text energyCountdown;
        public Image redEnergyBar;
        public TMP_Text redEnergyValue;
        public TMP_Text redMedicalKit;
        public Image blueEnergyBar;
        public TMP_Text blueEnergyValue;
        public TMP_Text blueMedicalKit;

        [Header("超级电容")] public TMP_Text batteryCapacity;
        [Header("热量条")] public Image heatBar;
        [Header("工程取矿进度条")] public Image takeBar;
        [Header("工程兑换进度条")] public Image exchangeBar;

        [Header("本征组件整体")] public GameObject localRobotInfo;

        [Header("大能量机关")] public Image bigEnergy;

        [Header("小能量机关")] public Image smallEnergy;

        [Header("飞坡增益")] public Image fly;

        public Image flyBar;

        [Header("冷却增益")] public Image cooling;

        [Header("攻击增益")] public Image fight;
        public Image fightBar;
        [Header("防御增益")] public Image defend;
        public Image defendBar;

        [Header("飞镖打击")] public Image dartAttack;

        [Header("无人机时间子弹")] public Slider restTime;

        public Slider restBullet;
        public TextMeshProUGUI strRestTime;
        public TextMeshProUGUI strRestBullet;

        [Header("虚拟护盾扣除时间")] public Image warning;

        public TextMeshProUGUI remainTime;

        [Header("功率")] public Slider powerSlider;
        public TMP_Text powerText;

        //小地图
        public List<MapRobot> mapRobots = new List<MapRobot>();
        public RawImage map;
        public bool mapActivated = true;
        private List<StoreBase> _campRobot;
        private bool _dartAttackFlag = true;
        private float _droneCountDown;
        private float _droneCountDownBlue;
        private float _droneCountDownRed;


        private bool _flyflag = true;
        private float _freeCountDown;
        private float _freeCountDownBlue;
        private float _freeCountDownRed;

        private Identity _localRobot;
        private RobotStoreBase _localRobotRef;
        private bool _powerRuneflag = true;
        private float _warningCount = 30;

        /// <summary>
        /// 初始状态下血条为 0，文字为空。
        /// </summary>
        private void Start()
        {
            foreach (var binding in healthBarBindings)
            {
                binding.healthBar.fillAmount = 0;
            }

            takeBar.fillAmount = 0;
            exchangeBar.fillAmount = 0;

            foreach (var binding in textBindings)
            {
                binding.text.text = "";
            }

            foreach (var binding in levelBindings)
            {
                binding.level.color = new Color(0, 0, 0, 0);
            }

            foreach (var mr in mapRobots) mr.image.gameObject.SetActive(false);
            map.enabled = true;
            _freeCountDown = 175;
        }

        /// <summary>
        /// 在客户端根据 ID 更新血条长度。
        /// </summary>
        private void FixedUpdate()
        {
            if (NetworkClient.active)
            {
                if (EntityManager.Instance() != null && EntityManager.Instance().initialized)
                {
                    // 遍历所有引用
                    foreach (var camp in new List<Identity.Camps> { Identity.Camps.Red, Identity.Camps.Blue })
                    {
                        foreach (var reference in EntityManager.Instance().CampRef(camp))
                        {
                            var id = reference.id;
                            if (healthBarBindings.Any(h => h.id == id))
                            {
                                var binding = healthBarBindings.First(h => h.id == id);

                                // 机器人血量上限从 AttributeManager 获取
                                if (id.IsRobot())
                                {
                                    var robotStore = (RobotStoreBase)reference;
                                    binding.healthBar.fillAmount =
                                        robotStore.health / AttributeManager.Instance()
                                            .RobotAttributes(robotStore).MaxHealth;
                                }
                                else
                                {
                                    // 建筑物血量上限自行记录
                                    switch (id.role)
                                    {
                                        case Identity.Roles.Base:
                                        {
                                            var baseStore = (BaseStore)reference;
                                            binding.healthBar.fillAmount = (baseStore.health + baseStore.shield) /
                                                                           baseStore.initialHealth;
                                            break;
                                        }
                                        case Identity.Roles.Outpost:
                                        {
                                            var outpostStore = (OutpostStore)reference;
                                            binding.healthBar.fillAmount =
                                                outpostStore.health / outpostStore.initialHealth;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (textBindings.Any(t => t.id == id))
                            {
                                foreach (var binding in textBindings.FindAll(t => t.id == id))
                                {
                                    switch (binding.type)
                                    {
                                        case TextBinding.ContentType.Health:
                                            if (id.IsRobot())
                                            {
                                                binding.text.text =
                                                    ((int)((RobotStoreBase)reference).health).ToString();
                                            }
                                            else
                                            {
                                                switch (id.role)
                                                {
                                                    case Identity.Roles.Outpost:
                                                        binding.text.text =
                                                            ((int)((OutpostStore)reference).health).ToString();
                                                        break;
                                                    case Identity.Roles.Base:
                                                        var baseStore = (BaseStore)reference;
                                                        binding.text.text = baseStore.health + (baseStore.shield > 0
                                                            ? " + " + baseStore.shield
                                                            : "");
                                                        break;
                                                }
                                            }

                                            break;
                                        case TextBinding.ContentType.LargeAmmo:
                                            if (id.IsRobot())
                                            {
                                                binding.text.text =
                                                    id.camp == EntityManager.Instance().LocalRobot().camp
                                                        ? ((RobotStoreBase)reference).magLarge.ToString()
                                                        : "?";
                                            }

                                            break;
                                        case TextBinding.ContentType.SmallAmmo:
                                            if (id.IsRobot())
                                            {
                                                // binding.text.text =
                                                //     id.camp == EntityManager.Instance().LocalRobot().camp
                                                //         ? ((RobotStoreBase) reference).magSmall.ToString()
                                                //         : "?";
                                                binding.text.text = ((RobotStoreBase)reference).magSmall.ToString();
                                            }

                                            break;
                                        case TextBinding.ContentType.Time:
                                            if (id.role == Identity.Roles.Drone)
                                            {
                                                var drone = (DroneStore)reference;
                                                //Debug.Log("drone id "+drone.id.camp+" fly "+drone.droneFly);
                                                if (binding.id.camp == Identity.Camps.Red)
                                                {
                                                    if (drone.droneFly)
                                                    {
                                                        _droneCountDownRed -= Time.deltaTime;
                                                        binding.text.text = $"{(int)_droneCountDownRed}s";
                                                        _freeCountDownRed = 175;
                                                    }
                                                    else if (drone.freeCount)
                                                    {
                                                        if (_freeCountDownRed > 0) _freeCountDownRed -= Time.deltaTime;
                                                        if (_freeCountDownRed < 0) _freeCountDownRed = 0;
                                                        binding.text.text = $"{(int)_freeCountDownRed}s";
                                                        _droneCountDownRed = 30;
                                                    }
                                                    else
                                                    {
                                                        _droneCountDownRed = 30;
                                                        _freeCountDownRed = 175;
                                                    }
                                                }
                                                else if (binding.id.camp == Identity.Camps.Blue)
                                                {
                                                    if (drone.droneFly)
                                                    {
                                                        _droneCountDownBlue -= Time.deltaTime;
                                                        binding.text.text = $"{(int)_droneCountDownBlue}s";
                                                        _freeCountDownBlue = 175;
                                                    }
                                                    else if (drone.freeCount)
                                                    {
                                                        if (_freeCountDownBlue > 0)
                                                            _freeCountDownBlue -= Time.deltaTime;
                                                        if (_freeCountDownBlue < 0) _freeCountDownBlue = 0;
                                                        binding.text.text = $"{(int)_freeCountDownBlue}s";
                                                        _droneCountDownBlue = 30;
                                                    }
                                                    else
                                                    {
                                                        _droneCountDownBlue = 30;
                                                        _freeCountDownBlue = 175;
                                                    }
                                                }
                                            }

                                            if (id.IsGroundRobot())
                                            {
                                                var robotStore = (RobotStoreBase)reference;
                                                if (robotStore.isDead)
                                                {
                                                    binding.text.text =
                                                        $"{robotStore.revivalProcessRequired + robotStore.requiredAdded - robotStore.revivalProcess}s";
                                                }
                                                else
                                                {
                                                    binding.text.text = "";
                                                }
                                            }

                                            break;
                                    }
                                }
                            }

                            if (levelBindings.Any(l => l.id == id))
                            {
                                var level = ((RobotStoreBase)reference).level;
                                foreach (var binding in levelBindings.FindAll(l => l.id == id))
                                {
                                    binding.level.sprite = levelImages[level - 1];
                                    binding.level.color = Color.white;
                                    binding.level.transform.localScale = level switch
                                    {
                                        1 => new Vector3(1, 0.6f, 1),
                                        2 => new Vector3(1, 0.8f, 1),
                                        3 => Vector3.one,
                                        _ => new Vector3(1, 0.6f, 1)
                                    };
                                }
                            }
                        }
                    }

                    // 其他属性
                    // 倒计时
                    var gameCountDown = ClockStore.Instance().countDown;
                    if (gameCountDown < 0) gameCountDown = 0;
                    var minute = gameCountDown / 60;
                    var second = gameCountDown - minute * 60;
                    var secondText = second < 10 ? "0" + second : second.ToString();
                    var countDownText = minute + ":" + secondText;
                    countDown.text = countDownText;
                    if (gameCountDown <= 10 && ClockStore.Instance().playing) countDown.color = Color.red;

                    // 经济
                    redCoins.text = CoinStore.Instance().CampCoinDescription(Identity.Camps.Red);
                    blueCoins.text = CoinStore.Instance().CampCoinDescription(Identity.Camps.Blue);

                    // 队名
                    var roomManager = (NetworkRoomManagerExt)NetworkManager.singleton;
                    redTeam.text = roomManager.redTeam;
                    blueTeam.text = roomManager.blueTeam;

                    _localRobot = EntityManager.Instance().LocalRobot();

                    _localRobotRef = (RobotStoreBase)EntityManager.Instance().Ref(_localRobot);

                    _campRobot ??= EntityManager.Instance().CampRef(_localRobot.camp);


                    // 发射机构信息
                    if (_localRobotRef.Guns.Count > 0)
                    {
                        velocity.text =
                            AttributeManager.Instance().GunAttributes(_localRobotRef).MaxMuzzleVelocity +
                            " m/s";
                        var currentBulletText = _localRobotRef.Guns[0].caliber == MechanicType.CaliberType.Large
                            ? _localRobotRef.launchedLarge.ToString()
                            : _localRobotRef.launchedSmall.ToString();
                        var totalBulletText = _localRobotRef.Guns[0].caliber == MechanicType.CaliberType.Large
                            ? _localRobotRef.totalMagLarge.ToString()
                            : _localRobotRef.totalMagSmall.ToString();
                        if (_localRobotRef.id.role == Identity.Roles.AutoSentinel)
                            totalBulletText = "750";
                        bullets.text = currentBulletText + " / " + totalBulletText;
                    }
                    else
                    {
                        velocity.text = I18N.instance.getValue("^disabled");
                        bullets.text = I18N.instance.getValue("^disabled");
                    }

                    // 热量信息及工程取矿兑换进度信息    
                    if (_localRobot.IsGroundRobot())
                    {
                        if (_localRobot.role == Identity.Roles.Engineer)
                        {
                            var engineerStore = (EngineerStore)_localRobotRef;
                            if (engineerStore.isTaking)
                            {
                                takeBar.fillAmount = (float)((NetworkTime.time -
                                                              engineerStore.pressedTime) /
                                                             engineerStore.takeOreRequiredTime);
                            }
                            else if (!engineerStore.isTaking)
                            {
                                takeBar.fillAmount = 0;
                            }

                            if (engineerStore.isExchanging)
                            {
                                exchangeBar.fillAmount = (float)((NetworkTime.time -
                                                                  engineerStore.pressedTime) /
                                                                 engineerStore.exchangeOreRequiredTime);
                            }
                            else if (!engineerStore.isExchanging)
                            {
                                exchangeBar.fillAmount = 0;
                            }
                        }
                        else
                        {
                            heatBar.fillAmount = _localRobotRef.GetHeat(0) / _localRobotRef.GetHeatLimit(0);
                        }
                    }

                    // 功率，图一乐吧
                    var power = _localRobotRef.GetPower();
                    var maxPower = AttributeManager.Instance().RobotAttributes(_localRobotRef).MaxChassisPower;

                    powerText.text = power.ToString("0.00") + "w";
                    powerSlider.value = power / maxPower;

                    //无人机信息更新
                    if (_localRobot.role == Identity.Roles.Drone)
                    {
                        var drone = (DroneStore)_localRobotRef;
                        if (drone.droneFly)
                        {
                            restBullet.value = (float)_localRobotRef.magSmall / 500;
                            strRestBullet.text = $"{_localRobotRef.magSmall}/500";
                            _droneCountDown -= Time.deltaTime;
                            restTime.value = _droneCountDown / 30;
                            strRestTime.text = $"{(int)_droneCountDown}s/30s";
                            _freeCountDown = 175;
                        }
                        else if (drone.freeCount)
                        {
                            if (_freeCountDown > 0) _freeCountDown -= Time.deltaTime;
                            if (_freeCountDown < 0) _freeCountDown = 0;
                            restBullet.value = 0;
                            strRestBullet.text = "0/500";
                            strRestTime.text = $"{(int)_freeCountDown}s/175s";
                            restTime.value = _freeCountDown / 175;
                            _droneCountDown = 30;
                        }
                        else
                        {
                            _droneCountDown = 30;
                            _freeCountDown = 175;
                        }

                        if (_localRobotRef.HasEffect(EffectID.Status.LeavePatrol))
                        {
                            var baseRef = (BaseStore)EntityManager.Instance()
                                .Ref(new Identity(_localRobotRef.id.camp, Identity.Roles.Base));
                            if (!baseRef.invincible)
                            {
                                var sentinelRef = (RobotStoreBase)EntityManager.Instance()
                                    .Ref(new Identity(_localRobotRef.id.camp, Identity.Roles.AutoSentinel));

                                if (sentinelRef.health > 0 && baseRef.shield > 0)
                                {
                                    warning.gameObject.SetActive(true);
                                    _warningCount = _warningCount - Time.deltaTime > 0
                                        ? _warningCount - Time.deltaTime
                                        : 0;
                                    remainTime.text = $"{(int)_warningCount}s";
                                }
                                else
                                    warning.gameObject.SetActive(false);
                            }
                        }

                        if (warning.IsActive() && !_localRobotRef.HasEffect(EffectID.Status.LeavePatrol))
                        {
                            warning.gameObject.SetActive(false);
                            _warningCount = 30;
                        }
                    }

                    heatBar.color = new Color(heatBar.fillAmount, 0, 0);

                    //增益UI显示:
                    //飞坡增益
                    if (_localRobotRef.HasEffect(EffectID.Buffs.LaunchRamp))
                    {
                        if (_flyflag)
                        {
                            GameObject o;
                            (o = fly.gameObject).SetActive(true);
                            flyBar.fillAmount = 1;
                            StartCoroutine(CountDownRoutine(20, EffectID.Buffs.LaunchRamp, o));
                        }
                    }

                    //冷却增益
                    if (_localRobotRef.HasEffect(EffectID.Buffs.Base)
                        || _localRobotRef.HasEffect(EffectID.Buffs.Outpost)
                        || _localRobotRef.HasEffect(EffectID.Buffs.HighlandCoin)
                        || _localRobotRef.HasEffect(EffectID.Buffs.HighlandCool)
                        || _localRobotRef.HasEffect(EffectID.Buffs.Snipe))
                    {
                        cooling.gameObject.SetActive(true);
                    }
                    else
                    {
                        cooling.gameObject.SetActive(false);
                    }

                    //大能量机关提示
                    if (_localRobotRef.HasEffect(EffectID.Buffs.LargePowerRune))
                    {
                        if (_powerRuneflag)
                        {
                            fight.gameObject.SetActive(true);
                            defend.gameObject.SetActive(true);
                            GameObject o;
                            (o = bigEnergy.gameObject).SetActive(true);
                            StartCoroutine(CountDownRoutine(1, EffectID.Buffs.LargePowerRune, o));
                            StartCoroutine(CountDownRoutine(45, null, fight.gameObject));
                            StartCoroutine(CountDownRoutine(45, null, defend.gameObject));
                        }
                    }

                    //小能量机关提示
                    if (_localRobotRef.HasEffect(EffectID.Buffs.SmallPowerRune))
                    {
                        if (_powerRuneflag)
                        {
                            fight.gameObject.SetActive(true);
                            GameObject o;
                            (o = smallEnergy.gameObject).SetActive(true);
                            StartCoroutine(CountDownRoutine(1, EffectID.Buffs.SmallPowerRune, o));
                            StartCoroutine(CountDownRoutine(45, null, fight.gameObject));
                        }
                    }

                    //超级电容显示
                    if (_localRobot.IsGroundRobot() && _localRobot.role != Identity.Roles.Engineer)
                    {
                        //开启超级电容，若检测到机器人正在运动或是开启小陀螺则为消耗状态
                        if (_localRobotRef.superBattery)
                        {
                            //机器人在运动
                            if (_localRobotRef.spinning || _localRobotRef.isMoving)
                            {
                                if (_localRobotRef.remainingCapacity > 0)
                                {
                                    _localRobotRef.remainingCapacity -= _localRobotRef.batteryConsumeSpeed *
                                                                        (NetworkTime.time - _localRobotRef.startTime);
                                    if (_localRobotRef.remainingCapacity < 0)
                                        InputManager.Instance().ButtonStatus[InputActionID.FunctionJ] = false;
                                    batteryCapacity.text = (int)_localRobotRef.remainingCapacity + "%";
                                }
                                else
                                {
                                    _localRobotRef.remainingCapacity = 0;
                                    InputManager.Instance().ButtonStatus[InputActionID.FunctionJ] = false;
                                    batteryCapacity.text = (int)_localRobotRef.remainingCapacity + "%";
                                }
                            }
                            //若机器人开启超级电容但未运动，也处于恢复状态,此时处于不运动恢复状态
                            else
                            {
                                if (_localRobotRef.remainingCapacity < 100)
                                {
                                    _localRobotRef.remainingCapacity += _localRobotRef.batteryRecoverSpeed *
                                                                        (NetworkTime.time -
                                                                         _localRobotRef.movingEndTime);
                                    batteryCapacity.text = (int)_localRobotRef.remainingCapacity + "%";
                                }
                                else
                                {
                                    _localRobotRef.remainingCapacity = _localRobotRef.batteryCapacity;
                                    batteryCapacity.text = (int)_localRobotRef.remainingCapacity + "%";
                                }
                            }
                        }
                        //关闭超级电容，处于恢复状态，为关闭超级电容的恢复状态
                        else
                        {
                            //自主关闭电容的恢复
                            if (_localRobotRef.remainingCapacity < 100)
                            {
                                _localRobotRef.remainingCapacity += _localRobotRef.batteryRecoverSpeed *
                                                                    (NetworkTime.time - _localRobotRef.switchEndTime);
                                batteryCapacity.text = (int)_localRobotRef.remainingCapacity + "%";
                            }
                            else
                            {
                                _localRobotRef.remainingCapacity = _localRobotRef.batteryCapacity;
                                batteryCapacity.text = (int)_localRobotRef.remainingCapacity + "%";
                            }
                        }

                        batteryCapacity.enabled = true;
                    }
                    else
                    {
                        batteryCapacity.enabled = false;
                        // batteryCapacity.text = "电容不可用";
                    }

                    //飞镖击中白屏
                    if (_localRobotRef.HasEffect(EffectID.Status.DartAttack))
                    {
                        dartAttack.gameObject.SetActive(true);
                        if (_dartAttackFlag)
                        {
                            StartCoroutine(CountDownRoutine(3, EffectID.Status.DartAttack, dartAttack.gameObject));
                        }

                        _localRobotRef.RemoveEffect(EffectID.Status.DartAttack);
                    }

                    // 本征信息
                    if (_localRobot.role == Identity.Roles.Drone)
                    {
                        localRobotInfo.SetActive(false);

                        restTime.gameObject.SetActive(true);
                        restBullet.gameObject.SetActive(true);
                    }
                    else
                    {
                        localRobotInfo.SetActive(true);
                        localHealthText.text = ((int)_localRobotRef.health).ToString();
                        localHealth.fillAmount = _localRobotRef.health /
                                                 AttributeManager
                                                     .Instance()
                                                     .RobotAttributes(_localRobotRef)
                                                     .MaxHealth;
                        localHealth.color = _localRobot.camp == Identity.Camps.Red
                            ? new Color(1, 0, 0)
                            : new Color(0, 0.55f, 1);
                        localLevel.sprite = _localRobotRef.level switch
                        {
                            1 => levelImages[0],
                            2 => levelImages[1],
                            3 => levelImages[2],
                            _ => levelImages[0]
                        };
                        localSerial.text = _localRobot.order.ToString();
                        localExperience.fillAmount = _localRobotRef.experience /
                                                     AttributeManager
                                                         .Instance()
                                                         .RobotAttributes(_localRobotRef)
                                                         .ExperienceToUpgrade;
                    }

                    heroAvatar.enabled = _localRobot.role == Identity.Roles.Hero;
                    infantryAvatar.enabled = (_localRobot.role == Identity.Roles.Infantry
                                              || _localRobot.role == Identity.Roles.BalanceInfantry);
                    engineerAvatar.enabled = _localRobot.role == Identity.Roles.Engineer;
                    droneAvatar.enabled = _localRobot.role == Identity.Roles.Drone;

                    // 能量
                    if (EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
                    {
                        energyBar.gameObject.SetActive(true);
                        var energyStore = CentralBuffStore.Instance();
                        redEnergyBar.fillAmount = energyStore.Energy[Identity.Camps.Red] / 100;
                        blueEnergyBar.fillAmount = energyStore.Energy[Identity.Camps.Blue] / 100;
                        redEnergyValue.text = ((int)energyStore.Energy[Identity.Camps.Red]).ToString();
                        blueEnergyValue.text = ((int)energyStore.Energy[Identity.Camps.Blue]).ToString();
                        redMedicalKit.text = energyStore.MedicalKit[Identity.Camps.Red].ToString();
                        blueMedicalKit.text = energyStore.MedicalKit[Identity.Camps.Blue].ToString();
                        energyCountdown.text =
                            energyStore.Countdown() == 0 ? "" : energyStore.Countdown().ToString();
                    }
                    else
                    {
                        energyBar.gameObject.SetActive(false);
                    }

                    //小地图显示
                    if (mapActivated)
                        foreach (var robot in _campRobot)
                        {
                            if (!robot.id.IsGroundRobot() && robot.id.role != Identity.Roles.Drone) continue;
                            //将小地图图标与该阵营的地面机器人一一对应
                            var maprobot = mapRobots.First(m => m.id == robot.id);
                            maprobot.SetColor(_localRobot.camp == Identity.Camps.Blue ? Color.blue : Color.red);
                            if (_localRobotRef.health == 0)
                            {
                                maprobot.SetColor(Color.gray);
                            }

                            //坐标映射
                            var p = robot.transform.position;
                            if (robot.id.role == Identity.Roles.Drone)
                            {
                                maprobot.SetColor((Color.green));
                                p = ((DroneStore)robot).pos;
                            }

                            maprobot.image.rectTransform.anchoredPosition = new Vector2(
                                p.x / 13.6f * 50.0f - 50.0f,
                                p.z / 7.1f * 25f + 25f
                            );
                        }
                }
            }
        }

        public void MapControl(bool mapOn)
        {
            map.enabled = mapOn;
            foreach (var robot in _campRobot)
            {
                if (!robot.id.IsGroundRobot()) continue;
                //将小地图图标与该阵营的地面机器人一一对应
                var maprobot = mapRobots.First(m => m.id == robot.id);
                maprobot.SetAct(mapOn);
            }
        }

        /// <summary>
        /// 用于增益计时的协程
        /// </summary>
        /// <param name="waitSeconds">增益持续时间</param>
        /// <param name="type"></param>
        /// <param name="effect">增益图标</param>
        /// <returns></returns>
        private IEnumerator CountDownRoutine(int waitSeconds, string type = null, GameObject effect = null)
        {
            switch (type)
            {
                case EffectID.Buffs.LaunchRamp:
                    _flyflag = false;
                    break;

                case EffectID.Buffs.LargePowerRune:
                case EffectID.Buffs.SmallPowerRune:
                    _powerRuneflag = false;
                    break;
                case EffectID.Status.DartAttack:
                    _dartAttackFlag = false;
                    break;
            }

            yield return new WaitForSeconds(waitSeconds);
            if (effect != null)
            {
                effect.SetActive(false);
            }

            //45秒后重新可激活
            yield return new WaitForSeconds(45);
            _flyflag = true;
            _powerRuneflag = true;
        }
    }
}