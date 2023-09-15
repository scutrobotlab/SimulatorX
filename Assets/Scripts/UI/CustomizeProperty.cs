using System;
using System.Collections;
using Doozy.Runtime.UIManager.Components;
using Gameplay;
using Gameplay.Customize;
using Honeti;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 自定义数值控件。
    /// </summary>
    public class CustomizeProperty : MonoBehaviour
    {
        public string property;
        public TMP_Text valueDisplay;
        public UISlider slider;

        private IEnumerator Start()
        {
            // 据说不要把 OnValueChange() 放在 Start() 里面
            // 否则会导致 EntityManager 还没有初始化时调用引发异常
            // 于是将初始化工作转移到了下面的 Update() 里面
            // 2023.8.7 By lqw 改为延迟Start()
            yield return new WaitUntil(CustomizeManager.IsInit);
            if (CustomizeManager.Instance().IsServer()) yield break;
            yield return new WaitUntil(() => EntityManager.Instance()?.initialized == true);
            if(!CustomizeManager.Instance().Has(EntityManager.Instance().LocalRobot(), property))
            {
                // 开始时根据编辑器中设定的值存储默认值
                OnValueChange();
            }
        }

        /// <summary>
        /// 针对每种数值转换存储和更新显示。
        /// </summary>
        public void OnValueChange()
        {
            var value = slider.value;
            var calculatedValue = value;
            switch (property)
            {
                // 底盘
                case CustomizeProperties.Chassis.Spinning:
                    valueDisplay.text = Math.Round(value, 1) + $" {I18N.instance.getValue("^times")}";
                    break;
                case CustomizeProperties.Chassis.Velocity:
                    valueDisplay.text = Math.Round(value, 1) + $" {I18N.instance.getValue("^times")}";
                    break;
                case CustomizeProperties.Chassis.SuperBattery:
                    valueDisplay.text = Math.Round(value, 1) + $" {I18N.instance.getValue("^times")}";
                    break;
                case CustomizeProperties.Chassis.SensorDrift:
                    calculatedValue = (value - 1) * 60;
                    valueDisplay.text =
                        Math.Round(calculatedValue, 1) + $" {I18N.instance.getValue("^degree_pre_min")}";
                    break;

                // 发射机构
                case CustomizeProperties.Gun.MuzzleVelocity:
                    valueDisplay.text = Mathf.Round((value - 1) * 100) + " %";
                    break;
                case CustomizeProperties.Gun.HorizontalBallisticJitter:
                    valueDisplay.text = Mathf.Round((value - 1) * 100) + " %";
                    break;
                case CustomizeProperties.Gun.VerticalBallisticJitter:
                    valueDisplay.text = Mathf.Round((value - 1) * 100) + " %";
                    break;

                //工程模式
                case CustomizeProperties.EngnieerSet.Ore:
                    break;
                case CustomizeProperties.EngnieerSet.Exchanging:
                    break;


                // 辅助瞄准
                case CustomizeProperties.Aimbot.Stability:
                    valueDisplay.text = Mathf.Round(value * 100) + " %";
                    break;
                case CustomizeProperties.Aimbot.Ballistic:
                    valueDisplay.text = value switch
                    {
                        0 => I18N.instance.getValue("^straight"),
                        1 => I18N.instance.getValue("^parabolic"),
                        _ => valueDisplay.text
                    };
                    break;
                case CustomizeProperties.Aimbot.Prediction:
                    valueDisplay.text = value switch
                    {
                        0 => I18N.instance.getValue("^none"),
                        1 => I18N.instance.getValue("^straight"),
                        2 => I18N.instance.getValue("^curve"),
                        _ => valueDisplay.text
                    };
                    break;
                //飞镖误差
                case CustomizeProperties.DartError.DartErr:
                    valueDisplay.text = Mathf.Round((value - 1) * 100) + " %";
                    calculatedValue -= 1;
                    break;
            }
            if (!CustomizeManager.Instance().IsServer() )
            {
                CustomizeManager.Instance().CmdSetData(EntityManager.Instance().LocalRobot(), property, calculatedValue);
            }


            // 存储默认键值
            if (!CustomizePanel.CustomizePropertyMap.Contains(this))
            {
                CustomizePanel.CustomizePropertyMap.Add(this, value);
            }
        }
    }
}