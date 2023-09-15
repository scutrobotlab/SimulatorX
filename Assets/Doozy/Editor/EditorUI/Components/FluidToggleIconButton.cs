// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Colors;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidToggleIconButton : FluidToggle<FluidToggleIconButton>
    {
        public override void Reset()
        {
            base.Reset();

            ResetElementSize();
            ResetAnimationTrigger();
            
            ClearIcon();

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
        public FluidToggleIconButton SetElementSize(ElementSize value)
        {
            UIElementsUtils.RemoveClass(elementSize.ToString(), elementSizeDependentElements);
            UIElementsUtils.AddClass(value.ToString(), elementSizeDependentElements);
            elementSize = value;
            return this;
        }
        public FluidToggleIconButton ResetElementSize() =>
            SetElementSize(ElementSize.Normal);

        #endregion

        #region IconAnimationTrigger

        private IconAnimationTrigger animationTrigger { get; set; }
        public FluidToggleIconButton SetAnimationTrigger(IconAnimationTrigger trigger)
        {
            animationTrigger = trigger;
            return this;
        }
        public FluidToggleIconButton ResetAnimationTrigger() =>
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

        public override bool canHaveMixedValues => false;
        
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        
        private static FluidToggleIconButton Get(string tooltip) =>
            Get().SetTooltip(tooltip);

        public static FluidToggleIconButton Get(Texture2D texture, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get(tooltip).SetIcon(texture).SetToggleAccentColor(accentColor);

        public static FluidToggleIconButton Get(Texture2D texture, string tooltip = "") =>
            Get(texture, null, tooltip);

        public static FluidToggleIconButton Get(IEnumerable<Texture2D> textures, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get(tooltip).SetIcon(textures).SetToggleAccentColor(accentColor);

        public static FluidToggleIconButton Get(IEnumerable<Texture2D> textures, string tooltip = "") =>
            Get(textures, null, tooltip);

        public FluidToggleIconButton()
        {
            this.SetStyleFlexShrink(0);
            
            Add(templateContainer = EditorLayouts.EditorUI.FluidToggleIconButton.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.FieldIcon)
                .AddStyle(EditorStyles.EditorUI.FluidToggleIconButton);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            icon = layoutContainer.Q<Image>(nameof(icon));

            elementSizeDependentElements = new List<VisualElement> { icon };
            iconDependentComponents = new List<VisualElement> { icon };

            fluidElement.OnStateChanged = StateChanged;
            fluidElement.OnPointerEnter = ExecuteOnPointerEnter;

            //RESET
            {
                selectionState = SelectionState.Normal;
            }
        }

        public override void UpdateVisualState(bool animateChange)
        {
            if (animationTrigger == IconAnimationTrigger.OnValueChanged)
            {
                iconReaction?.Play(!isOn);
            }
            else
            {
                iconReaction?.SetProgressAt(!isOn ? 0f : 1f);
            }
                
            fluidElement.StateChanged();
        }

        protected override void StateChanged()
        {
            base.StateChanged();
            icon.SetStyleBackgroundImageTintColor(fluidElement.iconColor); //Icon
        }

        /// <summary> Set Animated Icon </summary>
        /// <param name="textures"> Icon textures </param>
        public FluidToggleIconButton SetIcon(IEnumerable<Texture2D> textures)
        {
            UpdateIconType(IconType.Animated);
            iconReaction.SetTextures(textures);
            icon.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="iconTexture2D"> Icon texture </param>
        public FluidToggleIconButton SetIcon(Texture2D iconTexture2D)
        {
            UpdateIconType(IconType.Static);
            IconReaction?.Recycle();
            IconReaction = null;
            icon.SetStyleBackgroundImage(iconTexture2D);
            icon.SetStyleDisplay(DisplayStyle.Flex);
            SetAnimationTrigger(IconAnimationTrigger.None);
            return this;
        }
        
        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        public FluidToggleIconButton ClearIcon()
        {
            UpdateIconType(IconType.None);
            IconReaction?.Recycle();
            IconReaction = null;
            icon.SetStyleBackgroundImage((Texture2D)null);
            icon.SetStyleDisplay(DisplayStyle.None);
            return this;
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
