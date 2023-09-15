// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidButton : PoolableElement<FluidButton>
    {
        public override void Reset()
        {
            ResetElementSize();
            ResetLayoutOrientation();
            ResetButtonStyle();
            ResetAnimationTrigger();

            this.SetEnabled(true);
            this.ResetLayout();
            this.SetTooltip(string.Empty);
            this.SetName(string.Empty);
            this.ResetAccentColor();
            this.ClearIcon();
            this.ClearLabelText();
            this.ClearInfoContainer();
            this.ClearOnClick();
            this.SetSelectionState(SelectionState.Normal);
            
            
            buttonLabel.SetStyleTextAlign(TextAnchor.MiddleCenter);
        }

        public override void Dispose()
        {
            base.Dispose();
            iconReaction?.Recycle();
        }

        #region ElementSize

        private ElementSize elementSize { get; set; }
        private List<VisualElement> elementSizeDependentElements { get; }
        public FluidButton SetElementSize(ElementSize value)
        {
            UIElementsUtils.RemoveClass(elementSize.ToString(), elementSizeDependentElements);
            UIElementsUtils.AddClass(value.ToString(), elementSizeDependentElements);
            elementSize = value;
            return this;
        }
        public FluidButton ResetElementSize() =>
            SetElementSize(ElementSize.Normal);

        #endregion

        #region LayoutOrientation

        private LayoutOrientation layoutOrientation { get; set; }
        private List<VisualElement> layoutOrientationDependentElements { get; }
        public FluidButton SetLayoutOrientation(LayoutOrientation value)
        {
            UIElementsUtils.RemoveClass(layoutOrientation.ToString(), layoutOrientationDependentElements);
            UIElementsUtils.AddClass(value.ToString(), layoutOrientationDependentElements);
            layoutOrientation = value;
            return this;
        }
        public FluidButton ResetLayoutOrientation() =>
            SetLayoutOrientation(LayoutOrientation.Horizontal);

        #endregion

        #region ButtonStyle

        private ButtonStyle buttonStyle { get; set; }
        private List<VisualElement> buttonStyleDependentElements { get; }
        public FluidButton SetButtonStyle(ButtonStyle value)
        {
            UIElementsUtils.RemoveClass(buttonStyle.ToString(), buttonStyleDependentElements);
            UIElementsUtils.AddClass(value.ToString(), buttonStyleDependentElements);
            buttonStyle = value;
            fluidElement.StateChanged();
            return this;
        }
        public FluidButton ResetButtonStyle() =>
            SetButtonStyle(ButtonStyle.Clear);

        #endregion

        #region ButtonAnimationTrigger

        private ButtonAnimationTrigger animationTrigger { get; set; }
        public FluidButton SetAnimationTrigger(ButtonAnimationTrigger value)
        {
            animationTrigger = value;
            return this;
        }
        public FluidButton ResetAnimationTrigger() =>
            SetAnimationTrigger(ButtonAnimationTrigger.OnPointerEnter);

        #endregion

        #region IconType

        private IconType iconType { get; set; } = IconType.None;
        private List<VisualElement> iconDependentComponents { get; }
        internal void UpdateIconType(IconType value)
        {
            if (iconType != IconType.None) UIElementsUtils.RemoveClass(iconType.ToString(), iconDependentComponents);
            if (value != IconType.None) UIElementsUtils.AddClass(value.ToString(), iconDependentComponents);
            iconType = value;
        }

        #endregion

        public FluidElement fluidElement { get; }

        public SelectionState selectionState
        {
            get => fluidElement.selectionState;
            set => fluidElement.selectionState = value;
        }

        private static Font font => EditorFonts.Ubuntu.Light;

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement buttonContainer { get; }
        public Image buttonIcon { get; }
        public Label buttonLabel { get; }
        public VisualElement infoContainer { get; }

        public Texture2DReaction iconReaction { get; internal set; }

        public UnityAction OnClick;

        public static FluidButton Get(string labelText, string tooltip = "") =>
            Get().SetLabelText(labelText).SetTooltip(tooltip);

        public static FluidButton Get(string labelText, Texture2D icon, string tooltip = "") =>
            Get().SetLabelText(labelText).SetIcon(icon).SetTooltip(tooltip);

        public static FluidButton Get(string labelText, IEnumerable<Texture2D> iconTextures, string tooltip = "") =>
            Get().SetLabelText(labelText).SetIcon(iconTextures).SetTooltip(tooltip);

        public static FluidButton Get(string labelText, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetLabelText(labelText).SetAccentColor(accentColor).SetTooltip(tooltip);

        public static FluidButton Get(string labelText, Texture2D icon, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetLabelText(labelText).SetIcon(icon).SetAccentColor(accentColor).SetTooltip(tooltip);

        public static FluidButton Get(string labelText, IEnumerable<Texture2D> iconTextures, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetLabelText(labelText).SetIcon(iconTextures).SetAccentColor(accentColor).SetTooltip(tooltip);

        public static FluidButton Get(Texture2D icon, string tooltip = "") =>
            Get().SetIcon(icon).SetTooltip(tooltip);

        public static FluidButton Get(Texture2D icon, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetIcon(icon).SetAccentColor(accentColor).SetTooltip(tooltip);

        public static FluidButton Get(IEnumerable<Texture2D> iconTextures, string tooltip = "") =>
            Get().SetIcon(iconTextures).SetTooltip(tooltip);

        public static FluidButton Get(IEnumerable<Texture2D> iconTextures, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetIcon(iconTextures).SetAccentColor(accentColor).SetTooltip(tooltip);

        /// <summary>
        /// Do not use this as new FluidButton() as that option does not use the internal pooling system
        /// <para/> use: 'FluidButton.Get()' to get a new instance (this uses the internal pooling system) 
        /// </summary>
        public FluidButton()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidButton.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.FieldIcon)
                .AddStyle(EditorStyles.EditorUI.FieldName)
                .AddStyle(EditorStyles.EditorUI.FluidButton);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            buttonContainer = layoutContainer.Q<VisualElement>(nameof(buttonContainer));
            buttonIcon = layoutContainer.Q<Image>(nameof(buttonIcon));
            buttonLabel = layoutContainer.Q<Label>(nameof(buttonLabel));
            infoContainer = layoutContainer.Q<VisualElement>(nameof(infoContainer));

            elementSizeDependentElements = new List<VisualElement> { layoutContainer, buttonContainer, buttonIcon, buttonLabel };
            layoutOrientationDependentElements = new List<VisualElement>(elementSizeDependentElements);
            buttonStyleDependentElements = new List<VisualElement>(elementSizeDependentElements);
            iconDependentComponents = new List<VisualElement>(elementSizeDependentElements);

            buttonLabel.SetStyleUnityFont(font);

            fluidElement = new FluidElement(this)
            {
                OnStateChanged = StateChanged,
                OnClick = ExecuteOnClick,
                OnPointerEnter = ExecuteOnPointerEnter
            };

            //RESET
            {
                ResetElementSize();
                ResetLayoutOrientation();
                ResetButtonStyle();
                ResetAnimationTrigger();
                this.ResetLayout();
                this.ResetAccentColor();
                this.ClearIcon();
                this.ClearLabelText();
                this.ClearInfoContainer();
                this.ClearOnClick();
                this.SetTooltip(string.Empty);
                selectionState = SelectionState.Normal;
            }
        }

        private void StateChanged()
        {
            switch (buttonStyle)
            {
                case ButtonStyle.Contained:

                    break;
                case ButtonStyle.Outline:
                    fluidElement.containerColor = Color.clear;
                    fluidElement.containerBorderColor = fluidElement.textColor.WithAlpha(fluidElement.textColor.Alpha() * 0.3f);
                    break;
                case ButtonStyle.Clear:
                    if (selectionState == SelectionState.Normal)
                        fluidElement.iconColor = fluidElement.iconSelectableColor.GetColor(selectionState);

                    fluidElement.containerColor = Color.clear;
                    fluidElement.containerBorderColor = Color.clear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            buttonIcon.SetStyleBackgroundImageTintColor(fluidElement.iconColor); //Icon
            buttonLabel.SetStyleColor(fluidElement.textColor); //Label
            buttonContainer.SetStyleBackgroundColor(fluidElement.containerColor); //Background
            layoutContainer.SetStyleBorderColor(fluidElement.containerBorderColor); //Border
        }

        public void ExecuteOnClick(EventBase clickEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            OnClick?.Invoke();
            if (animationTrigger == ButtonAnimationTrigger.OnClick)
                iconReaction?.Play();
        }

        public void ExecuteOnPointerEnter(PointerEnterEvent enterEvent = null)
        {
            if (selectionState == SelectionState.Disabled) return;
            if (animationTrigger == ButtonAnimationTrigger.OnPointerEnter)
                iconReaction?.Play();
        }

    }

    public static class FluidButtonExtensions
    {
        /// <summary> Set the button's selection state </summary>
        /// <param name="target"> Target button </param>
        /// <param name="state"> New selection state </param>
        public static T SetSelectionState<T>(this T target, SelectionState state) where T : FluidButton
        {
            target.selectionState = state;
            return target;
        }

        /// <summary> Enable button and update its visual state </summary>
        /// <param name="target"> Target button </param>
        public static T EnableElement<T>(this T target) where T : FluidButton
        {
            target.fluidElement.Enable();
            return target;
        }

        /// <summary> Disable button and updates its visual state </summary>
        /// <param name="target"> Target Button </param>
        public static T DisableElement<T>(this T target) where T : FluidButton
        {
            target.fluidElement.Disable();
            return target;
        }

        public static T AddToInfoContainer<T>(this T target, VisualElement element) where T : FluidButton
        {
            target.infoContainer.SetStyleDisplay(DisplayStyle.Flex);
            target.infoContainer.AddChild(element);
            return target;
        }

        public static T ClearInfoContainer<T>(this T target) where T : FluidButton
        {
            target.infoContainer
                .RecycleAndClear()
                .SetStyleDisplay(DisplayStyle.None);
            
            return target;
        }

        #region Accent Color

        /// <summary> Set button's accent color </summary>
        /// <param name="target"> Target button </param>
        /// <param name="value"> New accent color </param>
        public static T SetAccentColor<T>(this T target, EditorSelectableColorInfo value) where T : FluidButton
        {
            target.fluidElement.SetAccentColor(value);
            return target;
        }

        /// <summary> Reset the accent color to its default value </summary>
        /// <param name="target"> Target Button </param>
        public static T ResetAccentColor<T>(this T target) where T : FluidButton
        {
            target.fluidElement.ResetAccentColor();
            return target;
        }

        #endregion

        #region Label

        /// <summary> Set label text </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="labelText"> Label text </param>
        public static T SetLabelText<T>(this T target, string labelText) where T : FluidButton
        {
            target.buttonLabel
                .SetText(labelText)
                .SetStyleDisplay(labelText.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public static T ClearLabelText<T>(this T target) where T : FluidButton =>
            target.SetLabelText(string.Empty).SetTooltip(string.Empty);

        #endregion

        #region Icon

        /// <summary> Set Animated Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="textures"> Icon textures </param>
        /// <param name="trigger"> Animated Icon animation trigger </param>
        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures, ButtonAnimationTrigger trigger = ButtonAnimationTrigger.OnPointerEnter) where T : FluidButton
        {
            target.UpdateIconType(IconType.Animated);
            if (target.iconReaction == null)
            {
                target.iconReaction = target.buttonIcon.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            }
            else
            {
                target.iconReaction.SetTextures(textures);
            }
            target.buttonIcon.SetStyleDisplay(DisplayStyle.Flex);
            target.SetAnimationTrigger(trigger);
            return target;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="iconTexture2D"> Icon texture </param>
        public static T SetIcon<T>(this T target, Texture2D iconTexture2D) where T : FluidButton
        {
            target.UpdateIconType(IconType.Static);
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.buttonIcon.SetStyleBackgroundImage(iconTexture2D);
            target.buttonIcon.SetStyleDisplay(DisplayStyle.Flex);
            target.SetAnimationTrigger(ButtonAnimationTrigger.None);
            return target;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="target"> Target Button </param>
        public static T ClearIcon<T>(this T target) where T : FluidButton
        {
            target.UpdateIconType(IconType.None);
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.buttonIcon.SetStyleBackgroundImage((Texture2D)null);
            target.buttonIcon.SetStyleDisplay(DisplayStyle.None);
            return target;
        }

        #endregion

        #region OnClick

        /// <summary> Set callback for OnClick (removes any other callbacks set to OnClick) </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="callback"> OnClick callback </param>
        public static T SetOnClick<T>(this T target, UnityAction callback) where T : FluidButton
        {
            if (callback == null) return target;
            target.OnClick = callback;
            return target;
        }

        /// <summary> Add callback to OnClick (adds another callback to OnClick) </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="callback"> OnClick callback </param>
        public static T AddOnClick<T>(this T target, UnityAction callback) where T : FluidButton
        {
            if (callback == null) return target;
            target.OnClick += callback;
            return target;
        }

        /// <summary> Clear any callbacks set to OnClick </summary>
        /// <param name="target"> Target Button</param>
        public static T ClearOnClick<T>(this T target) where T : FluidButton
        {
            target.OnClick = null;
            return target;
        }

        #endregion
    }
}
