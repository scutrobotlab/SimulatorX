// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidComponentHeader : PoolableElement<FluidComponentHeader>
    {
        public override void Reset()
        {
            this.ResetLayout();
            this.SetTooltip(string.Empty);

            ResetElementSize();
            ResetAccentColor();
            ClearIcon();
            ClearRotatedIcon();
            ClearSecondaryIcon();
            ClearBarRightContainer();
            ClearComponentNameText();
            ClearComponentTypeText();
            
            OnClick = TriggerAnimations;
            OnPointerEnter = null;
            OnPointerLeave = null;
            OnPointerDown = null;
            OnPointerUp = null;
        }

        public override void Dispose()
        {
            base.Dispose();

            iconReaction?.Recycle();
            rotatedIconReaction?.Recycle();
            secondaryIconReaction?.Recycle();
        }

        #region ElementSize

        private ElementSize elementSize { get; set; }
        private List<VisualElement> elementSizeDependentElements { get; }
        public FluidComponentHeader SetElementSize(ElementSize value)
        {
            UIElementsUtils.RemoveClass(elementSize.ToString(), elementSizeDependentElements);
            UIElementsUtils.AddClass(value.ToString(), elementSizeDependentElements);
            elementSize = value;
            return this;
        }
        public FluidComponentHeader ResetElementSize() =>
            SetElementSize(ElementSize.Large);

        #endregion

        public static Color componentBarColor => EditorColors.Default.Background;
        public static Color componentNameTextColor => EditorColors.Default.TextTitle;
        public static Color componentTypeTextColor => EditorColors.Default.TextSubtitle;
        public static Font componentNameFont => EditorFonts.Ubuntu.Regular;
        public static Font componentTypeFont => EditorFonts.Ubuntu.Light;

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement iconContainer { get; }
        public Image icon { get; }
        public VisualElement barContainer { get; }
        public VisualElement barLeftContainer { get; }
        public Image secondaryIcon { get; }
        public Label componentNameLabel { get; }
        public Label componentTypeLabel { get; }
        public Image rotatedIcon { get; }
        public VisualElement barRightContainer { get; }

        public IconType iconType { get; private set; } = IconType.None;
        public Texture2DReaction iconReaction { get; private set; }
        public bool hasAnimatedIcon => iconReaction != null;

        public IconType rotatedIconType { get; private set; } = IconType.None;
        public Texture2DReaction rotatedIconReaction { get; private set; }
        public bool hasAnimatedRotatedIcon => rotatedIconReaction != null;

        public IconType secondaryIconType { get; private set; } = IconType.None;
        public Texture2DReaction secondaryIconReaction { get; private set; }
        public bool hasAnimatedSecondaryIcon => secondaryIconReaction != null;

        public Clickable clickable { get; }
        public UnityAction OnClick;
        // ReSharper disable UnassignedField.Global
        public UnityAction<PointerEnterEvent> OnPointerEnter;
        public UnityAction<PointerLeaveEvent> OnPointerLeave;
        public UnityAction<PointerDownEvent> OnPointerDown;
        public UnityAction<PointerUpEvent> OnPointerUp;
        // ReSharper restore UnassignedField.Global

        public FluidComponentHeader()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidComponentHeader.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.FluidComponentHeader)
                .SetStyleFlexShrink(0);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            iconContainer = layoutContainer.Q<VisualElement>(nameof(iconContainer));
            icon = iconContainer.Q<Image>(nameof(icon));
            barContainer = layoutContainer.Q<VisualElement>(nameof(barContainer));
            barLeftContainer = barContainer.Q<VisualElement>(nameof(barLeftContainer));
            secondaryIcon = barLeftContainer.Q<Image>(nameof(secondaryIcon));
            componentNameLabel = barLeftContainer.Q<Label>(nameof(componentNameLabel));
            componentTypeLabel = barLeftContainer.Q<Label>(nameof(componentTypeLabel));
            rotatedIcon = barLeftContainer.Q<Image>(nameof(rotatedIcon));
            barRightContainer = barContainer.Q<VisualElement>(nameof(barRightContainer));

            elementSizeDependentElements = new List<VisualElement>
            {
                iconContainer, icon, rotatedIcon,
                barContainer,
                barLeftContainer, secondaryIcon, componentNameLabel, componentTypeLabel,
                barRightContainer
            };

            rotatedIcon.transform.rotation = Quaternion.Euler(0, 0, -20);

            barContainer.SetStyleBackgroundColor(componentBarColor);
            iconContainer.SetStyleBackgroundColor(componentBarColor);
            rotatedIcon.SetStyleBackgroundImageTintColor(componentNameTextColor.WithAlpha(0.075f));
            componentTypeLabel.SetStyleColor(componentTypeTextColor);


            componentNameLabel.SetStyleUnityFont(componentNameFont);
            componentTypeLabel.SetStyleUnityFont(componentTypeFont);

            ResetElementSize();

            clickable = new Clickable(() => OnClick?.Invoke());
            OnClick += TriggerAnimations;
            this.AddManipulator(clickable);

            RegisterCallback<PointerEnterEvent>(evt => OnPointerEnter?.Invoke(evt));
            RegisterCallback<PointerLeaveEvent>(evt => OnPointerLeave?.Invoke(evt));
            RegisterCallback<PointerDownEvent>(evt => OnPointerDown?.Invoke(evt));
            RegisterCallback<PointerUpEvent>(evt => OnPointerUp?.Invoke(evt));
        }

        private void TriggerAnimations()
        {
            iconReaction?.Play();
            rotatedIconReaction?.Play();
            secondaryIconReaction?.Play();
        }

        public FluidComponentHeader ClearBarRightContainer()
        {
            barRightContainer
                .RecycleAndClear();
            
            return this;
        }

        /// <summary> Add element to barRightContainer </summary>
        /// <param name="element"> Target element </param>
        public FluidComponentHeader AddElement(VisualElement element)
        {
            barRightContainer.SetStyleDisplay(DisplayStyle.Flex);
            barRightContainer.Add(element);
            return this;
        }

        private ElementSize GetButtonSize()
        {
            switch (elementSize)
            {
                case ElementSize.Tiny:
                case ElementSize.Small:
                    return ElementSize.Tiny;
                case ElementSize.Normal:
                case ElementSize.Large:
                    return ElementSize.Small;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private FluidButton GetNewButton(string buttonTooltip) =>
            FluidButton.Get().SetTooltip(buttonTooltip)
                .SetButtonStyle(ButtonStyle.Clear)
                .SetElementSize(GetButtonSize())
                .SetStyleFlexShrink(0);

        private FluidButton GetNewButton(Texture2D staticIcon, string buttonTooltip) =>
            GetNewButton(buttonTooltip).SetIcon(staticIcon);

        private FluidButton GetNewButton(IEnumerable<Texture2D> textures, string buttonTooltip) =>
            GetNewButton(buttonTooltip).SetIcon(textures);

        public FluidComponentHeader AddManualButton(string url = "http://bit.ly/DoozyKnowledgeBase4")
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return this; //do not add Manual button in play mode
            return AddElement(GetNewButton(EditorMicroAnimations.EditorUI.Icons.BookOpen, "Manual")
                .SetOnClick(() => Application.OpenURL(url)));
        }

        public FluidComponentHeader AddYouTubeButton(string url = "www.youtube.com/c/DoozyEntertainment")
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return this; //do not add YouTube button in play mode
            return AddElement(GetNewButton(EditorMicroAnimations.EditorUI.Icons.Youtube, "YouTube")
                .SetOnClick(() => Application.OpenURL(url)));
        }

        public FluidComponentHeader AddSettingsButton(UnityAction callback) =>
            AddElement(GetNewButton(EditorMicroAnimations.EditorUI.Icons.Settings, "Settings")
                .SetOnClick(callback));

        public FluidComponentHeader AddButton(List<Texture2D> animatedIcon, string buttonTooltip, UnityAction callback) =>
            AddElement(GetNewButton(animatedIcon, buttonTooltip)
                .SetOnClick(callback));

        public FluidComponentHeader AddButton(Texture2D staticIcon, string buttonTooltip, UnityAction callback) =>
            AddElement(GetNewButton(staticIcon, buttonTooltip)
                .SetOnClick(callback));

        public FluidComponentHeader SetAccentColor(Color accentColor)
        {
            icon.SetStyleBackgroundImageTintColor(accentColor);
            componentNameLabel.SetStyleColor(accentColor);
            secondaryIcon.SetStyleBackgroundImageTintColor(accentColor);
            return this;
        }

        public FluidComponentHeader ResetAccentColor()
        {
            icon.SetStyleBackgroundImageTintColor(componentNameTextColor);
            componentNameLabel.SetStyleColor(componentNameTextColor);
            secondaryIcon.SetStyleBackgroundImageTintColor(componentTypeTextColor);
            return this;
        }

        public FluidComponentHeader SetComponentNameText(string value)
        {
            componentNameLabel.text = value;
            return this;
        }

        public FluidComponentHeader ClearComponentNameText() =>
            SetComponentNameText(string.Empty);
        
        public FluidComponentHeader SetComponentTypeText(string value)
        {
            componentTypeLabel.text = value;
            componentTypeLabel.SetStyleDisplay(value.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return this;
        }

        public FluidComponentHeader ClearComponentTypeText() =>
            SetComponentTypeText(string.Empty);

        #region Icon

        /// <summary> Set Animated Icon </summary>
        /// <param name="textures"> Icon textures </param>
        /// <param name="showRotatedIcon"> True if this header also has (shows) a rotated icon with the same textures </param>
        public FluidComponentHeader SetIcon(List<Texture2D> textures, bool showRotatedIcon = true)
        {
            UpdateIconType(IconType.Animated);

            if (iconReaction == null)
                iconReaction = Reaction.Get<Texture2DReaction>().SetEditorHeartbeat().SetDuration(0.6f)
                    .SetSetter(value => icon.SetStyleBackgroundImage(value));

            iconReaction.SetTextures(textures);
            iconContainer.SetStyleDisplay(DisplayStyle.Flex);
            icon.SetStyleDisplay(DisplayStyle.Flex);

            if (showRotatedIcon) SetRotatedIcon(textures);
            return this;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="texture2D"> Icon texture </param>
        /// <param name="showRotatedIcon"> True if this header also has (shows) a rotated icon with the same texture </param>
        public FluidComponentHeader SetIcon(Texture2D texture2D, bool showRotatedIcon = true)
        {
            UpdateIconType(IconType.Static);

            iconReaction?.Recycle();
            iconReaction = null;

            icon.SetStyleBackgroundImage(texture2D);
            iconContainer.SetStyleDisplay(DisplayStyle.Flex);
            icon.SetStyleDisplay(DisplayStyle.Flex);

            if (showRotatedIcon) SetRotatedIcon(texture2D);
            return this;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="clearRotatedIcon"> Also clears the rotated icon </param>
        public FluidComponentHeader ClearIcon(bool clearRotatedIcon = true)
        {
            UpdateIconType(IconType.None);
            iconReaction?.Recycle();
            iconReaction = null;
            icon.SetStyleBackgroundImage((Texture2D)null);
            iconContainer.SetStyleDisplay(DisplayStyle.None);
            icon.SetStyleDisplay(DisplayStyle.None);
            if (clearRotatedIcon) ClearRotatedIcon();
            return this;
        }

        private void UpdateIconType(IconType value)
        {
            if (iconType != IconType.None) icon.RemoveClass(iconType.ToString());
            if (value != IconType.None) icon.AddClass(value.ToString());
            iconType = value;
        }

        #endregion

        #region Rotated Icon

        /// <summary> Set Animated Rotated Icon </summary>
        /// <param name="textures"> Icon textures </param>
        public FluidComponentHeader SetRotatedIcon(List<Texture2D> textures)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) //do not show rotated icon in play mode
                return this;

            UpdateRotatedIconType(IconType.Animated);

            if (rotatedIconReaction == null)
                rotatedIconReaction = Reaction.Get<Texture2DReaction>().SetEditorHeartbeat().SetDuration(0.6f)
                    .SetSetter(value => rotatedIcon.SetStyleBackgroundImage(value));

            rotatedIconReaction.SetTextures(textures);
            rotatedIcon.SetStyleDisplay(textures?.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None);
            return this;
        }

        /// <summary> Set Static Rotated Icon </summary>
        /// <param name="texture2D"> Icon texture </param>
        public FluidComponentHeader SetRotatedIcon(Texture2D texture2D)
        {
            UpdateRotatedIconType(IconType.Static);

            rotatedIconReaction?.Recycle();
            rotatedIconReaction = null;

            rotatedIcon.SetStyleBackgroundImage(texture2D);
            rotatedIcon.SetStyleDisplay(DisplayStyle.Flex);

            return this;
        }

        /// <summary> Clear the rotated icon. If the rotated icon is animated, its reaction will get recycled </summary>
        public FluidComponentHeader ClearRotatedIcon()
        {
            UpdateRotatedIconType(IconType.None);
            rotatedIconReaction?.Recycle();
            rotatedIconReaction = null;
            rotatedIcon.SetStyleBackgroundImage((Texture2D)null);
            rotatedIcon.SetStyleDisplay(DisplayStyle.None);
            return this;
        }

        private void UpdateRotatedIconType(IconType value)
        {
            if (rotatedIconType != IconType.None) rotatedIcon.RemoveClass(iconType.ToString());
            if (value != IconType.None) rotatedIcon.AddClass(value.ToString());
            rotatedIconType = value;
        }

        #endregion

        #region Secondary Icon

        /// <summary> Set Animated Secondary Icon </summary>
        /// <param name="textures"> Secondary icon textures </param>
        public FluidComponentHeader SetSecondaryIcon(List<Texture2D> textures)
        {
            UpdateSecondaryIconType(IconType.Animated);

            if (secondaryIconReaction == null)
                secondaryIconReaction = Reaction.Get<Texture2DReaction>().SetEditorHeartbeat().SetDuration(0.6f)
                    .SetSetter(value => secondaryIcon.SetStyleBackgroundImage(value));

            secondaryIconReaction.SetTextures(textures);
            secondaryIcon.SetStyleDisplay(textures?.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None);
            return this;
        }

        /// <summary> Set Static Secondary Icon </summary>
        /// <param name="texture2D"> Secondary icon texture </param>
        public FluidComponentHeader SetSecondaryIcon(Texture2D texture2D)
        {
            UpdateSecondaryIconType(IconType.Static);
            secondaryIconReaction?.Recycle();
            secondaryIconReaction = null;
            secondaryIcon.SetStyleBackgroundImage(texture2D);
            secondaryIcon.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        /// <summary> Clear the secondary icon. If the secondary icon is animated, its reaction will get recycled </summary>
        public FluidComponentHeader ClearSecondaryIcon()
        {
            UpdateSecondaryIconType(IconType.None);
            secondaryIconReaction?.Recycle();
            secondaryIconReaction = null;
            secondaryIcon.SetStyleBackgroundImage((Texture2D)null);
            secondaryIcon.SetStyleDisplay(DisplayStyle.None);
            return this;
        }

        private void UpdateSecondaryIconType(IconType value)
        {
            if (secondaryIconType != IconType.None) secondaryIcon.RemoveClass(secondaryIconType.ToString());
            if (value != IconType.None) secondaryIcon.AddClass(value.ToString());
            secondaryIconType = value;
        }

        #endregion
    }
}
