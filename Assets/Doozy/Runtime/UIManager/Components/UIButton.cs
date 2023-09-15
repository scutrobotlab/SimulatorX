// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Button component based on UISelectable with category/name id identifier.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Doozy/UI/Components/UI Button")]
    [SelectionBase]
    public partial class UIButton : UISelectableComponent<UIButton>, IPointerClickHandler, ISubmitHandler
    {
        [ClearOnReload]
        private static SignalStream s_stream;
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(k_StreamCategory, nameof(UIButton));

        /// <summary> All buttons that are active and enabled </summary>
        public static IEnumerable<UIButton> availableButtons => database.Where(item => item.isActiveAndEnabled);

        public override SelectableType selectableType => SelectableType.Button;

        public UIButtonId Id;

        protected UIButton()
        {
            Id = new UIButtonId();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            behaviours?.Disconnect();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Click();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);

            Click();

            if (!inputSettings.submitTriggersPointerClick) return;
            behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
            behaviours.GetBehaviour(UIBehaviour.Name.PointerLeftClick)?.Execute();
        }

        private IEnumerator RefreshSelectionState()
        {
            const float selectionDelay = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < selectionDelay)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        public void Click() =>
            Click(false);

        public void Click(bool forced)
        {
            if (!forced && (!IsActive() || !IsInteractable()))
                return;

            StartCoroutine(RefreshSelectionState());

            UISystemProfilerApi.AddMarker($"{nameof(UIButton)}.{nameof(Click)}", this);
            stream.SendSignal(new UIButtonSignalData(Id.Category, Id.Name, ButtonTrigger.Click, playerIndex, this));
        }

        #region Static Methods

        /// <summary> Get all the registered buttons with the given category and name </summary>
        /// <param name="category"> UIButton category </param>
        /// <param name="name"> UIButton name (from the given category) </param>
        public static IEnumerable<UIButton> GetButtons(string category, string name) =>
            database.Where(button => button.Id.Category.Equals(category)).Where(button => button.Id.Name.Equals(name));

        /// <summary> Get all the registered buttons with the given category </summary>
        /// <param name="category"> UIButton category </param>
        public static IEnumerable<UIButton> GetAllButtonsInCategory(string category) =>
            database.Where(button => button.Id.Category.Equals(category));

        /// <summary> Get all the buttons that are active and enabled (all the visible/available buttons) </summary>
        public static IEnumerable<UIButton> GetAvailableButtons() =>
            database.Where(button => button.isActiveAndEnabled);

        /// <summary> Get the selected button (if a button is not selected, this method returns null) </summary>
        public static UIButton GetSelectedButton() =>
            database.FirstOrDefault(button => button.isSelected);

        /// <summary> Select the button with the given category and name (if it is active and enabled) </summary>
        /// <param name="category"> UIButton category </param>
        /// <param name="name"> UIButton name (from the given category) </param>
        public static bool SelectButton(string category, string name)
        {
            UIButton button = availableButtons.FirstOrDefault(b => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
            if (button == null) return false;
            button.Select();
            return true;
        }

        #endregion
    }
}
