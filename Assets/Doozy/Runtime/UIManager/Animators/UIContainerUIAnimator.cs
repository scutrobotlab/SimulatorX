// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Layouts;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha by listening to a target UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Animators/Container/UI Container UI Animator")]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class UIContainerUIAnimator : BaseUIContainerAnimator
    {
        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        [SerializeField] private UIAnimation ShowAnimation;
        /// <summary> Container Show Animation </summary>
        public UIAnimation showAnimation => ShowAnimation;

        [SerializeField] private UIAnimation HideAnimation;
        /// <summary> Container Hide Animation </summary>
        public UIAnimation hideAnimation => HideAnimation;

        public bool anyAnimationIsActive => showAnimation.isActive || hideAnimation.isActive;
        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            ShowAnimation ??= new UIAnimation(rectTransform);
            HideAnimation ??= new UIAnimation(rectTransform);

            ResetAnimation(ShowAnimation);
            ResetAnimation(HideAnimation);

            ShowAnimation.animationType = UIAnimationType.Show;
            ShowAnimation.Move.enabled = true;
            ShowAnimation.Move.fromDirection = MoveDirection.Left;

            HideAnimation.animationType = UIAnimationType.Hide;
            HideAnimation.Move.enabled = true;
            HideAnimation.Move.toDirection = MoveDirection.Right;

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
            base.OnEnable();
            updateStartPositionInLateUpdate = true;
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying) return;
            base.OnDisable();
            if (showAnimation.isPlaying) showAnimation.SetProgressAtOne();
            if (hideAnimation.isPlaying) hideAnimation.SetProgressAtOne();
            RefreshLayout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShowAnimation?.Recycle();
            HideAnimation?.Recycle();
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            if (!isConnected) return;
            if (controller.visibilityState != VisibilityState.Visible) return;
            if (anyAnimationIsActive) return;
            if (!updateStartPositionInLateUpdate && localPosition == rectTransform.localPosition) return;
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
            // if (name.Contains("#")) Debug.Log($"({Time.frameCount}) [{name}] {nameof(UpdateStartPosition)}");
            Vector3 anchoredPosition3D = rectTransform.anchoredPosition3D;
            ShowAnimation.startPosition = anchoredPosition3D;
            HideAnimation.startPosition = anchoredPosition3D;
            if (ShowAnimation.Move.isPlaying) ShowAnimation.Move.UpdateValues();
            if (HideAnimation.Move.isPlaying) HideAnimation.Move.UpdateValues();
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        private void RefreshStartPosition()
        {
            if (!isConnected) return;
            if (anyAnimationIsActive) return;
            if (controller.visibilityState != VisibilityState.Visible) return;
            RefreshLayout();
            UpdateStartPosition();
        }

        protected override void ConnectToController()
        {
            base.ConnectToController();
            if (!controller) return;

            controller.showReactions.Add(ShowAnimation.Move);
            controller.showReactions.Add(ShowAnimation.Rotate);
            controller.showReactions.Add(ShowAnimation.Scale);
            controller.showReactions.Add(ShowAnimation.Fade);

            controller.hideReactions.Add(HideAnimation.Move);
            controller.hideReactions.Add(HideAnimation.Rotate);
            controller.hideReactions.Add(HideAnimation.Scale);
            controller.hideReactions.Add(HideAnimation.Fade);
        }

        protected override void DisconnectFromController()
        {
            base.DisconnectFromController();
            if (!controller) return;

            controller.showReactions.Remove(ShowAnimation.Move);
            controller.showReactions.Remove(ShowAnimation.Rotate);
            controller.showReactions.Remove(ShowAnimation.Scale);
            controller.showReactions.Remove(ShowAnimation.Fade);

            controller.hideReactions.Remove(HideAnimation.Move);
            controller.hideReactions.Remove(HideAnimation.Rotate);
            controller.hideReactions.Remove(HideAnimation.Scale);
            controller.hideReactions.Remove(HideAnimation.Fade);
        }

        public override void Show()
        {
            ShowAnimation?.Play(PlayDirection.Forward);
            if (animatorInitialized && isInLayoutGroup) updateStartPositionInLateUpdate = true;
        }

        public override void Hide()
        {
            if (animatorInitialized && isInLayoutGroup) RefreshStartPosition();
            HideAnimation?.Play(PlayDirection.Forward);
        }

        public override void InstantShow()
        {
            ShowAnimation?.SetProgressAtOne();
            if (animatorInitialized && isInLayoutGroup) updateStartPositionInLateUpdate = true;
        }

        public override void InstantHide()
        {
            if (animatorInitialized && isInLayoutGroup) RefreshStartPosition();
            HideAnimation?.SetProgressAtOne();
        }

        public override void ReverseShow() =>
            ShowAnimation?.Reverse();

        public override void ReverseHide()
        {
            HideAnimation?.Reverse();
            updateStartPositionInLateUpdate = true;
        }

        public override void UpdateSettings()
        {
            ShowAnimation?.SetTarget(rectTransform, canvasGroup);
            HideAnimation?.SetTarget(rectTransform, canvasGroup);
        }

        public override void StopAllReactions()
        {
            ShowAnimation?.Stop();
            HideAnimation?.Stop();
        }

        private static void ResetAnimation(UIAnimation target)
        {
            target.Move.Reset();
            target.Rotate.Reset();
            target.Scale.Reset();
            target.Fade.Reset();

            target.Move.fromReferenceValue = ReferenceValue.StartValue;
            target.Rotate.fromReferenceValue = ReferenceValue.StartValue;
            target.Scale.fromReferenceValue = ReferenceValue.StartValue;
            target.Fade.fromReferenceValue = ReferenceValue.StartValue;

            target.Move.settings.duration = UIContainer.k_DefaultAnimationDuration;
            target.Rotate.settings.duration = UIContainer.k_DefaultAnimationDuration;
            target.Scale.settings.duration = UIContainer.k_DefaultAnimationDuration;
            target.Fade.settings.duration = UIContainer.k_DefaultAnimationDuration;
        }
    }
}
