using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Events;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Child;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    /// <summary>
    /// 地面灯光控制器。
    /// 控制地面灯光亮灭、闪烁。
    /// TODO：高亮。
    /// </summary>
    public class GroundLightStore : StoreBase
    {
        /// <summary>
        /// 表示前哨站是否被击毁，以此来控制高地灯条的亮灭,初始状态为false(未被击毁)
        /// </summary>
        public bool outpostIsFall = false;

        public double activatedTime;
        public bool available;
        

        /// <summary>
        /// 编辑器中设置
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 标识输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.OutpostFall,
                ActionID.Stage.SentinelFall,
                ActionID.Clock.PowerRuneAvailable,
                ActionID.Stage.PowerRuneActivated,
                ActionID.Stage.TheCommandingPoint,
                ActionID.Stage.LeaveHighLand
            }).ToList();
        }

        /// <summary>
        /// 处理输入事件。
        /// </summary>
        /// <param name="action">事件</param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Stage.OutpostFall:
                    var outpostFallAction = (OutpostFall) action;
                    if (outpostFallAction.Camp == id.camp)
                    {
                        outpostIsFall = true;
                        BlinkOff(0);
                    }

                    break;

                case ActionID.Stage.SentinelFall:
                    var sentinelFallAction = (SentinelFall) action;
                    if (sentinelFallAction.Camp == id.camp)
                    {
                        BlinkOff(1);
                    }

                    break;

                case ActionID.Clock.PowerRuneAvailable:
                    var availableAction = (PowerRuneAvailable) action;
                    Dispatcher.Instance().SendChild(new TurnLight
                    {
                        Receiver = new ChildIdentity(ChildType.IconLight),
                        IsOn = availableAction.Available
                    }, id);
                    available = availableAction.Available;
                    break;
                
                case ActionID.Stage.PowerRuneActivated:
                    var activatedAction = (PowerRuneActivated) action;
                    activatedTime = activatedAction.ActivatedTime;
                    break;

                case ActionID.Stage.TheCommandingPoint:
                    var theCommandingPointAction = (TheCommandingPoint) action;
                    if (!outpostIsFall && id.camp == theCommandingPointAction.Camp)
                    {
                        Dispatcher.Instance().SendChild(new SetLightState()
                        {
                            Receiver = new ChildIdentity(ChildType.RingLight),
                            State = LightState.Bright,
                        }, id);
                    }

                    break;

                case ActionID.Stage.LeaveHighLand:
                    var leaveHighLandAction = (LeaveHighLand) action;
                    if (!outpostIsFall && id.camp == leaveHighLandAction.Camp)
                    {
                        Dispatcher.Instance().SendChild(new SetLightState()
                        {
                            Receiver = new ChildIdentity(ChildType.RingLight),
                            State = LightState.On,
                        }, id);
                    }

                    break;
            }
        }

        /// <summary>
        /// 闪烁并熄灭灯光。
        /// </summary>
        /// <param name="group">灯光组别</param>
        private void BlinkOff(int group)
        {
            StartCoroutine(Blink4TimesOff(group));
        }
        
        /// <summary>
        /// 更新组件。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;
            var time = NetworkTime.time - activatedTime;
            if (time > 75)
            {
                if (available)
                {
                    Dispatcher.Instance().SendChild(new TurnLight
                    {
                        Receiver = new ChildIdentity(ChildType.IconLight),
                        IsOn = true
                    }, id);
                    activatedTime = NetworkTime.time + 420;  
                }
            }
            else if(time > 45)
            {
                Dispatcher.Instance().SendChild(new TurnLight
                {
                    Receiver = new ChildIdentity(ChildType.IconLight),
                    IsOn = false
                }, id);
            }


        }

        /// <summary>
        /// 延时、闪烁并熄灭灯光的协程实现。
        /// </summary>
        /// <param name="group">灯光组别</param>
        /// <returns></returns>
        private IEnumerator Blink4TimesOff(int group)
        {
            for (var i = 0; i < 4; i++)
            {
                Dispatcher.Instance().SendChild(new TurnLight
                {
                    Receiver = new ChildIdentity(ChildType.Light, group),
                    IsOn = false
                }, id);
                Dispatcher.Instance().SendChild(new TurnLight
                {
                    Receiver = new ChildIdentity(ChildType.RingLight, group),
                    IsOn = false
                }, id);
                yield return new WaitForSeconds(0.3f);
                Dispatcher.Instance().SendChild(new TurnLight
                {
                    Receiver = new ChildIdentity(ChildType.Light, group),
                    IsOn = true
                }, id);
                Dispatcher.Instance().SendChild(new TurnLight
                {
                    Receiver = new ChildIdentity(ChildType.RingLight, group),
                    IsOn = true
                }, id);
                yield return new WaitForSeconds(0.3f);
            }

            Dispatcher.Instance().SendChild(new TurnLight
            {
                Receiver = new ChildIdentity(ChildType.Light, group),
                IsOn = false
            }, id);
            Dispatcher.Instance().SendChild(new TurnLight
            {
                Receiver = new ChildIdentity(ChildType.RingLight, group),
                IsOn = false
            }, id);
        }
    }
}