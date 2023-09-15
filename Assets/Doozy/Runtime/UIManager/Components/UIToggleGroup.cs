// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Mody;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Toggle component that can control other UIToggles, based on UIToggle. 
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Components/UI Toggle Group")]
    [SelectionBase]
    public class UIToggleGroup : UIToggle
    {
        #region Enums

        /// <summary> Defines all the values a Toggle Group can have </summary>
        public enum Value
        {
            /// <summary> All of the toggle group's toggles are OFF </summary>
            Off = 0,

            /// <summary> All of the toggle group's toggles are ON </summary>
            On = 1,

            /// <summary> The toggle group contains at least one toggle ON and one toggle OFF </summary>
            MixedValues = 2
        }

        /// <summary> Defines how a Toggle Group controls its toggles </summary>
        public enum ControlMode
        {
            /// <summary>
            /// Toggle values are not enforced in any way
            /// <para/> Allows for all toggles to be OFF
            /// </summary>
            Passive = 0,

            /// <summary>
            /// Only one Toggle can be ON at any given time
            /// <para/> Allows for all toggles to be OFF
            /// </summary>
            OneToggleOn = 1,

            /// <summary>
            /// Only one Toggle will to be ON at any given time
            /// <para/> One Toggle will be forced ON at all times
            /// </summary>
            OneToggleOnEnforced = 2,

            /// <summary>
            /// At least one Toggle needs to be ON at any given time
            /// <para/> Allows for multiple toggles to be ON
            /// <para/> One Toggle will be forced ON at all times
            /// </summary>
            AnyToggleOnEnforced = 3,
        }

        /// <summary> Defines the types of (auto) sorting available for toggle groups </summary>
        public enum SortMode
        {
            /// <summary> Auto sort is disabled </summary>
            Disabled = 0,

            /// <summary> Auto sort by sibling index (the order toggles appear in the Hierarchy) </summary>
            Hierarchy = 1,

            /// <summary> Auto sort by Toggle's GameObject name </summary>
            GameObjectName = 2,

            /// <summary> Auto sort by Toggle Id Name (ignores category) </summary>
            ToggleName = 3
        }

        #endregion

        [SerializeField] private bool OverrideInteractabilityForToggles;
        /// <summary> Override and control the interactable state for all the connected UIToggles </summary>
        public bool overrideInteractabilityForToggles
        {
            get => OverrideInteractabilityForToggles;
            set => OverrideInteractabilityForToggles = value;
        }

        [SerializeField] private Value ToggleGroupValue;
        /// <summary>
        /// Toggle group value
        /// <para/> Off - all of the toggle group's toggles are OFF
        /// <para/> On - all of the toggle group's toggles are ON
        /// <para/> Mixed Values - the toggle group contains at least one toggle ON and one toggle OFF
        /// </summary>
        public Value toggleGroupValue
        {
            get => ToggleGroupValue;
            private set => ToggleGroupValue = value;
        }

        [SerializeField] private ControlMode Mode;
        /// <summary>
        /// Toggle group's control mode for its toggles
        /// <para/> Passive - toggle values are not enforced in any way (allows for all toggles to be OFF)
        /// <para/> OneToggleOn - only one Toggle can be ON at any given time (allows for all toggles to be OFF)
        /// <para/> OneToggleOnEnforced - only one Toggle can be ON at any given time (one Toggle will be forced ON at all times)
        /// <para/> AnyToggleOnEnforced - at least one Toggle needs to be ON at any given time (one Toggle will be forced ON at all times)
        /// </summary>
        public ControlMode mode
        {
            get => Mode;
            set
            {
                Mode = value;
                UpdateGroupValue(false);
            }
        }

        [SerializeField] private bool HasMixedValues;
        /// <summary> Marks a toggle group as having toggles of different values </summary>
        public bool hasMixedValues
        {
            get => HasMixedValues;
            private set
            {
                if (HasMixedValues == value) return;
                HasMixedValues = value;
                if (HasMixedValues) OnToggleGroupMixedValuesCallback?.Execute();
                // ValueChanged(previousValue: isOn, newValue: isOn, animateChange: true);
            }
        }

        [SerializeField] private SortMode AutoSort = SortMode.Hierarchy;
        /// <summary> Sort mode used by the AutoSortToggles method
        /// <para/> Disabled - auto sort is disabled
        /// <para/> Hierarchy - auto sort by sibling index (the order toggles appear in the Hierarchy)
        /// <para/> GameObjectName - auto sort by Toggle's GameObject name
        /// <para/> ToggleName - auto sort by Toggle's Id Name (ignores category)
        /// </summary>
        public SortMode autoSort
        {
            get => AutoSort;
            set
            {
                AutoSort = value;
                SortToggles(value);
            }
        }

        /// <summary> Toggle group has mixed values - executed when hasMixedValues becomes TRUE </summary>
        public ModyEvent OnToggleGroupMixedValuesCallback;

        /// <summary> The first toggle that will be automatically turned ON OnEnable </summary>
        public UIToggle FirstToggle;

        /// <summary> List of all the toggles controlled by this toggle group </summary>
        public List<UIToggle> toggles { get; private set; } = new List<UIToggle>();

        /// <summary> Number of toggles controlled by this toggle group </summary>
        public int numberOfToggles => toggles?.Count ?? 0;

        /// <summary> Number of toggles that are ON </summary>
        public int numberOfTogglesOn => toggles?.Count(toggle => toggle.isOn) ?? 0;

        /// <summary> Number of toggles that are OFF </summary>
        public int numberOfTogglesOff => toggles?.Count(toggle => !toggle.isOn) ?? 0;

        /// <summary> Returns TRUE if at least one toggle is ON </summary>
        public bool anyOfTogglesOn => toggles?.Any(toggle => toggle.isOn) ?? false;

        /// <summary> Returns TRUE if at least one toggle is OFF </summary>
        public bool anyOfTogglesOff => toggles?.Any(toggle => !toggle.isOn) ?? false;

        /// <summary> Returns TRUE if all toggle are ON </summary>
        public bool allTogglesAreOn => toggles?.All(toggle => toggle.isOn) ?? false;

        /// <summary> Returns TRUE if all toggle are OFF </summary>
        public bool allTogglesAreOff => toggles?.All(toggle => !toggle.isOn) ?? false;

        /// <summary> Get all the toggles that are ON </summary>
        public IEnumerable<UIToggle> togglesOn => toggles?.Where(toggle => toggle.isOn);

        /// <summary> Get all the toggles that are OFF </summary>
        public IEnumerable<UIToggle> togglesOff => toggles?.Where(toggle => !toggle.isOn);

        /// <summary> Get the first toggle that is ON (returns null if no toggles are ON) </summary>
        public UIToggle firstToggleOn => toggles?.First(toggle => toggle.isOn);

        /// <summary> Get the first toggle that is OFF (returns null if no toggles are OFF) </summary>
        public UIToggle firstToggleOff => toggles?.First(toggle => !toggle.isOn);

        /// <summary> Get the last toggle that is ON (returns null if no toggles are ON) </summary>
        public UIToggle lastToggleOn => toggles?.Last(toggle => toggle.isOn);

        /// <summary> Get the last toggle that is OFF (returns null if no toggles are OFF) </summary>
        public UIToggle lastToggleOff => toggles?.Last(toggle => !toggle.isOn);

        /// <summary> Get the index for the first toggle that is ON (returns -1 if no toggles are ON) </summary>
        public int firstToggleOnIndex
        {
            get
            {
                CleanToggles();
                UIToggle firstOn = firstToggleOn;
                return firstOn == null ? -1 : toggles.IndexOf(firstOn);
            }
        }

        /// <summary> Get the index for the first toggle that is OFF (returns -1 if no toggles are OFF) </summary>
        public int firstToggleOffIndex
        {
            get
            {
                CleanToggles();
                UIToggle firstOff = firstToggleOff;
                return firstOff == null ? -1 : toggles.IndexOf(firstOff);
            }
        }

        /// <summary> Get the index for the last toggle that is ON (returns -1 if no toggles are ON) </summary>
        public int lastToggleOnIndex
        {
            get
            {
                CleanToggles();
                UIToggle lastOn = lastToggleOn;
                return lastOn == null ? -1 : toggles.IndexOf(lastOn);
            }
        }

        /// <summary> Get the index for the last toggle that is OFF (returns -1 if no toggles are OFF) </summary>
        public int lastToggleOffIndex
        {
            get
            {
                CleanToggles();
                UIToggle lastOff = lastToggleOff;
                return lastOff == null ? -1 : toggles.IndexOf(lastOff);
            }
        }

        private bool toggleGroupInitialized { get; set; }

        protected UIToggleGroup()
        {
            OnToggleGroupMixedValuesCallback = new ModyEvent(nameof(OnToggleGroupMixedValuesCallback));
        }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            toggleGroupInitialized = false;
            base.Awake();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            base.OnEnable();
            StartCoroutine(RefreshAllTogglesWithDelay());
        }

        private IEnumerator RefreshAllTogglesWithDelay()
        {
            yield return null;
            RefreshAllToggleValues();
            toggleGroupInitialized = true;
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying) return;
            base.OnDisable();
            toggleGroupInitialized = false;
        }

        private void LateUpdate()
        {
            if (!toggleGroupInitialized) return;
            if (!overrideInteractabilityForToggles) return;
            foreach (UIToggle toggle in toggles)
                toggle.interactable = interactable;
        }

        protected override void InitializeToggle()
        {
            if (toggleInitialized) return;
            AddToToggleGroup(toggleGroup);
            toggleInitialized = true;
        }

        /// <summary> Clean the toggles list by removing any null references and duplicates </summary>
        public UIToggleGroup CleanToggles()
        {
            toggles =
                toggles
                    .Where(toggle => toggle != null)
                    .Distinct()
                    .ToList();

            return this;
        }

        /// <summary> Automatically sort the toggles by sortMode </summary>
        public void AutoSortToggles() =>
            SortToggles(autoSort);

        /// <summary> Sort the toggles by the given sort mode </summary>
        /// <param name="toggleSortMode"> Toggle sort mode </param>
        public void SortToggles(SortMode toggleSortMode)
        {
            CleanToggles();

            switch (toggleSortMode)
            {
                case SortMode.Disabled:
                    return;
                case SortMode.Hierarchy:
                    toggles = toggles.OrderBy(t => t.rectTransform.GetSiblingIndex()).ToList();
                    break;
                case SortMode.GameObjectName:
                    toggles = toggles.OrderBy(t => t.gameObject.name).ToList();
                    break;
                case SortMode.ToggleName:
                    toggles = toggles.OrderBy(t => t.Id.Name).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(toggleSortMode), toggleSortMode, null);
            }
        }

        /// <summary> Add a toggle to this toggle group </summary>
        /// <param name="toggle"> Target toggle </param>
        public void AddToggle(UIToggle toggle)
        {
            if (toggle == null) return;
            if (toggle == this) return;
            if (toggles.Contains(toggle)) return;
            toggles.Add(toggle);
            toggle.toggleGroup = this;
            if (!toggleGroupInitialized) return;
            AutoSortToggles();
            UpdateGroupValue(true);
        }

        /// <summary> Remove a toggle from this toggle group </summary>
        /// <param name="toggle"> Target toggle </param>
        public void RemoveToggle(UIToggle toggle)
        {
            CleanToggles();
            if (toggle == null) return;
            if (!toggles.Contains(toggle)) return;
            toggles.Remove(toggle);
            toggle.toggleGroup = null;
            UpdateGroupValue(true);
        }

        /// <summary> Notify this toggle group that the given toggle has changed its value </summary>
        /// <param name="toggle"> Toggle that changed its value </param>
        /// <param name="animateChange"> Should the change be animated </param>
        public void ToggleChangedValue(UIToggle toggle, bool animateChange = false)
        {
            if (toggle == null) return;
            if (!toggles.Contains(toggle))
            {
                toggle.RemoveFromToggleGroup();
                return;
            }

            switch (mode)
            {
                case ControlMode.Passive:
                {
                    toggle.UpdateValueFromGroup(toggle.isOn, animateChange);
                    break;
                }
                case ControlMode.OneToggleOn:
                {
                    if (toggle.isOn && numberOfTogglesOn > 1)
                    {
                        foreach (UIToggle t in toggles.Where(t => t != toggle).Where(t => t.isOn))
                        {
                            t.UpdateValueFromGroup(newValue: false, animateChange);
                        }
                    }

                    toggle.UpdateValueFromGroup(toggle.isOn, animateChange);
                    break;
                }
                case ControlMode.OneToggleOnEnforced:
                {
                    if (allTogglesAreOff)
                    {
                        toggle.UpdateValueFromGroup(true, animateChange);
                    }
                    else if (toggle.isOn & numberOfTogglesOn > 1)
                    {
                        foreach (UIToggle t in toggles.Where(t => t != toggle).Where(t => t.isOn))
                        {
                            t.UpdateValueFromGroup(newValue: false, animateChange);
                        }
                        toggle.UpdateValueFromGroup(true, animateChange);
                    }
                    else
                    {
                        toggle.UpdateValueFromGroup(toggle.isOn, animateChange);
                    }
                    break;
                }
                case ControlMode.AnyToggleOnEnforced:
                {
                    if (!toggle.isOn & allTogglesAreOff)
                    {
                        toggle.UpdateValueFromGroup(true, animateChange);
                    }
                    else
                    {
                        toggle.UpdateValueFromGroup(toggle.isOn, animateChange);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }


            UpdateGroupValue(animateChange);
        }

        protected internal override void UpdateValueFromGroup(bool newValue, bool animateChange)
        {
            switch (mode)
            {
                case ControlMode.Passive:
                    if (newValue)
                    {
                        foreach (UIToggle toggle in toggles)
                            toggle.UpdateValueFromGroup(true, animateChange);

                        break;
                    }

                    foreach (UIToggle toggle in toggles)
                        toggle.UpdateValueFromGroup(false, animateChange);

                    break;
                case ControlMode.OneToggleOn:
                    if (newValue)
                    {
                        foreach (UIToggle toggle in toggles)
                            toggle.UpdateValueFromGroup(false, animateChange);

                        break;
                    }

                    toggles[0].UpdateValueFromGroup(true, animateChange);
                    break;
                case ControlMode.OneToggleOnEnforced:
                    break;
                case ControlMode.AnyToggleOnEnforced:
                    if (newValue)
                    {
                        foreach (UIToggle toggle in toggles)
                            toggle.UpdateValueFromGroup(true, animateChange);

                        break;
                    }

                    UIToggle firstToggle = toggles[0];
                    firstToggle.UpdateValueFromGroup(true, animateChange);
                    foreach (UIToggle toggle in toggles.Where(t => t != firstToggle))
                        toggle.UpdateValueFromGroup(false, animateChange);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateGroupValue(animateChange);
        }

        public void RefreshAllToggleValues(bool animateChange = true)
        {
            AutoSortToggles();

            if (toggles.Count == 0)
                return;

            bool setFirstToggleOn;

            switch (mode)
            {
                case ControlMode.Passive:
                    setFirstToggleOn = false;
                    foreach (UIToggle t in toggles)
                        t.UpdateValueFromGroup(t.isOn, false);
                    break;
                case ControlMode.OneToggleOn:
                    setFirstToggleOn = numberOfTogglesOn > 1;
                    if (!setFirstToggleOn)
                    {
                        foreach (UIToggle t in toggles)
                            t.UpdateValueFromGroup(t.isOn, false);
                    }

                    break;
                case ControlMode.OneToggleOnEnforced:
                    setFirstToggleOn = numberOfTogglesOn == 0 || numberOfTogglesOn > 1;
                    if (!setFirstToggleOn)
                    {
                        GetFirstToggle()?.UpdateValueFromGroup(true, false);
                    }
                    break;
                case ControlMode.AnyToggleOnEnforced:
                    setFirstToggleOn = numberOfTogglesOn != 1;
                    if (!setFirstToggleOn)
                    {
                        foreach (UIToggle t in toggles.Where(t => t.isOn))
                            t.UpdateValueFromGroup(true, false);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (setFirstToggleOn)
            {
                foreach (UIToggle t in toggles.Where(t => t != GetFirstToggle()))
                    t.UpdateValueFromGroup(false, animateChange);

                GetFirstToggle()?.UpdateValueFromGroup(true, animateChange);
            }

            UpdateGroupValue(animateChange);
        }

        public UIToggle GetFirstToggle() =>
            FirstToggle != null && toggles.Contains(FirstToggle)
                ? FirstToggle
                : toggles.Count == 0
                    ? null
                    : toggles[0];

        private void SetAllTogglesOff(bool animateChange = false)
        {
            foreach (UIToggle t in toggles)
                t.UpdateValueFromGroup(false, animateChange);
        }

        protected override void ToggleValue()
        {
            if (!IsActive() || !IsInteractable()) return;

            const bool animateChange = true;

            switch (mode)
            {
                case ControlMode.Passive:
                    switch (toggleGroupValue)
                    {
                        case Value.Off:
                        case Value.MixedValues:
                            foreach (UIToggle toggle in toggles)
                                toggle.UpdateValueFromGroup(true, animateChange);
                            break;
                        case Value.On:
                            foreach (UIToggle toggle in toggles)
                                toggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ControlMode.OneToggleOn:
                    switch (toggleGroupValue)
                    {
                        case Value.Off:
                            toggles[0].UpdateValueFromGroup(true, animateChange);
                            break;
                        case Value.On:
                        case Value.MixedValues:
                            foreach (UIToggle toggle in toggles)
                                toggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ControlMode.OneToggleOnEnforced:
                    break;
                case ControlMode.AnyToggleOnEnforced:
                    switch (toggleGroupValue)
                    {
                        case Value.On:
                            UIToggle firstToggle = toggles[0];
                            firstToggle.UpdateValueFromGroup(true, animateChange);
                            foreach (UIToggle toggle in toggles.Where(item => item != firstToggle))
                                toggle.UpdateValueFromGroup(false, animateChange);
                            break;
                        case Value.Off:
                        case Value.MixedValues:
                            foreach (UIToggle toggle in toggles)
                                toggle.UpdateValueFromGroup(true, animateChange);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateGroupValue(animateChange);

            behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
        }

        public void UpdateGroupValue(bool animateChange)
        {
            if (allTogglesAreOn)
            {
                toggleGroupValue = Value.On;
            }
            else if (allTogglesAreOff)
            {
                toggleGroupValue = Value.Off;
            }
            else
            {
                toggleGroupValue = Value.MixedValues;
            }

            hasMixedValues = toggleGroupValue == Value.MixedValues;
            bool previousValue = isOn;
            bool newValue = anyOfTogglesOn;
            this.SetIsOn(anyOfTogglesOn, animateChange);
            ValueChanged(previousValue, newValue, animateChange, true);
        }
    }
}
