using System;
using UnityEngine;

namespace Misc
{
    /// <summary>
    /// <c>BitUtil</c> 包括了位处理中常用的一些操作。
    /// <br/>目前有二维向量到字节的压缩和解压缩。
    /// </summary>
    public static class BitUtil
    {
        /// <summary>
        /// 将二维向量压缩为一个字节。
        /// </summary>
        /// <param name="axis">二维向量</param>
        /// <returns>压缩成的字节</returns>
        public static byte CompressAxisInput(Vector2 axis)
        {
            var axisClamp = (axis + Vector2.one) * 7.5f;
            var axisX = (uint)axisClamp.x << 4;
            var axisY = (uint)axisClamp.y;
            return (byte)(axisX | axisY);
        }

        /// <summary>
        /// 将字节解压缩为二维向量。
        /// </summary>
        /// <param name="input">字节</param>
        /// <returns>解压出的二维向量</returns>
        public static Vector2 DecompressAxisInput(byte input)
        {
            var axisX = (uint)input >> 4;
            var axisY = (uint)input & ~(((1 << 4) - 1) << 4);
            var axisXClamp = axisX < 7 ? (axisX / 6.0f) - 1 : (axisX - 7) / 8.0f;
            var axisYClamp = axisY < 7 ? (axisY / 6.0f) - 1 : (axisY - 7) / 8.0f;
            return new Vector2(axisXClamp, axisYClamp);
        }

        /// <summary>
        /// 获取字节
        /// </summary>
        /// <param name="src">源</param>
        /// <param name="srcOffset">源偏移</param>
        /// <param name="count">字节数</param>
        /// <returns></returns>
        public static byte[] GetBytes(Array src, int srcOffset, int count)
        {
            var result = new byte[count];
            Buffer.BlockCopy(src, srcOffset, result, 0, count);
            return result;
        }

        /// <summary>
        /// 设置字节
        /// </summary>
        /// <param name="src">源</param>
        /// <param name="srcOffset">源偏移</param>
        /// <param name="dst">目标</param>
        /// <param name="dstOffset">目标偏移</param>
        /// <param name="count">字节数</param>
        public static void SetBytes(Array src, int srcOffset, Array dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
        }

        /// <summary>
        /// 获取比特
        /// </summary>
        /// <param name="src">源</param>
        /// <param name="byteOffset">数组字节偏移量</param>
        /// <param name="bitOffset">比特偏移量</param>
        /// <param name="count">截取比特长度</param>
        /// <returns></returns>
        public static uint GetBits(byte[] src, int byteOffset, int bitOffset, int count)
        {
            var uInt32 = BitConverter.ToUInt32(src, byteOffset);
            // 将要截取的段落右移
            uInt32 >>= bitOffset;
            // 与上与截取长度匹配的掩码
            return uInt32 & GetMask(count);
        }

        /// <summary>
        /// 写入比特
        /// </summary>
        /// <param name="value">写入值</param>
        /// <param name="dst">目标数组</param>
        /// <param name="byteOffset">数组字节偏移量</param>
        /// <param name="bitOffset">比特偏移量</param>
        /// <param name="count">截取比特长度</param>
        public static void SetBits(uint value, byte[] dst, int byteOffset, int bitOffset, int count)
        {
            var uInt32 = BitConverter.ToUInt32(dst, byteOffset);
            // 复位旧的位
            uInt32 = uInt32 & (~GetMask(count) << bitOffset) | GetMask(bitOffset);
            // 置位新的位
            uInt32 |= (value & GetMask(count) << bitOffset);
            Buffer.BlockCopy(BitConverter.GetBytes(uInt32), 0, dst, byteOffset, 4);
        }

        // 掩码
        private static readonly uint[] Mask = new uint[32];

        static BitUtil()
        {
            // 预先生成掩码
            for (var i = 0; i < Mask.Length; i++)
            {
                Mask[i] = (1U << i) - 1;
            }
        }

        private static uint GetMask(int count)
        {
            return Mask[count];
        }
    }
}