using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using Gameplay.Events;
using Infrastructure;
using Misc;
using UnityEngine;

namespace UI
{
    public class TutorialPanel : StoreBase
    {
        private readonly ToggleHelper _escapeHelper = new ToggleHelper();
        public CanvasGroup canvas;
        public Animator animator;
        
        protected override void Start()
        {
            if (PlayerPrefs.GetString("TutorialsSwitch", "true") == "false")
            {
                gameObject.SetActive(false);
                return;
            }
            
            base.Start();
            
            animator.Play("Panel In");
        }

        public override List<string> InputActions() => new List<string>
        {
            ActionID.Input.StateControl
        };

        public override void Receive(IAction action)
        {
            if (action.ActionName() != ActionID.Input.StateControl) return;
            
            var stateControlAction = (StateControl)action;
            if (_escapeHelper.Toggle(stateControlAction.FunctionI) != ToggleHelper.State.Hold)
            {
                animator.Play(canvas.alpha > 0 ? "Panel Out" : "Panel In");
            }
        }
    }
}