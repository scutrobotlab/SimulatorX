using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Effects;
using Gameplay.Events;
using Infrastructure.Child;
using Mirror;
using Misc;
using UI;
using UnityEngine;

namespace Infrastructure
{
    /// <summary>
    /// <c>StoreBase</c> 是所有 Store 的基类。
    /// <br/>Store 可以接收和发送事件。
    /// </summary>
    public class StoreBase : NetworkBehaviour
    {
        [SyncVar] public Identity id;

        private readonly Queue<IChildAction> _delayCache = new Queue<IChildAction>();
        private readonly List<StoreChildBase> _storeChild = new List<StoreChildBase>();

        protected readonly SyncList<EffectBase> Effects = new SyncList<EffectBase>();

        // 针对地图类型改变
        private bool _initializeByMapType;

        protected YieldInstruction AsyncUpdateDelta = new WaitForFixedUpdate();

        /// <summary>
        /// 被固连
        /// </summary>
        protected bool BeCaught;

        protected bool UIInitialized;

        /// <summary>
        /// 查找全局的Dispatcher，将自己注册到这个Dispatcher上。
        /// </summary>
        protected virtual void Start()
        {
            if (!isServer) return;
            Identify();
            Dispatcher.Instance().Register(this);

            StartCoroutine(AsyncUpdateCoroutine());
        }

        /// <summary>
        /// 移除超时效果。
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (!UIInitialized && UIManager.Instance() != null && UIManager.Instance().Loaded)
            {
                LoadUI();
                UIInitialized = true;
            }

            if (!isServer) return;

            // 初始化地图相关内容
            if (!_initializeByMapType && EntityManager.Instance().initialized)
            {
                InitializeByMapType(EntityManager.Instance().CurrentMap());
                _initializeByMapType = true;
            }

            // 移除超时效果
            var currentTime = Time.time;
            foreach (
                var effect in Effects.Where(
                    effect => effect.timeout != -1).Where(
                    effect => currentTime > effect.createTime + effect.timeout))
            {
                // TODO: Test
                Effects.Remove(effect);
            }

            // 尝试延迟分发事件
            if (_delayCache.Count > 0)
            {
                var delayedActions = new List<IChildAction>();
                while (_delayCache.Count > 0) delayedActions.Add(_delayCache.Dequeue());
                foreach (var delayedAction in delayedActions)
                {
                    ChildReceive(delayedAction);
                }
            }
        }

        /// <summary>
        /// 注册新的子组件。
        /// </summary>
        /// <param name="child">子组件</param>
        public void RegisterChild(StoreChildBase child)
        {
            _storeChild.Add(child);
        }

        protected virtual void Identify() => id = new Identity();

        public virtual void Identify(Identity newID)
        {
            id = newID;
            IdentifyRpc(newID);
        }

        [ClientRpc]
        private void IdentifyRpc(Identity newID)
        {
            id = newID;
        }

        /// <summary>
        /// 在子类中可以对这个函数进行覆盖，分别把自己感兴趣的字符串存储到Dispatcher中。
        /// </summary>
        public virtual List<string> InputActions() => new List<string>
            { ActionID.Effect.AddEffect, ActionID.Effect.RemoveEffect, ActionID.Engineer.CatchState };

        public bool HasEffect(string effectType) => Effects.Count(e => e.type == effectType) > 0;

        public void AddEffect(EffectBase effect) => Effects.Add(effect);

        public void RemoveEffect(string effectType) => Effects.RemoveAll(e => e.type == effectType);

        /// <summary>
        /// 事件处理函数，可重写。
        /// </summary>
        /// <param name="action">事件</param>
        public virtual void Receive(IAction action)
        {
            if (!isServer) return;
            if (action is IChildAction childAction)
            {
                SyncChildReceive(childAction);
            }
            else
            {
                switch (action.ActionName())
                {
                    case ActionID.Effect.AddEffect:
                        var addEffectAction = (AddEffect)action;
                        if (addEffectAction.Receiver == id)
                        {
                            if (!HasEffect(addEffectAction.Effect.type))
                            {
                                var effect = ((AddEffect)action).Effect;
                                if (Dispatcher.Instance().replay)
                                {
                                    effect.createTime = Time.time;
                                }

                                AddEffect(effect);
                            }
                            //修改之前增益(目前赛规暂不需要)
                            /*else
                            {
                                if (addEffectAction.isDown)
                                {
                                    var effect = addEffectAction.Effect;
                                    RemoveEffect(effect.type);
                                   // effect.isDown = true;
                                    if (Dispatcher.Instance().replay)
                                    {
                                        effect.createTime = Time.time;
                                    }

                                    AddEffect(effect);
                                }
                            }*/
                        }

                        break;

                    case ActionID.Effect.RemoveEffect:
                        var removeEffectAction = (RemoveEffect)action;
                        if (removeEffectAction.Receiver == id)
                        {
                            RemoveEffect(removeEffectAction.Effect.type);
                        }

                        break;

                    case ActionID.Engineer.CatchState:
                        var catchStateAction = (CatchState)action;
                        if (catchStateAction.Receiver == id)
                        {
                            BeCaught = catchStateAction.Catching;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// 获得当前受增益影响属性值。
        /// </summary>
        /// <returns></returns>
        public IBuff CurrentBuff()
        {
            var maxDamage = 1f;
            var maxShield = 0f;
            var maxCooling = 1f;
            var maxRecover = 0f;
            foreach (var effect in Effects.Where(e => e is IBuff))
            {
                var buff = (IBuff)effect;
                if (buff.damage > maxDamage) maxDamage = buff.damage;
                if (buff.shield > maxShield) maxShield = buff.shield;
                if (buff.cooling > maxCooling) maxCooling = buff.cooling;
                if (buff.recover > maxRecover) maxRecover = buff.recover;
            }

            return new BuffAttributes(maxDamage, maxShield, maxCooling, maxRecover);
        }

        public void ChildDispatcherSend(IAction action)
        {
            if (!isServer) return;
            if (action is IChildAction childAction)
            {
                Dispatcher.Instance().SendChild(childAction, id);
            }
            else
            {
                Dispatcher.Instance().Send(action);
            }
        }

        /// <summary>
        /// 在客户端和服务端同时转发子组件事件。
        /// </summary>
        /// <param name="action"></param>
        [Server]
        private void SyncChildReceive(IChildAction action)
        {
            if (!isServer) return;
            if (isServerOnly)
            {
                ChildReceive(action);
            }

            ChildReceiveRpc(PolymorphicSerializer.Serialize(action));
        }

        /// <summary>
        /// 利用 ClientRpc 同步事件。
        /// </summary>
        /// <param name="serializedAction">多态序列化事件</param>
        [ClientRpc]
        private void ChildReceiveRpc(string serializedAction)
        {
            ChildReceive(
                (IChildAction)PolymorphicSerializer.Deserialize(serializedAction));
        }

        /// <summary>
        /// 将事件转发给子组件。
        /// </summary>
        /// <param name="action"></param>
        private void ChildReceive(IChildAction action)
        {
            if (action.ActionName() == ChildActionID.LightControl.TurnPowerRuneLight ||
                action.ActionName() == ChildActionID.LightControl.SetPowerRuneLightState ||
                action.ActionName() == ChildActionID.LightControl.SetPowerRunePercentage)
            {
                IPowerRuneChildAction powerRuneAction = (IPowerRuneChildAction)action;
                // 分发给对应子组件
                var receiver = _storeChild
                    .Where(
                        child => powerRuneAction.ReceiverParentType() == child.id)
                    .Where(
                        child => child.InputActions()
                            .Any(inputAction => inputAction == powerRuneAction.ActionName())).ToArray();
                if (receiver.Length > 0)
                {
                    foreach (var child in receiver)
                    {
                        child.Receive(action);
                    }
                }
            }
            else
            {
                // 分发给对应子组件
                var receiver = _storeChild
                    .Where(
                        child => action.ReceiverChildType() == child.id)
                    .Where(
                        child => child.InputActions()
                            .Any(inputAction => inputAction == action.ActionName())).ToArray();
                if (receiver.Length > 0)
                {
                    foreach (var child in receiver)
                    {
                        child.Receive(action);
                    }
                }
                else
                {
                    _delayCache.Enqueue(action);
                }
            }
        }

        /// <summary>
        /// 用于加载 UI 面板。
        /// </summary>
        protected virtual void LoadUI()
        {
        }

        /// <summary>
        /// 用于初始化地图相关内容。
        /// </summary>
        /// <param name="mapType"></param>
        protected virtual void InitializeByMapType(MapType mapType)
        {
        }

        private IEnumerator AsyncUpdateCoroutine()
        {
            while (true)
            {
                yield return AsyncUpdateDelta;
                AsyncUpdate();
            }
        }

        protected virtual void AsyncUpdate()
        {
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return id.Data().Sum(d => d);
        }
    }
}