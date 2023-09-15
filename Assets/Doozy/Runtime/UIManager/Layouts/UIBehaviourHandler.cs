// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Layouts
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class UIBehaviourHandler : UnityEngine.EventSystems.UIBehaviour
    {
        private RectTransform m_RectTransform;
        public RectTransform rectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();

        private LayoutGroup m_LayoutGroup;
        public LayoutGroup layoutGroup => m_LayoutGroup ? m_LayoutGroup : m_LayoutGroup = GetComponent<LayoutGroup>();

        public UnityAction onRectTransformDimensionsChanged { get; set; }

        private int lastDirty { get; set; } = -1;         // makes sure that SetDirty can run only once per frame and no more
        private Coroutine setDirtyCoroutine { get; set; } // makes sure that there is only one coroutine that runs SetDirty 
        private int lastRefreshLayout { get; set; } = -1; // makes sure that RefreshLayout can run only once per frame and no more
        private int activeChildCount { get; set; } = -1;  // makes sure that when the child count changes, RefreshLayout is called in LateUpdate

        private bool hasLayoutGroup { get; set; }

        #if UNITY_EDITOR

        protected override void OnValidate()
        {
            // SetDirty();
            ForceRebuildLayoutImmediate();
        }

        #endif

        protected override void Awake()
        {
            lastDirty = -1;
            lastRefreshLayout = -1;
            activeChildCount = -1;

            base.Awake();
            m_RectTransform = GetComponent<RectTransform>();
            m_LayoutGroup = GetComponent<LayoutGroup>();
            hasLayoutGroup = layoutGroup != null;
            RefreshLayout();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_LayoutGroup = GetComponent<LayoutGroup>();
            hasLayoutGroup = layoutGroup != null;
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetDirty();
            onRectTransformDimensionsChanged?.Invoke();
        }

        private void LateUpdate()
        {
            if (!hasLayoutGroup) return;
            if (rectTransform.childCount == 0) return;
            int activeChildren = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (rectTransform.GetChild(i).gameObject.activeInHierarchy)
                    activeChildren++;
            }

            if (activeChildCount == activeChildren) return;
            activeChildCount = activeChildren;

            // if (name.Contains("#")) Debug.Log($"({Time.frameCount}) [{name}] activeChildren {activeChildren} --- {nameof(LateUpdate)}");
            SetDirty();
            ForceRebuildLayoutImmediate();
        }

        public void RefreshLayout()
        {
            if (Application.isPlaying)
            {
                if (lastRefreshLayout == Time.frameCount) return;
                lastRefreshLayout = Time.frameCount;
            }

            if (!hasLayoutGroup) return;

            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        public void ForceRebuildLayoutImmediate() =>
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        public void MarkLayoutForRebuild() =>
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);


        /// <summary> Mark as dirty </summary>
        public void SetDirty()
        {
            if (Application.isPlaying)
            {
                if (lastDirty == Time.frameCount) return;
                lastDirty = Time.frameCount;
            }

            if (!IsActive()) return;

            RefreshLayout();

            if (!CanvasUpdateRegistry.IsRebuildingLayout())
            {
                MarkLayoutForRebuild();
                return;
            }

            if (setDirtyCoroutine != null)
            {
                StopCoroutine(setDirtyCoroutine);
                setDirtyCoroutine = null;
            }
            setDirtyCoroutine = StartCoroutine(DelayedSetDirty());
        }

        private IEnumerator DelayedSetDirty()
        {
            yield return null;
            MarkLayoutForRebuild();
        }
    }
}
