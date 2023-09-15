using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Gameplay;
using Infrastructure;
using Infrastructure.Child;
//using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers.Child
{
    /// <summary>
    /// 灯条状态对应的材料。
    /// </summary>
    [Serializable]
    public class LightMaterial
    {
        public LightState state;
        public Material red;
        public Material blue;
    }

    /// <summary>
    /// 灯光控制器基类。
    /// 用于控制包括装甲板、场地灯效的灯光效果。
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class LightController : StoreChildBase
    {
        public Identity.Camps camp;
        public bool isOn;
        public float percentage = 1;
        /// <summary>
        /// 场地所需要的材质列表
        /// </summary>
        [FormerlySerializedAs("states")] public List<LightMaterial> materials = new List<LightMaterial>();

        public Material percentageTemplate;
        private Color _customPercentageColor;
        private bool _customPercentageColorSet;
        private LightState _state;
        private Renderer _renderer;

        // TODO：美化名称
        private static readonly int ColorProperty = Shader.PropertyToID("Color_318fd94095e04f1cb1f62d78e6eecd65");
        private static readonly int PercentageProperty =
            Shader.PropertyToID("Vector1_ae8b51245bf343d5af8ae195b1fdb881");

        /// <summary>
        /// 声明关注事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.LightControl.TurnLight,
                ChildActionID.LightControl.SetLightState,
                ChildActionID.LightControl.SetPercentage
            }).ToList();
        }

        /// <summary>
        /// 灯光默认开启
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _renderer = GetComponent<Renderer>();
            SetLightState(LightState.On);
        }

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 事件处理。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.LightControl.TurnLight:
                    var turnLightAction = (TurnLight) action;
                    isOn = turnLightAction.IsOn;
                    SetLightState(LightState.On);
                    break;

                case ChildActionID.LightControl.SetLightState:
                    var setStateAction = (SetLightState) action;
                    SetLightState(setStateAction.State);
                    break;

                case ChildActionID.LightControl.SetPercentage:
                    var setPercentageAction = (SetPercentage) action;
                    percentage = setPercentageAction.Percentage;
                    break;
            }
        }

        /// <summary>
        /// 更改进度条状态颜色。
        /// </summary>
        /// <param name="color">颜色</param>
        public void SetCustomPercentageColor(Color color)
        {
            _customPercentageColor = color;
            _customPercentageColorSet = true;
        }

        /// <summary>
        /// 设置灯条状态。
        /// </summary>
        /// <param name="state">需要设置的状态</param>
        private void SetLightState(LightState state)
        {
            _state = state;
            //除了伸缩灯效需要特殊处理外，其他的材质替换在else中进行
            if (_state == LightState.Percentage)
            {
                if (percentageTemplate == null)
                {
                    throw new Exception("Switching to percentage state without a template.");
                }
                
                var material = new Material(percentageTemplate);
                var color = materials.Count > 0
                    ? camp switch
                    {
                        Identity.Camps.Red => materials[0].red.color,
                        Identity.Camps.Blue => materials[0].blue.color,
                        _ => camp.GetColor()
                    }
                    : camp.GetColor();
                material.SetColor(
                    ColorProperty,
                    color);
                if (_customPercentageColorSet)
                {
                    material.SetColor(ColorProperty, _customPercentageColor);
                }
                material.SetFloat(PercentageProperty, 1.0f);
                percentage = 1.0f;
                //在生成完材质后，实现材质的替换
                GetComponent<Renderer>().material = material;
            }
            else
            {
                foreach (var lightMat in materials.Where(lightMat => lightMat.state == state))
                {
                    GetComponent<Renderer>().material = camp == Identity.Camps.Red ? lightMat.red : lightMat.blue;
                    break;
                }
            }
        }

        /// <summary>
        /// 更新灯柱显示。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            _renderer.enabled = isOn;
            if (_state == LightState.Percentage)
            {
                // TODO: 更新频率优化
                var material = GetComponent<Renderer>().material;
                material.SetFloat(PercentageProperty, percentage);
            }
        }
    }
}