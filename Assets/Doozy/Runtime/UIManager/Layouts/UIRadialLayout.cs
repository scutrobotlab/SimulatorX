// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Layouts.Internal;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Layouts
{
    /// <summary>
    /// The Radial Layout component sets child elements in a radial or circular arrangement.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Layouts/UI Radial Layout")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class UIRadialLayout : UILayoutGroup
    {
        public const bool k_AutoRebuildDefaultValue = true;
        public const bool k_ClockwiseDefaultValue = true;
        public const bool k_ControlChildHeightDefaultValue = false;
        public const bool k_ControlChildWidthDefaultValue = false;
        public const bool k_RadiusControlsHeightDefaultValue = false;
        public const bool k_RadiusControlsWidthDefaultValue = false;
        public const bool k_RotateChildrenDefaultValue = false;
        public const float k_ChildHeightDefaultValue = k_RadiusDefaultValue;
        public const float k_ChildRotationDefaultValue = 0f;
        public const float k_ChildWidthDefaultValue = k_RadiusDefaultValue;
        public const float k_MAXAngle = 360f;
        public const float k_MAXAngleDefaultValue = 360f;
        public const float k_MAXRadiusDefaultValue = 1000f;
        public const float k_MINAngle = 0f;
        public const float k_MINAngleDefaultValue = 0f;
        public const float k_RadiusDefaultValue = 100f;
        public const float k_RadiusHeightFactorDefaultValue = 1f;
        public const float k_RadiusWidthFactorDefaultValue = 1f;
        public const float k_SpacingDefaultValue = 0f;
        public const float k_StartAngleDefaultValue = 180f;

        [SerializeField] protected bool AutoRebuild = k_AutoRebuildDefaultValue;
        [SerializeField] protected float ChildHeight = k_ChildHeightDefaultValue;
        [SerializeField] protected float ChildRotation = k_ChildRotationDefaultValue;
        [SerializeField] protected float ChildWidth = k_ChildWidthDefaultValue;
        [SerializeField] protected bool Clockwise = k_ClockwiseDefaultValue;
        [SerializeField] protected bool ControlChildHeight = k_ControlChildHeightDefaultValue;
        [SerializeField] protected bool ControlChildWidth = k_ControlChildWidthDefaultValue;
        [Range(k_MINAngle, k_MAXAngle)]
        [SerializeField] protected float MaxAngle = k_MAXAngleDefaultValue;
        [SerializeField] protected float MaxRadius = k_MAXRadiusDefaultValue;
        [Range(k_MINAngle, k_MAXAngle)]
        [SerializeField] protected float MinAngle = k_MINAngleDefaultValue;
        [SerializeField] protected float Radius = k_RadiusDefaultValue;
        [SerializeField] protected bool RadiusControlsHeight = k_RadiusControlsHeightDefaultValue;
        [SerializeField] protected bool RadiusControlsWidth = k_RadiusControlsWidthDefaultValue;
        [SerializeField] protected float RadiusHeightFactor = k_RadiusHeightFactorDefaultValue;
        [SerializeField] protected float RadiusWidthFactor = k_RadiusWidthFactorDefaultValue;
        [SerializeField] protected bool RotateChildren = k_RotateChildrenDefaultValue;
        [SerializeField] protected float Spacing = k_SpacingDefaultValue;
        [Range(k_MINAngle, k_MAXAngle)]
        [SerializeField] protected float StartAngle = k_StartAngleDefaultValue;

        /// <summary> Internal list used to count the number of child elements this layout has. It's main purpose is to improve layout performance by reducing GC </summary>
        private List<RectTransform> m_ChildList = new List<RectTransform>();

        /// <summary> Automatically rebuild the layout when a parameter has changed and update the layout </summary>
        public bool autoRebuild
        {
            get => AutoRebuild;
            set
            {
                if (AutoRebuild == value) return;
                AutoRebuild = value;
                OnValueChanged();
            }
        }

        /// <summary> Child elements height when control child height is enabled </summary>
        public float childHeight
        {
            get => ChildHeight;
            set
            {
                if (Mathf.Approximately(ChildHeight, value)) return;
                ChildHeight = value;
                OnValueChanged();
            }
        }

        /// <summary> Child elements custom rotation </summary>
        public float childRotation
        {
            get => ChildRotation;
            set
            {
                if (Mathf.Approximately(ChildRotation, value)) return;
                ChildRotation = value;
                OnValueChanged();
            }
        }

        /// <summary> Child elements width when control child width is enabled </summary>
        public float childWidth
        {
            get => ChildWidth;
            set
            {
                if (Mathf.Approximately(ChildWidth, value)) return;
                ChildWidth = value;
                OnValueChanged();
            }
        }

        /// <summary> Order the child elements clockwise and update the layout </summary>
        public bool clockwise
        {
            get => Clockwise;
            set
            {
                if (Clockwise == value) return;
                Clockwise = value;
                OnValueChanged();
            }
        }

        /// <summary> Override the child elements height and update the layout </summary>
        public bool controlChildHeight
        {
            get => ControlChildHeight;
            set
            {
                ControlChildHeight = value;
                OnValueChanged();
            }
        }

        /// <summary> Override the child elements width and update the layout </summary>
        public bool controlChildWidth
        {
            get => ControlChildWidth;
            set
            {
                ControlChildWidth = value;
                OnValueChanged();
            }
        }

        /// <summary> Maximum angle a child element can have inside the layout. Used to make the radial layout look as an arc </summary>
        public float maxAngle
        {
            get => MaxAngle;
            set
            {
                if (Mathf.Approximately(MaxAngle, value)) return;
                MaxAngle = value;
                OnValueChanged();
            }
        }

        /// <summary> Minimum angle a child element can have inside the layout. Used to make the radial layout look as an arc </summary>
        public float minAngle
        {
            get => MinAngle;
            set
            {
                if (Mathf.Approximately(MinAngle, value)) return;
                MinAngle = value;
                OnValueChanged();
            }
        }

        /// <summary> Layout radius that determines the size of the circle </summary>
        public float radius
        {
            get => Radius;
            set
            {
                if (Mathf.Approximately(Radius, value)) return;
                Radius = value;
                OnValueChanged();
            }
        }

        /// <summary> Set the child elements height to be influenced by the layout radius and update the layout </summary>
        public bool radiusControlsHeight
        {
            get => RadiusControlsHeight;
            set
            {
                RadiusControlsHeight = value;
                OnValueChanged();
            }
        }

        /// <summary> Set the child elements width to be influenced by the layout radius and update the layout </summary>
        public bool radiusControlsWidth
        {
            get => RadiusControlsWidth;
            set
            {
                RadiusControlsWidth = value;
                OnValueChanged();
            }
        }

        /// <summary> Factor by which the radius influences the child elements height, if radius controls height is enabled </summary>
        public float radiusHeightFactor
        {
            get => RadiusHeightFactor;
            set
            {
                if (Mathf.Approximately(RadiusHeightFactor, value)) return;
                RadiusHeightFactor = value;
                OnValueChanged();
            }
        }

        /// <summary> Factor by which the radius influences the child elements width, if the radius controls width is enabled </summary>
        public float radiusWidthFactor
        {
            get => RadiusWidthFactor;
            set
            {
                if (Mathf.Approximately(RadiusWidthFactor, value)) return;
                RadiusWidthFactor = value;
                OnValueChanged();
            }
        }

        /// <summary> Automatically rotate child elements with the layout, when the start angle changes and update the layout </summary>
        public bool rotateChildren
        {
            get => RotateChildren;
            set
            {
                RotateChildren = value;
                OnValueChanged();
            }
        }

        /// <summary> Extra spacing between child elements </summary>
        public float spacing
        {
            get => Spacing;
            set
            {
                if (Mathf.Approximately(Spacing, value)) return;
                Spacing = value;
                OnValueChanged();
            }
        }

        /// <summary> Start angle for the first child element of the layout. This places all the child elements around the layout radius </summary>
        public float startAngle
        {
            get => StartAngle;
            set
            {
                if (Mathf.Approximately(StartAngle, value)) return;
                StartAngle = value;
                OnValueChanged();
            }
        }

        private bool runUpdateAnimatorsStartPosition { get; set; }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            CalculateRadial();
        }
        #endif

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            // base.OnEnable();
            runUpdateAnimatorsStartPosition = false;
            CalculateRadial();
        }

        public override void SetLayoutHorizontal() {}

        public override void SetLayoutVertical() {}

        public override void CalculateLayoutInputVertical() =>
            CalculateRadial();

        public override void CalculateLayoutInputHorizontal() =>
            CalculateRadial();

        /// <summary> Rebuild the layout </summary>
        public void CalculateRadial()
        {
            m_ChildList ??= new List<RectTransform>();
            m_ChildList.Clear();
            int activeChildCount = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i) as RectTransform;
                if (child == null) continue;

                LayoutElement childLayout = child.GetComponent<LayoutElement>();
                if (child == null || !child.gameObject.activeSelf || (childLayout != null && childLayout.ignoreLayout)) continue;
                m_ChildList.Add(child);
                activeChildCount++;
            }

            m_Tracker.Clear();
            if (activeChildCount == 0) return;

            if (Application.isPlaying & !runUpdateAnimatorsStartPosition)
            {
                runUpdateAnimatorsStartPosition = true;
                UpdateAnimatorsStartValues();
            }

            rectTransform.sizeDelta = new Vector2(Radius, Radius) * 2f;

            float sAngle = 360f / activeChildCount * (activeChildCount - 1f);
            float angleOffset = MinAngle;
            if (angleOffset > sAngle) angleOffset = sAngle;
            float maximumAngle = 360f - MaxAngle;
            if (maximumAngle > sAngle) maximumAngle = sAngle;
            if (angleOffset > sAngle) angleOffset = sAngle;
            float buff = sAngle - angleOffset;
            float fOffsetAngle = ((buff - maximumAngle)) / (activeChildCount - 1f) + Spacing;
            float fAngle = StartAngle + angleOffset;
            bool controlChildrenSize = ControlChildWidth | ControlChildHeight;

            DrivenTransformProperties drivenTransformProperties = DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot;
            if (ControlChildWidth) drivenTransformProperties |= DrivenTransformProperties.SizeDeltaX;
            if (ControlChildHeight) drivenTransformProperties |= DrivenTransformProperties.SizeDeltaY;
            if (RotateChildren) drivenTransformProperties |= DrivenTransformProperties.Rotation;

            if (Clockwise) fOffsetAngle *= -1f;

            foreach (RectTransform child in m_ChildList)
            {
                if (child == null || !child.gameObject.activeSelf) continue;                                     //if child is null or not active -> continue
                m_Tracker.Add(this, child, drivenTransformProperties);                                           //add elements to the tracker to stop the user from modifying their positions via the editor
                var vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0); //calculate the child position
                child.localPosition = vPos * Radius;                                                             //set the child position
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);                       //force children to be center aligned, to keep all of the objects with the same anchor points

                float elementAngle = ChildRotation;
                if (RotateChildren) elementAngle += fAngle;
                child.localEulerAngles = new Vector3(0f, 0f, elementAngle);

                if (controlChildrenSize)
                {
                    Vector2 childSizeDelta = child.sizeDelta;

                    if (controlChildWidth)
                        childSizeDelta.x = RadiusControlsWidth
                            ? ChildWidth * Radius * RadiusWidthFactor / 100
                            : ChildWidth;

                    if (controlChildHeight)
                        childSizeDelta.y = RadiusControlsHeight
                            ? ChildHeight * Radius * RadiusHeightFactor / 100
                            : ChildHeight;

                    child.sizeDelta = childSizeDelta;
                }

                fAngle += fOffsetAngle;
            }
        }

        private void UpdateAnimatorsStartValues()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i) as RectTransform;
                if (child == null) continue;

                UIAnimator uiAnimator = child.GetComponent<UIAnimator>();
                if (uiAnimator != null)
                {
                    uiAnimator.animation.startPosition = uiAnimator.rectTransform.anchoredPosition3D;
                    uiAnimator.animation.startRotation = uiAnimator.rectTransform.localEulerAngles;
                    if (uiAnimator.animation.isPlaying) uiAnimator.UpdateValues();
                }

                UIContainerUIAnimator uiContainerUIAnimator = child.GetComponent<UIContainerUIAnimator>();
                if (uiContainerUIAnimator != null)
                {
                    if (uiContainerUIAnimator.isConnected && uiContainerUIAnimator.controller.isVisible)
                    {
                        uiContainerUIAnimator.showAnimation.startPosition = uiContainerUIAnimator.rectTransform.anchoredPosition3D;
                        uiContainerUIAnimator.showAnimation.startRotation = uiContainerUIAnimator.rectTransform.localEulerAngles;
                    }
                    // uiContainerAnimator.UpdateSettings();
                }

                UISelectableUIAnimator uiSelectableUIAnimator = child.GetComponent<UISelectableUIAnimator>();
                if (uiSelectableUIAnimator != null)
                {
                    if (uiSelectableUIAnimator.isConnected && uiSelectableUIAnimator.controller.currentUISelectionState == UISelectionState.Normal & !uiSelectableUIAnimator.anyAnimationIsActive)
                    {
                        foreach (UISelectionState state in UISelectable.uiSelectionStates)
                        {
                            UIAnimation uiAnimation = uiSelectableUIAnimator.GetAnimation(state);
                            if (uiAnimation == null) continue;
                            uiAnimation.startPosition = uiAnimation.rectTransform.anchoredPosition3D;
                            uiAnimation.startRotation = uiAnimation.rectTransform.localEulerAngles;
                        }
                    }
                    // uiSelectableUIAnimator.UpdateSettings();
                }
            }

            runUpdateAnimatorsStartPosition = false;
        }

        private void OnValueChanged()
        {
            if (!AutoRebuild) return;
            CalculateRadial();
        }
    }
}
