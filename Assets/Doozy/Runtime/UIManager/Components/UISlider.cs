// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
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
    /// Slider component based on UISelectable with category/name id identifier.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Components/UI Slider")]
    [SelectionBase]
    public partial class UISlider : UISelectableComponent<UISlider>, IDragHandler, IInitializePotentialDragHandler
    {
        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> Signal stream for this component type </summary>
        public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream(k_StreamCategory, nameof(UISlider)));

        /// <summary> All sliders that are active and enabled </summary>
        public static IEnumerable<UISlider> availableSliders => database.Where(item => item.isActiveAndEnabled);

        public override SelectableType selectableType => SelectableType.Button;

        /// <summary> Category Name Id </summary>
        public UISliderId Id;

        /// <summary> Slider changed its value - executed when the slider changes its value </summary>
        public FloatEvent OnValueChangedCallback;

        [SerializeField] private RectTransform FillRect;
        /// <summary> Optional RectTransform to use as fill for the slider </summary>
        public RectTransform fillRect
        {
            get => FillRect;
            set
            {
                if (value == FillRect)
                    return;

                FillRect = value;
                UpdateCachedReferences();
                UpdateVisuals();
            }
        }

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
        /// <summary> The direction of the slider, from minimum to maximum value </summary>
        public SlideDirection direction
        {
            get => Direction;
            set
            {
                Direction = value;
                UpdateVisuals();
            }
        }

        [SerializeField] private float MinValue = 0f;
        /// <summary> The minimum allowed value of the slider </summary>
        public float minValue
        {
            get => MinValue;
            set
            {
                MinValue = value;
                Value.Clamp(MinValue, MaxValue);
                UpdateVisuals();
            }
        }

        [SerializeField] private float MaxValue = 1f;
        /// <summary> The maximum allowed value of the slider </summary>
        public float maxValue
        {
            get => MaxValue;
            set
            {
                MaxValue = value;
                Value.Clamp(MinValue, MaxValue);
                UpdateVisuals();
            }
        }

        [SerializeField] private bool WholeNumbers = false;
        /// <summary> Should the value only be allowed to be whole numbers? </summary>
        public bool wholeNumbers
        {
            get => WholeNumbers;
            set
            {
                WholeNumbers = value;
                if (!value)
                    return;
                MinValue = Mathf.Round(MinValue);
                MaxValue = Mathf.Round(MaxValue);
                Value.Clamp(MinValue, MaxValue);
                UpdateVisuals();
            }
        }

        [SerializeField] protected float Value;
        /// <summary> The current value of the slider </summary>
        public virtual float value
        {
            get => wholeNumbers ? Mathf.Round(Value) : Value;
            set => Set(value);
        }

        /// <summary> The current value of the slider normalized into a value between 0 and 1 </summary>
        public float normalizedValue
        {
            get => Mathf.Approximately(minValue, maxValue) ? 0 : Mathf.InverseLerp(minValue, maxValue, value);
            set => this.value = Mathf.Lerp(minValue, maxValue, value);
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

        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        // Size of each step.
        private float stepSize => wholeNumbers ? 1 : (maxValue - minValue) * 0.1f;

        private UISlider()
        {
            Id = new UISliderId();
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {

            MinValue = WholeNumbers ? MinValue.Round(0) : MinValue;
            MaxValue = WholeNumbers ? MaxValue.Round(0) : MaxValue;
            Value = WholeNumbers ? Value.Round(0) : Value;


            if (IsActive())
            {
                UpdateCachedReferences();
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

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            Value = ClampValue(Value);
            float previousNormalizedValue = normalizedValue;
            if (m_FillContainerRect != null)
            {
                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    previousNormalizedValue = m_FillImage.fillAmount;
                }
                else
                {
                    previousNormalizedValue = reverseValue ? 1 - FillRect.anchorMin[(int)axis] : FillRect.anchorMax[(int)axis];
                }
            }
            else if (m_HandleContainerRect != null)
            {
                Vector2 anchorMin = HandleRect.anchorMin;
                previousNormalizedValue = reverseValue ? 1 - anchorMin[(int)axis] : anchorMin[(int)axis];
            }

            UpdateVisuals();

            if (Mathf.Approximately(previousNormalizedValue, normalizedValue))
                return;

            UISystemProfilerApi.AddMarker("Slider.value", this);
            OnValueChangedCallback.Invoke(Value);
        }

        private void UpdateCachedReferences()
        {
            if (FillRect && FillRect != (RectTransform)transform)
            {
                m_FillTransform = FillRect.transform;
                m_FillImage = FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                FillRect = null;
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            if (HandleRect && HandleRect != (RectTransform)transform)
            {
                m_HandleTransform = HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                HandleRect = null;
                m_HandleContainerRect = null;
            }
        }

        private float ClampValue(float input) =>
            wholeNumbers
                ? input.Clamp(minValue, maxValue).Round(0)
                : input.Clamp(minValue, maxValue);

        /// <summary> Set the value of the slider without invoking OnValueChanged callback </summary>
        /// <param name="input"> The new value for the slider </param>
        public virtual void SetValueWithoutNotify(float input) =>
            Set(input, false);

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        /// <param name="sendCallback">If the OnValueChanged callback should be invoked.</param>
        /// <remarks>
        /// Process the input to ensure the value is between min and max value. If the input is different set the value and send the callback is required.
        /// </remarks>
        private void Set(float input, bool sendCallback = true)
        {
            float newValue = ClampValue(input); //clamp between min and max
            if (newValue.Approximately(Value)) return; //value hasn't changed -> stop
            Value = newValue;
            UpdateVisuals();
            if (!sendCallback) return;
            UISystemProfilerApi.AddMarker($"{nameof(UISlider)}.{nameof(value)}", this);
            OnValueChangedCallback.Invoke(newValue);
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
        /// Force-update the slider.
        /// Useful if the properties changed and a visual update is needed.
        /// </summary>
        public void UpdateVisuals()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
            #endif //UNITY_EDITOR

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    m_FillImage.fillAmount = normalizedValue;
                }
                else
                {
                    if (reverseValue)
                        anchorMin[(int)axis] = 1 - normalizedValue;
                    else
                        anchorMax[(int)axis] = normalizedValue;
                }

                FillRect.anchorMin = anchorMin;
                FillRect.anchorMax = anchorMax;
            }

            if (m_HandleContainerRect == null)
                return;
            {
                m_Tracker.Add(this, HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[(int)axis] = anchorMax[(int)axis] = reverseValue ? 1 - normalizedValue : normalizedValue;
                HandleRect.anchorMin = anchorMin;
                HandleRect.anchorMax = anchorMax;
            }
        }

        /// <summary> Update the slider's position based on the pointer event data </summary>
        /// <param name="eventData"> Data </param>
        /// <param name="cam"> Camera </param>
        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect ? m_HandleContainerRect : m_FillContainerRect;

            if (clickRect == null)
                return;

            if (!(clickRect.rect.size[(int)axis] > 0))
                return;

            Vector2 position = Vector2.zero;
            if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out Vector2 localCursor))
                return;

            Rect rect = clickRect.rect;
            localCursor -= rect.position;

            float val = Mathf.Clamp01((localCursor - m_Offset)[(int)axis] / rect.size[(int)axis]);
            normalizedValue = reverseValue ? 1f - val : val;
        }

        private bool AllowDrag(PointerEventData eventData) =>
            IsActive() &&
            IsInteractable() &&
            eventData.button == PointerEventData.InputButton.Left;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!AllowDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out Vector2 localMousePos))
                {
                    m_Offset = localMousePos;
                }
                return;
            }

            // Outside the slider handle - jump to this point instead
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!AllowDrag(eventData))
                return;

            UpdateDrag(eventData, eventData.pressEventCamera);
        }

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
                        Set(reverseValue ? value + stepSize : value - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
                        Set(reverseValue ? value - stepSize : value + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (axis == Axis.Vertical && FindSelectableOnUp() == null)
                        Set(reverseValue ? value - stepSize : value + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (axis == Axis.Vertical && FindSelectableOnDown() == null)
                        Set(reverseValue ? value + stepSize : value - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        /// <summary>
        /// See Selectable.FindSelectableOnLeft
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnRight
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnUp
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnDown
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// Sets the direction of this slider, optionally changing the layout as well.
        /// </summary>
        /// <param name="slideDirection">The direction of the slider</param>
        /// <param name="includeRectLayouts">Should the layout be flipped together with the slider direction</param>
        public void SetDirection(SlideDirection slideDirection, bool includeRectLayouts)
        {
            Axis oldAxis = axis;
            bool oldReverse = reverseValue;
            direction = slideDirection;

            if (!includeRectLayouts)
                return;

            if (axis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }

        #region Static Methods

        /// <summary> Get all the registered sliders with the given category and name </summary>
        /// <param name="category"> UISlider category </param>
        /// <param name="name"> UISlider name (from the given category) </param>
        public static IEnumerable<UISlider> GetSliders(string category, string name) =>
            database.Where(slider => slider.Id.Category.Equals(category)).Where(slider => slider.Id.Name.Equals(name));

        /// <summary> Get all the registered sliders with the given category </summary>
        /// <param name="category"> UISlider category </param>
        public static IEnumerable<UISlider> GetAllSlidersInCategory(string category) =>
            database.Where(slider => slider.Id.Category.Equals(category));

        /// <summary> Get all the sliders that are active and enabled (all the visible/available sliders) </summary>
        public static IEnumerable<UISlider> GetAvailableSliders() =>
            database.Where(slider => slider.isActiveAndEnabled);

        /// <summary> Get the selected slider (if a slider is not selected, this method returns null) </summary>
        public static UISlider GetSelectedSlider() =>
            database.FirstOrDefault(slider => slider.isSelected);

        /// <summary> Select the slider with the given category and name (if it is active and enabled) </summary>
        /// <param name="category"> UISlider category </param>
        /// <param name="name"> UISlider name (from the given category) </param>
        public static bool SelectSlider(string category, string name)
        {
            UISlider slider = availableSliders.FirstOrDefault(b => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
            if (slider == null) return false;
            slider.Select();
            return true;
        }

        #endregion
    }
}
