using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class LobbyInfantryCard : RoleChoice
    {
        public Image avatar;
        public TMP_Text robotName;
        public InfantrySelector[] selectors;

        public UnityEvent onStartSelect;
        public UnityEvent onEndSelect;

        private readonly Dictionary<string, InfantrySelector> _selectors = new Dictionary<string, InfantrySelector>();

        protected override void Start()
        {
            base.Start();
            foreach (var selector in selectors)
            {
                _selectors.Add(selector.name, selector);
            }
        }

        public void OnClickStartSelect()
        {
            StartCoroutine(StartSelect());
        }

        private IEnumerator StartSelect()
        {
            foreach (var card in FindObjectsOfType<LobbyInfantryCard>())
            {
                card.OnClickEndSelect();
            }

            onStartSelect.Invoke();

            yield return null;
        }

        public void OnClickEndSelect()
        {
            StopCoroutine(StartSelect());
            onEndSelect.Invoke();
        }

        public void OnConfirmSelector(string selector)
        {
            SelectionPanel.OnInfantryTypeChange(role.order, role.camp, selector);
        }

        public void UpdateSelection(string selector)
        {
            var s = _selectors[selector];
            avatar.sprite = s.avatar.sprite;
            role.role = s.role.role;
            role.serial = s.role.serial;
            robotName.text = s.nameText.text;
            OnClickEndSelect();
        }
    }

    [Serializable]
    public struct InfantrySelector
    {
        public string name;
        public Identity role;
        public Image avatar;
        public TMP_Text nameText;
    }
}