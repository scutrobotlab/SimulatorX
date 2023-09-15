using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Networking;
using Infrastructure;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UL
{
    /// <summary>
    /// 一个 ID 与一个血条对象绑定,针对于裁判视角上方组件的显示。
    /// </summary>
    [Serializable]
    public class HealthBarBindingForUpPanel
    {
        public Identity id;
        public Image healthBar;
    }

    [Serializable]
    public class TextBindingForJudge
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
    public class LevelBindingForJudge
    {
        public Identity id;
        public Image level;
    }

    /// <summary>
    /// 用于控制裁判界面上的全局信息显示以及各机器人与建筑的血条跟随显示。
    /// </summary>
    public class JudgePanelUl : MonoBehaviour
    {
        [Header("双方队名")] public TMP_Text redTeam;
        public TMP_Text blueTeam;

        [Header("上方组件的其他属性")] public TMP_Text countDown;
        public TMP_Text redCoins;
        public TMP_Text blueCoins;

        [Header("上方组件的血条绑定")]
        public List<HealthBarBindingForUpPanel> healthBarBindings = new List<HealthBarBindingForUpPanel>();

        [Header("上方组件文字绑定")] public List<TextBindingForJudge> textBindings = new List<TextBindingForJudge>();

        [Header("等级绑定")] public List<LevelBindingForJudge> levelBindings = new List<LevelBindingForJudge>();
        public List<Sprite> levelImages = new List<Sprite>();

        [Header("能量组件")] public Image energyBar;
        public TMP_Text energyCountdown;
        public Image redEnergyBar;
        public TMP_Text redEnergyValue;
        public TMP_Text redMedicalKit;
        public Image blueEnergyBar;
        public TMP_Text blueEnergyValue;
        public TMP_Text blueMedicalKit;

        [Header("AR血条预制件")] public GameObject arHealthBarPrefab;

        public CanvasScaler canvasScaler;

        private readonly Dictionary<Identity, ARHealthBar> _arHealthBars = new Dictionary<Identity, ARHealthBar>();

        /// <summary>
        /// 初始化上方组件与跟随器的血条状态为0，且文字为空，并初始化相机。
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
                    var localRobot = EntityManager.Instance().Ref(EntityManager.Instance().LocalRobot());
                    // 遍历所有引用
                    foreach (var camp in new List<Identity.Camps> {Identity.Camps.Red, Identity.Camps.Blue})
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
                                    var robotStore = (RobotStoreBase) reference;
                                    //上方组件的血条伸缩
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
                                            var baseStore = (BaseStore) reference;
                                            binding.healthBar.fillAmount = (baseStore.health + baseStore.shield) /
                                                                           baseStore.initialHealth;
                                            break;
                                        }
                                        case Identity.Roles.Outpost:
                                        {
                                            var outpostStore = (OutpostStore) reference;
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
                                        case TextBindingForJudge.ContentType.Health:
                                            if (id.IsRobot())
                                            {
                                                binding.text.text =
                                                    ((int) ((RobotStoreBase) reference).health).ToString();
                                            }
                                            else
                                            {
                                                switch (id.role)
                                                {
                                                    case Identity.Roles.Outpost:
                                                        binding.text.text =
                                                            ((int) ((OutpostStore) reference).health).ToString();
                                                        break;
                                                    case Identity.Roles.Base:
                                                        var baseStore = (BaseStore) reference;
                                                        binding.text.text = baseStore.health + (baseStore.shield > 0
                                                            ? " + " + baseStore.shield
                                                            : "");
                                                        break;
                                                }
                                            }

                                            break;
                                        case TextBindingForJudge.ContentType.LargeAmmo:
                                            if (id.IsRobot())
                                            {
                                                binding.text.text = ((RobotStoreBase) reference).magLarge.ToString();
                                            }

                                            break;
                                        case TextBindingForJudge.ContentType.SmallAmmo:
                                            if (id.IsRobot())
                                            {
                                                binding.text.text = ((RobotStoreBase) reference).magSmall.ToString();
                                            }

                                            break;
                                    }
                                }
                            }
                            
                            if (levelBindings.Any(l => l.id == id))
                            {
                                var level = ((RobotStoreBase) reference).level;
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
                            
                            if (id.IsRobot() || id.role == Identity.Roles.Outpost)
                            {
                                if (id.role != Identity.Roles.Drone)
                                {
                                    var scaler = new Vector2(
                                        canvasScaler.referenceResolution.x / Screen.width,
                                        canvasScaler.referenceResolution.y / Screen.height
                                    );

                                    if (!_arHealthBars.ContainsKey(id))
                                    {
                                        var newHealthBar = Instantiate(arHealthBarPrefab, transform, true);
                                        _arHealthBars[id] = newHealthBar.GetComponent<ARHealthBar>();
                                    }

                                    _arHealthBars[id].Display(((RobotStoreBase) localRobot).fpvCamera, reference,
                                        scaler);
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
                    var roomManager = (NetworkRoomManagerExt) NetworkManager.singleton;
                    redTeam.text = roomManager.redTeam;
                    blueTeam.text = roomManager.blueTeam;

                    // 能量
                    if (EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
                    {
                        energyBar.gameObject.SetActive(true);
                        var energyStore = CentralBuffStore.Instance();
                        redEnergyBar.fillAmount = energyStore.Energy[Identity.Camps.Red] / 100;
                        blueEnergyBar.fillAmount = energyStore.Energy[Identity.Camps.Blue] / 100;
                        redEnergyValue.text = ((int) energyStore.Energy[Identity.Camps.Red]).ToString();
                        blueEnergyValue.text = ((int) energyStore.Energy[Identity.Camps.Blue]).ToString();
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