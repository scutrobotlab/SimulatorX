// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidToggleButtonTab : FluidToggle<FluidToggleButtonTab>
    {
       public override void Reset()
        {
            base.Reset();

            RemoveFromToggleGroup();

            ResetElementSize();
            ResetTabPosition();
            ResetLayoutOrientation();
            ResetTabContent();
            ResetIconAnimationTrigger();

            this.SetStyleMinWidth(StyleKeyword.Auto);
            this.SetStyleWidth(StyleKeyword.Auto);
            this.SetStyleMaxWidth(StyleKeyword.Auto);
            
            this.SetStyleMinHeight(StyleKeyword.Auto);
            this.SetStyleHeight(StyleKeyword.Auto);
            this.SetStyleMaxHeight(StyleKeyword.Auto);
            
            this.ResetLayout();
            this.ResetAccentColor();
            ClearIcon();
            ClearLabelText();
            ClearInfoContainer();
            this.ClearOnClick();
            this.ClearOnValueChanged();
            this.SetTooltip(string.Empty);
            
            selectionState = SelectionState.Normal;
        }

        public override void Dispose()
        {
            base.Dispose();

            fluidElement.OnPointerEnter -= ExecuteOnPointerEnter;
        }

        #region ElementSize

        private ElementSize elementSize { get; set; }
        private List<VisualElement> elementSizeDependentElements { get; }
        public FluidToggleButtonTab SetElementSize(ElementSize value)
        {
            UIElementsUtils.RemoveClass(elementSize.ToString(), elementSizeDependentElements);
            UIElementsUtils.AddClass(value.ToString(), elementSizeDependentElements);
            elementSize = value;
            return this;
        }
        public FluidToggleButtonTab ResetElementSize() =>
            SetElementSize(ElementSize.Normal);

        #endregion

        #region TabPosition

        private TabPosition tabPosition { get; set; }
        private List<VisualElement> tabPositionDependentElements { get; }

        public FluidToggleButtonTab SetTabPosition(TabPosition value)
        {
            UIElementsUtils.RemoveClass(tabPosition.ToString(), tabPositionDependentElements);
            UIElementsUtils.AddClass(value.ToString(), tabPositionDependentElements);
            tabPosition = value;
            return this;
        }

        public FluidToggleButtonTab ResetTabPosition() =>
            SetTabPosition(TabPosition.FloatingTab);

        #endregion

        #region LayoutOrientation

        private LayoutOrientation layoutOrientation { get; set; }
        private List<VisualElement> layoutOrientationDependentElements { get; }
        public FluidToggleButtonTab SetLayoutOrientation(LayoutOrientation value)
        {
            UIElementsUtils.RemoveClass(layoutOrientation.ToString(), layoutOrientationDependentElements);
            UIElementsUtils.AddClass(value.ToString(), layoutOrientationDependentElements);
            layoutOrientation = value;
            return this;
        }
        public FluidToggleButtonTab ResetLayoutOrientation() =>
            SetLayoutOrientation(LayoutOrientation.Horizontal);

        #endregion

        #region TabContent

        private TabContent tabContent { get; set; } = TabContent.Undefined;
        private List<VisualElement> contentDependentElements { get; }

        public FluidToggleButtonTab SetTabContent(TabContent value)
        {
            if (tabContent != TabContent.Undefined)
                UIElementsUtils.RemoveClass(tabContent.ToString(), contentDependentElements);

            if (value != TabContent.Undefined)
                UIElementsUtils.AddClass(value.ToString(), contentDependentElements);

            bool undefined = value == TabContent.Undefined;
            icon.SetStyleDisplay(value == TabContent.TextOnly || undefined ? DisplayStyle.None : DisplayStyle.Flex);
            buttonLabel.SetStyleDisplay(value == TabContent.IconOnly || undefined ? DisplayStyle.None : DisplayStyle.Flex);
            tabContent = value;
            return this;
        }

        public FluidToggleButtonTab ResetTabContent() =>
            SetTabContent(TabContent.Undefined);

        #endregion

        #region IconAnimationTrigger

        private IconAnimationTrigger animationTrigger { get; set; }
        public FluidToggleButtonTab SetAnimationTrigger(IconAnimationTrigger trigger)
        {
            // Debug.Log($"{nameof(SetAnimationTrigger)} - from: {animationTrigger} to: {trigger}");
            animationTrigger = trigger;
            return this;
        }
        public FluidToggleButtonTab ResetIconAnimationTrigger() =>
            SetAnimationTrigger(IconAnimationTrigger.OnPointerEnter);

        #endregion

        #region IconType

        private IconType iconType { get; set; } = IconType.None;
        private List<VisualElement> iconDependentComponents { get; }
        private void UpdateIconType(IconType value)
        {
            if (iconType != IconType.None) UIElementsUtils.RemoveClass(iconType.ToString(), iconDependentComponents);
            if (value != IconType.None) UIElementsUtils.AddClass(value.ToString(), iconDependentComponents);
            iconType = value;
        }

        #endregion

        private static Font font => EditorFonts.Ubuntu.Regular;

        protected internal Color ContainerColorOff = Color.clear;

        public override bool canHaveMixedValues => false;

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement buttonContainer { get; }
        public Label buttonLabel { get; }
        public VisualElement infoContainer { get; }

        public static FluidToggleButtonTab Get(string labelText, EditorSelectableColorInfo accentColor = null, string tooltip = "") =>
            Get().SetLabelText(labelText).SetToggleAccentColor(accentColor).SetTooltip(tooltip);

        public static FluidToggleButtonTab Get(string labelText, Texture2D texture, EditorSelectableColorInfo accentColor = null, string tooltip = "") =>
            Get(labelText, accentColor, tooltip).SetIcon(texture);

        public static FluidToggleButtonTab Get(Texture2D texture, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get(string.Empty, texture, accentColor, tooltip);

        public static FluidToggleButtonTab Get(Texture2D texture, string tooltip = "") =>
            Get(string.Empty, texture, null, tooltip);

        public static FluidToggleButtonTab Get(string labelText, IEnumerable<Texture2D> textures, EditorSelectableColorInfo accentColor = null, string tooltip = "") =>
            Get(labelText, accentColor, tooltip).SetIcon(textures);

        public static FluidToggleButtonTab Get(IEnumerable<Texture2D> textures, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get(string.Empty, textures, accentColor, tooltip);

        public static FluidToggleButtonTab Get(IEnumerable<Texture2D> textures, string tooltip = "") =>
            Get(string.Empty, textures, null, tooltip);

        public FluidToggleButtonTab()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidToggleButton.CloneTree());
            templateContainer
                // .AddStyle(EditorStyles.EditorUI.LayoutContainer)
                .AddStyle(EditorStyles.EditorUI.FieldIcon)
                .AddStyle(EditorStyles.EditorUI.FieldName)
                .AddStyle(EditorStyles.EditorUI.FluidToggleButton);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            buttonContainer = layoutContainer.Q<VisualElement>(nameof(buttonContainer));
            icon = layoutContainer.Q<Image>(nameof(icon));
            buttonLabel = layoutContainer.Q<Label>(nameof(buttonLabel));
            infoContainer = layoutContainer.Q<VisualElement>(nameof(infoContainer));

            elementSizeDependentElements = new List<VisualElement> { buttonContainer, icon, buttonLabel };
            layoutOrientationDependentElements = new List<VisualElement> { buttonContainer };
            contentDependentElements = new List<VisualElement> { buttonContainer };
            iconDependentComponents = new List<VisualElement> { icon };
            tabPositionDependentElements = new List<VisualElement> { layoutContainer, buttonContainer };

            buttonLabel.SetStyleUnityFont(font);

            fluidElement.OnStateChanged = StateChanged;
            fluidElement.OnPointerEnter = ExecuteOnPointerEnter;

            //RESET

            ResetElementSize();
            ResetTabPosition();
            ResetLayoutOrientation();
            ResetTabContent();
            ResetIconAnimationTrigger();

            ClearIcon();
            ClearLabelText();

            selectionState = SelectionState.Normal;

            iconReaction.SetDuration(0.6f);
        }

        public FluidToggleButtonTab AddToInfoContainer(VisualElement element)
        {
            infoContainer.SetStyleDisplay(DisplayStyle.Flex);
            infoContainer.AddChild(element);
            return this;
        }

        public FluidToggleButtonTab ClearInfoContainer()
        {
            infoContainer
                .RecycleAndClear()
                .SetStyleDisplay(DisplayStyle.None);
            
            return this;
        }
        
        /// <summary> Set label text </summary>
        /// <param name="labelText"> Label text </param>
        public FluidToggleButtonTab SetLabelText(string labelText)
        {
            buttonLabel.SetText(labelText);
            if (labelText.IsNullOrEmpty())
            {
                switch (tabContent)
                {
                    case TabContent.TextOnly:
                        SetTabContent(TabContent.Undefined);
                        return this;
                    case TabContent.IconAndText:
                        SetTabContent(TabContent.IconOnly);
                        break;
                }

                return this;
            }

            switch (tabContent)
            {
                case TabContent.Undefined:
                    SetTabContent(TabContent.TextOnly);
                    break;

                case TabContent.IconOnly:
                    SetTabContent(TabContent.IconAndText);
                    break;
            }

            return this;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public FluidToggleButtonTab ClearLabelText() =>
            SetLabelText(string.Empty);

        /// <summary> Set Animated Icon </summary>
        /// <param name="textures"> Icon textures </param>
        public FluidToggleButtonTab SetIcon(IEnumerable<Texture2D> textures)
        {
            UpdateIconType(IconType.Animated);
            iconReaction.SetTextures(textures);
            icon.SetStyleDisplay(DisplayStyle.Flex);
            switch (tabContent)
            {
                case TabContent.Undefined:
                    SetTabContent(TabContent.IconOnly);
                    break;

                case TabContent.TextOnly:
                    SetTabContent(TabContent.IconAndText);
                    break;
            }
            return this;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="iconTexture2D"> Icon texture </param>
        public FluidToggleButtonTab SetIcon(Texture2D iconTexture2D)
        {
            UpdateIconType(IconType.Static);
            IconReaction?.Recycle();
            IconReaction = null;
            icon.SetStyleBackgroundImage(iconTexture2D);
            icon.SetStyleDisplay(DisplayStyle.Flex);
            SetAnimationTrigger(IconAnimationTrigger.None);
            switch (tabContent)
            {
                case TabContent.Undefined:
                    SetTabContent(TabContent.IconOnly);
                    break;

                case TabContent.TextOnly:
                    SetTabContent(TabContent.IconAndText);
                    break;
            }
            return this;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        public FluidToggleButtonTab ClearIcon()
        {
            UpdateIconType(IconType.None);
            IconReaction?.Recycle();
            IconReaction = null;
            icon.SetStyleBackgroundImage((Texture2D)null);
            icon.SetStyleDisplay(DisplayStyle.None);
            switch (tabContent)
            {
                case TabContent.IconOnly:
                    SetTabContent(TabContent.Undefined);
                    break;

                case TabContent.IconAndText:
                    SetTabContent(TabContent.TextOnly);
                    break;
            }
            return this;
        }

        public FluidToggleButtonTab SetContainerColorOff(Color value)
        {
            ContainerColorOff = value;
            fluidElement.StateChanged();
            return this;
        }

        public override void UpdateVisualState(bool animateChange)
        {
            if (animationTrigger == IconAnimationTrigger.OnValueChanged)
            {
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

        protected override void StateChanged()
        {
            base.StateChanged();

            switch (selectionState)
            {
                case SelectionState.Normal:
                    if (isOn == false)
                        fluidElement.containerColor = ContainerColorOff;
                    break;
                case SelectionState.Highlighted:
                    break;
                case SelectionState.Pressed:
                    break;
                case SelectionState.Selected:
                    break;
                case SelectionState.Disabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            icon.SetStyleBackgroundImageTintColor(fluidElement.iconColor); //Icon
            buttonLabel.SetStyleColor(fluidElement.textColor); //Label
            buttonContainer.SetStyleBackgroundColor(fluidElement.containerColor); //Background
            // layoutContainer.SetStyleBorderColor(fluidElement.containerBorderColor); //Border
        }

        protected override void ExecuteOnClick(EventBase clickEvent)
        {
            base.ExecuteOnClick(clickEvent);
            if (selectionState == SelectionState.Disabled) return;
            if (animationTrigger == IconAnimationTrigger.OnClick)
                iconReaction?.Play();
        }

        private void ExecuteOnPointerEnter(PointerEnterEvent enterEvent)
        {
            if (selectionState == SelectionState.Disabled) return;
            if (animationTrigger == IconAnimationTrigger.OnPointerEnter)
                iconReaction?.Play();
        }

    }
}
