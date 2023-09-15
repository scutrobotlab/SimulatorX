// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Visual
{
    /// <summary>
    /// Specialized visual component used to swap a Sprites for a Reactor Sprite Target by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Visual/Container/UI Container Sprite Swapper")]
    public class UIContainerSpriteSwapper : BaseUIContainerAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;

        [SerializeField] private Sprite ShowSprite;
        /// <summary> Container Show Sprite </summary>
        public Sprite showSprite => ShowSprite;

        [SerializeField] private Sprite HideSprite;
        /// <summary> Container Hide Sprite </summary>
        public Sprite hideSprite => HideSprite;

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

        public override void Show()
        {
            if (!hasSpriteTarget)
                return;

            if (showSprite != null)
                SpriteTarget.SetSprite(showSprite);
        }

        public override void ReverseShow() =>
            Hide();

        public override void Hide()
        {
            if (!hasSpriteTarget)
                return;

            if (hideSprite != null)
                SpriteTarget.SetSprite(hideSprite);
        }

        public override void ReverseHide() =>
            Show();

        public override void InstantShow() =>
            Show();

        public override void InstantHide() =>
            Hide();
    }
}
