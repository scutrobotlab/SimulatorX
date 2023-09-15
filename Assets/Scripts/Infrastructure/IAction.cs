namespace Infrastructure
{
    /// <summary>
    /// <c>IAction</c> 是所有事件的共有接口。
    /// <br/>所有事件必须能返回自身类型标识。
    /// </summary>
    public interface IAction
    {
        public string ActionName();
    }
}