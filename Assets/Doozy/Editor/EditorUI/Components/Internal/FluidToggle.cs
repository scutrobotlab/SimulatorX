// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Pooler;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components.Internal
{
    public abstract class FluidToggle<T> : PoolableElement<T>, IToggle where T : VisualElement, IPoolable, new()
    {
        public static class Default
        {
            public const float k_IconReactionDuration = 0.2f;

            public static EditorSelectableColorInfo toggleOnIconSelectableColor => EditorSelectableColors.Default.ButtonIcon;
            public static EditorSelectableColorInfo toggleOffIconSelectableColor => EditorSelectableColors.Default.ToggleOffIcon;
            public static EditorSelectableColorInfo iconContainerSelectableColor => EditorSelectableColors.Default.ButtonContainer;
            public static EditorSelectableColorInfo toggleOffTextSelectableColor => EditorSelectableColors.Default.ToggleOffText;
        }

        public override void Reset()
        {
            this.ResetLayout();
            this.SetTooltip(string.Empty);
            this.SetEnabled(true);
            this.SetStyleAlignSelf(Align.Auto);

            ClearBind();

            OnClick = null;
            OnValueChanged = null;

            ResetColors();
            fluidElement.ResetAccentColor();

            RemoveFromToggleGroup();
            IsOn = false;

            const bool animateChange = false;
            UpdateVisualState(animateChange);

            selectionState = SelectionState.Normal;
        }

        public override void Dispose()
        {
            base.Dispose();

            fluidElement.OnClick -= ExecuteOnClick;

            IconReaction?.Recycle();
            m_MixedValuesIconReaction?.Recycle();
        }

        public FluidElement fluidElement { get; }

        public SelectionState selectionState
        {
            get => fluidElement.selectionState;
            set => fluidElement.selectionState = value;
        }

        public EditorSelectableColorInfo toggleOnIconSelectableColor { get; set; }
        public EditorSelectableColorInfo toggleOffIconSelectableColor { get; set; }
        public EditorSelectableColorInfo iconContainerSelectableColor { get; set; }
        public EditorSelectableColorInfo toggleOffTextSelectableColor { get; set; }
        public Color iconContainerBorderColor { get; set; }

        public bool inToggleGroup => toggleGroup != null;
        public IToggleGroup toggleGroup { get; set; }

        public virtual bool canHaveMixedValues => true;

        public void AddToToggleGroup(IToggleGroup value)
        {
            if (value == null) return;
            if (toggleGroup == value) return;
            toggleGroup?.UnregisterToggle(this);
            value.RegisterToggle(this);
        }

        public void RemoveFromToggleGroup()
        {
            toggleGroup?.UnregisterToggle(this);
        }

        public virtual void UpdateValueFromGroup(bool newValue, bool animateChange, bool silent = false)
        {
            bool previousValue = IsOn;
            IsOn = newValue;
            if (silent)
            {
                UpdateVisualState(animateChange);
                return;
            }
            ValueChanged(previousValue, newValue, animateChange);
        }

        protected Texture2DReaction IconReaction;
        public Texture2DReaction iconReaction =>
            IconReaction ??
            (
                IconReaction =
                    icon.GetTexture2DReaction().SetEditorHeartbeat()
                        .SetTextures(EditorMicroAnimations.EditorUI.Components.RadioCircle)
                        .SetDuration(Default.k_IconReactionDuration)
            );

        private Texture2DReaction m_MixedValuesIconReaction;
        public Texture2DReaction mixedValuesIconReaction =>
            m_MixedValuesIconReaction ??
            (
                m_MixedValuesIconReaction = icon.GetTexture2DReaction().SetEditorHeartbeat()
                    .SetTextures(EditorMicroAnimations.EditorUI.Components.LineMixedValues)
                    .SetDuration(0.15f)
            );


        protected internal bool IsOn;
        public bool isOn
        {
            get => IsOn;
            set
            {
                // Debug.Log($"{nameof(isOn)} - from: {ToggleValue} to: {value}");
                bool previousValue = IsOn;
                IsOn = value;

                if (inToggleGroup)
                {
                    toggleGroup.ToggleChangedValue(this, animateChange: true);
                    return;
                }

                ValueChanged(previousValue, value, animateChange: true);
            }
        }

        protected const string k_ClassNameMixedValues = "MixedValues";
        private bool m_HasMixedValues;
        protected bool hasMixedValues
        {
            get => canHaveMixedValues && m_HasMixedValues;
            set
            {
                if (!canHaveMixedValues)
                {
                    m_HasMixedValues = false;
                    return;
                }

                if (m_HasMixedValues == value) return;

                m_HasMixedValues = value;

                if (value)
                    icon?.AddClass(k_ClassNameMixedValues);
                else
                    icon?.RemoveClass(k_ClassNameMixedValues);

                bool previousValue = isOn;
                bool newValue = isOn;
                const bool animateChange = true;
                ValueChanged(previousValue, newValue, animateChange);
            }
        }

        public Toggle invisibleToggle { get; private set; }
        public T BindToProperty(SerializedProperty property) =>
            BindToProperty(property.propertyPath);

        public T BindToProperty(string bindingPath)
        {
            ClearBind();

            Add(invisibleToggle = DesignUtils.NewToggle(bindingPath, true));

            invisibleToggle.RegisterValueChangedCallback(evt =>
            {
                if (isOn == evt.newValue) return;
                isOn = evt.newValue;
            });

            schedule.Execute(() =>
            {
                // if (isOn == invisibleToggle.value) return;
                if (invisibleToggle != null)
                    IsOn = invisibleToggle.value;
                UpdateVisualState(false);
                // isOn = invisibleToggle.value;
            });

            return this as T;
        }

        public T ClearBind()
        {
            if (invisibleToggle == null) return this as T;
            invisibleToggle.RemoveFromHierarchy();
            invisibleToggle = null;
            return this as T;
        }

        public Image icon { get; protected set; }

        public UnityAction OnClick;
        public UnityAction<FluidBoolEvent> OnValueChanged;

        protected FluidToggle()
        {
            fluidElement = new FluidElement(this)
            {
                OnStateChanged = StateChanged,
                OnClick = ExecuteOnClick,
            };

            //RESET
            ResetColors();
        }

        public void ResetColors()
        {
            toggleOnIconSelectableColor = Default.toggleOnIconSelectableColor;
            toggleOffIconSelectableColor = Default.toggleOffIconSelectableColor;
            iconContainerSelectableColor = Default.iconContainerSelectableColor;
            toggleOffTextSelectableColor = Default.toggleOffTextSelectableColor;
        }

        protected virtual void ExecuteOnClick(EventBase clickEvent)
        {
            if (selectionState == SelectionState.Disabled) return;
            OnClick?.Invoke();
            if (invisibleToggle != null)
            {
                invisibleToggle.value = !invisibleToggle.value;
                return;
            }
            isOn = !isOn;
        }

        protected internal virtual void ValueChanged(bool previousValue, bool newValue, bool animateChange)
        {
            UpdateVisualState(animateChange);
            OnValueChanged?.Invoke(new FluidBoolEvent(previousValue, newValue, animateChange));
            // schedule.Execute(() => OnValueChanged?.Invoke(new FluidBoolEvent(previousValue, newValue, animateChange)));
        }

        public virtual void UpdateVisualState(bool animateChange)
        {
            if (canHaveMixedValues && hasMixedValues)
            {
                iconReaction?.Stop();
                if (animateChange)
                {
                    mixedValuesIconReaction?.Play();
                }
                else
                {
                    mixedValuesIconReaction?.SetProgressAtOne();
                }
            }
            else
            {
                if (canHaveMixedValues) mixedValuesIconReaction?.Stop();

                if (animateChange)
                {
                    iconReaction?.Play(!isOn);
                }
                else
                {
                    iconReaction?.SetProgressAt(!isOn ? 0f : 1f);
                }
            }

            fluidElement.StateChanged();
        }

        protected virtual void StateChanged()
        {
            fluidElement.containerColor = iconContainerSelectableColor.GetColor(selectionState);

            fluidElement.iconColor =
                isOn || canHaveMixedValues && hasMixedValues
                    ? fluidElement.iconColor
                    : toggleOffIconSelectableColor.GetColor(selectionState);

            fluidElement.textColor =
                isOn
                    ? fluidElement.textColor
                    : toggleOffTextSelectableColor.GetColor(selectionState);

            iconContainerBorderColor = Color.clear;
            bool showBorder;

            switch (selectionState)
            {
                case SelectionState.Normal:
                    showBorder = false;
                    break;

                case SelectionState.Highlighted:
                    showBorder = true;
                    break;

                case SelectionState.Pressed:
                    showBorder = true;
                    break;

                case SelectionState.Selected:
                    showBorder = true;
                    break;

                case SelectionState.Disabled:
                    showBorder = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (showBorder) iconContainerBorderColor = fluidElement.iconColor.WithAlpha(0.6f); //ICON CONTAINER BORDER COLOR
        }
    }

    public static class FluidToggleExtensions
    {
        public static T Enable<T>(this T target) where T : FluidToggle<T>, new()
        {
            target.fluidElement.Enable();
            return target;
        }

        public static T Disable<T>(this T target) where T : FluidToggle<T>, new()
        {
            target.fluidElement.Disable();
            return target;
        }

        public static T SetBindingPath<T>(this T target, string bindingPath) where T : FluidToggle<T>, new()
        {
            target.BindToProperty(bindingPath);
            return target;
        }

        public static T SetIsOn<T>(this T target, bool newValue, bool animateChange = true) where T : FluidToggle<T>, new()
        {
            bool previousValue = target.IsOn;
            target.IsOn = newValue;
            if (target.inToggleGroup)
            {
                target.toggleGroup.ToggleChangedValue(target, animateChange);
                return target;
            }
            target.ValueChanged(previousValue, newValue, animateChange);
            return target;
        }

        public static T SetToggleAccentColor<T>(this T target, EditorSelectableColorInfo value) where T : FluidToggle<T>, new()
        {
            target.fluidElement.SetAccentColor(value);
            return target;
        }

        public static T ResetAccentColor<T>(this T target) where T : FluidToggle<T>, new()
        {
            target.fluidElement.ResetAccentColor();
            return target;
        }

        public static T SetIconContainerColor<T>(this T target, EditorSelectableColorInfo value) where T : FluidToggle<T>, new()
        {
            target.iconContainerSelectableColor = value;
            target.fluidElement.StateChanged();
            return target;
        }

        #region OnValueChanged

        public static T SetOnValueChanged<T>(this T target, UnityAction<FluidBoolEvent> callback) where T : FluidToggle<T>, new()
        {
            if (callback == null) return target;
            target.OnValueChanged = callback;
            return target;
        }

        public static T AddOnValueChanged<T>(this T target, UnityAction<FluidBoolEvent> callback) where T : FluidToggle<T>, new()
        {
            if (callback == null) return target;
            target.OnValueChanged += callback;
            return target;
        }

        public static T ClearOnValueChanged<T>(this T target) where T : FluidToggle<T>, new()
        {
            target.OnValueChanged = null;
            return target;
        }

        #endregion

        #region OnClick

        public static T SetOnClick<T>(this T target, UnityAction callback) where T : FluidToggle<T>, new()
        {
            if (callback == null) return target;
            target.OnClick = callback;
            return target;
        }

        public static T AddOnClick<T>(this T target, UnityAction callback) where T : FluidToggle<T>, new()
        {
            if (callback == null) return target;
            target.OnClick += callback;
            return target;
        }

        public static T ClearOnClick<T>(this T target) where T : FluidToggle<T>, new()
        {
            target.OnClick = null;
            return target;
        }

        #endregion
    }
}
