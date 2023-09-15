using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Networking;
using Honeti;
using Infrastructure;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UL
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
            SmallAmmo
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

    /// <summary>
    /// 用于控制操作手界面上的血条、数值显示。
    /// </summary>
    public class OperatorPanelUl : MonoBehaviour
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
        public List<Sprite> levelImages = new List<Sprite>();

        [Header("能量组件")] public Image energyBar;
        public TMP_Text energyCountdown;
        public Image redEnergyBar;
        public TMP_Text redEnergyValue;
        public TMP_Text redMedicalKit;
        public Image blueEnergyBar;
        public TMP_Text blueEnergyValue;
        public TMP_Text blueMedicalKit;

        [Header("热量条")] public Image heatBar;

        [Header("本征组件整体")] public GameObject localRobotInfo;

        /// <summary>
        /// 初始状态下血条为 0，文字为空。
        /// </summary>
        private void Start()
        {
            foreach (var binding in healthBarBindings)
            {
                binding.healthBar.fillAmount = 0;
            }

            foreach (var binding in textBindings)
            {
                binding.text.text = "";
            }

            foreach (var binding in levelBindings)
            {
                binding.level.color = new Color(0, 0, 0, 0);
            }
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
                                                binding.text.text =
                                                    id.camp == EntityManager.Instance().LocalRobot().camp
                                                        ? ((RobotStoreBase)reference).magSmall.ToString()
                                                        : "?";
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

                    var localRobot = EntityManager.Instance().LocalRobot();
                    var localRobotRef = (RobotStoreBase)EntityManager.Instance().Ref(localRobot);

                    // 发射机构信息
                    if (localRobotRef.Guns.Count > 0)
                    {
                        velocity.text =
                            AttributeManager.Instance().GunAttributes(localRobotRef).MaxMuzzleVelocity +
                            " m/s";
                        var currentBulletText = localRobotRef.Guns[0].caliber == MechanicType.CaliberType.Large
                            ? localRobotRef.launchedLarge.ToString()
                            : localRobotRef.launchedSmall.ToString();
                        var totalBulletText = localRobotRef.Guns[0].caliber == MechanicType.CaliberType.Large
                            ? localRobotRef.totalMagLarge.ToString()
                            : localRobotRef.totalMagSmall.ToString();
                        bullets.text = currentBulletText + " / " + totalBulletText;
                    }
                    else
                    {
                        velocity.text = I18N.instance.getValue("^disabled");
                        bullets.text = I18N.instance.getValue("^disabled");
                    }

                    // 热量信息    
                    if (localRobot.IsGroundRobot() && localRobot.role != Identity.Roles.Engineer)
                    {
                        heatBar.fillAmount = localRobotRef.GetHeat(0) / localRobotRef.GetHeatLimit(0);
                    }
                    else
                    {
                        heatBar.fillAmount = 0;
                    }

                    heatBar.color = new Color(heatBar.fillAmount, 0, 0);

                    // 本征信息
                    if (localRobot.role == Identity.Roles.Drone)
                    {
                        localRobotInfo.SetActive(false);
                    }
                    else
                    {
                        localRobotInfo.SetActive(true);
                        localHealthText.text = ((int)localRobotRef.health).ToString();
                        localHealth.fillAmount = localRobotRef.health /
                                                 AttributeManager
                                                     .Instance()
                                                     .RobotAttributes(localRobotRef)
                                                     .MaxHealth;
                        localHealth.color = localRobot.camp == Identity.Camps.Red
                            ? new Color(1, 0, 0)
                            : new Color(0, 0.55f, 1);
                        localLevel.sprite = localRobotRef.level switch
                        {
                            1 => levelImages[0],
                            2 => levelImages[1],
                            3 => levelImages[2],
                            _ => levelImages[0]
                        };
                        localSerial.text = localRobot.order.ToString();
                        localExperience.fillAmount = localRobotRef.experience /
                                                     AttributeManager
                                                         .Instance()
                                                         .RobotAttributes(localRobotRef)
                                                         .ExperienceToUpgrade;
                    }

                    heroAvatar.enabled = localRobot.role == Identity.Roles.Hero;
                    infantryAvatar.enabled = (localRobot.role == Identity.Roles.Infantry ||
                                              localRobot.role == Identity.Roles.BalanceInfantry);
                    engineerAvatar.enabled = localRobot.role == Identity.Roles.Engineer;

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
                }
            }
        }
    }
}