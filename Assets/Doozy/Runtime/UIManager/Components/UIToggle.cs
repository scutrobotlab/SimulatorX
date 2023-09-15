// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components.Internal;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Toggle component based on UISelectable with category/name id identifier.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Components/UI Toggle")]
    [SelectionBase]
    public partial class UIToggle : UISelectableComponent<UIToggle>, IPointerClickHandler, ISubmitHandler
    {
        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> Signal stream for this component type </summary>
        public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream(k_StreamCategory, nameof(UIToggle)));

        /// <summary> All buttons that are active and enabled </summary>
        public static IEnumerable<UIToggle> availableToggles => database.Where(item => item.isActiveAndEnabled);

        public override SelectableType selectableType => SelectableType.Toggle;

        /// <summary> Category Name Id </summary>
        public UIToggleId Id = new UIToggleId();

        /// <summary> Toggle became ON - executed when isOn becomes TRUE </summary>
        public ModyEvent OnToggleOnCallback = new ModyEvent(nameof(OnToggleOnCallback));

        // <summary> Toggle became ON with instant animations - executed when isOn becomes TRUE </summary>
        public ModyEvent OnInstantToggleOnCallback = new ModyEvent(nameof(OnInstantToggleOnCallback));

        /// <summary> Toggle became OFF - executed when isOn becomes FALSE </summary>
        public ModyEvent OnToggleOffCallback = new ModyEvent(nameof(OnToggleOffCallback));

        /// <summary> Toggle became OFF with instant animations - executed when isOn becomes FALSE </summary>
        public ModyEvent OnInstantToggleOffCallback = new ModyEvent(nameof(OnInstantToggleOffCallback));

        /// <summary> Toggle changed its value - executed when isOn changes its value </summary>
        public BoolEvent OnValueChangedCallback = new BoolEvent();

        /// <summary> Toggle value changed callback. This special callback also sends when the event happened, the previousValue and the newValue </summary>
        public UnityAction<ToggleValueChangedEvent> onToggleValueChangedCallback { get; set; }

        /// <summary> Returns TRUE if this toggle has a toggle group reference </summary>
        public bool inToggleGroup => ToggleGroup != null && ToggleGroup.toggles.Contains(this);

        [SerializeField] private UIToggleGroup ToggleGroup;
        /// <summary> Reference to the toggle group that this toggle belongs to </summary>
        public UIToggleGroup toggleGroup
        {
            get => ToggleGroup;
            internal set => ToggleGroup = value;
        }

        public override bool isOn
        {
            get => IsOn;
            set
            {
                bool previousValue = IsOn;
                IsOn = value;

                if (inToggleGroup)
                {
                    toggleGroup.ToggleChangedValue(toggle: this, animateChange: true);
                    return;
                }

                ValueChanged(previousValue: previousValue, newValue: value, animateChange: true, triggerValueChanged: true);
            }
        }

        protected bool toggleInitialized { get; set; }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            toggleInitialized = false;
            base.Awake();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            base.OnEnable();
            if (!toggleInitialized) return;
            AddToToggleGroup(toggleGroup);
            if (inToggleGroup) return;
            ValueChanged(isOn, isOn, false, true);
        }

        protected override void Start()
        {
            if (!Application.isPlaying) return;
            base.Start();
            InitializeToggle();
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying) return;
            base.OnDisable();
            behaviours?.Disconnect();
        }

        protected virtual void InitializeToggle()
        {
            if (toggleInitialized) return;
            AddToToggleGroup(toggleGroup);
            if (!inToggleGroup) ValueChanged(isOn, isOn, false, true);
            toggleInitialized = true;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive() || !IsInteractable())
                return;
            
            ToggleValue();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            ToggleValue();
            
            if (!inputSettings.submitTriggersPointerClick) return;
            behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
            behaviours.GetBehaviour(UIBehaviour.Name.PointerLeftClick)?.Execute();
        }

        protected virtual void ToggleValue()
        {
            isOn = !isOn;
            stream.SendSignal(new UIToggleSignalData(Id.Category, Id.Name, isOn ? CommandToggle.On : CommandToggle.Off, playerIndex, this));
        }

        public void AddToToggleGroup(UIToggleGroup targetToggleGroup)
        {
            if (targetToggleGroup == null)
                return;

            if (inToggleGroup && targetToggleGroup != toggleGroup)
                RemoveFromToggleGroup();

            targetToggleGroup.AddToggle(this);
        }

        public void RemoveFromToggleGroup()
        {
            if (toggleGroup == null)
                return;

            toggleGroup.RemoveToggle(this);
        }

        protected internal virtual void UpdateValueFromGroup(bool newValue, bool animateChange)
        {
            bool previousValue = IsOn;
            IsOn = newValue;
            ValueChanged(previousValue, newValue, animateChange, true);
        }

        internal void ValueChanged(bool previousValue, bool newValue, bool animateChange, bool triggerValueChanged)
        {
            RefreshState();

            switch (newValue)
            {
                case true:
                    if (animateChange)
                    {
                        OnToggleOnCallback?.Execute();
                    }
                    else
                    {
                        OnInstantToggleOnCallback?.Execute();
                    }
                    break;

                case false:
                    if (animateChange)
                    {
                        OnToggleOffCallback?.Execute();
                    }
                    else
                    {
                        OnInstantToggleOffCallback?.Execute();
                    }
                    break;
            }

            if (!triggerValueChanged)
                return;

            OnValueChangedCallback?.Invoke(newValue);
            onToggleValueChangedCallback?.Invoke(new ToggleValueChangedEvent(previousValue, newValue, animateChange));
        }

        #region Static Methods

        /// <summary> Get all the registered toggles with the given category and name </summary>
        /// <param name="category"> UIToggle category </param>
        /// <param name="name"> UIToggle name (from the given category) </param>
        public static IEnumerable<UIToggle> GetToggles(string category, string name) =>
            database.Where(toggle => toggle.Id.Category.Equals(category)).Where(toggle => toggle.Id.Name.Equals(name));

        /// <summary> Get all the registered toggles with the given category </summary>
        /// <param name="category"> UIToggle category </param>
        public static IEnumerable<UIToggle> GetAllTogglesInCategory(string category) =>
            database.Where(toggle => toggle.Id.Category.Equals(category));

        /// <summary> Get all the toggles that are active and enabled (all the visible/available toggles) </summary>
        public static IEnumerable<UIToggle> GetAvailableToggles() =>
            database.Where(toggle => toggle.isActiveAndEnabled);

        /// <summary> Get the selected toggle (if a toggle is not selected, this method returns null) </summary>
        public static UIToggle GetSelectedToggle() =>
            database.FirstOrDefault(toggle => toggle.isSelected);

        /// <summary> Select the toggle with the given category and name (if it is active and enabled) </summary>
        /// <param name="category"> UIToggle category </param>
        /// <param name="name"> UIToggle name (from the given category) </param>
        public static bool SelectToggle(string category, string name)
        {
            UIToggle toggle = availableToggles.FirstOrDefault(b => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
            if (toggle == null) return false;
            toggle.Select();
            return true;
        }

        #endregion
    }

    public static class UIToggleExtensions
    {
        public static T SetIsOn<T>(this T target, bool newValue, bool animateChange = true) where T : UIToggle
        {
            bool previousValue = target.isOn;
            target.IsOn = newValue;
            if (target.inToggleGroup)
            {
                target.toggleGroup.ToggleChangedValue(target, animateChange);
                return target;
            }
            target.ValueChanged(previousValue, newValue, animateChange, true);
            return target;
        }
    }
}
