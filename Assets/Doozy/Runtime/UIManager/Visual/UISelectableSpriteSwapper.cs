// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Visual
{
    /// <summary>
    /// Specialized visual component used to swap a Sprites for a Reactor Sprite Target by listening to a UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Visual/Selectable/UI Selectable Sprite Swapper")]
    public class UISelectableSpriteSwapper : BaseUISelectableAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;

        [SerializeField] private Sprite NormalSprite;
        /// <summary> Container Normal Sprite </summary>
        public Sprite normalSprite => NormalSprite;

        [SerializeField] private Sprite HighlightedSprite;
        /// <summary> Container Highlighted Sprite </summary>
        public Sprite highlightedSprite => HighlightedSprite;

        [SerializeField] private Sprite PressedSprite;
        /// <summary> Container Pressed Sprite </summary>
        public Sprite pressedSprite => PressedSprite;

        [SerializeField] private Sprite SelectedSprite;
        /// <summary> Container Selected Sprite </summary>
        public Sprite selectedSprite => SelectedSprite;

        [SerializeField] private Sprite DisabledSprite;
        /// <summary> Container Disabled Sprite </summary>
        public Sprite disabledSprite => DisabledSprite;

        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();
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

        public override void UpdateSettings() {}   //ignored
        public override void StopAllReactions() {} //ignored

        public override bool IsStateEnabled(UISelectionState state) =>
            true;

        public override void Play(UISelectionState state)
        {
            if (!hasSpriteTarget)
                return;

            switch (state)
            {
                case UISelectionState.Normal:
                    if (normalSprite != null)
                        SpriteTarget.SetSprite(normalSprite);
                    break;
                case UISelectionState.Highlighted:
                    if (highlightedSprite != null)
                        SpriteTarget.SetSprite(highlightedSprite);
                    break;
                case UISelectionState.Pressed:
                    if (pressedSprite != null)
                        SpriteTarget.SetSprite(pressedSprite);
                    break;
                case UISelectionState.Selected:
                    if (selectedSprite != null)
                        SpriteTarget.SetSprite(selectedSprite);
                    break;
                case UISelectionState.Disabled:
                    if (disabledSprite != null)
                        SpriteTarget.SetSprite(disabledSprite);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
