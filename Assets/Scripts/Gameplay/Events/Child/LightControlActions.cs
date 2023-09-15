using Controllers.Child;
using Infrastructure;
using Infrastructure.Child;

namespace Infrastructure.Child
{
    public static partial class ChildActionID
    {
        public static class LightControl
        {
            public const string TurnLight = "TurnLight";
            public const string TurnPowerRuneLight = "TurnPowerRuneLight";
            public const string SetLightState = "SetLightState";
            public const string SetPowerRuneLightState = "SetPowerRuneLightState";
            public const string SetPercentage = "SetPercentage";
            public const string SetPowerRunePercentage = "SetPowerRunePercentage";
            public const string SyncLightBar = "SyncLightBar";
            public const string SetLightBarState = "SetLightBarState";
        }
    }
}

namespace Gameplay.Events.Child
{
    /// <summary>
    /// 启停灯光。
    /// </summary>
    public class TurnLight : IChildAction
    {
        public bool IsOn;
        public ChildIdentity Receiver;

        public string ActionName() => ChildActionID.LightControl.TurnLight;
        public ChildIdentity ReceiverChildType() => Receiver;
    }
    
    /// <summary>
    /// 启停能量机关的灯光。
    /// </summary>
    public class TurnPowerRuneLight : IPowerRuneChildAction
    {
        public bool IsOn;
        public ChildIdentity Parent;
        public ChildIdentity Receiver;
        public string ActionName() => ChildActionID.LightControl.TurnPowerRuneLight;
        public ChildIdentity ReceiverChildType() => Receiver;
        public ChildIdentity ReceiverParentType() => Parent	;
    }

    /// <summary>
    /// 设置灯光类型。
    /// </summary>
    public class SetLightState : IChildAction
    {
        public LightState State;
        public ChildIdentity Receiver;

        public string ActionName() => ChildActionID.LightControl.SetLightState;
        public ChildIdentity ReceiverChildType() => Receiver;
    }
    
    /// <summary>
    /// 设置能量机关灯光类型。
    /// </summary>
    public class SetPowerRuneLightState : IPowerRuneChildAction
    {
        public LightState State;
        public ChildIdentity Parent;
        public ChildIdentity Receiver;
        public string ActionName() => ChildActionID.LightControl.SetPowerRuneLightState	;
        public ChildIdentity ReceiverChildType() => Receiver;
        public ChildIdentity ReceiverParentType() => Parent	;
    }

    /// <summary>
    /// 设置灯光百分比。
    /// </summary>
    public class SetPercentage : IChildAction
    {
        public float Percentage;
        public ChildIdentity Receiver;

        public string ActionName() => ChildActionID.LightControl.SetPercentage;

        public ChildIdentity ReceiverChildType() => Receiver;
    }
    
    /// <summary>
    /// 设置能量灯光百分比。
    /// </summary>
    public class SetPowerRunePercentage : IPowerRuneChildAction
    {
        public float Percentage;
        public ChildIdentity Parent;
        public ChildIdentity Receiver;

        public string ActionName() => ChildActionID.LightControl.SetPowerRunePercentage;

        public ChildIdentity ReceiverChildType() => Receiver;
        public ChildIdentity ReceiverParentType() => Parent	;
    }

    public class SyncLightBar : IChildAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ChildActionID.LightControl.SyncLightBar;
        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.RobotLightBar);
    }

    public class SetLightBarState : IChildAction
    {
        public float Health;
        public float Revive;
        public bool Buff;
        public string ActionName() => ChildActionID.LightControl.SetLightBarState;
        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.RobotLightBar);
    }
}