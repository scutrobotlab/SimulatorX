using UnityEngine;
using TMPro;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIManagerText : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;
        public TextMeshProUGUI textObject;

        [Header("Settings")]
        public bool keepAlphaValue = false;
        public bool useCustomColor = false;
        public ColorType colorType;
        public FontType fontType;

        bool dynamicUpdateEnabled;

        public enum ColorType
        {
            Primary,
            Secondary,
            PrimaryReversed,
            Negative,
            Background
        }

        public enum FontType
        {
            Light,
            Regular,
            Medium,
            Semibold,
            Bold
        }

        void OnEnable()
        {
            if (UIManagerAsset == null)
            {
                try { UIManagerAsset = Resources.Load<UIManager>("Shift UI Manager"); }
                catch { Debug.Log("No UI Manager found. Assign it manually, otherwise it won't work properly.", this); }
            }
        }

        void Awake()
        {
            if (dynamicUpdateEnabled == false)
            {
                this.enabled = true;
                UpdateButton();
            }

            if (textObject == null)
                textObject = gameObject.GetComponent<TextMeshProUGUI>();
        }

        void LateUpdate()
        {
            if (UIManagerAsset != null)
            {
                if (UIManagerAsset.enableDynamicUpdate == true)
                    dynamicUpdateEnabled = true;
                else
                    dynamicUpdateEnabled = false;

                if (dynamicUpdateEnabled == true)
                    UpdateButton();
            }
        }

        void UpdateButton()
        {
            try
            {
                // Colors
                if (useCustomColor == false)
                {
                    if (keepAlphaValue == false)
                    {
                        if (colorType == ColorType.Primary)
                            textObject.color = UIManagerAsset.primaryColor;
                        else if (colorType == ColorType.Secondary)
                            textObject.color = UIManagerAsset.secondaryColor;
                        else if (colorType == ColorType.PrimaryReversed)
                            textObject.color = UIManagerAsset.primaryReversed;
                        else if (colorType == ColorType.Negative)
                            textObject.color = UIManagerAsset.negativeColor;
                        else if (colorType == ColorType.Background)
                            textObject.color = UIManagerAsset.backgroundColor;
                    }

                    else
                    {
                        if (colorType == ColorType.Primary)
                            textObject.color = new Color(UIManagerAsset.primaryColor.r, UIManagerAsset.primaryColor.g, UIManagerAsset.primaryColor.b, textObject.color.a);
                        else if (colorType == ColorType.Secondary)
                            textObject.color = new Color(UIManagerAsset.secondaryColor.r, UIManagerAsset.secondaryColor.g, UIManagerAsset.secondaryColor.b, textObject.color.a);
                        else if (colorType == ColorType.PrimaryReversed)
                            textObject.color = new Color(UIManagerAsset.primaryReversed.r, UIManagerAsset.primaryReversed.g, UIManagerAsset.primaryReversed.b, textObject.color.a);
                        else if (colorType == ColorType.Negative)
                            textObject.color = new Color(UIManagerAsset.negativeColor.r, UIManagerAsset.negativeColor.g, UIManagerAsset.negativeColor.b, textObject.color.a);
                        else if (colorType == ColorType.Background)
                            textObject.color = new Color(UIManagerAsset.backgroundColor.r, UIManagerAsset.backgroundColor.g, UIManagerAsset.backgroundColor.b, textObject.color.a);
                    }
                }

                // Fonts
                if (fontType == FontType.Light)
                    textObject.font = UIManagerAsset.lightFont;
                else if (fontType == FontType.Regular)
                    textObject.font = UIManagerAsset.regularFont;
                else if (fontType == FontType.Medium)
                    textObject.font = UIManagerAsset.mediumFont;
                else if (fontType == FontType.Semibold)
                    textObject.font = UIManagerAsset.semiBoldFont;
                else if (fontType == FontType.Bold)
                    textObject.font = UIManagerAsset.boldFont;
            }

            catch { }
        }
    }
}