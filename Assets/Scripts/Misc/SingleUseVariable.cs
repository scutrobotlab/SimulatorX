using Doozy.Runtime.Reactor;
using JetBrains.Annotations;

namespace Misc
{
    /// <summary>
    /// 一次性变量
    /// 阅后即焚变量
    /// </summary>
    public class SingleUseVariable<T>
    {
        [CanBeNull]
        public T Get()
        {
            if (IsNull) return Default;
            IsNull = true;
            return _value;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public void Set(T value)
        {
            _value = value;
            IsNull = _value == null;
        }

        public SingleUseVariable(T @default)
        {
            Default = @default;
        }

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void Reset()
        {
            IsNull = true;
            _value = Default;
        }

        public bool IsNull { get; private set; }

        public bool IsNotNull => !IsNull;

        public T Default = default;

        [CanBeNull] private T _value;
    }
}