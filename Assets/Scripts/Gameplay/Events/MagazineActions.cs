using Gameplay.Attribute;
using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Magazine
        {
            public const string AddBullet = "AddBullet";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 子弹补给。
    /// </summary>
    public class AddBullet : IAction
    {
        public string ActionName() => ActionID.Magazine.AddBullet;
        public Identity Receiver;
        public MechanicType.CaliberType Type;
        public int Amount = 5;
    }
}