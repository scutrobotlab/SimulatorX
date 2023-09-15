using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class SpotlightButton : MonoBehaviour
    {
        [Header("Text")]
        public bool useCustomText = false;
        public string buttonTitle = "My Title";
        public string buttonDescription = "My Description";

        [Header("Image")]
        public bool useCustomImage = false;
        public Sprite firstImage;
        public Sprite secondImage;

        TextMeshProUGUI titleText;
        TextMeshProUGUI descriptionText;
        Image image1;
        Image image2;

        void Start()
        {
            if (useCustomText == false)
            {
                titleText = gameObject.transform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
                descriptionText = gameObject.transform.Find("Content/Description").GetComponent<TextMeshProUGUI>();

                titleText.text = buttonTitle;
                descriptionText.text = buttonDescription;
            }

            if (useCustomImage == false)
            {
                image1 = gameObject.transform.Find("Content/Background/Image 1").GetComponent<Image>();
                image2 = gameObject.transform.Find("Content/Background/Image 2").GetComponent<Image>();

                image1.sprite = firstImage;
                image2.sprite = secondImage;
            }
        }
    }
}