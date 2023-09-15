using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIManagerGameLogo : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;
        public Image logoObject;

        [Header("Settings")]
        public bool keepAlphaValue = false;
        public bool useCustomColor = false;

        bool dynamicUpdateEnabled;

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
                UpdateLogo();
            }

            if (logoObject == null)
                logoObject = gameObject.GetComponent<Image>();
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
                    UpdateLogo();
            }
        }

        void UpdateLogo()
        {
            try
            {
                logoObject.sprite = UIManagerAsset.gameLogo;

                if (useCustomColor == false)
                {
                    if (keepAlphaValue == false)
                        logoObject.color = UIManagerAsset.logoColor;

                    else
                        logoObject.color = new Color(UIManagerAsset.logoColor.r, UIManagerAsset.logoColor.g, UIManagerAsset.logoColor.b, logoObject.color.a);
                }
            }

            catch { }
        }
    }
}