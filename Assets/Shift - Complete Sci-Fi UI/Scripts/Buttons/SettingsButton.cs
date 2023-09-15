using Honeti;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class SettingsButton : MonoBehaviour, IPointerEnterHandler
    {
        [Header("Resources")] public Image detailImage;

        public Image detailIcon;
        public Image detailBackground;
        public TextMeshProUGUI detailTitle;
        public TextMeshProUGUI detailDescription;
        public TextMeshProUGUI buttonTitleObj;

        [Header("Content")] public bool useCustomContent;

        public string buttonTitle;

        [Header("Preview")] public bool enableIconPreview;

        public string title;
        [TextArea] public string description;
        public Sprite imageSprite;
        public Sprite iconSprite;
        public Sprite iconBackground;

        void Start()
        {
            I18N.OnLanguageChanged += OnLanguageChanged;
            OnLanguageChanged(I18N.instance.gameLang);
        }

        private void OnDestroy()
        {
            I18N.OnLanguageChanged -= OnLanguageChanged;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableIconPreview == true)
            {
                detailImage.gameObject.SetActive(false);
                detailIcon.gameObject.SetActive(true);
                detailBackground.gameObject.SetActive(true);
                detailIcon.sprite = iconSprite;
                detailBackground.sprite = iconBackground;
            }

            else
            {
                detailImage.gameObject.SetActive(true);
                detailIcon.gameObject.SetActive(false);
                detailBackground.gameObject.SetActive(false);
                detailImage.sprite = imageSprite;
            }

            OnLanguageChanged(I18N.instance.gameLang);
        }

        private void OnLanguageChanged(LanguageCode newLang)
        {
            if (!isActiveAndEnabled)
                return;

            if (useCustomContent == false)
            {
                buttonTitleObj.text = I18N.instance.getValue(buttonTitle);
            }

            detailTitle.text = I18N.instance.getValue(title);
            detailDescription.text = I18N.instance.getValue(description);
        }
    }
}