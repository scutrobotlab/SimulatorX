// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidFoldout : VisualElement, IDisposable
    {
        //VisualElement
        //--TabButton
        //--Container
        //----Content
        //--Footer

        public virtual void Dispose()
        {
            tabButton?.Dispose();
            colorReaction?.Recycle();
            animatedContainer?.Dispose();
        }

        //SETTINGS
        public const int k_DefaultContentLeftPadding = 16;
        public const int k_DefaultContentMinimumPadding = 4;
        public static Ease expandReactionEase => FluidAnimatedContainer.k_ReactionEase;
        public static float expandReactionDuration => FluidAnimatedContainer.k_ReactionDuration;

        private static Color defaultContainerColor => EditorColors.Default.Background;

        // private FloatReaction expandReaction { get; }
        // private float expandTargetValue { get; set; }

        private ColorReaction colorReaction { get; }
        private bool initialized { get; set; }

        private Color containerColor { get; set; }

        private Color GetFooterColor() =>
            tabButton.fluidElement.hasAccentColor
                ? tabButton.fluidElement.selectableAccentColor.normalColor.WithAlpha(EditorGUIUtility.isProSkin ? 0.3f : 0.4f)
                : defaultContainerColor;

        #region IToggle & TabButton

        public FluidToggleButtonTab tabButton { get; }

        public IToggleGroup toggleGroup
        {
            get => tabButton.toggleGroup;
            set => tabButton.toggleGroup = value;
        }

        public bool isOn
        {
            get => tabButton.isOn;
            set => tabButton.isOn = value;
        }

        public FluidFoldout SetIsOn(bool newValue, bool animateChange)
        {
            tabButton.SetIsOn(newValue, animateChange);
            return this;
        }

        public FluidFoldout AddToToggleGroup(IToggleGroup value)
        {
            tabButton.AddToToggleGroup(value);
            return this;
        }

        public FluidFoldout RemoveFromToggleGroup()
        {
            tabButton.RemoveFromToggleGroup();
            return this;
        }

        public void UpdateValueFromGroup(bool newValue, bool silent = false) =>
            tabButton.UpdateValueFromGroup(newValue, silent);

        #endregion

        private TemplateContainer templateContainer { get; }
        private VisualElement layoutContainer { get; }
        private VisualElement header { get; }
        private VisualElement content { get; }
        private VisualElement contentLayoutContainer { get; }
        private VisualElement footer { get; }

        public FluidAnimatedContainer animatedContainer { get; }

        public FluidFoldout(string labelText, EditorSelectableColorInfo accentColor, string tooltip = "") : this()
        {
            this
                .SetLabelText(labelText)
                .SetAccentColor(accentColor)
                .SetTooltip(tooltip);
        }

        public FluidFoldout(string labelText, string tooltip = "") : this()
        {
            this
                .SetLabelText(labelText)
                .SetTooltip(tooltip);
        }

        public FluidFoldout(string labelText, IEnumerable<Texture2D> textures, EditorSelectableColorInfo accentColor = null, string tooltip = "") : this()
        {
            this
                .SetLabelText(labelText)
                .SetIcon(textures)
                .SetAccentColor(accentColor)
                .SetTooltip(tooltip);
        }

        public FluidFoldout(IEnumerable<Texture2D> textures, EditorSelectableColorInfo accentColor, string tooltip = "") : this()
        {
            this
                .SetIcon(textures)
                .SetAccentColor(accentColor)
                .SetTooltip(tooltip);
        }

        public FluidFoldout(IEnumerable<Texture2D> textures, string tooltip = "") : this()
        {
            this
                .SetIcon(textures)
                .SetTooltip(tooltip);
        }

        public FluidFoldout()
        {
            Add(templateContainer = EditorLayouts.EditorUI.FluidFoldout.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.FieldIcon)
                // .AddStyle(EditorStyles.EditorUI.FieldName)
                .AddStyle(EditorStyles.EditorUI.FluidFoldout);

            layoutContainer = templateContainer.Q<VisualElement>("LayoutContainer");
            header = layoutContainer.Q<VisualElement>("Header");
            contentLayoutContainer = layoutContainer.Q<VisualElement>("ContentLayoutContainer");
            content = layoutContainer.Q<VisualElement>("ContentContainer");
            footer = layoutContainer.Q<VisualElement>("Footer");

            content.Add(animatedContainer = new FluidAnimatedContainer().SetClearOnHide(false));

            tabButton =
                FluidToggleButtonTab.Get()
                    .SetElementSize(ElementSize.Normal)
                    .SetTabPosition(TabPosition.TabOnTop)
                    .SetIcon(EditorMicroAnimations.EditorUI.Components.CarretRightToDown)
                    .SetAnimationTrigger(IconAnimationTrigger.OnValueChanged);

            tabButton.SetOnValueChanged(value => schedule.Execute(ValueChanged));

            tabButton.Q<VisualElement>(nameof(FluidToggleButtonTab.layoutContainer)).RemoveClass("ToggleButton").AddClass(nameof(FluidFoldout));
            tabButton.Q<VisualElement>(nameof(FluidToggleButtonTab.buttonContainer)).RemoveClass("ToggleButton").AddClass(nameof(FluidFoldout));
            tabButton.Q<Image>(nameof(FluidToggleButtonTab.icon)).AddClass(nameof(FluidFoldout));
            tabButton.iconReaction.SetDuration(expandReactionDuration);

            header.AddChild(tabButton);

            colorReaction =
                Reaction.Get<ColorReaction>()
                    .SetEditorHeartbeat()
                    .SetSetter(color => footer.SetStyleBackgroundColor(color))
                    .SetDuration(expandReactionDuration)
                    .SetEase(expandReactionEase);

            //RESET
            {
                ResetContentPadding();
                ResetSize();
                ResetColors();
            }
        }

        /// <summary> Add elements to this foldout's content container </summary>
        /// <param name="element"> Target element </param>
        public FluidFoldout AddContent(VisualElement element)
        {
            animatedContainer.AddContent(element);
            return this;
        }

        public FluidFoldout ClearContent()
        {
            animatedContainer.ClearContent();
            return this;
        }



        /// <summary> Add a new PropertyField to this foldout's content container and get a reference to it </summary>
        /// <param name="bindingPath"> Property field bindingPath </param>
        public PropertyField AddContentPropertyField(string bindingPath)
        {
            PropertyField field = DesignUtils.NewPropertyField(bindingPath);
            animatedContainer.AddContent(field);
            return field;
        }


        #region Value Changed -> Open / Close

        private void ValueChanged()
        {
            // Debug.Log($"{nameof(FluidFoldout)}.{nameof(ValueChanged)} - isOn:{isOn}");
            animatedContainer.Toggle(isOn);
            colorReaction.PlayToValue(isOn ? GetFooterColor() : defaultContainerColor);
        }

        public FluidFoldout Open(bool animateChange = true)
        {
            tabButton.SetIsOn(true, animateChange);
            return this;
        }

        public FluidFoldout Close(bool animateChange = true)
        {
            tabButton.SetIsOn(false, animateChange);
            return this;
        }

        #endregion

        #region Icon

        /// <summary> Set custom a custom animated icon </summary>
        /// <param name="textures"> Icon textures </param>
        public FluidFoldout SetIcon(IEnumerable<Texture2D> textures)
        {
            tabButton
                .SetIcon(textures)
                .SetAnimationTrigger(IconAnimationTrigger.OnValueChanged);
            return this;
        }

        #endregion

        #region Label

        /// <summary> Set label text </summary>
        /// <param name="labelText"> Label text </param>
        public FluidFoldout SetLabelText(string labelText)
        {
            tabButton.SetLabelText(labelText);
            return this;
        }

        /// <summary> Clear the text and tooltip values from the button's label </summary>
        public FluidFoldout ClearLabelText()
        {
            tabButton.ClearLabelText();
            return this;
        }

        #endregion

        #region Size

        /// <summary> Set the foldout side </summary>
        /// <param name="value"> New size </param>
        public FluidFoldout SetElementSize(ElementSize value)
        {
            tabButton.SetElementSize(value);
            return this;
        }

        /// <summary> Reset the foldout to its default value (small) </summary>
        public FluidFoldout ResetSize() =>
            SetElementSize(ElementSize.Small);

        #endregion

        #region Color

        public FluidFoldout SetAccentColor(EditorSelectableColorInfo value)
        {
            tabButton.SetToggleAccentColor(value);
            return this;
        }

        public FluidFoldout ResetColors()
        {
            SetContainerColor(defaultContainerColor);
            tabButton.ResetColors();
            tabButton.fluidElement.ResetAccentColor();
            return this;
        }

        public FluidFoldout SetContainerColor(Color color)
        {
            containerColor = color;
            tabButton.SetContainerColorOff(containerColor.WithAlpha(1f));
            contentLayoutContainer.SetStyleBorderColor(containerColor.WithAlpha(0.4f));
            animatedContainer.fluidContainer.SetStyleBackgroundColor(containerColor.WithAlpha(0.2f));
            footer.SetStyleBackgroundColor(containerColor.WithAlpha(1f));
            return this;
        }

        #endregion

        #region Padding

        /// <summary> Set the left padding for the foldout's content </summary>
        /// <param name="leftPadding"> Padding value for the left side </param>
        public FluidFoldout SetContentLeftPadding(int leftPadding = k_DefaultContentLeftPadding)
        {
            animatedContainer.fluidContainer.SetStylePaddingLeft(leftPadding);
            return this;
        }

        /// <summary> Reset the foldout's content left padding to its default value (16) </summary>
        public FluidFoldout ResetContentLeftPadding() =>
            SetContentLeftPadding();


        /// <summary> Set the foldout's content padding values (left, top, right, bottom) </summary>
        /// <param name="padding"> Padding value for all sides </param>
        public FluidFoldout SetContentPadding(int padding = k_DefaultContentMinimumPadding)
        {
            animatedContainer.fluidContainer.SetStylePadding(padding);
            return this;
        }

        /// <summary> Set the foldout's content padding values (left, top, right, bottom) to the minimum value (4) </summary>
        public FluidFoldout RemoveContentPadding() =>
            SetContentPadding();

        /// <summary> Set the foldout's content padding values (left, top, right, bottom) </summary>
        /// <param name="left"> Padding value the left side </param>
        /// <param name="top"> Padding value the top side </param>
        /// <param name="right">  Padding value the right side </param>
        /// <param name="bottom"> Padding value the bottom side </param>
        public FluidFoldout SetContentPadding(int left, int top, int right, int bottom)
        {
            animatedContainer.fluidContainer.SetStylePadding(left, top, right, bottom);
            return this;
        }

        /// <summary> Set the foldout's content padding values (left, top, right, bottom) </summary>
        /// <param name="edge"> Padding value for all sides </param>
        public FluidFoldout SetContentPadding(EdgeValues edge)
        {
            animatedContainer.fluidContainer.SetStylePadding(edge);
            return this;
        }

        /// <summary> Reset the foldout's content padding values (left, top, right, bottom) to their default values (16, 4, 4, 4) </summary>
        public FluidFoldout ResetContentPadding() =>
            SetContentPadding(k_DefaultContentLeftPadding, k_DefaultContentMinimumPadding, k_DefaultContentMinimumPadding, k_DefaultContentMinimumPadding);

        #endregion




    }
}
