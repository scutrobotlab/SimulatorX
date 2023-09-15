using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class QuickMatchButton : MonoBehaviour
    {
        [Header("Text")]
        public bool useCustomText = false;
        public string buttonTitle = "My Title";

        [Header("Image")]
        public bool useCustomImage = false;
        public Sprite backgroundImage;

        TextMeshProUGUI titleText;
        Image image1;

        void Start()
        {
            if (useCustomText == false)
            {
                titleText = gameObject.transform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
                titleText.text = buttonTitle;
            }

            if (useCustomImage == false)
            {
                image1 = gameObject.transform.Find("Content/Background").GetComponent<Image>();
                image1.sprite = backgroundImage;
            }
        }
    }
}