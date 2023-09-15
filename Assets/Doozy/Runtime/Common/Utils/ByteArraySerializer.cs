// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common.Utils
{
    public static class ByteArraySerializer
    {
        public static byte[] Serialize<T>(this T m)
        {
            using var ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, m);
            return ms.ToArray();
        }

        public static T Deserialize<T>(this byte[] byteArray)
        {
            using var ms = new MemoryStream(byteArray);
            return (T)new BinaryFormatter().Deserialize(ms);
        }
    }
}
