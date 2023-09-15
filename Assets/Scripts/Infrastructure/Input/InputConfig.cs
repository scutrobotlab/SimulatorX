namespace Infrastructure.Input
{
    /// <summary>
    /// <c>InputActionID</c> 是 InputSystem 中所注册动作的对应名称。
    /// <br/>目前包括主辅轴、视角控制（单独处理）、主副开火、A~J六个功能键。
    /// </summary>
    public static class InputActionID
    {
        // Vector2 Axis
        public const string PrimaryAxis = "PrimaryAxis";
        public const string SecondaryAxis = "SecondaryAxis";

        // Buttons
        public const string Fire = "Fire";
        public const string SecondaryFire = "SecondaryFire";
        public const string FunctionA = "FunctionA";
        public const string FunctionB = "FunctionB";
        public const string FunctionC = "FunctionC";
        public const string FunctionD = "FunctionD";
        public const string FunctionE = "FunctionE";
        public const string FunctionF = "FunctionF";
        public const string FunctionG = "FunctionG";
        public const string FunctionH = "FunctionH";
        public const string FunctionI = "FunctionI";
        public const string FunctionJ = "FunctionJ";
        public const string FunctionK = "FunctionK";
        public const string FunctionL = "FunctionL";
        public const string FunctionM = "FunctionM";
        public const string FunctionN = "FunctionN";
    }

    /// <summary>
    /// <c>InputButtonConfig</c> 表示功能键状态映射方式。
    /// <br/>目前包括按下、开关两种。
    /// </summary>
    public enum InputButtonConfig
    {
        Push,
        Toggle
    }
}