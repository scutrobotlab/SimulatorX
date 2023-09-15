using System;
using System.Collections.Generic;
using Misc;

namespace Gameplay.Embedded.CustomizeUI
{
    public enum CustomGraphicsCount
    {
        Draw1Graphic = 0x0101, // 绘制一个图形
        Draw2Graphics = 0x0102, // 绘制两个图形
        Draw5Graphics = 0x0103, // 绘制五个图形
        Draw7Graphics = 0x0104, // 绘制七个图形
    }

    public enum GraphicOperation
    {
        Null = 0,
        Add = 1,
        Modify = 2,
        Delete = 3
    }

    public enum GraphicType
    {
        StraightLine = 0,
        Rectangle = 1,
        Circle = 2,
        Ellipse = 3,
        Arc = 4,
        Float = 5,
        Integer = 6,
        Character = 7
    }

    public enum GraphicColor
    {
        Main = 0, // 红方为红色 蓝方为蓝色
        Yellow = 1,
        Green = 2,
        Orange = 3,
        Purple = 4,
        Pink = 5,
        Cyan = 6,
        Black = 7,
        White = 8
    }

    /// <summary>
    /// 自定义图形
    /// </summary>
    public class CustomGraphic
    {
        public CustomGraphic(byte[] data, int offset)
        {
            _data = data;
            _offset = offset;
        }

        // Byte 0-2
        public byte[] Name
        {
            get => BitUtil.GetBytes(_data, _offset + 0, 3);
            set => BitUtil.SetBytes(value, 0, _data, _offset + 0, 3);
        }

        // Byte 3-6
        public GraphicOperation Operation // Bit 0-2
        {
            get => (GraphicOperation)BitUtil.GetBits(_data, _offset + 3, 0, 3);
            set => BitUtil.SetBits((uint)value, _data, _offset + 3, 0, 3);
        }

        public GraphicType Type // Bit 3-5
        {
            get => (GraphicType)BitUtil.GetBits(_data, _offset + 3, 3, 3);
            set => BitUtil.SetBits((uint)value, _data, _offset + 3, 3, 3);
        }

        public uint Layer // Bit 6-9
        {
            get => BitUtil.GetBits(_data, _offset + 3, 6, 4);
            set => BitUtil.SetBits(value, _data, _offset + 3, 6, 4);
        }

        public GraphicColor Color // Bit 10-13
        {
            get => (GraphicColor)BitUtil.GetBits(_data, _offset + 3, 10, 4);
            set => BitUtil.SetBits((uint)value, _data, _offset + 3, 10, 4);
        }

        public uint StartAngle // Bit 14-22
        {
            get => BitUtil.GetBits(_data, _offset + 3, 14, 9);
            set => BitUtil.SetBits(value, _data, _offset + 3, 14, 9);
        }

        public uint EndAngle // Bit 23-31
        {
            get => BitUtil.GetBits(_data, _offset + 3, 23, 9);
            set => BitUtil.SetBits(value, _data, _offset + 3, 23, 9);
        }

        // Byte 7-10
        public uint Width // Bit 0-9
        {
            get => BitUtil.GetBits(_data, _offset + 7, 0, 10);
            set => BitUtil.SetBits(value, _data, _offset + 7, 0, 10);
        }

        public uint StartXPos // Bit 10-20
        {
            get => BitUtil.GetBits(_data, _offset + 7, 10, 11);
            set => BitUtil.SetBits(value, _data, _offset + 7, 10, 11);
        }

        public uint EndXPos // Bit 21-31
        {
            get => BitUtil.GetBits(_data, _offset + 7, 21, 11);
            set => BitUtil.SetBits(value, _data, _offset + 7, 21, 11);
        }

        // Byte 11-14
        public uint FontSizeRadius // Bit 0-9
        {
            get => BitUtil.GetBits(_data, _offset + 11, 0, 10);
            set => BitUtil.SetBits(value, _data, _offset + 11, 0, 10);
        }

        public uint StartYPos // Bit 10-20
        {
            get => BitUtil.GetBits(_data, _offset + 11, 10, 11);
            set => BitUtil.SetBits(value, _data, _offset + 11, 10, 11);
        }

        public uint EndYPos // Bit 21-31
        {
            get => BitUtil.GetBits(_data, _offset + 11, 21, 11);
            set => BitUtil.SetBits(value, _data, _offset + 11, 21, 11);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                   $"Name = ${Name}, " +
                   $"Ope = {Operation}, " +
                   $"Type = {Type}, " +
                   $"Layer = {Layer}, " +
                   $"Color = {Color}, " +
                   $"Width = {Width}, " +
                   $"Angle = ({StartAngle} => {EndAngle})" +
                   $"FontSizeRadius = {FontSizeRadius}, " +
                   $"Start = ({StartXPos}, {StartYPos}), " +
                   $"End = ({EndXPos}, {EndYPos})";
        }

        private readonly byte[] _data;
        private readonly int _offset;
    }

    /// <summary>
    /// 自定义图形数据
    /// </summary>
    public class CustomGraphicData : InteractiveData
    {
        public readonly CustomGraphicsCount GraphicsCount;

        public List<CustomGraphic> CustomGraphic { get; private set; } = new List<CustomGraphic>();

        public CustomGraphicData(CustomGraphicsCount graphicsCount, byte[] bytes) : base(bytes)
        {
            GraphicsCount = graphicsCount;
            for (var i = 0; i < GraphicsCountNum(); i++)
            {
                CustomGraphic.Add(new CustomGraphic(Data, 15 * i));
            }
        }

        public CustomGraphicData(CustomGraphicsCount graphicsCount, InteractiveHeader header, byte[] data)
            : base(header, data)
        {
            GraphicsCount = graphicsCount;
            for (var i = 0; i < GraphicsCountNum(); i++)
            {
                CustomGraphic.Add(new CustomGraphic(Data, 15 * i));
            }
        }

        public override DataCommandId GetCommandId()
        {
            return (DataCommandId)GraphicsCount;
        }

        public int GraphicsCountNum()
        {
            return GraphicsCount switch
            {
                CustomGraphicsCount.Draw1Graphic => 1,
                CustomGraphicsCount.Draw2Graphics => 2,
                CustomGraphicsCount.Draw5Graphics => 5,
                CustomGraphicsCount.Draw7Graphics => 7,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}