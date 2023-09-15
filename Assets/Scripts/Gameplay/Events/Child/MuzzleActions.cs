using Infrastructure.Child;
using UnityEngine;

namespace Infrastructure.Child
{
    public static partial class ChildActionID
    {
        public static class Muzzle
        {
            public const string Fire = "Fire";
            public const string HeatStatus = "HeatStatus";
        }
    }
}

namespace Gameplay.Events.Child
{
    public class Fire : IChildAction
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public int salt;

        public Fire()
        {
            salt = (int) (Time.time * 1000);
        }

        public string ActionName() => ChildActionID.Muzzle.Fire;
        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.MuzzleLightBar);
    }

    public class HeatStatus : IChildAction
    {
        public float percentage;
        public string ActionName() => ChildActionID.Muzzle.HeatStatus;
        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.MuzzleLightBar);
    }
}