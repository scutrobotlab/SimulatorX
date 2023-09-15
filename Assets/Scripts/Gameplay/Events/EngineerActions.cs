using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Engineer
        {
            public const string CatchState = "CatchState";
            public const string Revive = "Revive";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 更新被工程固连状态。
    /// </summary>
    public class CatchState : IAction
    {
        public Identity Receiver;
        public bool Catching;
        public string ActionName() => ActionID.Engineer.CatchState;
    }

    /// <summary>
    /// 表示正在被刷卡复活。
    /// </summary>
    public class CardRevive : IAction
    {
        public Identity Receiver;
        public string ActionName() => ActionID.Engineer.Revive;
    }
}