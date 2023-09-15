namespace Gameplay.Effects
{
    /// <summary>
    /// 作为加成表示使用的 <c>Effect</c> 的共有接口。
    /// <br/>包含攻击力、防御力、枪口热量冷却、生命回复四种加成类型。
    /// </summary>
    public interface IBuff
    {
        public float damage { get; }
        public float shield { get; }
        public float cooling { get; }
        public float recover { get; }
    }
}