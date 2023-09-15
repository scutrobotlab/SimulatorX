using Honeti;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class MainPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Text")] public bool useCustomText = false;

        public string buttonText = "My Title";

        [Header("Icon")] public bool hasIcon = false;

        public Sprite iconSprite;

        [Header("Resources")] public Animator buttonAnimator;

        public TextMeshProUGUI normalText;
        public TextMeshProUGUI highlightedText;
        public TextMeshProUGUI pressedText;
        public Image normalIcon;
        public Image highlightedIcon;
        public Image pressedIcon;

        private void Start()
        {
            I18N.OnLanguageChanged += OnLanguageChanged;
            OnLanguageChanged(I18N.instance.gameLang);
        }

        void OnEnable()
        {
            if (buttonAnimator == null)
                buttonAnimator = gameObject.GetComponent<Animator>();

            if (hasIcon == true)
            {
                if (normalIcon != null)
                {
                    normalIcon.sprite = iconSprite;
                }

                if (highlightedIcon != null)
                {
                    highlightedIcon.sprite = iconSprite;
                }

                if (pressedIcon != null)
                {
                    pressedIcon.sprite = iconSprite;
                }
            }

            else if (hasIcon == false)
            {
                if (normalIcon != null)
                {
                    Destroy(normalIcon.gameObject);
                }

                if (highlightedIcon != null)
                {
                    Destroy(highlightedIcon.gameObject);
                }

                if (pressedIcon != null)
                {
                    Destroy(pressedIcon.gameObject);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Dissolve to Normal");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Normal to Dissolve");
        }

        private void OnLanguageChanged(LanguageCode newLang)
        {
            if (useCustomText == false)
            {
                if (normalText != null)
                {
                    normalText.text = I18N.instance.getValue(buttonText);
                }

                if (highlightedText != null)
                {
                    highlightedText.text = I18N.instance.getValue(buttonText);
                }

                if (pressedText != null)
                {
                    pressedText.text = I18N.instance.getValue(buttonText);
                }
            }
        }
    }
}