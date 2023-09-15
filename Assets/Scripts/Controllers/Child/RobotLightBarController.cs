using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Gameplay.Events.Child;
using Infrastructure.Child;
using UnityEngine;

namespace Controllers.Child
{
    public class RobotLightBarController : StoreChildBase
    {
        public Color reviveColor;
        public LightController healthBar;
        public LightController reviveBar;
        public LightController buffBar;

        /// <summary>
        /// 初始化时设置。
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 初始化灯条 ID。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            healthBar.id = id;
            reviveBar.id = id;
            buffBar.id = id;
            reviveBar.SetCustomPercentageColor(reviveColor);
            buffBar.SetCustomPercentageColor(reviveColor);
        }

        /// <summary>
        /// 标识输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.LightControl.SyncLightBar,
                ChildActionID.LightControl.SetLightBarState
            }).ToList();
        }

        /// <summary>
        /// 处理输入事件。
        /// </summary>
        /// <param name="action">事件</param>
        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.LightControl.SyncLightBar:
                    var syncAction = (SyncLightBar) action;
                    healthBar.camp = syncAction.Camp;
                    reviveBar.camp = syncAction.Camp;
                    buffBar.camp = syncAction.Camp;
                    DispatcherSend(new TurnLight
                    {
                        IsOn = true,
                        Receiver = id
                    });
                    DispatcherSend(new SetLightState
                    {
                        State = LightState.Percentage,
                        Receiver = id
                    });
                    break;

                case ChildActionID.LightControl.SetLightBarState:
                    var setStateAction = (SetLightBarState) action;
                    var isHealth = setStateAction.Health > 0;
                    if (!setStateAction.Buff)
                    {
                        buffBar.gameObject.SetActive(false);
                        healthBar.gameObject.SetActive(isHealth);
                        reviveBar.gameObject.SetActive(!isHealth);
                        DispatcherSend(new SetPercentage
                        {
                            Percentage = isHealth ? setStateAction.Health : setStateAction.Revive,
                            Receiver = id
                        });
                    }
                    else
                    {
                        buffBar.gameObject.SetActive(true);
                        healthBar.gameObject.SetActive(false);
                        reviveBar.gameObject.SetActive(false);
                        DispatcherSend(new SetPercentage
                        {
                            Percentage = 1,
                            Receiver = id
                        });
                    }

                    break;
            }
        }
    }
}