using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Input
        {
            public const string PrimaryAxis = "PrimaryAxis";
            public const string SecondaryAxis = "SecondaryAxis";
            public const string ViewControl = "ViewControl";
            public const string StateControl = "StateControl";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// <c>PrimaryAxis</c> 附带主轴输入信息。
    /// <br/>二维向量。
    /// </summary>
    public class PrimaryAxis : IAction
    {
        public Identity Receiver;
        public float X;
        public float Y;
        public string ActionName() => ActionID.Input.PrimaryAxis;
    }

    /// <summary>
    /// <c>SecondaryAxis</c> 附带副轴输入信息。
    /// <br/>二维向量。
    /// </summary>
    public class SecondaryAxis : IAction
    {
        public Identity Receiver;
        public float X;
        public float Y;
        public string ActionName() => ActionID.Input.SecondaryAxis;
    }

    /// <summary>
    /// <c>ViewControl</c> 附带视角控制输入信息。
    /// <br/>二维向量。
    /// </summary>
    public class ViewControl : IAction
    {
        public Identity Receiver;
        public float X;
        public float Y;
        public string ActionName() => ActionID.Input.ViewControl;
    }

    /// <summary>
    /// <c>StateControl</c> 附带功能输入信息。
    /// <br/>包含主开火键，副开火键，A~F六个功能键布尔状态。
    /// </summary>
    public class StateControl : IAction
    {
        public Identity Receiver;
        public bool Fire;
        public bool SecondaryFire;
        public bool FunctionA;
        public bool FunctionB;
        public bool FunctionC;
        public bool FunctionD;
        public bool FunctionE;
        public bool FunctionF;
        public bool FunctionG;
        public bool FunctionH;
        public bool FunctionI;
        public bool FunctionJ;
        public bool FunctionK;
        public bool FunctionL;
        public bool FunctionM;
        public bool FunctionN;
        public string ActionName() => ActionID.Input.StateControl;
    }
}