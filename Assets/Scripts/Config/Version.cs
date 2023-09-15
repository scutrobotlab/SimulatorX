using System;
using Honeti;
using UnityEngine;
using UnityEngine.Serialization;

namespace Config
{
    /// <summary>
    /// 版本号配置类
    /// </summary>
    public class Version : MonoBehaviour
    {
        private const int SeasonVersionBitLength = 8;
        private const int MajorVersionBitLength = 6;
        private const int SecondVersionBitLength = 8;
        private const int PhaseVersionBitLength = 10;

        private const int MaxSeasonVersion = 1 << SeasonVersionBitLength - 1; // 最大赛季版本号
        private const int MaxMajorVersion = 1 << MajorVersionBitLength - 1; // 最大主版本号
        private const int MaxSecondVersion = 1 << SecondVersionBitLength - 1; // 最大次版本号
        private const int MaxPhaseVersion = 1 << PhaseVersionBitLength - 1; // 最大阶段版本号

        private static Version _version;

        /// <summary>
        /// 版本修饰符
        /// </summary>
        [FormerlySerializedAs("VersionModifier")]
        public VersionModifier versionModifier;

        /// <summary>
        /// 服务器最低支持的客户端版本名称
        /// 在发布版本时需要检查兼容性，必要时修改此处
        /// <example>0.9.0.0</example>
        /// </summary>
        [FormerlySerializedAs("MinVersionName")]
        public string minVersionName;

        /// <summary>
        /// 服务器最低支持的客户端版本号代码
        /// <example>2359296</example>
        /// </summary>
        public int MinVersionCode =>
            CurrentVersionCode;
            //GetVersionCodeByName(minVersionName);

        // 当前版本号代码
        public static int CurrentVersionCode => GetVersionCodeByName(VersionName);

        // 版本号名称
        public static string VersionName =>
            $"{StaticVersion.SeasonVersion}.{StaticVersion.MajorVersion}" +
            $".{StaticVersion.SecondVersion}.{StaticVersion.PhaseVersion}";

        public void Start()
        {
            Debug.Log($"Version Name = {VersionName}\n" +
                      $"Version Code = {CurrentVersionCode}");
        }

        /// <summary>
        /// 获取版本水印文字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetVersionWatermark()
        {
            switch (versionModifier)
            {
                case VersionModifier.Release:
                    return $"SimulatorX {VersionName} {versionModifier.ToString()}" +
                           $" {I18N.instance.getValue("^release_version_watermark_annotation")}";
                case VersionModifier.Insider:
                case VersionModifier.Alpha:
                case VersionModifier.Beta:
                    return $"SimulatorX {VersionName} {versionModifier.ToString()}" +
                           $" {I18N.instance.getValue("^test_version_watermark_annotation")}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static Version Instance()
        {
            if (_version == null)
            {
                _version = FindObjectOfType<Version>();

                if (_version == null)
                {
                    // 在其他场景打开时避免错误
                    _version = new Version();
                    Debug.Log("仅供调试：没有在 Offline 场景中启动");
                }
            }

            return _version;
        }

        /// <summary>
        /// 通过版本名称获取版本号代码
        /// </summary>
        /// <param name="versionName">版本名称</param>
        /// <returns></returns>
        private static int GetVersionCodeByName(string versionName)
        {
            var split = versionName.Split('.');
            return (Convert.ToInt32(split[0]) <<
                    (MajorVersionBitLength + SecondVersionBitLength + PhaseVersionBitLength)) +
                   (Convert.ToInt32(split[1]) << (SecondVersionBitLength + PhaseVersionBitLength)) +
                   (Convert.ToInt32(split[2]) << (PhaseVersionBitLength)) + Convert.ToInt32(split[3]);
        }

        /// <summary>
        /// 静态版本类
        /// </summary>
        /// 用于解决 MonoBehavior 中无法在构造函数中获取version的报错
        private struct StaticVersion
        {
            public static int SeasonVersion; // 赛季版本号
            public static int MajorVersion; // 主版本号
            public static int SecondVersion; // 次版本号
            public static int PhaseVersion; // 阶段版本号

            static StaticVersion()
            {
                try
                {
                    var versionSplit = Application.version.Split('.');
                    if (versionSplit.Length != 4)
                        throw new ApplicationException("版本号不是 a.b.c.d 格式");
                    SeasonVersion = Convert.ToInt32(versionSplit[0]);
                    MajorVersion = Convert.ToInt32(versionSplit[1]);
                    SecondVersion = Convert.ToInt32(versionSplit[2]);
                    PhaseVersion = Convert.ToInt32(versionSplit[3]);
                    if (SeasonVersion > MaxSeasonVersion || MajorVersion > MaxMajorVersion ||
                        SecondVersion > MaxSecondVersion || PhaseVersion > MaxPhaseVersion)
                        throw new ArgumentException("版本号超过最长长度");
                }
                catch (Exception)
                {
                    Debug.LogError("Project Settings 中版本号不合法，请在 Edit/Project Settings/Player/Version 处修改");
                    throw;
                }
            }
        }
    }
}