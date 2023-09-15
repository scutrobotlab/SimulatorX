// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.UIManager.Animators
{
    public abstract class BaseUIContainerAnimator : BaseTargetComponentAnimator<UIContainer>
    {
        private Coroutine executeCommandCoroutine { get; set; }

        protected override void ConnectToController()
        {
            if (controller == null) return;
            controller.showHideExecute += Execute;
            if (controller.executedFirstCommand)
                Execute(controller.previouslyExecutedCommand);
        }

        protected override void DisconnectFromController()
        {
            if (controller == null) return;
            controller.showHideExecute -= Execute;
        }

        protected virtual void Execute(ShowHideExecute execute)
        {
            if (executeCommandCoroutine != null)
            {
                StopCoroutine(executeCommandCoroutine);
                executeCommandCoroutine = null;
            }

            if (!animatorInitialized)
            {
                executeCommandCoroutine = StartCoroutine(ExecuteCommandAfterAnimatorInitialized(execute));
                return;
            }

            switch (execute)
            {
                case ShowHideExecute.Show:
                    Show();
                    return;

                case ShowHideExecute.Hide:
                    Hide();
                    return;

                case ShowHideExecute.InstantShow:
                    InstantShow();
                    return;

                case ShowHideExecute.InstantHide:
                    InstantHide();
                    return;

                case ShowHideExecute.ReverseShow:
                    ReverseShow();
                    return;

                case ShowHideExecute.ReverseHide:
                    ReverseHide();
                    return;

                default:
                    throw new ArgumentOutOfRangeException(nameof(execute), execute, null);
            }
        }

        private IEnumerator ExecuteCommandAfterAnimatorInitialized(ShowHideExecute execute)
        {
            yield return new WaitUntil(() => animatorInitialized);
            Execute(execute);
            executeCommandCoroutine = null;
        }

        public abstract void Show();
        public abstract void ReverseShow();

        public abstract void Hide();
        public abstract void ReverseHide();

        public abstract void InstantShow();
        public abstract void InstantHide();

    }
}
