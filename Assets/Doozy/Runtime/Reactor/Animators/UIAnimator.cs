// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIManager.Layouts;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/Reactor/Animators/UI Animator")]
    public class UIAnimator : ReactorAnimator
    {
        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        private RectTransform m_RectTransform;
        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform rectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();

        [SerializeField] private UIAnimation Animation;
        public new UIAnimation animation => Animation ??= new UIAnimation(rectTransform, canvasGroup);

        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }
        private float lastMoveAnimationProgress { get; set; }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            animatorInitialized = false;
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
            UpdateLayout();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            base.OnEnable();
            UpdateLayout();
            updateStartPositionInLateUpdate = true;
        }

        private void OnDisable()
        {
            RefreshLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            updateStartPositionInLateUpdate = true;
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            if (animation.isActive)
            {
                lastMoveAnimationProgress = animation.Move.progress;
                return;
            }
            if (localPosition != rectTransform.localPosition) updateStartPositionInLateUpdate = true;
            if (!updateStartPositionInLateUpdate) return;
            if (CanvasUpdateRegistry.IsRebuildingLayout()) return;
            UpdateStartPosition();
            RefreshLayout();
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
            // if (name.Contains("#")) Debug.Log($"({Time.frameCount}) [{name}] {nameof(UpdateStartPosition)} sp:({animation.startPosition}) rp:({rectTransform.anchoredPosition}) lp:({rectTransform.localPosition})");
            animation.startPosition = rectTransform.anchoredPosition3D;
            if (animation.isPlaying) animation.UpdateValues();
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        public override void Play(PlayDirection playDirection)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Play(playDirection));
                return;
            }
            animation.Play(playDirection);
        }

        public override void Play(bool inReverse = false)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Play(inReverse));
                return;
            }
            animation.Play(inReverse);
        }

        public override void SetTarget(object target) =>
            SetTarget(target as RectTransform);

        public void SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null) =>
            animation.SetTarget(targetRectTransform, targetCanvasGroup);

        public override void ResetToStartValues(bool forced = false) =>
            animation.ResetToStartValues(forced);

        public override void UpdateSettings()
        {
            SetTarget(rectTransform, canvasGroup);
            if (animation.isPlaying) UpdateValues();
        }

        public override void UpdateValues() =>
            animation.UpdateValues();

        public override void PlayToProgress(float toProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.PlayToProgress(toProgress));
                return;
            }
            animation.PlayToProgress(toProgress);
        }

        public override void PlayFromProgress(float fromProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.PlayFromProgress(fromProgress));
                return;
            }
            animation.PlayFromProgress(fromProgress);
        }

        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.PlayFromToProgress(fromProgress, toProgress));
                return;
            }
            animation.PlayFromToProgress(fromProgress, toProgress);
        }

        public override void Stop()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Stop());
                return;
            }
            animation.Stop();
        }

        public override void Finish()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Finish());
                return;
            }
            animation.Finish();
        }

        public override void Reverse()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Reverse());
                return;
            }
            animation.Reverse();
        }

        public override void Rewind()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Rewind());
                return;
            }
            animation.Rewind();
        }

        public override void Pause()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Pause());
                return;
            }
            animation.Pause();
        }

        public override void Resume()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Resume());
                return;
            }
            animation.Resume();
        }

        public override void SetProgressAtOne()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.SetProgressAtOne());
                return;
            }
            animation.SetProgressAtOne();
        }

        public override void SetProgressAtZero()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.SetProgressAtZero());
                return;
            }
            animation.SetProgressAtZero();
        }

        public override void SetProgressAt(float targetProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => SetProgressAt(targetProgress));
                return;
            }
            animation.SetProgressAt(targetProgress);
        }

        protected override void Recycle() =>
            animation?.Recycle();
    }
}
