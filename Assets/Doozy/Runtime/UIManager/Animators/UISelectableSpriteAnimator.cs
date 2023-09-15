// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a set of Sprites for a Reactor Sprite Target by listening to a UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Animators/Selectable/UI Selectable Sprite Animator")]
    public class UISelectableSpriteAnimator : BaseUISelectableAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;
        
        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;
        
        [SerializeField] private SpriteAnimation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public SpriteAnimation normalAnimation => NormalAnimation;

        [SerializeField] private SpriteAnimation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public SpriteAnimation highlightedAnimation => HighlightedAnimation;

        [SerializeField] private SpriteAnimation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public SpriteAnimation pressedAnimation => PressedAnimation;

        [SerializeField] private SpriteAnimation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public SpriteAnimation selectedAnimation => SelectedAnimation;

        [SerializeField] private SpriteAnimation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public SpriteAnimation disabledAnimation => DisabledAnimation;
        
        /// <summary> Get the animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public SpriteAnimation GetAnimation(UISelectionState state)
        {
            switch (state)
            {
                case UISelectionState.Normal: return NormalAnimation;
                case UISelectionState.Highlighted: return HighlightedAnimation;
                case UISelectionState.Pressed: return PressedAnimation;
                case UISelectionState.Selected: return SelectedAnimation;
                case UISelectionState.Disabled: return DisabledAnimation;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();

            NormalAnimation ??= new SpriteAnimation(spriteTarget);
            HighlightedAnimation ??= new SpriteAnimation(spriteTarget);
            PressedAnimation ??= new SpriteAnimation(spriteTarget);
            SelectedAnimation ??= new SpriteAnimation(spriteTarget);
            DisabledAnimation ??= new SpriteAnimation(spriteTarget);

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                ResetAnimation(state);

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
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Recycle();
        }
        
        public override bool IsStateEnabled(UISelectionState state)
        {
            SpriteAnimation spriteAnimation = GetAnimation(state);
            return spriteAnimation is { isEnabled: true };
        }

        public override void UpdateSettings()
        {
            if(spriteTarget == null)
                return;

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.SetTarget(spriteTarget);
        }

        public override void StopAllReactions()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Stop();
        }

        public override void Play(UISelectionState state) =>
            GetAnimation(state)?.Play();

        private void ResetAnimation(UISelectionState state)
        {
            SpriteAnimation a = GetAnimation(state);
            
            a.animation.Reset();
            a.animation.enabled = false;
            a.animation.settings.duration = 0.5f;
        }
    }
}
