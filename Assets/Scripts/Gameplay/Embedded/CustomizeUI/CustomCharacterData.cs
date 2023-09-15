using System;
using System.Text;

namespace Gameplay.Embedded.CustomizeUI
{
    /// <summary>
    /// 自定义字符数据
    /// </summary>
    public class CustomCharacterData : InteractiveData
    {
        // 字符配置
        public CustomGraphic CharConfig;

        // 字符内容
        public string Character
        {
            get => Encoding.Default.GetString(Data, 15, 30);
            set => Buffer.BlockCopy(Encoding.Default.GetBytes(value), 0, Data, 15, 30);
        }

        public CustomCharacterData(byte[] bytes) : base(bytes)
        {
            CharConfig = new CustomGraphic(Data, 0);
        }

        public CustomCharacterData(InteractiveHeader header, byte[] data) : base(header, data)
        {
            CharConfig = new CustomGraphic(Data, 0);
        }

        public override DataCommandId GetCommandId()
        {
            return DataCommandId.DrawCharGraphic;
        }
    }
}