// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Colors;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components.Internal
{
    public class FluidElement
    {
        public static class Default
        {
            public static EditorSelectableColorInfo buttonContainerSelectableColor => EditorSelectableColors.Default.ButtonContainer;
            public static EditorSelectableColorInfo iconSelectableColor => EditorSelectableColors.Default.ButtonIcon;
            public static EditorSelectableColorInfo textSelectableColor => EditorSelectableColors.Default.ButtonText;
        }

        public bool hasAccentColor => selectableAccentColor != null;
        public EditorSelectableColorInfo selectableAccentColor { get; private set; }
        public EditorSelectableColorInfo buttonContainerSelectableColor { get; private set; }
        public EditorSelectableColorInfo iconSelectableColor { get; private set; }
        public EditorSelectableColorInfo textSelectableColor { get; private set; }

        public Color containerColor { get; set; }
        public Color containerBorderColor { get; set; }
        public Color iconColor { get; set; }
        public Color textColor { get; set; }

        private SelectionState m_SelectionState;
        public SelectionState selectionState
        {
            get => m_SelectionState;
            set
            {
                m_SelectionState = value;
                StateChanged();
            }
        }

        public UnityAction OnStateChanged;
        public UnityAction<EventBase> OnClick;
        public UnityAction<PointerEnterEvent> OnPointerEnter;
        public UnityAction<PointerLeaveEvent> OnPointerLeave;
        public UnityAction<PointerDownEvent> OnPointerDown;
        public UnityAction<PointerUpEvent> OnPointerUp;

        private Clickable clickable { get; set; }

        public bool hasTarget => target != null;
        public VisualElement target { get; private set; }

        public FluidElement()
        {
            Reset();
        }

        public FluidElement(VisualElement target) : this()
        {
            SetTarget(target);
        }

        public void Reset()
        {
            buttonContainerSelectableColor = Default.buttonContainerSelectableColor;
            iconSelectableColor = Default.iconSelectableColor;
            textSelectableColor = Default.textSelectableColor;
            selectableAccentColor = null;

            ClearTarget();

            OnStateChanged = null;
            OnClick = null;
            OnPointerEnter = null;
            OnPointerLeave = null;
            OnPointerDown = null;
            OnPointerUp = null;

            selectionState = SelectionState.Normal;
        }

        
        /// <summary> Set an accent selectable color for this element </summary>
        /// <param name="value"> Selectable color info </param>
        public FluidElement SetAccentColor(EditorSelectableColorInfo value)
        {
            selectableAccentColor = value;
            StateChanged();
            return this;
        }

        public FluidElement ResetAccentColor() =>
            SetAccentColor(null);

        /// <summary> Set target VisualElement and connect to it (null value disconnects) </summary>
        /// <param name="value"> Target VisualElement </param>
        /// <param name="focusable"> True if target can be focused </param>
        public FluidElement SetTarget(VisualElement value, bool focusable = false)
        {
            clickable = clickable ?? new Clickable(ExecuteOnClick);

            if (hasTarget)
            {
                //ENTER & EXIT
                target.UnregisterCallback<PointerEnterEvent>(ExecuteOnPointerEnter);
                target.UnregisterCallback<PointerLeaveEvent>(ExecuteOnPointerLeave);

                //DOWN & UP
                target.UnregisterCallback<PointerDownEvent>(ExecuteOnPointerDown);
                target.UnregisterCallback<PointerUpEvent>(ExecuteOnPointerUp);

                //FOCUS IN & OUT
                target.UnregisterCallback<FocusInEvent>(ExecuteFocusIn);
                target.UnregisterCallback<FocusOutEvent>(ExecuteFocusOut);

                //CLICK
                target.RemoveManipulator(clickable);
            }

            target = value;

            if (target == null)
            {
                selectionState = SelectionState.Normal;
                return this;
            }

            //ENTER & EXIT
            target.RegisterCallback<PointerEnterEvent>(ExecuteOnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(ExecuteOnPointerLeave);

            //DOWN & UP
            target.RegisterCallback<PointerDownEvent>(ExecuteOnPointerDown);
            target.RegisterCallback<PointerUpEvent>(ExecuteOnPointerUp);

            //FOCUS IN & OUT
            target.RegisterCallback<FocusInEvent>(ExecuteFocusIn);
            target.RegisterCallback<FocusOutEvent>(ExecuteFocusOut);
            target.focusable = focusable;

            //CLICK
            target.AddManipulator(clickable);

            selectionState = SelectionState.Normal;

            // target.schedule.Execute(() => selectionState = SelectionState.Normal);
            return this;
        }

        /// <summary> Remove target VisualElement and disconnect from it </summary>
        public FluidElement ClearTarget() =>
            SetTarget(null);

        /// <summary> Trigger a state change </summary>
        public void StateChanged()
        {
            containerColor = buttonContainerSelectableColor.GetColor(selectionState);
            containerBorderColor = containerColor.WithRGBShade(0.2f);
            iconColor = hasAccentColor ? selectableAccentColor.normalColor : iconSelectableColor.GetColor(selectionState);
            textColor = hasAccentColor ? selectableAccentColor.normalColor : textSelectableColor.GetColor(selectionState);

            switch (selectionState)
            {
                case SelectionState.Normal:
                    break;
                case SelectionState.Highlighted:
                    break;
                case SelectionState.Pressed:
                    break;
                case SelectionState.Selected:
                    iconColor = hasAccentColor ? selectableAccentColor.normalColor.gamma : iconColor;
                    textColor = hasAccentColor ? selectableAccentColor.selectedColor.gamma : textColor;
                    break;
                case SelectionState.Disabled:
                    const float alpha = 0.6f;
                    iconColor = hasAccentColor ? selectableAccentColor.normalColor.WithAlpha(alpha) : iconColor;
                    containerBorderColor = buttonContainerSelectableColor.pressedColor.WithAlpha(alpha);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            OnStateChanged?.Invoke();
        }

        public void ExecuteFocusIn(FocusInEvent evt = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            selectionState = SelectionState.Selected;
        }

        public void ExecuteFocusOut(FocusOutEvent evt = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            selectionState = SelectionState.Normal;
        }

        public void ExecuteOnClick(EventBase clickEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            OnClick?.Invoke(clickEvent);
        }

        public void ExecuteOnPointerEnter(PointerEnterEvent enterEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            selectionState = SelectionState.Highlighted;
            OnPointerEnter?.Invoke(enterEvent);
        }

        public void ExecuteOnPointerLeave(PointerLeaveEvent leaveEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            selectionState =
                hasTarget && target.focusController.focusedElement == target
                    ? SelectionState.Selected
                    : SelectionState.Normal;
            OnPointerLeave?.Invoke(leaveEvent);
        }

        public void ExecuteOnPointerDown(PointerDownEvent downEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            selectionState = SelectionState.Pressed;
            OnPointerDown?.Invoke(downEvent);
        }

        public void ExecuteOnPointerUp(PointerUpEvent upEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            if (upEvent != null && hasTarget && target.ContainsPoint(upEvent.localPosition))
                selectionState = SelectionState.Highlighted;

            if (upEvent != null && hasTarget && !target.ContainsPoint(upEvent.localPosition) && selectionState == SelectionState.Pressed)
                selectionState = SelectionState.Normal;

            OnPointerUp?.Invoke(upEvent);
        }

        public FluidElement Enable()
        {
            target?.SetEnabled(true);
            selectionState = SelectionState.Normal;
            return this;
        }

        public FluidElement Disable()
        {
            target?.SetEnabled(false);
            selectionState = SelectionState.Disabled;
            return this;
        }
    }
}
