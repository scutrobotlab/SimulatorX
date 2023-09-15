using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Effect
        {
            public const string AddEffect = "AddEffect";
            public const string RemoveEffect = "RemoveEffect";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// <c>AddEffect</c> 用于给实体添加效果。
    /// </summary>
    public class AddEffect : IAction
    {
        public Identity Receiver;
        public EffectBase Effect;
       // public bool isDown;
        public string ActionName() => ActionID.Effect.AddEffect;
    }

    /// <summary>
    /// <c>RemoveEffect</c> 用于从实体移除效果。
    /// </summary>
    public class RemoveEffect : IAction
    {
        public Identity Receiver;
        public EffectBase Effect;
        public string ActionName() => ActionID.Effect.RemoveEffect;
    }
}