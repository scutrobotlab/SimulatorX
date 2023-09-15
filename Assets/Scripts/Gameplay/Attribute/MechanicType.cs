using System;

namespace Gameplay.Attribute
{
    /// <summary>
    /// 可选机构类型。
    /// </summary>
    public static class MechanicType
    {
        /// <summary>
        /// 底盘类型。
        /// </summary>
        public enum Chassis
        {
            /// <summary>
            /// 未选择。
            /// </summary>
            Default,

            /// <summary>
            /// 功率优先。
            /// </summary>
            Power,

            /// <summary>
            /// 血量优先。
            /// </summary>
            Armor,

            /// <summary>
            /// 平衡底盘
            /// </summary>
            Balance
        }

        public static readonly string[] ChassisTypeName =
            { "^not_select", "^power_preferred", "^armor_preferred", "^balanced_chassis" };

        /// <summary>
        /// 发射机构类型。
        /// </summary>
        public enum GunType
        {
            /// <summary>
            /// 未选择。
            /// </summary>
            Default,

            /// <summary>
            /// 爆发优先。
            /// </summary>
            Burst,

            /// <summary>
            /// 冷却优先。
            /// </summary>
            CoolDown,

            /// <summary>
            /// 弹速优先。
            /// </summary>
            Velocity,
        }

        public static readonly string[] GunTypeNames =
            { "^not_select", "^burst_preferred", "^cooldown_preferred", "^velocity_preferred" };

        /// <summary>
        /// 弹丸类型。
        /// </summary>
        public enum CaliberType
        {
            /// <summary>
            /// 小口径（17mm）
            /// </summary>
            Small,

            /// <summary>
            /// 大口径（42mm）
            /// </summary>
            Large,

            /// <summary>
            /// 飞镖
            /// </summary>
            Dart
        }

        /// <summary>
        /// 发射机构信息。
        /// </summary>
        [Serializable]
        public class GunInfo
        {
            public GunType type;
            public CaliberType caliber;
        }
    }
}