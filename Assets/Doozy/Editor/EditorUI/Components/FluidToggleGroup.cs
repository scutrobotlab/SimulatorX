// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FluidToggleGroup : FluidToggle<FluidToggleGroup>, IToggleGroup
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


        public enum GroupValue
        {
            Off = 0,
            On = 1,
            MixedValues = 2
        }

        public GroupValue currentGroupValue { get; private set; }

        private void UpdateGroupValue(bool animateChange)
        {
            if (allTogglesAreOn)
            {
                currentGroupValue = GroupValue.On;
            }
            else if (allTogglesAreOff)
            {
                currentGroupValue = GroupValue.Off;
            }
            else
            {
                currentGroupValue = GroupValue.MixedValues;
            }

            hasMixedValues = currentGroupValue == GroupValue.MixedValues;

            bool previousValue = isOn;
            bool newValue = anyTogglesOn;
            this.SetIsOn(anyTogglesOn, animateChange);
            // isOn = anyTogglesOn;

            ValueChanged(previousValue, newValue, animateChange);
        }

        public enum ControlMode
        {
            /// <summary>
            /// Only one Toggle can be ON at any given time
            /// <para/> Allows for all Toggles to be OFF
            /// </summary>
            OneToggleOn,

            /// <summary>
            /// Only one Toggle will to be ON at any given time
            /// <para/> One Toggle will be forced ON at all times
            /// </summary>
            OneToggleOnEnforced,

            /// <summary>
            /// At least one Toggle needs to be ON at any given time
            /// <para/> Allows for multiple Toggles to be ON
            /// <para/> One Toggle will be forced ON at all times
            /// </summary>
            AnyToggleOnEnforced,

            /// <summary>
            /// Toggle values are not enforced in any way
            /// <para/> Allows for all Toggles to be OFF
            /// </summary>
            Passive,
        }

        private const ControlMode DEFAULT_CONTROL_MODE = ControlMode.OneToggleOnEnforced;
        public ControlMode currentControlMode { get; private set; }

        public FluidToggleGroup SetControlMode(ControlMode controlMode, bool animateChange = false)
        {
            currentControlMode = controlMode;
            UpdateGroupValue(animateChange);
            return this;
        }

        public readonly List<IToggle> Toggles = new List<IToggle>();

        public int numberOfToggles => Toggles?.Count ?? 0;
        public int numberOfTogglesOn => Toggles?.Count(toggle => toggle.isOn) ?? 0;
        public int numberOfTogglesOff => Toggles?.Count(toggle => !toggle.isOn) ?? 0;
        public bool anyTogglesOn => Toggles?.Any(toggle => toggle.isOn) ?? false;
        public bool anyTogglesOff => Toggles?.Any(toggle => !toggle.isOn) ?? false;
        public bool allTogglesAreOn => Toggles?.All(toggle => toggle.isOn) ?? false;
        public bool allTogglesAreOff => Toggles?.All(toggle => !toggle.isOn) ?? false;

        public List<IToggle> togglesOn => Toggles?.Where(toggle => toggle.isOn).ToList();
        public IToggle firstToggleOn => Toggles?.First(toggle => toggle.isOn);
        public int firstToggleOnIndex
        {
            get
            {
                if (Toggles == null) return -1;
                for (int index = 0; index < Toggles.Count; index++)
                {
                    IToggle toggle = Toggles[index];
                    if (toggle.isOn)
                        return index;
                }
                return -1;
            }
        }
        public IToggle lastToggleOn => Toggles?.Last(toggle => toggle.isOn);
        public int lastToggleOnIndex
        {
            get
            {
                if (Toggles == null) return -1;
                for (int index = Toggles.Count - 1; index >= 0; index--)
                {
                    IToggle toggle = Toggles[index];
                    if (toggle.isOn)
                        return index;
                }
                return -1;
            }
        }

        public List<IToggle> togglesOff => Toggles?.Where(toggle => !toggle.isOn).ToList();
        public IToggle firstToggleOff => Toggles?.First(toggle => !toggle.isOn);
        public int firstToggleOffIndex
        {
            get
            {
                if (Toggles == null) return -1;
                for (int index = 0; index < Toggles.Count; index++)
                {
                    IToggle toggle = Toggles[index];
                    if (!toggle.isOn)
                        return index;
                }
                return -1;
            }
        }
        public IToggle lastToggleOff => Toggles?.Last(toggle => !toggle.isOn);
        public int lastToggleOffIndex
        {
            get
            {
                if (Toggles == null) return -1;
                for (int index = Toggles.Count - 1; index >= 0; index--)
                {
                    IToggle toggle = Toggles[index];
                    if (!toggle.isOn)
                        return index;
                }
                return -1;
            }
        }


        public static FluidToggleGroup Get(string labelText, bool value, EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get().SetLabelText(labelText).SetToggleAccentColor(accentColor).SetIsOn(value).SetTooltip(tooltip);

        public static FluidToggleGroup Get(string labelText) =>
            Get(labelText, false, null, string.Empty);

        public static FluidToggleGroup Get(bool value, EditorSelectableColorInfo accentColor = null, string tooltip = "") =>
            Get(string.Empty, value, accentColor, tooltip);

        public static FluidToggleGroup Get(EditorSelectableColorInfo accentColor, string tooltip = "") =>
            Get(string.Empty, false, accentColor, tooltip);

        public FluidToggleGroup()
        {
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

            const string componentClassName = "ToggleCheckbox";
            iconContainer.AddClass(componentClassName);
            icon.AddClass(componentClassName);

            iconReaction
                .SetTextures(EditorMicroAnimations.EditorUI.Components.Checkmark)
                .SetDuration(0.15f);

            selectionState = SelectionState.Normal;

            currentControlMode = DEFAULT_CONTROL_MODE;
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

        public FluidToggleGroup SetLabelType(ToggleLabelType toggleLabelType)
        {
            labelType = toggleLabelType;
            return this;
        }

        public FluidToggleGroup SetLabelText(string text, ToggleLabelType toggleLabelType = ToggleLabelType.RightLabel)
        {
            leftLabel.text = text;
            rightLabel.text = text;
            labelType = text.IsNullOrEmpty() ? ToggleLabelType.NoLabel : toggleLabelType;
            return this;
        }

        public void RegisterToggle(IToggle toggle)
        {
            if (toggle == null) return;
            if (toggle == this) return;
            if (Toggles.Contains(toggle)) return;
            Toggles.Add(toggle);
            toggle.toggleGroup = this;
            ToggleChangedValue(toggle);
            UpdateGroupValue(true);
        }

        public void UnregisterToggle(IToggle toggle)
        {
            if (toggle == null) return;
            if (!Toggles.Contains(toggle)) return;
            Toggles.Remove(toggle);
            toggle.toggleGroup = null;
            UpdateGroupValue(true);
        }

        public void ToggleChangedValue(IToggle toggle, bool animateChange = false)
        {
            if (toggle == null) return;
            if (!Toggles.Contains(toggle))
            {
                toggle.RemoveFromToggleGroup();
                return;
            }

            switch (currentControlMode)
            {
                case ControlMode.OneToggleOn:
                    if (toggle.isOn && numberOfTogglesOn > 1)
                        foreach (IToggle fluidToggle in Toggles.Where(t => t != toggle).Where(t => t.isOn))
                            fluidToggle.UpdateValueFromGroup(false, animateChange);

                    break;
                case ControlMode.OneToggleOnEnforced:
                    switch (toggle.isOn)
                    {
                        case true when numberOfTogglesOn > 1:
                        {
                            foreach (IToggle fluidToggle in Toggles.Where(t => t != toggle).Where(t => t.isOn))
                                fluidToggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        }
                        case false when allTogglesAreOff:
                            toggle.UpdateValueFromGroup(true, animateChange);
                            break;
                    }
                    break;
                case ControlMode.AnyToggleOnEnforced:
                    if (toggle.isOn == false && allTogglesAreOff)
                        toggle.UpdateValueFromGroup(true, animateChange);
                    break;
                case ControlMode.Passive:
                    //ignored
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            toggle.UpdateValueFromGroup(toggle.isOn, animateChange);

            UpdateGroupValue(animateChange);
        }

        protected override void ExecuteOnClick(EventBase clickEvent)
        {
            if (selectionState == SelectionState.Disabled) return;
            if (numberOfToggles == 0) return;

            const bool animateChange = true;

            switch (currentControlMode)
            {
                case ControlMode.OneToggleOn:
                    switch (currentGroupValue)
                    {
                        case GroupValue.Off:
                            Toggles[0].UpdateValueFromGroup(true, animateChange);
                            break;
                        case GroupValue.On:
                        case GroupValue.MixedValues:
                            foreach (IToggle toggle in Toggles)
                                toggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ControlMode.OneToggleOnEnforced:
                    break;
                case ControlMode.AnyToggleOnEnforced:
                    switch (currentGroupValue)
                    {
                        case GroupValue.On:
                            IToggle firstToggle = Toggles[0];
                            firstToggle.UpdateValueFromGroup(true, animateChange);
                            foreach (IToggle toggle in Toggles.Where(item => item != firstToggle))
                                toggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        case GroupValue.Off:
                        case GroupValue.MixedValues:
                            foreach (IToggle toggle in Toggles)
                                toggle.UpdateValueFromGroup(true, animateChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ControlMode.Passive:
                    switch (currentGroupValue)
                    {
                        case GroupValue.Off:
                        case GroupValue.MixedValues:
                            foreach (IToggle toggle in Toggles)
                                toggle.UpdateValueFromGroup(true, animateChange);
                            break;
                        case GroupValue.On:
                            foreach (IToggle toggle in Toggles)
                                toggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            schedule.Execute(() => UpdateGroupValue(animateChange));

            OnClick?.Invoke();
        }

        public override void UpdateValueFromGroup(bool newValue, bool animateChange, bool silent = false)
        {
            switch (currentControlMode)
            {
                case ControlMode.OneToggleOn:
                    if (newValue)
                    {
                        foreach (IToggle toggle in Toggles)
                            toggle.UpdateValueFromGroup(false, animateChange);

                        break;
                    }

                    Toggles[0].UpdateValueFromGroup(true, animateChange);
                    break;
                case ControlMode.OneToggleOnEnforced:
                    break;
                case ControlMode.AnyToggleOnEnforced:
                    if (newValue)
                    {
                        foreach (IToggle toggle in Toggles)
                            toggle.UpdateValueFromGroup(true, animateChange);

                        break;
                    }

                    IToggle firstToggle = Toggles[0];
                    firstToggle.UpdateValueFromGroup(true, animateChange);
                    foreach (IToggle toggle in Toggles.Where(item => item != firstToggle))
                        toggle.UpdateValueFromGroup(false, animateChange);
                    break;
                case ControlMode.Passive:
                    if (newValue)
                    {
                        foreach (IToggle toggle in Toggles)
                            toggle.UpdateValueFromGroup(true, animateChange);

                        break;
                    }

                    foreach (IToggle toggle in Toggles)
                        toggle.UpdateValueFromGroup(false, animateChange);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            schedule.Execute(() => UpdateGroupValue(animateChange));
        }
    }
}
