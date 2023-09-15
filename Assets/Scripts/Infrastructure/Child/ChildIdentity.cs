using System;
using System.Diagnostics.CodeAnalysis;
using Mirror;

namespace Infrastructure.Child
{
    /// <summary>
    /// 所有子组件类型。
    /// </summary>
    public enum ChildType
    {
        Nothing,
        Magazine,
        GroundChassisVisual,
        Armor,
        Light,
        RingLight,
        FrameLight,
        BranchLight,
        TargetLight,
        IconLight,
        RobotLightBar,
        SpinIndicator,
        BulletproofIndicator,
        GunLockedIndicator,
        MuzzleLightBar,
        SuperBatteryIndicator,
        Rectifier,
        MagazineIndicator,
        ArmorTurningIndicator,
        Fan
    }

    /// <summary>
    /// 用于 StoreChild 的标识。
    /// </summary>
    [Serializable]
    public class ChildIdentity
    {
        public ChildType type;
        public int serial;

        /// <summary>
        /// 初始化子组件。
        /// </summary>
        /// <param name="childType">组件类型</param>
        /// <param name="serialNumber">序列号</param>
        public ChildIdentity(ChildType childType = ChildType.Nothing, int serialNumber = 0)
        {
            type = childType;
            serial = serialNumber;
        }

        /// <summary>
        /// 从序列化字符串初始化子组件。
        /// </summary>
        /// <param name="data">序列化字符串</param>
        public ChildIdentity(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                type = ChildType.Nothing;
                serial = 0;
            }

            LoadData(data);
        }

        /// <summary>
        /// 将数据序列化为字符串。
        /// </summary>
        /// <returns></returns>
        public string Data()
        {
            return type + ";" + serial;
        }

        /// <summary>
        /// 反序列化字符串。
        /// </summary>
        /// <param name="data">序列化字符串</param>
        public void LoadData(string data)
        {
            var splitData = data.Split(';');
            type = (ChildType) Enum.Parse(typeof(ChildType), splitData[0]);
            serial = int.Parse(splitData[1]);
        }

        public static bool operator ==(ChildIdentity a, ChildIdentity b)
        {
            return a?.type == b?.type && a?.serial == b?.serial;
        }

        public static bool operator !=(ChildIdentity a, ChildIdentity b)
        {
            return !(a == b);
        }

        protected bool Equals(ChildIdentity other)
        {
            return type == other.type && serial == other.serial;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChildIdentity) obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) type * 397) ^ serial;
            }
        }
    }

    /// <summary>
    /// 让 ChildIdentity 支持网络同步的拓展函数。
    /// </summary>
    public static class ChildIdentityReaderWriter
    {
        public static void WriteChildIdentity(this NetworkWriter writer, ChildIdentity childIdentity)
        {
            writer.WriteString(childIdentity != null ? childIdentity.Data() : "");
        }

        public static ChildIdentity ReadChildIdentity(this NetworkReader reader)
        {
            return new ChildIdentity(reader.ReadString());
        }
    }
}