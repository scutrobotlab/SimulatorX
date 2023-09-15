using UnityEngine;

namespace Infrastructure
{
    /// <summary>
    /// <c>Singleton</c> 用于创建单例。
    /// <br/>该单例基类基于 MonoBehaviour 实现。
    /// </summary>
    /// <typeparam name="T">子类类型</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Object
    {
        private static T _instance;

        /// <summary>
        /// 保证单例。
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance != null) Destroy(gameObject);
        }

        /// <summary>
        /// 获取实例。
        /// </summary>
        /// <returns>单例实例</returns>
        public static T Instance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }

            return _instance;
        }
    }
}