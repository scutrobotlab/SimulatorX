// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidContainer : PoolableElement<FluidContainer>
    {
        public override void Reset()
        {
            this.SetEnabled(true);
            this.ResetLayout();
            this.SetTooltip(string.Empty);
            this.ResetAccentColor();
            this.ClearIcon();
            this.ClearLabelText();
            this.ClearHeaderContent();
            this.ClearFluidContent();
            this.ClearFooterContent();
        }

        public override void Dispose()
        {
            base.Dispose();
            iconReaction?.Recycle();
        }

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
        
        public static Color defaultBackgroundColor => EditorColors.Default.Background;
        public static Color defaultIconColor => EditorColors.Default.FieldIcon;
        public static Color defaultTitleColor => EditorColors.Default.TextTitle;

        public static Font titleFont => EditorFonts.Ubuntu.Light;
        public static int titleFontSize => 12;
        
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement headerContainer { get; }
        public Image icon { get; }
        public Label title { get; }
        public VisualElement headerContent { get; }
        public VisualElement fluidContainer { get; }
        public VisualElement fluidContent { get; }
        public VisualElement footerContainer { get; }
        public VisualElement footerContent { get; }

        public Texture2DReaction iconReaction { get; internal set; }
        
        public FluidContainer()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidContainer.CloneTree());

            templateContainer
                .AddStyle(EditorStyles.EditorUI.FluidContainer);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            headerContainer = layoutContainer.Q<VisualElement>(nameof(headerContainer));
            icon = headerContainer.Q<Image>(nameof(icon));
            title = headerContainer.Q<Label>(nameof(title));
            headerContent = headerContainer.Q<VisualElement>(nameof(headerContent));
            fluidContainer = layoutContainer.Q<VisualElement>(nameof(fluidContainer));
            fluidContent = fluidContainer.Q<VisualElement>(nameof(fluidContent));
            footerContainer = layoutContainer.Q<VisualElement>(nameof(footerContainer));
            footerContent = footerContainer.Q<VisualElement>(nameof(footerContent));

            iconDependentComponents = new List<VisualElement> { icon };

            title
                .SetStyleFontSize(titleFontSize)
                .SetStyleUnityFont(titleFont);
            
            this.ResetAccentColor();
            
            layoutContainer.RegisterCallback<PointerEnterEvent>(evt => iconReaction?.Play());
            layoutContainer.AddManipulator(new Clickable(() => iconReaction?.Play()));
        }
    }

    public static class FluidContainerExtensions
    {
        #region Header Container

        public static T AddToHeaderContent<T>(this T target, VisualElement element) where T : FluidContainer
        {
            target.headerContent.AddChild(element);
            target.headerContainer.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }
        
        public static T ClearHeaderContent<T>(this T target) where T : FluidContainer
        {
            target.headerContent.RecycleAndClear();
            target.headerContainer.SetStyleDisplay(DisplayStyle.None);
            return target;
        }

        #endregion

        #region Fluid Content

        public static T AddToFluidContent<T>(this T target, VisualElement element) where T : FluidContainer
        {
            target.fluidContent.AddChild(element);
            return target;
        }
        
        public static T ClearFluidContent<T>(this T target) where T : FluidContainer
        {
            target.fluidContent.RecycleAndClear();
            return target;
        }

        #endregion

        #region Footer Content

        public static T AddToFooterContent<T>(this T target, VisualElement element) where T : FluidContainer
        {
            target.footerContent.AddChild(element);
            target.footerContainer.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }
                
        public static T ClearFooterContent<T>(this T target) where T : FluidContainer
        {
            target.footerContent.RecycleAndClear();
            target.footerContainer.SetStyleDisplay(DisplayStyle.None);
             return target;
        }

        #endregion
        
        #region Accent Color

        /// <summary> Set accent color </summary>
        /// <param name="target"> Target </param>
        /// <param name="color"> New accent color </param>
        public static T SetAccentColor<T>(this T target, Color color) where T : FluidContainer
        {
            float backgroundAlpha = EditorGUIUtility.isProSkin ? 0.05f : 0.1f;
            target.layoutContainer.SetStyleBackgroundColor(color.WithAlpha(backgroundAlpha));
            target.icon.SetStyleBackgroundImageTintColor(color);
            target.title.SetStyleColor(FluidContainer.defaultTitleColor);
            return target;
        }

        /// <summary> Reset the accent color to its default value </summary>
        /// <param name="target"> Target </param>
        public static T ResetAccentColor<T>(this T target) where T : FluidContainer
        {
            target.layoutContainer.SetStyleBackgroundColor(FluidContainer.defaultBackgroundColor);
            target.icon.SetStyleBackgroundImageTintColor(FluidContainer.defaultIconColor);
            target.title.SetStyleColor(FluidContainer.defaultTitleColor);
            return target;
        }

        #endregion

        #region Label

        /// <summary> Set label text </summary>
        /// <param name="target"> Target </param>
        /// <param name="labelText"> Label text </param>
        public static T SetLabelText<T>(this T target, string labelText) where T : FluidContainer
        {
            target.title.SetText(labelText);
            target.title.SetStyleDisplay(labelText.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the text and tooltip values from the title label </summary>
        public static T ClearLabelText<T>(this T target) where T : FluidContainer =>
            target.SetLabelText(string.Empty).SetTooltip(string.Empty);

        #endregion

        #region Icon

        /// <summary> Set Animated Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="textures"> Icon textures </param>
        /// <param name="trigger"> Animated Icon animation trigger </param>
        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures, ButtonAnimationTrigger trigger = ButtonAnimationTrigger.OnPointerEnter) where T : FluidContainer
        {
            target.UpdateIconType(IconType.Animated);
            if (target.iconReaction == null)
            {
                target.iconReaction = target.icon.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            }
            else
            {
                target.iconReaction.SetTextures(textures);
            }
            target.icon.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="iconTexture2D"> Icon texture </param>
        public static T SetIcon<T>(this T target, Texture2D iconTexture2D) where T : FluidContainer
        {
            target.UpdateIconType(IconType.Static);
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.icon.SetStyleBackgroundImage(iconTexture2D);
            target.icon.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="target"> Target Button </param>
        public static T ClearIcon<T>(this T target) where T : FluidContainer
        {
            target.UpdateIconType(IconType.None);
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.icon.SetStyleBackgroundImage((Texture2D)null);
            target.icon.SetStyleDisplay(DisplayStyle.None);
            return target;
        }

        #endregion
    }
}