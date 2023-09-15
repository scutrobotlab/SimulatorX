using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class ChangeTarget
        {
            public const string ToOutpost = "ToOutpost";
            public const string ToBase = "ToBase";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 目标切换为前哨站
    /// </summary>
    public class ToOutpost : IAction
    {
        public string ActionName() => ActionID.ChangeTarget.ToOutpost;
    }

    /// <summary>
    /// 目标切换为基地
    /// </summary>
    public class ToBase : IAction
    {
        public string ActionName() => ActionID.ChangeTarget.ToBase;
    }
}