using System.Collections.Generic;
using System.Linq;
using Gameplay.Events;
using Gameplay;
using Infrastructure;
using Mirror;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// 裁判控制器。
    /// 用于控制裁判、观赛视角。
    /// </summary>
    public class JudgeStore : RobotStoreBase
    { 
        /// <summary>
        /// 在编辑器中设置或生成时设置。
        /// </summary>
        protected override void Identify()
        {
        }
        
        //裁判移动组件
        public Transform moveElement;
        
        //裁判视角旋转组件
        public Transform rotateElement;
        
        /// <summary>
        /// 声明接收事件，用于裁判的移动以及视角的旋转。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                // 裁判的移动控制
                ActionID.Input.PrimaryAxis,
                //裁判接收鼠标的旋转控制
                ActionID.Input.ViewControl,
                //裁判判罚撞车
                ActionID.Stage.Crash
            }).ToList();
        }
        
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Input.PrimaryAxis:
                    var act = (PrimaryAxis) action;
                    if (act.Receiver == id)
                    { 
                        var move = new Vector3(act.X, 0, act.Y) * Time.fixedDeltaTime * 3;
                        moveElement.transform.position = rotateElement.transform.TransformPoint(move);
                    }
                    break;

                case ActionID.Input.ViewControl:
                    var viewControlAction = (ViewControl) action;
                    if (viewControlAction.Receiver == id)
                    {
                        // 控制视角
                        var rotation = new Vector3(viewControlAction.Y * -1, viewControlAction.X, 0) * Time.fixedDeltaTime;
                        rotateElement.Rotate(rotation);
                        rotateElement.Rotate(Vector3.back * rotateElement.rotation.eulerAngles.z);
                    }
                    break;
                case ActionID.Stage.Crash:
                    Debug.Log("fight it");
                    break;
            }
        }
    }
}