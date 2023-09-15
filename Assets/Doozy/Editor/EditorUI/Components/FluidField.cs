// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidField : PoolableElement<FluidField>
    {
        public sealed override void Reset()
        {
            templateContainer.SetStyleFlexGrow(1);
            
            this.ResetLayout();
            this.SetTooltip(string.Empty);
            this.SetStyleFlexGrow(1);
            this.SetStyleFlexShrink(1);
            this.SetStyleFlexBasisStyleLength(new StyleLength(StyleKeyword.Auto));
            this.SetEnabled(true);
            
            fieldContainer.SetStyleFlexGrow(1);
            fieldContent.SetStyleFlexGrow(1);
            DisableIconAnimation(false);
            ResetElementSize();

            ClearLabelText();
            fieldLabel.SetStyleWidth(StyleKeyword.Auto);
            
            ClearIcon();
            
            
            ClearFieldContent();
            fieldContent.ResetLayout();
            
            ClearInfoContainer();
            
            ResetBackground();
            
            LayoutContainerRestoreLayout();
        }

        public override void Dispose()
        {
            base.Dispose();
            iconReaction?.Recycle();
        }

        public const float k_IconReactionDuration = 1f;

        #region ElementSize

        private ElementSize elementSize { get; set; }
        private List<VisualElement> elementSizeDependentElements { get; }
        public FluidField SetElementSize(ElementSize value)
        {
            UIElementsUtils.RemoveClass(elementSize.ToString(), elementSizeDependentElements);
            UIElementsUtils.AddClass(value.ToString(), elementSizeDependentElements);
            elementSize = value;
            return this;
        }
        public FluidField ResetElementSize() =>
            SetElementSize(ElementSize.Small);

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

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement iconContainer { get; }
        public Image fieldIcon { get; }
        public VisualElement fieldContainer { get; }
        public Label fieldLabel { get; }
        public VisualElement fieldContent { get; }
        public VisualElement infoContainer { get; }

        public Texture2DReaction iconReaction { get; private set; }

        public bool disableIconAnimation { get; private set; }

        public static FluidField Get(string fieldName) =>
            Get().SetLabelText(fieldName);

        public static FluidField Get(VisualElement content) =>
            Get().AddFieldContent(content);

        public static FluidField Get(string fieldName, VisualElement content) =>
            Get().SetLabelText(fieldName).AddFieldContent(content);

        public static FluidField Get<T>(string bindingPath, string labelText = "", string tooltip = "", bool tryToRemovePropertyLabel = false) where T : VisualElement, IBindable, new()
        {
            T bindable =
                new T { bindingPath = bindingPath }
                    .ResetLayout()
                    .SetName($"{typeof(T).Name}: {bindingPath}")
                    .SetTooltip(tooltip);

            if (tryToRemovePropertyLabel)
                bindable.TryToHideLabel();

            return Get().SetLabelText(labelText, tooltip).AddFieldContent(bindable);
        }

        public static FluidField Get<T>(T fieldContentElement, string labelText = "", string tooltip = "") where T : VisualElement =>
            Get().SetLabelText(labelText, tooltip).AddFieldContent(fieldContentElement);

        /// <summary>
        /// Do not use this as new FluidField() as that option does not use the internal pooling system
        /// <para/> use: 'FluidField.Get()' to get a new instance (this uses the internal pooling system) 
        /// </summary>
        public FluidField()
        {
            this.SetStyleFlexGrow(1);

            Add(templateContainer = EditorLayouts.EditorUI.FluidField.CloneTree());
            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.EditorUI.FluidField);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            iconContainer = layoutContainer.Q<VisualElement>(nameof(iconContainer));
            fieldIcon = iconContainer.Q<Image>(nameof(fieldIcon));
            fieldContainer = layoutContainer.Q<VisualElement>(nameof(fieldContainer));
            fieldLabel = fieldContainer.Q<Label>(nameof(fieldLabel));
            fieldContent = fieldContainer.Q<VisualElement>(nameof(fieldContent));
            infoContainer = layoutContainer.Q<VisualElement>(nameof(infoContainer));

            layoutContainer.SetStyleBackgroundColor(DesignUtils.fieldBackgroundColor);
            fieldIcon.SetStyleBackgroundImageTintColor(DesignUtils.fieldIconColor);
            fieldLabel.SetStyleUnityFont(DesignUtils.fieldNameTextFont);

            elementSizeDependentElements = new List<VisualElement> { layoutContainer, iconContainer, fieldIcon, fieldContainer, fieldLabel, fieldContent, infoContainer };
            iconDependentComponents = new List<VisualElement> { iconContainer, fieldIcon };
            fieldIcon.AddManipulator(new Clickable(PlayIconAnimation));
            layoutContainer.RegisterCallback<PointerEnterEvent>(ExecuteOnPointerEnter);
            Reset();
        }

        public FluidField ClearBackground()
        {
            layoutContainer.SetStyleBackgroundColor(Color.clear);
            return this;
        }

        public FluidField ResetBackground()
        {
            layoutContainer.SetStyleBackgroundColor(DesignUtils.fieldBackgroundColor);
            return this;
        }

        private void ExecuteOnPointerEnter(PointerEnterEvent enterEvent) =>
            PlayIconAnimation();

        private void PlayIconAnimation()
        {
            if (disableIconAnimation) return;
            iconReaction?.Play();
        }

        /// <summary> Add an element to the field content container </summary>
        /// <param name="element"> Target element </param>
        public FluidField AddFieldContent(VisualElement element)
        {
            if (element == null) return this;
            fieldContent.SetStyleDisplay(DisplayStyle.Flex);
            fieldContent.AddChild(element);
            return this;
        }

        /// <summary> Recycle and clear all elements from the field content container </summary>
        public FluidField ClearFieldContent()
        {
            fieldContent
                .RecycleAndClear()
                .SetStyleDisplay(DisplayStyle.None);
            
            return this;
        }

        /// <summary> Add an element (button) to the buttons container </summary>
        /// <param name="element"> Target element (button) </param>
        public FluidField AddInfoElement(VisualElement element)
        {
            if (element == null) return this;
            infoContainer.AddChild(element);
            infoContainer.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        /// <summary> Recycle and clear all the elements (buttons) from the buttons container </summary>
        public FluidField ClearInfoContainer()
        {
            infoContainer
                .RecycleAndClear()
                .SetStyleDisplay(DisplayStyle.None);
            
            return this;
        }

        /// <summary> Set label text and tooltip </summary>
        /// <param name="labelText"> Label text </param>
        /// <param name="labelTooltip"> Label tooltip </param>
        public FluidField SetLabelText(string labelText, string labelTooltip = "")
        {
            fieldLabel.SetText(labelText).SetTooltip(labelTooltip);
            fieldLabel.SetStyleDisplay(labelText.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return this;
        }

        /// <summary> Clear the text and tooltip values from the field's label </summary>
        public FluidField ClearLabelText() =>
            SetLabelText(string.Empty, string.Empty);

        /// <summary> Set Animated Icon </summary>
        /// <param name="textures"> Icon textures </param>
        public FluidField SetIcon(IEnumerable<Texture2D> textures)
        {
            UpdateIconType(IconType.Animated);
            iconReaction = fieldIcon.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(k_IconReactionDuration);
            iconContainer.SetStyleDisplay(DisplayStyle.Flex);
            fieldIcon.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="iconTexture2D"> Icon texture </param>
        public FluidField SetIcon(Texture2D iconTexture2D)
        {
            UpdateIconType(IconType.Static);
            iconReaction?.Recycle();
            iconReaction = null;
            fieldIcon.SetStyleBackgroundImage(iconTexture2D);
            iconContainer.SetStyleDisplay(DisplayStyle.Flex);
            fieldIcon.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        public FluidField ClearIcon()
        {
            UpdateIconType(IconType.None);
            iconReaction?.Recycle();
            iconReaction = null;
            fieldIcon.SetStyleBackgroundImage((Texture2D)null);
            fieldIcon.SetStyleBackgroundImageTintColor(DesignUtils.fieldIconColor);
            iconContainer.SetStyleDisplay(DisplayStyle.None);
            fieldIcon.SetStyleDisplay(DisplayStyle.None);
            return this;
        }

        /// <summary> Call ResetLayout() for this field's LayoutContainer </summary>
        public FluidField LayoutContainerResetLayout()
        {
            layoutContainer.ResetLayout();
            return this;
        }

        /// <summary> Restore this field's LayoutContainer to its default style settings </summary>
        public FluidField LayoutContainerRestoreLayout()
        {
            layoutContainer.SetStylePadding(4);
            return this;
        }

        public FluidField DisableIconAnimation(bool disabled)
        {
            disableIconAnimation = disabled;
            return this;
        }


    }
}
