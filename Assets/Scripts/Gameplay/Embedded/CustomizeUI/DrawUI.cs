using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Gameplay.Embedded.CustomizeUI
{
    /// <summary>
    /// 每个面板与其层数相对应，注意加上[Serializable]
    /// </summary>
    [Serializable]
    public class EmbeddedUI
    {
        public GameObject panel;
        public GameObject texture;
        public GameObject message;
        public uint layer;
    }
    
    
    public class DrawUI : MonoBehaviour,IDrawCustomUI
    {
        
        /// <summary>
        /// 清理贴图
        /// </summary>
        private void ClearTexture(Texture2D targetTexture)
        {
            for (var x = 0; x < targetTexture.width; x++)
            {
                for (var y = 0; y < targetTexture.height; y++)
                {
                    targetTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
                }
            }
            targetTexture.Apply();
        }

        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="startPoint">起始点</param>
        /// <param name="endPoint">结束点</param>
        /// <param name="targetTexture">要操作的贴图</param>
        /// <param name="lineColor">画线的颜色</param>
        private void OperateTexture(Point startPoint,Point endPoint, Texture2D targetTexture,Color lineColor)
        {
            //渲染次数，不宜太大
            const float lineNum = 1000f;
            const int lineWidth = 3;
            for (var j = 0; j < lineNum; j++)
            {
                var scaleX = Mathf.Lerp(startPoint.XPos, endPoint.XPos, j / lineNum);
                var scaleY = Mathf.Lerp(startPoint.YPos, endPoint.YPos, j / lineNum);
                var textureX = (int)(scaleX * Screen.width);
                var textureY = (int)(scaleY * Screen.height);
                // 线条加粗
                for (var a = textureX - lineWidth; a < textureX + lineWidth; a++)
                {
                    for (var b = textureY - lineWidth; b < textureY + lineWidth; b++)
                    {
                        targetTexture.SetPixel(a, b, lineColor);
                    }
                }
            }
            targetTexture.Apply();
        }
        
        [Header("九层UI")]
        public List<EmbeddedUI> embeddedUis = new List<EmbeddedUI>();

        private void Start()
        {
        }

        private void Update()
        {
            
        }

        public bool DeleteGraphicLayer(DeleteCustomGraphicOperation operation, uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                //清除之前在贴图上画的图
                ClearTexture((Texture2D)embeddedUI.texture.GetComponent<RawImage>().texture);
                embeddedUI.message.GetComponent<TextMeshPro>().text = string.Empty;
                embeddedUI.panel.SetActive(false);
            }
            else
            {
                //想要操作的层不存在
                return false;
            }
            return true;
        }

        public bool DrawStraightLine(CommonVariable common, Point start, Point end,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                embeddedUI.panel.SetActive(true);
                OperateTexture(start,end,(Texture2D) embeddedUI.texture.GetComponent<RawImage>().texture,common.Color);
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool DrawRectangle(CommonVariable common, Point start, Point end,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                embeddedUI.panel.SetActive(true);
                OperateTexture(new Point(start.XPos, start.YPos), new Point(start.XPos, end.YPos),
                    (Texture2D) embeddedUI.texture.GetComponent<RawImage>().texture, common.Color);
                OperateTexture(new Point(start.XPos, end.YPos), new Point(end.XPos, end.YPos),
                    (Texture2D) embeddedUI.texture.GetComponent<RawImage>().texture, common.Color);
                OperateTexture(new Point(start.XPos, start.YPos), new Point(end.XPos, start.YPos),
                    (Texture2D) embeddedUI.texture.GetComponent<RawImage>().texture, common.Color);
                OperateTexture(new Point(end.XPos, start.YPos), new Point(end.XPos, end.YPos),
                    (Texture2D) embeddedUI.texture.GetComponent<RawImage>().texture, common.Color);
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool DrawCircle(CommonVariable common, Point center, uint radius,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                
            }
            throw new System.NotImplementedException();
        }

        public bool DrawEllipse(CommonVariable common, Point center, uint radius, uint xAxisLength, uint yAxisLength,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                
            }
            throw new System.NotImplementedException();
        }

        public bool DrawArc(CommonVariable common, uint startAngle, uint endAngle, 
            Point center, uint xAxisLength, uint yAxisLength,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                
            }
            throw new System.NotImplementedException();
        }

        public bool DrawFloat(CommonVariable common, uint fontSize, Point start, uint decimalDigits, float value,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                // 一个gameObject上只能有一个图形属性，所以在添加text之前要把RawImage移除
                // 拿TextMeshPro-Text（UI）需要GetComponent<TextMeshProUGUI>()而不是GetComponent<TextMeshPro>()
                var floatText = embeddedUI.message.GetComponent<TextMeshProUGUI>();
                floatText.rectTransform.position = new Vector3(start.XPos*472*2,start.YPos*172*2,0);
                floatText.text = value.ToString();
                floatText.fontSize = fontSize;
                floatText.color = common.Color;
                
                return true;
            }
            
            return false;
        }

        public bool DrawInteger(CommonVariable common, uint fontSize, Point start, int value,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                // 一个gameObject上只能有一个图形属性，所以在添加text之前要把RawImage移除
                // 拿TextMeshPro-Text（UI）需要GetComponent<TextMeshProUGUI>()而不是GetComponent<TextMeshPro>()
                var floatText = embeddedUI.message.GetComponent<TextMeshProUGUI>();
                floatText.rectTransform.position = new Vector3(start.XPos*472*2,start.YPos*172*2,0);
                floatText.text = value.ToString();
                floatText.fontSize = fontSize;
                floatText.color = common.Color;
                
                return true;
            }
            
            return false;
        }

        public bool DrawCharacter(CommonVariable common, uint fontSize, Point start, string text,uint layer)
        {
            if (embeddedUis.Any(ui => ui.layer == layer))
            {
                var embeddedUI = embeddedUis.First(ui => ui.layer == layer);
                // 一个gameObject上只能有一个图形属性，所以在添加text之前要把RawImage移除
                // 拿TextMeshPro-Text（UI）需要GetComponent<TextMeshProUGUI>()而不是GetComponent<TextMeshPro>()
                var floatText = embeddedUI.message.GetComponent<TextMeshProUGUI>();
                floatText.rectTransform.position = new Vector3(start.XPos*472*2,start.YPos*172*2,0);
                floatText.text = text;
                floatText.fontSize = fontSize;
                floatText.color = common.Color;
                
                return true;
            }
            
            return false;
        }
    }
}