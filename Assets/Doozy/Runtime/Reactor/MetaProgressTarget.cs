using UnityEngine;
namespace Doozy.Runtime.Reactor
{
    public abstract class MetaProgressTarget<T> : ProgressTarget
    {
        [SerializeField] protected T Target;
        public T target => Target;
    }
}
