using System;

namespace Config
{
    public enum VersionModifier
    {
        // 内部开发版本
        Insider = 0,

        // 内测版本
        Alpha = 1,

        // 公测版本
        Beta = 2,

        // 发布版本
        Release = 3
    }

    // Parse version modifier from string
    public static class VersionModifierParser
    {
        public static VersionModifier Parse(string modifier)
        {
            return modifier switch
            {
                "insider" => VersionModifier.Insider,
                "alpha" => VersionModifier.Alpha,
                "beta" => VersionModifier.Beta,
                "release" => VersionModifier.Release,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}