using Infrastructure.Child;

namespace Infrastructure.Child
{
    public static partial class ChildActionID
    {
        public static class Indicator
        {
            public const string SpinStatus = "SpinStatus";
            public const string BulletproofStatus = "BulletproofStatus";
            public const string GunLockedStatus = "GunLockedStatus";
            public const string SuperBatteryStatus = "SuperBatteryStatus";
            public const string MagazineStatus = "MagazineStatus";
            public const string ArmorTurningStatus = "ArmorTurningStatus";
        }
    }
}

namespace Gameplay.Events.Child
{
    public class SpinStatus : IChildAction
    {
        public bool spinning;
        public string ActionName() => ChildActionID.Indicator.SpinStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.SpinIndicator);
    }

    public class BulletproofStatus : IChildAction
    {
        public bool bulletproof;
        public string ActionName() => ChildActionID.Indicator.BulletproofStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.BulletproofIndicator);
    }

    public class GunLockedStatus : IChildAction
    {
        public bool gunLocked;
        public string ActionName() => ChildActionID.Indicator.GunLockedStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.GunLockedIndicator);
    }
    public class SuperBatteryStatus : IChildAction
    {
        public bool supperBattery;
        public string ActionName() => ChildActionID.Indicator.SuperBatteryStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.SuperBatteryIndicator);
    }
    public class MagazineStatus : IChildAction
    {
        public bool magazine;
        public string ActionName() => ChildActionID.Indicator.MagazineStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.MagazineIndicator);
    }
    public class ArmorTurningStatus : IChildAction
    {
        public bool ArmorTurning;
        public string ActionName() => ChildActionID.Indicator.ArmorTurningStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.ArmorTurningIndicator);
    }
}