// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.UIMenu
{
    public class UIMenuItemButton : PoolableElement<UIMenuItemButton>
    {
        private static IEnumerable<Texture2D> lockedIconTextures => EditorMicroAnimations.EditorUI.Icons.Locked;

        public override void Reset()
        {
            ResetAnimationTrigger();

            this.SetTooltip(string.Empty);
            this.SetEnabled(true);
            this.ResetLayout();
            this.SetTooltip(string.Empty);
            this.ResetAccentColor();
            this.ClearIcon();
            this.ClearLabelText();
            this.ClearInfoTagText();
            this.ClearOnClick();
            this.SetSelectionState(SelectionState.Normal);
        }

        public override void Dispose()
        {
            base.Dispose();

            iconReaction?.Recycle();
        }

        #region ButtonAnimationTrigger

        private ButtonAnimationTrigger animationTrigger { get; set; }
        public UIMenuItemButton SetAnimationTrigger(ButtonAnimationTrigger value)
        {
            animationTrigger = value;
            return this;
        }
        public UIMenuItemButton ResetAnimationTrigger() =>
            SetAnimationTrigger(ButtonAnimationTrigger.OnPointerEnter);

        #endregion

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Image backgroundTexture { get; }
        public VisualElement fluidContent { get; }
        public Image previewImage { get; }
        public Label prefabNameLabel { get; }
        private Image lockedIcon { get; set; }
        public Label infoTag { get; set; }

        private Texture2DReaction lockedIconReaction { get; set; }
        public FluidElement fluidElement { get; }
        public SelectionState selectionState
        {
            get => fluidElement.selectionState;
            set => fluidElement.selectionState = value;
        }

        public UnityAction OnClick;

        public Texture2DReaction iconReaction { get; internal set; }

        public UIMenuItem menuItem { get; private set; }

        public UIMenuItemButton()
        {
            Add(templateContainer = EditorLayouts.UIManager.UIMenuItemButton.CloneTree());

            templateContainer
                .AddStyle(EditorStyles.UIManager.UIMenuItemButton);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            backgroundTexture = layoutContainer.Q<Image>(nameof(backgroundTexture));
            fluidContent = layoutContainer.Q<VisualElement>(nameof(fluidContent));
            previewImage = layoutContainer.Q<Image>(nameof(previewImage));
            prefabNameLabel = layoutContainer.Q<Label>(nameof(prefabNameLabel));
            lockedIcon = layoutContainer.Q<Image>(nameof(lockedIcon));
            infoTag = layoutContainer.Q<Label>(nameof(infoTag));

            prefabNameLabel.style.textOverflow = new StyleEnum<TextOverflow>(TextOverflow.Ellipsis);

            backgroundTexture
                .SetStyleBackgroundImage(EditorTextures.EditorUI.Placeholders.TransparencyGrid);

            lockedIcon.SetStyleBackgroundImageTintColor(EditorColors.Default.UnityThemeInversed.WithAlpha(0.4f));
            lockedIconReaction = lockedIcon.GetTexture2DReaction(lockedIconTextures).SetEditorHeartbeat();
            lockedIcon.RegisterCallback<PointerEnterEvent>(evt =>
            {
                if (lockedIcon.GetStyleDisplay() == DisplayStyle.None) return;
                lockedIconReaction?.Play();
            });

            fluidElement = new FluidElement(this)
            {
                OnStateChanged = StateChanged,
                OnClick = ExecuteOnClick,
                OnPointerEnter = ExecuteOnPointerEnter
            };

            //RESET
            {
                ResetAnimationTrigger();
                this.ResetLayout();
                this.ResetAccentColor();
                this.ClearIcon();
                this.ClearLabelText();
                this.ClearOnClick();
                this.SetTooltip(string.Empty);
                selectionState = SelectionState.Normal;
            }
        }

        private void StateChanged()
        {
            previewImage.SetStyleBackgroundImageTintColor(menuItem != null && menuItem.colorize ? fluidElement.iconColor : Color.white); //Preview Image - Icon Color
            prefabNameLabel.SetStyleColor(fluidElement.textColor);                                                                       //Label
            layoutContainer.SetStyleBorderColor(fluidElement.containerBorderColor);                                                      //Border
            layoutContainer.SetStyleBackgroundColor(fluidElement.containerColor);                                                        //Background

            infoTag
                .SetStyleColor(fluidElement.textColor.WithAlpha(0.6f))
                .SetStyleBackgroundColor(fluidElement.containerBorderColor.WithAlpha(0.8f));
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


        public UIMenuItemButton SetUIMenuItem(UIMenuItem item)
        {
            if (item == null) return this;
            menuItem = item;

            string prefabName = menuItem.prefabName;
            if (menuItem.hasInfoTag) prefabName = prefabName.Replace($"({menuItem.infoTag})", "");
            this.SetLabelText(prefabName);
            this.SetTooltip(menuItem.prefabName);
            this.SetIcon(UIMenuUtils.GetIconTextures(item));
            this.SetInfoTagText(menuItem.infoTag);
            this.SetOnClick(() => menuItem.AddToScene());

            lockedIcon
                .SetStyleDisplay
                (
                    menuItem.lockInstantiateMode
                        ? DisplayStyle.Flex
                        : DisplayStyle.None
                )
                .SetTooltip
                (
                    menuItem.lockInstantiateMode
                        ? $"Instantiate Mode Locked in '{menuItem.instantiateMode}' Mode"
                        : string.Empty
                );

            StateChanged();
            return this;
        }


    }

    public static class UIMenuItemButtonExtensions
    {
        /// <summary> Set the button's selection state </summary>
        /// <param name="target"> Target button </param>
        /// <param name="state"> New selection state </param>
        public static T SetSelectionState<T>(this T target, SelectionState state) where T : UIMenuItemButton
        {
            target.selectionState = state;
            return target;
        }

        #region Accent Color

        /// <summary> Set button's accent color </summary>
        /// <param name="target"> Target button </param>
        /// <param name="value"> New accent color </param>
        public static T SetAccentColor<T>(this T target, EditorSelectableColorInfo value) where T : UIMenuItemButton
        {
            target.fluidElement.SetAccentColor(value);
            return target;
        }

        /// <summary> Reset the accent color to its default value </summary>
        /// <param name="target"> Target Button </param>
        public static T ResetAccentColor<T>(this T target) where T : UIMenuItemButton
        {
            target.fluidElement.ResetAccentColor();
            return target;
        }

        #endregion

        #region Label

        /// <summary> Set label text </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="labelText"> Label text </param>
        public static T SetLabelText<T>(this T target, string labelText) where T : UIMenuItemButton
        {
            target
                .prefabNameLabel
                .SetText(labelText)
                .SetStyleDisplay(labelText.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public static T ClearLabelText<T>(this T target) where T : UIMenuItemButton =>
            target.SetLabelText(string.Empty).SetTooltip(string.Empty);

        #endregion


        #region Info Tag

        /// <summary> Set info tag text </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="tagText"> Tag text </param>
        public static T SetInfoTagText<T>(this T target, string tagText) where T : UIMenuItemButton
        {
            target
                .infoTag
                .SetText(tagText)
                .SetStyleDisplay(tagText.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public static T ClearInfoTagText<T>(this T target) where T : UIMenuItemButton =>
            target.SetInfoTagText(string.Empty).SetTooltip(string.Empty);

        #endregion

        #region Icon

        /// <summary> Set Animated Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="textures"> Icon textures </param>
        /// <param name="trigger"> Animated Icon animation trigger </param>
        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures, ButtonAnimationTrigger trigger = ButtonAnimationTrigger.OnPointerEnter) where T : UIMenuItemButton
        {
            if (target.iconReaction == null)
            {
                target.iconReaction = target.previewImage.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            }
            else
            {
                target.iconReaction.SetTextures(textures);
            }
            target.previewImage.SetStyleDisplay(DisplayStyle.Flex);
            target.SetAnimationTrigger(trigger);
            return target;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="iconTexture2D"> Icon texture </param>
        public static T SetIcon<T>(this T target, Texture2D iconTexture2D) where T : UIMenuItemButton
        {
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.previewImage.SetStyleBackgroundImage(iconTexture2D);
            target.previewImage.SetStyleDisplay(DisplayStyle.Flex);
            target.SetAnimationTrigger(ButtonAnimationTrigger.None);
            return target;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="target"> Target Button </param>
        public static T ClearIcon<T>(this T target) where T : UIMenuItemButton
        {
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.previewImage.SetStyleBackgroundImage((Texture2D)null);
            target.previewImage.SetStyleDisplay(DisplayStyle.None);
            return target;
        }

        #endregion

        #region OnClick

        /// <summary> Set callback for OnClick (removes any other callbacks set to OnClick) </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="callback"> OnClick callback </param>
        public static T SetOnClick<T>(this T target, UnityAction callback) where T : UIMenuItemButton
        {
            if (callback == null) return target;
            target.OnClick = callback;
            return target;
        }

        /// <summary> Add callback to OnClick (adds another callback to OnClick) </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="callback"> OnClick callback </param>
        public static T AddOnClick<T>(this T target, UnityAction callback) where T : UIMenuItemButton
        {
            if (callback == null) return target;
            target.OnClick += callback;
            return target;
        }

        /// <summary> Clear any callbacks set to OnClick </summary>
        /// <param name="target"> Target Button</param>
        public static T ClearOnClick<T>(this T target) where T : UIMenuItemButton
        {
            target.OnClick = null;
            return target;
        }

        #endregion
    }
}
