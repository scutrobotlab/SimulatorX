// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Layouts;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha by listening to a target UISelectable (controller) selection state changes.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Animators/Selectable/UI Selectable UI Animator")]
    public class UISelectableUIAnimator : BaseUISelectableAnimator
    {
        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        [SerializeField] private UIAnimation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public UIAnimation normalAnimation => NormalAnimation;

        [SerializeField] private UIAnimation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public UIAnimation highlightedAnimation => HighlightedAnimation;

        [SerializeField] private UIAnimation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public UIAnimation pressedAnimation => PressedAnimation;

        [SerializeField] private UIAnimation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public UIAnimation selectedAnimation => SelectedAnimation;

        [SerializeField] private UIAnimation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public UIAnimation disabledAnimation => DisabledAnimation;

        public bool anyAnimationIsActive => normalAnimation.isActive || highlightedAnimation.isActive || pressedAnimation.isActive || selectedAnimation.isActive || disabledAnimation.isActive;
        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }
        private bool playStateAnimationFromLateUpdate { get; set; }

        /// <summary> Get the Animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public UIAnimation GetAnimation(UISelectionState state)
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
            NormalAnimation ??= new UIAnimation(rectTransform);
            HighlightedAnimation ??= new UIAnimation(rectTransform);
            PressedAnimation ??= new UIAnimation(rectTransform);
            SelectedAnimation ??= new UIAnimation(rectTransform);
            DisabledAnimation ??= new UIAnimation(rectTransform);

            ResetAnimation(NormalAnimation);
            ResetAnimation(HighlightedAnimation);
            ResetAnimation(PressedAnimation);
            ResetAnimation(SelectedAnimation);
            ResetAnimation(DisabledAnimation);

            NormalAnimation.animationType = UIAnimationType.Reset;
            NormalAnimation.Move.enabled = true;
            NormalAnimation.Rotate.enabled = true;
            NormalAnimation.Scale.enabled = true;
            NormalAnimation.Fade.enabled = true;

            base.Reset();
        }
        #endif


        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            animatorInitialized = false;
            m_RectTransform = GetComponent<RectTransform>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            UpdateLayout();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            playStateAnimationFromLateUpdate = true;
            base.OnEnable();
            UpdateLayout();
            updateStartPositionInLateUpdate = true;
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying) return;
            base.OnDisable();
            RefreshLayout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Recycle();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!isConnected) return;
            if (!isInLayoutGroup) return;
            updateStartPositionInLateUpdate = true;
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;

            if (playStateAnimationFromLateUpdate)
            {
                if (isConnected)
                {
                    Play(controller.currentUISelectionState);
                    playStateAnimationFromLateUpdate = false;
                }
            }

            if (!isInLayoutGroup) return;
            if (!isConnected) return;
            if (anyAnimationIsActive) return;
            if (!updateStartPositionInLateUpdate && localPosition == rectTransform.localPosition) return;
            if (controller.currentUISelectionState != UISelectionState.Normal) return;
            if (CanvasUpdateRegistry.IsRebuildingLayout()) return;
            RefreshLayout();
            UpdateStartPosition();
        }

        private void UpdateLayout()
        {
            isInLayoutGroup = rectTransform.IsInLayoutGroup();
            uiBehaviourHandler = null;
            if (!isInLayoutGroup) return;
            LayoutGroup layout = rectTransform.GetLayoutGroupInParent();
            if (layout == null) return;
            uiBehaviourHandler = layout.GetUIBehaviourHandler();
            System.Diagnostics.Debug.Assert(uiBehaviourHandler != null, nameof(uiBehaviourHandler) + " != null");
            uiBehaviourHandler.SetDirty();
        }

        private void RefreshLayout()
        {
            if (uiBehaviourHandler == null) return;
            uiBehaviourHandler.RefreshLayout();
        }

        public void UpdateStartPosition()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
            {
                UIAnimation uiAnimation = GetAnimation(state);
                uiAnimation.startPosition = rectTransform.anchoredPosition3D;
                if (uiAnimation.Move.isPlaying) uiAnimation.Move.UpdateValues();
            }
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        public override bool IsStateEnabled(UISelectionState state)
        {
            UIAnimation uiAnimation = GetAnimation(state);
            return uiAnimation != null && uiAnimation.isEnabled;
        }

        public override void UpdateSettings()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state).SetTarget(rectTransform, canvasGroup);
        }

        public override void StopAllReactions()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Stop();
        }

        public override void Play(UISelectionState state)
        {
            if (playStateAnimationFromLateUpdate)
            {
                GetAnimation(state)?.SetProgressAtOne();
                return;
            }
            GetAnimation(state)?.Play();
        }

        private static void ResetAnimation(UIAnimation target)
        {
            target.Move.Reset();
            target.Rotate.Reset();
            target.Scale.Reset();
            target.Fade.Reset();

            target.animationType = UIAnimationType.State;

            target.Move.fromReferenceValue = ReferenceValue.CurrentValue;
            target.Rotate.fromReferenceValue = ReferenceValue.CurrentValue;
            target.Scale.fromReferenceValue = ReferenceValue.CurrentValue;
            target.Fade.fromReferenceValue = ReferenceValue.CurrentValue;

            target.Move.settings.duration = UISelectable.k_DefaultAnimationDuration;
            target.Rotate.settings.duration = UISelectable.k_DefaultAnimationDuration;
            target.Scale.settings.duration = UISelectable.k_DefaultAnimationDuration;
            target.Fade.settings.duration = UISelectable.k_DefaultAnimationDuration;
        }
    }
}
