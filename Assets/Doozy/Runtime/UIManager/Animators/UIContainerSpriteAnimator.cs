// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a set of Sprites for a Reactor Sprite Target by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Animators/Container/UI Container Sprite Animator")]
    public class UIContainerSpriteAnimator : BaseUIContainerAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;
        
        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;
        
        [SerializeField] private SpriteAnimation ShowAnimation;
        /// <summary> Container Show Animation </summary>
        public SpriteAnimation showAnimation => ShowAnimation;

        [SerializeField] private SpriteAnimation HideAnimation;
        /// <summary> Container Hide Animation </summary>
        public SpriteAnimation hideAnimation => HideAnimation;
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();

            ShowAnimation ??= new SpriteAnimation(spriteTarget);
            HideAnimation ??= new SpriteAnimation(spriteTarget);

            ResetAnimation(ShowAnimation);
            ResetAnimation(HideAnimation);

            base.Reset();
        }
        #endif
        
        public void FindTarget()
        {
            if (SpriteTarget != null)
                return;
            
            SpriteTarget = ReactorSpriteTarget.FindTarget(gameObject);
            UpdateSettings();
        }
        
        protected override void Awake()
        {
            FindTarget();
            UpdateSettings();
            base.Awake();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShowAnimation?.Recycle();
            HideAnimation?.Recycle();
        }
        
        protected override void ConnectToController()
        {
            base.ConnectToController();
            if (!controller) return;
            
            controller.showReactions.Add(ShowAnimation.animation);
            controller.hideReactions.Add(HideAnimation.animation);
        }
        
        protected override void DisconnectFromController()
        {
            base.DisconnectFromController();
            if (!controller) return;
            
            controller.showReactions.Remove(ShowAnimation.animation);
            controller.hideReactions.Remove(HideAnimation.animation);
        }
        
        public override void Show() =>
            ShowAnimation?.Play(PlayDirection.Forward);

        public override void Hide() =>
            HideAnimation?.Play(PlayDirection.Forward);

        public override void InstantShow() =>
            ShowAnimation?.SetProgressAtOne();
        
        public override void InstantHide() =>
            HideAnimation?.SetProgressAtOne();

        public override void ReverseShow() =>
            ShowAnimation?.Reverse();

        public override void ReverseHide() =>
            HideAnimation?.Reverse();
        
        public override void UpdateSettings()
        {
            if(spriteTarget == null)
                return;

            ShowAnimation?.SetTarget(spriteTarget);
            HideAnimation?.SetTarget(spriteTarget);
        }
        
        public override void StopAllReactions()
        {
            ShowAnimation?.Stop();
            HideAnimation?.Stop();
        }
        
        private static void ResetAnimation(SpriteAnimation target)
        {
            target.animation.Reset();
            target.animation.enabled = false;
            target.animation.settings.duration = 1f;
        }
    }
}
