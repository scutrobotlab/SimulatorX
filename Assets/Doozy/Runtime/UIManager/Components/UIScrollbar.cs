// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components.Internal;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Scrollbar component based on UISelectable.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Components/UI Scrollbar")]
    [SelectionBase]
    public class UIScrollbar : UISelectableComponent<UIScrollbar>, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler
    {
        public const float k_MINValue = 0f;
        public const float k_MAXValue = 1f;
        public const int k_MINNumberOfSteps = 0;
        public const int k_MAXNumberOfSteps = 20;
        
        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> Signal stream for this component type </summary>
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(k_StreamCategory, nameof(UIScrollbar));

        /// <summary> All scrollbars that are active and enabled </summary>
        public static IEnumerable<UIScrollbar> availableScrollbars => database.Where(item => item.isActiveAndEnabled);

        public override SelectableType selectableType => SelectableType.Button;

        /// <summary> Scrollbar changed its value - executed when the scrollbar changes its value </summary>
        public FloatEvent OnValueChangedCallback;

        [SerializeField] private RectTransform HandleRect;
        /// <summary> Optional RectTransform to use as a handle for the slider </summary>
        public RectTransform handleRect
        {
            get => HandleRect;
            set
            {
                if (value == HandleRect)
                    return;

                HandleRect = value;
                UpdateCachedReferences();
                UpdateVisuals();
            }
        }

        [SerializeField] private SlideDirection Direction = SlideDirection.LeftToRight;
        /// <summary> The direction of the scrollbar, from minimum to maximum value </summary>
        public SlideDirection direction
        {
            get => Direction;
            set
            {
                Direction = value;
                UpdateVisuals();
            }
        }

        [SerializeField] protected float Value;
        /// <summary> The current value of the scrollbar (between 0 and 1) </summary>
        public virtual float value
        {
            get => NumberOfSteps > 1 ? Mathf.Round(Value * (NumberOfSteps - 1)) / (NumberOfSteps - 1) : Value;
            set => Set(value);
        }

        [SerializeField] private float Size = 0.2f;
        /// <summary> The size of the scrollbar handle where 1 means it fills the entire scrollbar </summary>
        public float size
        {
            get => Size;
            set
            {
                Size = value.Clamp01();
                UpdateVisuals();
            }
        }

        [Range(0, 11)]
        [SerializeField] private int NumberOfSteps = 0;
        /// <summary> The number of steps to use for the value. A value of 0 disables use of steps </summary>
        public int numberOfSteps
        {
            get => NumberOfSteps;
            set
            {
                NumberOfSteps = value.Clamp(0, 11);
                UpdateVisuals();
            }
        }

        private Axis axis
        {
            get
            {
                switch (Direction)
                {
                    case SlideDirection.LeftToRight:
                        return Axis.Horizontal;
                    case SlideDirection.RightToLeft:
                        return Axis.Horizontal;
                    case SlideDirection.BottomToTop:
                        return Axis.Vertical;
                    case SlideDirection.TopToBottom:
                        return Axis.Vertical;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool reverseValue => Direction == SlideDirection.RightToLeft || Direction == SlideDirection.TopToBottom;



        // Private fields
        private RectTransform m_ContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        // Size of each step.
        private float stepSize =>
            NumberOfSteps > 1
                ? 1f / (NumberOfSteps - 1)
                : 0.1f;

        private DrivenRectTransformTracker m_Tracker;
        private Coroutine m_PointerDownRepeat;
        private bool m_IsPointerDownAndNotDragging = false;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        private UIScrollbar() {}

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            Size = Size.Clamp01();

            if (IsActive())
            {
                UpdateCachedReferences();
                Set(Value, false);
                m_DelayedUpdateVisuals = true;
            }

            base.OnValidate();
        }
        #endif //UNITY_EDITOR

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);

            #if UNITY_EDITOR

            if (executing == CanvasUpdate.Prelayout)
                OnValueChangedCallback?.Invoke(value);

            #endif //UNITY_EDITOR
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(Value, false);
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        private void Update()
        {
            if (!m_DelayedUpdateVisuals)
                return;
            m_DelayedUpdateVisuals = false;
            Set(Value, false);
            UpdateVisuals();
        }

        private void UpdateCachedReferences()
        {
            Transform parent = HandleRect != null ? HandleRect.parent : null;
            m_ContainerRect = parent != null ? parent.GetComponent<RectTransform>() : null;
        }

        /// <summary> Set the value of the scrollbar without invoking OnValueChanged callback </summary>
        /// <param name="input"> The new value for the slider </param>
        public virtual void SetValueWithoutNotify(float input) =>
            Set(input, false);

        private void Set(float input, bool sendCallback = true)
        {
            float newValue = input; //clamp01 input in callee before calling this function, this allows inertia from dragging content to go past extremities without being clamped
            if (newValue.Approximately(value)) return; //value hasn't changed -> stop
            Value = newValue;
            UpdateVisuals();
            if (!sendCallback) return;
            UISystemProfilerApi.AddMarker($"{nameof(UIScrollbar)}.{nameof(value)}", this);
            OnValueChangedCallback.Invoke(value);
            stream.SendSignal(newValue);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //this can be invoked before OnEnabled is called
            //we shouldn't be accessing other objects, before OnEnable is called
            if (!IsActive()) return;

            UpdateVisuals();
        }

        /// <summary>
        /// Force-update the scrollbar.
        /// Useful if the properties changed and a visual update is needed.
        /// </summary>
        public void UpdateVisuals()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
            #endif //UNITY_EDITOR

            m_Tracker.Clear();

            if (m_ContainerRect == null) return;

            m_Tracker.Add(this, HandleRect, DrivenTransformProperties.Anchors);
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.one;

            float movement = value.Clamp01() * (1 - size);
            if (reverseValue)
            {
                anchorMin[(int)axis] = 1 - movement - size;
                anchorMax[(int)axis] = 1 - movement;
            }
            else
            {
                anchorMin[(int)axis] = movement;
                anchorMax[(int)axis] = movement + size;
            }

            HandleRect.anchorMin = anchorMin;
            HandleRect.anchorMax = anchorMax;
        }

        // Update the scroll bar's position based on the mouse.
        void UpdateDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (m_ContainerRect == null)
                return;

            Vector2 position = Vector2.zero;
            if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ContainerRect, position, eventData.pressEventCamera, out Vector2 localCursor))
                return;

            Rect containerRectRect = m_ContainerRect.rect;
            Vector2 handleCenterRelativeToContainerCorner = localCursor - m_Offset - containerRectRect.position;
            Vector2 handleCorner = handleCenterRelativeToContainerCorner - (HandleRect.rect.size - HandleRect.sizeDelta) * 0.5f;

            float parentSize = axis == 0 ? containerRectRect.width : containerRectRect.height;
            float remainingSize = parentSize * (1 - size);
            if (remainingSize <= 0)
                return;

            DoUpdateDrag(handleCorner, remainingSize);
        }

        //this function is testable, it is found using reflection in ScrollbarClamp test
        private void DoUpdateDrag(Vector2 handleCorner, float remainingSize)
        {
            switch (Direction)
            {
                case SlideDirection.LeftToRight:
                    Set(Mathf.Clamp01(handleCorner.x / remainingSize));
                    break;
                case SlideDirection.RightToLeft:
                    Set(Mathf.Clamp01(1f - (handleCorner.x / remainingSize)));
                    break;
                case SlideDirection.BottomToTop:
                    Set(Mathf.Clamp01(handleCorner.y / remainingSize));
                    break;
                case SlideDirection.TopToBottom:
                    Set(Mathf.Clamp01(1f - (handleCorner.y / remainingSize)));
                    break;
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        /// <summary>
        /// Handling for when the scrollbar value is begin being dragged.
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            m_IsPointerDownAndNotDragging = false;

            if (!MayDrag(eventData))
                return;

            if (m_ContainerRect == null)
                return;

            m_Offset = Vector2.zero;
            if (!RectTransformUtility.RectangleContainsScreenPoint(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
                return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out Vector2 localMousePos))
                m_Offset = localMousePos - HandleRect.rect.center;
        }

        /// <summary>
        /// Handling for when the scrollbar value is dragged.
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            if (m_ContainerRect != null)
                UpdateDrag(eventData);
        }

        /// <summary>
        /// Event triggered when pointer is pressed down on the scrollbar.
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);
            m_IsPointerDownAndNotDragging = true;
            m_PointerDownRepeat = StartCoroutine(ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera));
        }

        protected IEnumerator ClickRepeat(PointerEventData eventData)
        {
            return ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera);
        }

        /// <summary>
        /// Coroutine function for handling continual press during Scrollbar.OnPointerDown.
        /// </summary>
        protected IEnumerator ClickRepeat(Vector2 screenPosition, Camera sourceCamera)
        {
            while (m_IsPointerDownAndNotDragging)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(HandleRect, screenPosition, sourceCamera))
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, screenPosition, sourceCamera, out Vector2 localMousePos))
                    {
                        float axisCoordinate = axis == 0 ? localMousePos.x : localMousePos.y;

                        // modifying value depending on direction, fixes (case 925824)

                        float change = axisCoordinate < 0 ? size : -size;
                        value += reverseValue ? change : -change;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            StopCoroutine(m_PointerDownRepeat);
        }

        /// <summary>
        /// Event triggered when pointer is released after pressing on the scrollbar.
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            m_IsPointerDownAndNotDragging = false;
        }

        /// <summary>
        /// Handling for movement events.
        /// </summary>
        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
                        Set(Mathf.Clamp01(reverseValue ? value + stepSize : value - stepSize));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
                        Set(Mathf.Clamp01(reverseValue ? value - stepSize : value + stepSize));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (axis == Axis.Vertical && FindSelectableOnUp() == null)
                        Set(Mathf.Clamp01(reverseValue ? value - stepSize : value + stepSize));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (axis == Axis.Vertical && FindSelectableOnDown() == null)
                        Set(Mathf.Clamp01(reverseValue ? value + stepSize : value - stepSize));
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        /// <summary>
        /// Prevents selection if we we move on the Horizontal axis. See Selectable.FindSelectableOnLeft.
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// Prevents selection if we we move on the Horizontal axis.  See Selectable.FindSelectableOnRight.
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// Prevents selection if we we move on the Vertical axis. See Selectable.FindSelectableOnUp.
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// Prevents selection if we we move on the Vertical axis. See Selectable.FindSelectableOnDown.
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        /// <summary>
        /// See: IInitializePotentialDragHandler.OnInitializePotentialDrag
        /// </summary>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// Set the direction of the scrollbar, optionally setting the layout as well.
        /// </summary>
        /// <param name="slideDirection"> The direction of the scrollbar </param>
        /// <param name="includeRectLayouts"> Should the layout be flipped together with the direction? </param>
        public void SetDirection(SlideDirection slideDirection, bool includeRectLayouts)
        {
            Axis oldAxis = axis;
            bool oldReverse = reverseValue;
            this.direction = slideDirection;

            if (!includeRectLayouts)
                return;

            if (axis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }
    }
}
