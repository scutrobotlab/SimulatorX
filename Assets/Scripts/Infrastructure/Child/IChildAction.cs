namespace Infrastructure.Child
{
    /// <summary>
    /// 用于标识发送给子组件的回环事件。
    /// </summary>
    public interface IChildAction : IAction
    {
        public ChildIdentity ReceiverChildType();
    }
    
    /// <summary>
    /// 用于标识发送给能量机关子组件的回环事件。
    /// </summary>
    public interface IPowerRuneChildAction : IChildAction
    {
        public ChildIdentity ReceiverParentType();
    }
}