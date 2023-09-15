namespace Misc
{
    /// <summary>
    /// <c>BitMap32</c> 是一个32位的数据容器。
    /// <br/>提供按位、按字节写入、读取功能。
    /// </summary>
    public class BitMap32
    {
        private uint _data;

        /// <summary>
        /// 初始化数据容器。
        /// </summary>
        /// <param name="init">初始数据</param>
        public BitMap32(uint init = 0) => _data = init;

        /// <summary>
        /// 获取整个32位数据。
        /// </summary>
        /// <returns>32位无符号整形数据</returns>
        public uint GetData() => _data;

        /// <summary>
        /// 设置某一位的状态。
        /// </summary>
        /// <param name="index">位索引（0-31）</param>
        /// <param name="value">位状态</param>
        public void SetBit(int index, bool value)
        {
            if (index < 0 || index > 31) return;
            if (value)
            {
                _data |= (uint) 1 << index;
            }
            else
            {
                _data &= ~((uint) 1 << index);
            }
        }

        /// <summary>
        /// 获取某一位的状态。
        /// </summary>
        /// <param name="index">位索引（0-31）</param>
        /// <returns>位状态</returns>
        public bool GetBit(int index)
        {
            if (index >= 0 && index <= 31)
            {
                return ((_data >> index) & 1) == 1;
            }

            return false;
        }

        /// <summary>
        /// 存储一个字节。
        /// 共四个存储位置（index=0~3）。
        /// </summary>
        /// <param name="index">位置索引（0~3）</param>
        /// <param name="value">字节数据</param>
        public void SetByte(int index, byte value)
        {
            if (index >= 0 && index <= 3)
            {
                _data |= (uint) value << index * 8;
            }
        }

        /// <summary>
        /// 读出一个字节。
        /// 共四个存储位置（index=0~3）。
        /// </summary>
        /// <param name="index">位置索引（0~3）</param>
        /// <returns>字节数据</returns>
        public byte GetByte(int index)
        {
            if (index >= 0 && index <= 3)
            {
                return (byte) (_data << (3 - index) * 8 >> 3 * 8);
            }

            return 0;
        }

        /// <summary>
        /// 将存储的二进制数据转换为可读字符串（由“0”或“1”组成）。
        /// </summary>
        /// <returns>长度为32的可读字符串</returns>
        public override string ToString()
        {
            var str = "";
            for (var i = 31; i >= 0; i--)
            {
                str += GetBit(i) ? "1" : "0";
            }

            return str;
        }
    }
}