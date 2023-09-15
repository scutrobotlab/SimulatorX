using Honeti;
using UnityEngine;

namespace Config
{
    /// <summary>
    /// 全局公共配置类
    /// </summary>
    public struct GlobalConfig
    {
        public static readonly Version Version = Version.Instance();

        public static LanguageCode? LanguageCode = null;
    }
}