using System;
using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class MainButton : MonoBehaviour
    {
        [Header("Settings")]
        public string buttonText = "My Title";
        public bool useCustomText = false;
        private string _previousText;

        [Header("Resources")]
        public TextMeshProUGUI normalText;
        public TextMeshProUGUI highlightedText;
        public TextMeshProUGUI pressedText;

        void OnEnable()
        {
            if (useCustomText == false)
            {
                if (normalText != null) { normalText.text = buttonText; }
                if (highlightedText != null) { highlightedText.text = buttonText; }
                if (pressedText != null) { pressedText.text = buttonText; }

                _previousText = buttonText;
            }
        }

        private void Update()
        {
            if (useCustomText == false && buttonText != _previousText)
            {
                if (normalText != null) { normalText.text = buttonText; }
                if (highlightedText != null) { highlightedText.text = buttonText; }
                if (pressedText != null) { pressedText.text = buttonText; }

                _previousText = buttonText;
            }
        }
    }
}