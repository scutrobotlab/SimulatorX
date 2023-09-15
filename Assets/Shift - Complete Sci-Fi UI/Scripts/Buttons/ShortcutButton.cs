using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class ShortcutButton : MonoBehaviour
    {
        [Header("Resources")]
        public string keyText = "A";
        public string buttonText = "My Title";

        [Header("Settings")]
        public bool useCustomText = false;
        public bool isGamepad = false;

        [Header("Resources")]
        public TextMeshProUGUI normalText;
        public TextMeshProUGUI highlightedText;
        public TextMeshProUGUI normalKeyText;
        public TextMeshProUGUI highlightedKeyText;

        void OnEnable()
        {
            if (useCustomText == false)
            {
                if (isGamepad == false)
                {
                    if (normalText != null) { normalText.text = buttonText; }
                    if (highlightedText != null) { highlightedText.text = buttonText; }
                    if (normalKeyText != null) { normalKeyText.text = keyText; }
                    if (highlightedKeyText != null) { highlightedKeyText.text = keyText; }
                }

                else
                {
                    if (normalText != null) { normalText.text = buttonText; }
                    if (normalKeyText != null) { normalKeyText.text = keyText; }
                }
            }
        }
    }
}