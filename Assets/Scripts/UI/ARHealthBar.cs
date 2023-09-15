using System;
using Controllers;
using Gameplay;
using Gameplay.Attribute;
using Infrastructure;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// AR 血条组件。
    /// </summary>
    public class ARHealthBar : MonoBehaviour
    {
        public Identity owner;

        [Header("元素")] public TMP_Text serial;
        public Image healthBar;
        public Image level;

        [Header("素材")] public Sprite blueHealth;
        public Sprite redHealth;
        public Sprite level1;
        public Sprite level2;
        public Sprite level3;

        /// <summary>
        /// 更新 AR 血条状态。
        /// </summary>
        /// <param name="fpvCamera">观察摄像机</param>
        /// <param name="store">目标实体</param>
        /// <param name="scaler">缩放系数</param>
        /// <exception cref="Exception"></exception>
        public void Display(Camera fpvCamera, StoreBase store, Vector2 scaler)
        {
            owner = store.id;

            var visible = IfVisualOnTarget.HasVisualOnTarget(fpvCamera, store.gameObject, false);
            GetComponent<RawImage>().enabled = visible;
            healthBar.enabled = visible;
            level.enabled = visible;
            if (!visible) serial.text = "";
            if (!visible) return;

            //进行伸缩血条AR的核心算法
            var storePos = store.transform.position;
            var screenPos = fpvCamera.WorldToScreenPoint(storePos + Vector3.up * 0.7f);
            screenPos.x *= scaler.x;
            screenPos.y *= scaler.y;
            var heightScale = Screen.height / (Screen.width / 16f * 9);
            screenPos.y *= heightScale;
            GetComponent<RectTransform>().anchoredPosition = screenPos;
            GetComponent<RectTransform>().localScale =
                5 / (fpvCamera.transform.position - storePos).magnitude * Vector3.one;

            healthBar.sprite = owner.camp switch
            {
                Identity.Camps.Red => redHealth,
                Identity.Camps.Blue => blueHealth,
                _ => redHealth
            };

            if (store is RobotStoreBase robotStore)
            {
                serial.text = store.id.order.ToString();
                healthBar.fillAmount = robotStore.health /
                                       AttributeManager.Instance()
                                           .RobotAttributes(robotStore)
                                           .MaxHealth;
                level.sprite = robotStore.level switch
                {
                    1 => level1,
                    2 => level2,
                    3 => level3,
                    _ => level1
                };
                level.transform.localScale = robotStore.level switch
                {
                    1 => new Vector3(1, 0.6f, 1),
                    2 => new Vector3(1, 0.8f, 1),
                    3 => Vector3.one,
                    _ => new Vector3(1, 0.6f, 1)
                };
            }
            else
            {
                if (store is OutpostStore outpostStore)
                {
                    serial.text = "O";
                    healthBar.fillAmount = outpostStore.health / outpostStore.initialHealth;
                    level.sprite = level1;
                }
            }
        }
    }
}