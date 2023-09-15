using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Child
{
    /// <summary>
    /// 用于非根组件的网络视觉效果、控制事件同步。
    /// 可发送、接收控制事件，控制事件将由 <c>StoreBase</c> 自动在端间同步。
    /// </summary>
    public class StoreChildBase : MonoBehaviour
    {
        public ChildIdentity id;
        protected StoreBase Root;
        private bool _rootFound;
        private readonly Queue<IAction> _actionCache = new Queue<IAction>();

        /// <summary>
        /// 确认组件身份。
        /// 每个子类覆写。
        /// </summary>
        protected virtual void Identify() => id = new ChildIdentity();

        /// <summary>
        /// 标识身份
        /// </summary>
        protected virtual void Start()
        {
            Identify();
        }

        protected virtual void StartWithRoot()
        {
        }

        /// <summary>
        /// 寻找根组件
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (_rootFound) return;
            Root = FindRoot();
            if (Root.id == new Identity()) return;
            Root.RegisterChild(this);
            StartWithRoot();
            _rootFound = true;
        }

        /// <summary>
        /// 标识关注事件。
        /// </summary>
        /// <returns></returns>
        public virtual List<string> InputActions() => new List<string>();

        /// <summary>
        /// 可以在客户端和服务端同时收到信息的 <c>Receive</c> 函数。
        /// 信息的跨端同步由 <c>StoreBase</c> 负责。
        /// 只能接收 <c>IChildAction</c> 类型事件。
        /// </summary>
        /// <param name="action">待处理事件</param>
        public virtual void Receive(IChildAction action)
        {
        }

        /// <summary>
        /// 向事件系统发送事件。
        /// </summary>
        /// <param name="action">事件</param>
        protected void DispatcherSend(IAction action)
        {
            _actionCache.Enqueue(action);
            if (Root != null && Root.id != new Identity())
            {
                while (_actionCache.Count > 0)
                {
                    Root.ChildDispatcherSend(_actionCache.Dequeue());
                }
            }
            else
            {
                _actionCache.Enqueue(action);
            }
        }

        /// <summary>
        /// 找到用于提供网络功能的根 StoreBase 组件。
        /// </summary>
        private StoreBase FindRoot()
        {
            var current = transform;
            while (true)
            {
                var root = current.GetComponent<StoreBase>();
                if (root) return root;
                if (current.parent == null) break;
                current = current.parent;
            }

            throw new Exception("StoreChild without StoreBase is invalid.");
        }
    }
}