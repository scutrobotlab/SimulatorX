using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.RobotSensor;
using Gameplay.Effects;
using Gameplay.Events;
using Gameplay;
using Gameplay.Attribute;
using Infrastructure;
using Mirror;
using UnityEngine;

namespace Lab.NPC
{
    public class NpcStore : StoreBase
    {
        private Animator _npc;
        private static readonly int SaluteTrig = Animator.StringToHash("Salute_trig");

        protected override void Start()
        {
            base.Start();
            _npc = GetComponent<Animator>();
        }
        
        /// <summary>
        /// 标识兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.Crash
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ActionID.Stage.Crash:
                    _npc.SetTrigger(SaluteTrig);
                    break;
            }
        }
        
        private void OnCollisionEnter()
        {
            //使用set_trigger 是只执行运动指令一次，而使用set_integer是直接修改默认值，会一直执行
            //在Animator界面设置动作的conditions就可修改 
            _npc.SetTrigger(SaluteTrig);
        }
    }
}
