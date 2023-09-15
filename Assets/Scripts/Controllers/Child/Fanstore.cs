using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Gameplay;
using Infrastructure;
using Infrastructure.Child;
//using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers.Child
{
    public class Fanstore : StoreChildBase
    {
        public Identity.Camps camp;
        private readonly List<PowerRuneLightController> _storeChild = new List<PowerRuneLightController>();
        private readonly Queue<IChildAction> _delayCache = new Queue<IChildAction>();
        
        /// <summary>
        /// 声明关注事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.LightControl.TurnPowerRuneLight,
                ChildActionID.LightControl.SetPowerRuneLightState,
                ChildActionID.LightControl.SetPowerRunePercentage
            }).ToList();
        }

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }
        
        /// <summary>
        /// 事件处理。转发给子组件
        /// 在这里使用不同的方式发给不同的灯光组件？
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.LightControl.TurnPowerRuneLight:
                    var turnAction = (TurnPowerRuneLight)action;
                    if (turnAction.Parent == id)
                    {
                        TurnLight childAction = new TurnLight() 
                        { 
                            IsOn=turnAction.IsOn,
                            Receiver=turnAction.Receiver 
                        };
                        ChildReceive(childAction);
                    }
                    break;

                case ChildActionID.LightControl.SetPowerRuneLightState:
                    var stateAction = (SetPowerRuneLightState)action;
                    if (stateAction.Parent == id)
                    {
                        SetLightState childAction = new SetLightState() 
                        { 
                            State= stateAction.State,
                            Receiver=stateAction.Receiver 
                        };
                        ChildReceive(childAction);
                    }
                    break;

                case ChildActionID.LightControl.SetPowerRunePercentage:
                    var percentageAction = (SetPowerRunePercentage)action;
                    if (percentageAction.Parent == id)
                    {
                        SetPercentage childAction = new SetPercentage() 
                        { 
                            Percentage	= percentageAction.Percentage,
                            Receiver=percentageAction.Receiver 
                        };
                        ChildReceive(childAction);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// 模仿storebase,尝试延迟分发事件
        /// </summary>
        protected override void FixedUpdate()
        {
            //寻找父组件
            base.FixedUpdate();

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
        /// 将事件转发给灯光子组件。
        /// </summary>
        /// <param name="action"></param>
        private void ChildReceive(IChildAction action)
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
        
        
        
        /// <summary>
        /// 注册灯光子组件。
        /// </summary>
        /// <param name="child">子组件</param>
        public void RegisterChild(PowerRuneLightController child)
        {
            _storeChild.Add(child);
        }
        
    }
}
