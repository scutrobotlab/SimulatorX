using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Events;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Child;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// 矿石爪控制器。
    /// </summary>
    public class OreFallStore : StoreBase
    {
        public GameObject[] goldOres;

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 声明兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Clock.DropOre
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Clock.DropOre:
                    var oreFallAction = (DropOre) action;
                    DropOre(oreFallAction.Index);
                    break;
            }
        }

        /// <summary>
        /// 给要掉落的金矿石添加 <c>Rigidbody</c>
        /// </summary>
        /// <param name="i">矿石序号</param>
        private void DropOre(int i)
        {
            BlinkOffAndDrop(i);
        }

        /// <summary>
        ///  闪灯并掉落矿石。
        /// </summary>
        /// <param name="index">矿石序号</param>
        private void BlinkOffAndDrop(int index)
        {
            StartCoroutine(Blink3TimesOff(index));
        }

        /// <summary>
        /// 闪灯并掉落矿石的协程实现。3Hz,1.5s
        /// </summary>
        /// <param name="index">矿石序号</param>
        /// <returns></returns>
        private IEnumerator Blink3TimesOff(int index)
        {
            int count = 0;
            bool lightOn = false;
            var time = new WaitForSeconds(0.33f);

            while (count < 4)
            {
                Dispatcher.Instance().SendChild(new TurnLight
                {
                    Receiver = new ChildIdentity(ChildType.Light, index),
                    IsOn = lightOn
                }, id);
                yield return time;

                lightOn = !lightOn;
                ++count;
            }

            Dispatcher.Instance().SendChild(new TurnLight
            {
                Receiver = new ChildIdentity(ChildType.Light, index),
                IsOn = false
            }, id);
            
            if (goldOres[index].GetComponent<Rigidbody>() == null)
            {
                var rigid = goldOres[index].AddComponent<Rigidbody>();
                rigid.mass = 3;
            }
        }
    }
}