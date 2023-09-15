using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIElementSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [Header("Resources")] public UIManager UIManagerAsset;

        public AudioSource audioObject;

        [Header("Settings")] public bool enableHoverSound = true;

        public bool enableClickSound = true;
        public bool checkForInteraction = true;

        private Button sourceButton;

        void OnEnable()
        {
            if (UIManagerAsset == null)
            {
                try
                {
                    UIManagerAsset = Resources.Load<UIManager>("Shift UI Manager");
                }
                catch
                {
                    Debug.Log("<b>[UI Element Sound]</b> No UI Manager found.", this);
                    this.enabled = false;
                }
            }

            if (Application.isPlaying == true && audioObject == null)
            {
                try
                {
                    audioObject = FindObjectOfType<AudioSource>();
                }
                catch
                {
                    Debug.Log("<b>[UI Element Sound]</b> No Audio Source found.", this);
                }
            }

            if (checkForInteraction == true)
            {
                sourceButton = gameObject.GetComponent<Button>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (checkForInteraction == true && sourceButton != null && sourceButton.interactable == false)
                return;

            if (enableClickSound == true)
            {
                audioObject.PlayOneShot(UIManagerAsset.clickSound);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (checkForInteraction == true && sourceButton != null && sourceButton.interactable == false)
                return;

            if (enableHoverSound == true)
            {
                audioObject.PlayOneShot(UIManagerAsset.hoverSound);
            }
        }
    }
}