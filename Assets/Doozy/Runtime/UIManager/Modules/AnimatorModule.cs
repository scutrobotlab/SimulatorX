// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Modules
{
    [AddComponentMenu("Doozy/UI/Modules/Animator Module")]
    public class AnimatorModule : ModyModule
    {
        public const string k_DefaultModuleName = "Animator Module";

        public AnimatorModule() : this(k_DefaultModuleName) {}

        public AnimatorModule(string moduleName) : base(moduleName.IsNullOrEmpty() ? k_DefaultModuleName : moduleName) {}

        public List<ReactorAnimator> Animators = new List<ReactorAnimator>();

        public SimpleModyAction PlayForward;
        public SimpleModyAction PlayReverse;
        public SimpleModyAction Stop;
        public SimpleModyAction Finish;
        public SimpleModyAction Reverse;
        public SimpleModyAction Rewind;
        public SimpleModyAction Pause;
        public SimpleModyAction Resume;
        public FloatModyAction SetProgressAt;
        public SimpleModyAction SetProgressAtZero;
        public SimpleModyAction SetProgressAtOne;
        public FloatModyAction PlayToProgress;
        public FloatModyAction PlayFromProgress;
        public SimpleModyAction UpdateValues;

        protected override void SetupActions()
        {
            this.AddAction(PlayForward ??= new SimpleModyAction(this, nameof(PlayForward), ExecutePlayForward));
            this.AddAction(PlayReverse ??= new SimpleModyAction(this, nameof(PlayReverse), ExecutePlayReverse));
            this.AddAction(Stop ??= new SimpleModyAction(this, nameof(Stop), ExecuteStop));
            this.AddAction(Finish ??= new SimpleModyAction(this, nameof(Finish), ExecuteFinish));
            this.AddAction(Reverse ??= new SimpleModyAction(this, nameof(Reverse), ExecuteReverse));
            this.AddAction(Rewind ??= new SimpleModyAction(this, nameof(Rewind), ExecuteRewind));
            this.AddAction(Pause ??= new SimpleModyAction(this, nameof(Pause), ExecutePause));
            this.AddAction(Resume ??= new SimpleModyAction(this, nameof(Resume), ExecuteResume));
            this.AddAction(SetProgressAt ??= new FloatModyAction(this, nameof(SetProgressAt), ExecuteSetProgressAt));
            this.AddAction(SetProgressAtZero ??= new SimpleModyAction(this, nameof(SetProgressAtZero), ExecuteSetProgressAtZero));
            this.AddAction(SetProgressAtOne ??= new SimpleModyAction(this, nameof(SetProgressAtOne), ExecuteSetProgressAtOne));
            this.AddAction(PlayToProgress ??= new FloatModyAction(this, nameof(PlayToProgress), ExecutePlayToProgress));
            this.AddAction(PlayFromProgress ??= new FloatModyAction(this, nameof(PlayFromProgress), ExecutePlayFromProgress));
            this.AddAction(UpdateValues ??= new SimpleModyAction(this, nameof(UpdateValues), ExecuteUpdateValues));
        }

        public void CleanAnimatorsList()
        {
            for (int i = Animators.Count - 1; i >= 0; i--)
                if (Animators[i] == null)
                    Animators.RemoveAt(i);
        }

        public void ExecutePlayForward()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Play(PlayDirection.Forward);
        }

        public void ExecutePlayReverse()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Play(PlayDirection.Reverse);
        }

        public void ExecuteStop()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Stop();
        }

        public void ExecuteFinish()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Finish();
        }

        public void ExecuteReverse()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Reverse();
        }

        public void ExecuteRewind()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Rewind();
        }
        
        public void ExecutePause()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Pause();
        }
        
        public void ExecuteResume()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Resume();
        }
        
        public void ExecuteSetProgressAt(float value)
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.SetProgressAt(value);
        }
        
        public void ExecuteSetProgressAtZero()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.SetProgressAtZero();
        }
        
        public void ExecuteSetProgressAtOne()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.SetProgressAtOne();
        }
        
        public void ExecutePlayToProgress(float value)
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.PlayToProgress(value);
        }
        
        public void ExecutePlayFromProgress(float value)
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.PlayFromProgress(value);
        }
        
        public void ExecuteUpdateValues()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.UpdateValues();
        }
    }
}
