// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidToggleRadio : FluidToggle<FluidToggleRadio>
    {
        public override void Reset()
        {
            base.Reset();
            
            SetLabelText(string.Empty);
        }

        #region LabelType

        private ToggleLabelType m_LabelType;
        public ToggleLabelType labelType
        {
            get => m_LabelType;
            set
            {
                m_LabelType = value;
                leftLabel.SetStyleDisplay(labelType == ToggleLabelType.LeftLabel ? DisplayStyle.Flex : DisplayStyle.None);
                rightLabel.SetStyleDisplay(labelType == ToggleLabelType.RightLabel ? DisplayStyle.Flex : DisplayStyle.None);
            }
        }

        #endregion

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public Image iconContainer { get; }
        public Label leftLabel { get; }
        public Label rightLabel { get; }
    
        public static FluidToggleRadio Get(string labelText, bool value, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetLabelText(labelText).SetToggleAccentColor(accentColor).SetIsOn(value).SetTooltip(tooltip);

        public static FluidToggleRadio Get(string labelText) =>
            Get(labelText, false, null, string.Empty);

        public static FluidToggleRadio Get(bool value, EditorSelectableColorInfo accentColor = null, string tooltip = "") =>
            Get(string.Empty, value, accentColor, tooltip);

        public static FluidToggleRadio Get(EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get(string.Empty, false, accentColor, tooltip);
        
        public FluidToggleRadio()
        {
            this.SetStyleFlexShrink(0);
            
            Add(templateContainer = EditorLayouts.EditorUI.FluidToggle.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.EditorUI.LayoutContainer)
                .AddStyle(EditorStyles.EditorUI.FieldName)
                .AddStyle(EditorStyles.EditorUI.FluidToggle);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            iconContainer = layoutContainer.Q<Image>(nameof(iconContainer));
            icon = iconContainer.Q<Image>(nameof(icon));
            leftLabel = layoutContainer.Q<Label>(nameof(leftLabel));
            rightLabel = layoutContainer.Q<Label>(nameof(rightLabel));
        
            fluidElement.OnStateChanged = StateChanged;
        
            const string componentClassName = "ToggleRadio";
            iconContainer.AddClass(componentClassName);
            icon.AddClass(componentClassName);

            layoutContainer.SetStyleDisplay(DisplayStyle.None);

            iconReaction
                .SetTextures(EditorMicroAnimations.EditorUI.Components.RadioCircle)
                .SetDuration(0.15f);
            
            selectionState = SelectionState.Normal;
        }
        
        protected override void StateChanged()
        {
            base.StateChanged();
            icon.SetStyleBackgroundImageTintColor(fluidElement.iconColor); //ICON COLOR
            iconContainer.SetStyleBackgroundColor(fluidElement.containerColor); //ICON CONTAINER COLOR
            iconContainer.SetStyleBorderColor(iconContainerBorderColor); //ICON CONTAINER BORDER COLOR
            leftLabel.SetStyleColor(fluidElement.textColor); //TEXT COLOR
            rightLabel.SetStyleColor(fluidElement.textColor); //TEXT COLOR
            
            MarkDirtyRepaint();
        }
        
        public FluidToggleRadio SetLabelType(ToggleLabelType toggleLabelType)
        {
            labelType = toggleLabelType;
            return this;
        }

        public FluidToggleRadio SetLabelText(string text, ToggleLabelType toggleLabelType = ToggleLabelType.RightLabel)
        {
            leftLabel.text = text;
            rightLabel.text = text;
            labelType = text.IsNullOrEmpty() ? ToggleLabelType.NoLabel : toggleLabelType;
            return this;
        }
    }
}
