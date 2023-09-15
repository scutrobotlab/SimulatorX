using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mirror;
using UnityEngine;

namespace Infrastructure
{
    /// <summary>
    /// <c>Identity</c> 用于表示实体的身份。
    /// <br/>实体包括机器人、建筑物、抽象 Store、比赛参与者、矿石等。
    /// </summary>
    [Serializable]
    public class Identity
    {
        public enum Camps
        {
            Red,
            Blue,
            Other,
            Unknown
        }

        public enum Roles
        {
            Nothing,
            Player,
            Judge,
            Spectator,
            Hero,
            Engineer,
            Infantry,
            Sentinel,
            Drone,
            Missile,
            Outpost,
            Depot,
            Exchange,
            Energy,
            Base,
            Coin,
            GoldOre,
            SilverOre,
            GroundLight,
            OreFallControl,
            Obstacle,
            DroneManipulator,
            LaunchingStation,
            BalanceInfantry,
            AutoSentinel
        }

        private static readonly HashSet<Roles> RobotRoles = new HashSet<Roles>
        {
            Roles.Hero, Roles.Engineer, Roles.Infantry, Roles.AutoSentinel, Roles.Sentinel, Roles.Drone,
            Roles.BalanceInfantry
        };

        private static readonly HashSet<Roles> GroundRoles = new HashSet<Roles>
        {
            Roles.Hero, Roles.Engineer, Roles.AutoSentinel, Roles.Infantry, Roles.BalanceInfantry
        };

        private static readonly HashSet<Roles> OreRoles = new HashSet<Roles>
        {
            Roles.GoldOre, Roles.SilverOre
        };

        private static readonly HashSet<Roles> JudgeRoles = new HashSet<Roles>
        {
            Roles.Judge
        };

        [SerializeField] public Camps camp;

        [SerializeField] public Roles role;

        // Role下的子区分
        [SerializeField] public int serial;

        // 编号
        [SerializeField] public uint order;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="camp">实体所属阵营</param>
        /// <param name="role">实体类型</param>
        /// <param name="serial">系列</param>
        /// <param name="order">机器人编号</param>
        public Identity(Camps camp = Camps.Other, Roles role = Roles.Nothing, int serial = 0, uint order = 0)
        {
            this.camp = camp;
            this.role = role;
            this.serial = serial;
            this.order = order;
        }

        /// <summary>
        /// 从字符串反序列化数据的构造函数。
        /// </summary>
        /// <param name="data">序列化字符串</param>
        public Identity(string data) : this()
        {
            if (data != "")
            {
                LoadData(data);
            }
        }

        /// <summary>
        /// 检查此标识是否指代机器人。
        /// </summary>
        /// <returns>是否指代机器人</returns>
        public bool IsRobot() => RobotRoles.Contains(role);

        /// <summary>
        /// 检查此标识是否指代地面机器人。
        /// </summary>
        /// <returns>是否指代地面机器人</returns>
        public bool IsGroundRobot() => GroundRoles.Contains(role);

        /// <summary>
        /// 检查此标识是否指代矿石。
        /// </summary>
        /// <returns>是否指代矿石</returns>
        public bool IsOre() => OreRoles.Contains(role);

        /// <summary>
        /// 检查此标识是否指代裁判
        /// </summary>
        /// <returns></returns>
        public bool IsJudge() => JudgeRoles.Contains(role);

        public char Order() => order > 0 ? char.Parse(order.ToString()) : ' ';

        /// <summary>
        /// 序列化实体标签为字符串形式。
        /// </summary>
        /// <returns>序列化字符串</returns>
        public string Data() => $"{camp.ToString()};{role};{serial};{order}";

        /// <summary>
        /// 将字符串反序列化为实体标签。
        /// </summary>
        /// <param name="dataStr">输入字符串</param>
        public void LoadData(string dataStr)
        {
            try
            {
                var data = dataStr.Split(';');
                camp = (Camps)Enum.Parse(typeof(Camps), data[0]);
                role = (Roles)Enum.Parse(typeof(Roles), data[1]);
                serial = int.Parse(data[2]);
            }
            catch (Exception)
            {
                Debug.Log("Fail parsing Identity string: " + dataStr);
                throw;
            }
        }

        public static bool operator ==(Identity a, Identity b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(null, b) || ReferenceEquals(null, a)) return false;
            if (a.camp != b.camp) return false;
            if (a.order != 0 && b.order != 0) return a.order == b.order;
            return a.role == b.role && a.serial == b.serial;
        }

        public static bool operator !=(Identity a, Identity b) => !(a == b);

        private bool Equals(Identity other) => this == other;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Identity)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)camp;
                hashCode = (hashCode * 397) ^ (int)role;
                hashCode = (hashCode * 397) ^ serial;
                return hashCode;
            }
        }

        public override string ToString() => Data();

        public byte[] Serialize()
        {
            var serialized = new byte[sizeof(int) * 3];
            Array.Copy(
                BitConverter.GetBytes((int)role),
                0, serialized,
                0, 4);
            Array.Copy(
                BitConverter.GetBytes((int)camp),
                0, serialized,
                4, 4);
            Array.Copy(
                BitConverter.GetBytes(serial),
                0, serialized,
                8, 4);
            Array.Copy(BitConverter.GetBytes(order),
                0, serialized,
                12, 4);
            return serialized;
        }

        public void Deserialize(byte[] serialized)
        {
            role = (Roles)BitConverter.ToInt32(serialized, 0);
            camp = (Camps)BitConverter.ToInt32(serialized, 4);
            serial = BitConverter.ToInt32(serialized, 8);
            order = BitConverter.ToUInt32(serialized, 12);
        }
    }

    /// <summary>
    /// 让 Identity 支持网络同步的拓展函数。
    /// </summary>
    public static class IdentityReaderWriter
    {
        public static void WriteIdentity(this NetworkWriter writer, Identity identity)
        {
            writer.WriteInt((int)identity.role);
            writer.WriteInt((int)identity.camp);
            writer.WriteInt(identity.serial);
            writer.WriteUInt(identity.order);
            // writer.WriteBytes(identity.Serialize(), 0, sizeof(int) * 3);
        }

        public static Identity ReadIdentity(this NetworkReader reader)
        {
            var role = (Identity.Roles)reader.ReadInt();
            var camp = (Identity.Camps)reader.ReadInt();
            var serial = reader.ReadInt();
            var order = reader.ReadUInt();
            return new Identity(camp, role, serial, order);
            // var data = reader.ReadBytes(sizeof(int) * 3);
            // var newIdentity = new Identity();
            // newIdentity.Deserialize(data);
            // return newIdentity;
        }

        public static Color GetColor(this Identity.Camps camp)
        {
            return camp switch
            {
                Identity.Camps.Red => Color.red,
                Identity.Camps.Blue => Color.blue,
                _ => Color.yellow
            };
        }

        public static Identity.Camps Opposite(this Identity.Camps camp)
        {
            return camp switch
            {
                Identity.Camps.Red => Identity.Camps.Blue,
                Identity.Camps.Blue => Identity.Camps.Red,
                _ => Identity.Camps.Other
            };
        }
    }
}