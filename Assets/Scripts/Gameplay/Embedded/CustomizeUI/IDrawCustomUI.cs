using UnityEngine;

namespace Gameplay.Embedded.CustomizeUI
{
    /// <summary>
    /// 公用变量
    /// </summary>
    public struct CommonVariable
    {
        public byte[] Name;
        public GraphicOperation Operation;
        public uint Layer;
        public Color Color;
        public uint Width;
    }

    /// <summary>
    /// 坐标点
    /// </summary>
    public class Point
    {
        public Point(float xPos, float yPos)
        {
            XPos = xPos;
            YPos = yPos;
        }

        public float XPos;
        public float YPos;
    }

    /// <summary>
    /// 绘制自定义UI接口
    /// </summary>
    public interface IDrawCustomUI
    {
        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="operation">操作</param>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public bool DeleteGraphicLayer(DeleteCustomGraphicOperation operation, uint layer);

        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="common"></param>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="layer"></param>
        /// <returns>是否成功</returns>
        public bool DrawStraightLine( CommonVariable common,Point start, Point end, uint layer);

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="common"></param>
        /// <param name="start">对角线起点</param>
        /// <param name="end">对角线终点</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawRectangle(CommonVariable common, Point start, Point end,uint layer);

        /// <summary>
        /// 绘制整圆
        /// </summary>
        /// <param name="common"></param>
        /// <param name="center">圆形</param>
        /// <param name="radius">半径</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawCircle(CommonVariable common, Point center, uint radius,uint layer);

        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="common"></param>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <param name="xAxisLength">X半轴长度</param>
        /// <param name="yAxisLength">Y半轴长度</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawEllipse(CommonVariable common, Point center, uint radius, uint xAxisLength, uint yAxisLength,uint layer);

        /// <summary>
        /// 绘制圆弧
        /// </summary>
        /// <param name="common"></param>
        /// <param name="startAngle">起始角度</param>
        /// <param name="endAngle">结束角度</param>
        /// <param name="center">圆心</param>
        /// <param name="xAxisLength">X半轴长度</param>
        /// <param name="yAxisLength">Y半轴长度</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawArc(CommonVariable common, uint startAngle, uint endAngle, Point center,
            uint xAxisLength, uint yAxisLength,uint layer);

        /// <summary>
        /// 绘制浮点数
        /// </summary>
        /// <param name="common"></param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="start">文字起点</param>
        /// <param name="decimalDigits">小数位有效个数</param>
        /// <param name="value">数值</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawFloat(CommonVariable common, uint fontSize, Point start, uint decimalDigits, float value,uint layer);

        /// <summary>
        /// 绘制整数
        /// </summary>
        /// <param name="common"></param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="start">文字起点</param>
        /// <param name="value">数值</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawInteger(CommonVariable common, uint fontSize, Point start, int value,uint layer);

        /// <summary>
        /// 绘制字符
        /// </summary>
        /// <param name="common"></param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="start">文字起点</param>
        /// <param name="text">文字内容</param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool DrawCharacter(CommonVariable common, uint fontSize, Point start, string text,uint layer);
    }
}