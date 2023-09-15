using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Misc
{
    /// <summary>
    /// 实现摄像机投影到屏幕。
    /// TODO: 性能问题
    /// </summary>
    [RequireComponent(typeof(Camera))]
    // [ExecuteAlways]
    public class MirrorCamera : MonoBehaviour
    {
        public MeshRenderer display;
        public int resWidth;
        public int resHeight;
        public float intensity = 2;
        public float exposureWeight = 0.7f;

        public Material materialTemplate;

        private RenderTexture _renderTexture;
        private Material _material;

        /// <summary>
        /// 运行时生成 RenderTexture 和 Material。
        /// </summary>
        private void Start()
        {
            // 动态生成材质
            _renderTexture = new RenderTexture(resWidth, resHeight, 16)
            {
                antiAliasing = 2,
                filterMode = FilterMode.Bilinear
            };
            GetComponent<Camera>().targetTexture = _renderTexture;
            if (materialTemplate == null)
            {
                throw new Exception("Creating mirror without a template.");
            }

            _material = new Material(materialTemplate);
            display.material = _material;
        }

        private void Update()
        {
            UpdateMaterialAttributes();
        }

        /// <summary>
        /// 生成 HDR 白色。
        /// </summary>
        /// <param name="value">发光强度</param>
        [SuppressMessage("ReSharper", "Unity.PreferAddressByIdToGraphicsParams")]
        private void SetEmissionIntensity(float value)
        {
            var intense = Mathf.Pow(2, value);
            var hdrColor = new Color(intense, intense, intense, intense);
            _material.SetColor("_EmissiveColor", hdrColor);
        }

        /// <summary>
        /// 调整材质发光。
        /// </summary>
        [SuppressMessage("ReSharper", "Unity.PreferAddressByIdToGraphicsParams")]
        private void UpdateMaterialAttributes()
        {
            _material.SetTexture("_BaseColorMap", _renderTexture);
            _material.SetTexture("_EmissiveColorMap", _renderTexture);
            _material.SetFloat("_EmissiveExposureWeight", exposureWeight);
            SetEmissionIntensity(intensity);
        }
    }
}