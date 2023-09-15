using Infrastructure;

namespace Infrastructure
{
    //有关中央控制区的动作
    public static partial class ActionID
    {
        public static class ControlAction
        {
            public const string SearchControlEffect = "SearchControlEffect";
            public const string SendOccupiedMessage = "SendOccupiedMessage";
            public const string SendLeftMessage = "SendLeftMessage";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 对中央控制区效果寻找
    /// </summary>
    public class SearchControlEffect : IAction
    {
        public string ActionName() => ActionID.ControlAction.SearchControlEffect;
    }

    /// <summary>
    /// 中央控制区是否有机器人进入，开始占领动作（尚未完成占领）
    /// </summary>
    public class SendOccupiedMessage : IAction
    {
        public bool CanCount;
        public float Time;
        public Identity.Camps Camp;
        public string ActionName() => ActionID.ControlAction.SendOccupiedMessage;
    }
    
    /// <summary>
    /// 机器人离开中央控制区
    /// </summary>
    public class SendLeftMessage : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.ControlAction.SendLeftMessage;
    }
}