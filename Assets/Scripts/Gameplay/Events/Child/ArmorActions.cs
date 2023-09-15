using Gameplay.Attribute;
using Infrastructure;
using Infrastructure.Child;
using UnityEngine;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Armor
        {
            public const string ArmorHit = "ArmorHit";
        }
    }
}

namespace Infrastructure.Child
{
    public static partial class ChildActionID
    {
        public static class Armor
        {
            public const string SyncArmor = "SyncArmor";
            public const string TurnArmor = "TurnArmor";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 击中装甲板事件。
    /// </summary>
    public class ArmorHit : IAction
    {
        public Identity Receiver;
        public Identity Hitter;
        public MechanicType.CaliberType Caliber;
        public ChildIdentity Armor;
        public Vector3 Position; //击中点位置
        public Vector3 CenterPos;//装甲板中心位置
        public string ActionName() => ActionID.Armor.ArmorHit;
    }
}

namespace Gameplay.Events.Child
{
    /// <summary>
    /// 同步装甲板信息。
    /// </summary>
    public class SyncArmor : IChildAction
    {
        public Identity.Camps Camp;
        public char Text;
        public ChildIdentity ReceiverChild;
        
        public string ActionName() => ChildActionID.Armor.SyncArmor;
        public ChildIdentity ReceiverChildType() => ReceiverChild;
    }

    /// <summary>
    /// 打开或关闭装甲板。
    /// </summary>
    public class TurnArmor : IChildAction
    {
        public bool IsOn;
        public ChildIdentity ReceiverChild;

        public string ActionName() => ChildActionID.Armor.TurnArmor;

        public ChildIdentity ReceiverChildType() => ReceiverChild;
    }
}