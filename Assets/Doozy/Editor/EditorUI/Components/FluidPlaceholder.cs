// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidPlaceholder : PoolableElement<FluidPlaceholder>
    {
        public override void Reset()
        {
            this.ResetLayout();
            this.SetTooltip(string.Empty);
            this.SetStyleFlexGrow(0);
            
            ClearLabelText();
            ResetIcon();
            ResetAccentColor();
            Show(false);
        }

        public override void Dispose()
        {
            base.Dispose();

            iconReaction?.Recycle();
        }

        private const float ANIMATION_DURATION = 1f;

        public static Color defaultAccentColor => EditorColors.Default.Placeholder;
        public static Font defaultFont => EditorFonts.Ubuntu.Light;
        public static IEnumerable<Texture2D> defaultTextures => EditorMicroAnimations.EditorUI.Placeholders.Empty;

        public Color accentColor { get; private set; }

        public TemplateContainer templateContainer { get; private set; }
        public VisualElement layoutContainer { get; private set; }
        public Image placeholderImage { get; private set; }
        public Label placeholderLabel { get; private set; }
        public Texture2DReaction iconReaction { get; private set; }

        public bool isVisible { get; private set; }
        
        public static FluidPlaceholder Get(IEnumerable<Texture2D> textures) =>
            Get().SetIcon(textures);

        public static FluidPlaceholder Get(string labelText, IEnumerable<Texture2D> textures) =>
            Get(textures).SetLabelText(labelText);

        public static FluidPlaceholder Get(string labelText, IEnumerable<Texture2D> textures, Color accentColor) =>
            Get(labelText, textures).SetAccentColor(accentColor);

        public static FluidPlaceholder Get(Texture2D iconTexture2D) =>
            Get().SetIcon(iconTexture2D);

        public static FluidPlaceholder Get(string labelText, Texture2D iconTexture2D) =>
            Get(iconTexture2D).SetLabelText(labelText);

        public static FluidPlaceholder Get(string labelText, Texture2D iconTexture2D, Color accentColor) =>
            Get(labelText, iconTexture2D).SetAccentColor(accentColor);

        public FluidPlaceholder()
        {
            // this.SetStyleFlexGrow(1);
            this.SetStyleFlexShrink(0);
            this.SetStyleAlignSelf(Align.Center);
            this.SetStyleJustifyContent(Justify.Center);

            Add(templateContainer = EditorLayouts.EditorUI.FluidPlaceholder.CloneTree());

            templateContainer
                .AddStyle(EditorStyles.EditorUI.FluidPlaceholder);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            placeholderImage = layoutContainer.Q<Image>(nameof(placeholderImage));
            placeholderLabel = layoutContainer.Q<Label>(nameof(placeholderLabel));

            placeholderLabel.SetStyleUnityFont(defaultFont);

            ResetAccentColor();
            ResetIcon();

            // RegisterCallback<PointerEnterEvent>(evt => Play());
            this.AddManipulator(new Clickable(() => Play()));

            Hide();
        }

        public FluidPlaceholder Play()
        {
            iconReaction?.Play();
            return this;
        }

        public FluidPlaceholder Show(bool playAnimation = true)
        {
            this.SetStyleDisplay(DisplayStyle.Flex);
            if (playAnimation) Play();
            isVisible = true;
            return this;
        }

        public FluidPlaceholder Hide()
        {
            this.SetStyleDisplay(DisplayStyle.None);
            isVisible = false;
            return this;
        }

        public FluidPlaceholder Toggle(bool show) =>
            show ? Show() : Hide();


        #region Accent Color

        /// <summary> Set accent color </summary>
        /// <param name="color"> New accent color </param>
        public FluidPlaceholder SetAccentColor(Color color)
        {
            accentColor = color;
            placeholderLabel.SetStyleColor(accentColor);
            placeholderImage.SetStyleBackgroundImageTintColor(accentColor);
            return this;
        }

        /// <summary> Reset the accent color to its default value </summary>
        public FluidPlaceholder ResetAccentColor() =>
            SetAccentColor(defaultAccentColor);

        #endregion

        #region Label

        /// <summary> Set label text </summary>
        /// <param name="labelText"> Label text </param>
        public FluidPlaceholder SetLabelText(string labelText)
        {
            placeholderLabel.SetText(labelText);
            placeholderLabel.SetStyleDisplay(labelText.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return this;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public FluidPlaceholder ClearLabelText() =>
            SetLabelText(string.Empty).SetTooltip(string.Empty);

        #endregion

        #region Icon

        /// <summary> Set Animated Icon </summary>
        /// <param name="textures"> Icon Textures </param>
        /// <param name="play"> Play animation </param>
        public FluidPlaceholder SetIcon(IEnumerable<Texture2D> textures, bool play = true)
        {
            if (iconReaction == null)
            {
                iconReaction = placeholderImage.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(ANIMATION_DURATION);
            }
            else
            {
                iconReaction.SetTextures(textures);
            }

            ResizeToCurrentTextureSize();
            placeholderImage.SetStyleDisplay(DisplayStyle.Flex);
            if (play) Play();
            return this;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="iconTexture2D"> Icon texture </param>
        public FluidPlaceholder SetIcon(Texture2D iconTexture2D)
        {
            iconReaction?.Recycle();
            iconReaction = null;
            placeholderImage.SetStyleBackgroundImage(iconTexture2D);
            ResizeToCurrentTextureSize();
            placeholderImage.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        public FluidPlaceholder ClearIcon()
        {
            iconReaction?.Recycle();
            iconReaction = null;
            placeholderImage.SetStyleBackgroundImage((Texture2D)null);
            ResizeToCurrentTextureSize();
            placeholderImage.SetStyleDisplay(DisplayStyle.None);
            return this;
        }

        /// <summary> Set the default placeholder animated icon </summary>
        public FluidPlaceholder ResetIcon() =>
            SetIcon(defaultTextures, false);

        #endregion

        #region Resize

        /// <summary> Resize the image to the given width and height </summary>
        /// <param name="width"> Target width </param>
        /// <param name="height"> Target height </param>
        public FluidPlaceholder Resize(float width, float height)
        {
            placeholderImage.SetStyleSize(width, height);
            return this;
        }

        /// <summary>
        /// Resize the image proportionately to the given width
        /// <para/> Adjusts the height automatically
        /// </summary>
        /// <param name="referenceWidth"> Target width </param>
        public FluidPlaceholder ResizeToWidth(float referenceWidth)
        {
            if (placeholderImage.GetStyleBackgroundImageTexture2D() == null)
                return this;

            float width = placeholderImage.GetStyleBackgroundImageTexture2D().width;
            float height = placeholderImage.GetStyleBackgroundImageTexture2D().height;
            float ratio = referenceWidth / width;
            width = referenceWidth;
            height *= ratio;
            return Resize(width, height);
        }

        /// <summary>
        /// Resize the image proportionately to the given height
        /// <para/> Adjusts the width automatically
        /// </summary>
        /// <param name="referenceHeight"> Target height </param>
        public FluidPlaceholder ResizeToHeight(float referenceHeight)
        {
            if (placeholderImage.GetStyleBackgroundImageTexture2D() == null)
                return this;

            float width = placeholderImage.GetStyleBackgroundImageTexture2D().width;
            float height = placeholderImage.GetStyleBackgroundImageTexture2D().height;
            float ratio = referenceHeight / height;
            height = referenceHeight;
            width *= ratio;
            return Resize(width, height);
        }

        /// <summary> Resize the image to the current texture size and apply a given ratio (to make it bigger or smaller proportionately) </summary>
        /// <param name="ratio"> Size change ratio </param>
        public FluidPlaceholder ResizeToCurrentTextureSize(float ratio = 1)
        {
            bool hasTexture = placeholderImage.GetStyleBackgroundImageTexture2D() != null;
            float width = hasTexture ? placeholderImage.GetStyleBackgroundImageTexture2D().width : 0f;
            float height = hasTexture ? placeholderImage.GetStyleBackgroundImageTexture2D().height : 0f;
            ratio = Mathf.Max(0, ratio);
            width *= ratio;
            height *= ratio;
            return Resize(width, height);
        }

        #endregion

    }
}
