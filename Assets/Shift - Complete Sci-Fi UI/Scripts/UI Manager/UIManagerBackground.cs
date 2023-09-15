using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIManagerBackground : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;
        Image backgroundObject;
        RawImage backgroundVideoImage;
        VideoPlayer backgroundVideo;

        [Header("Settings")]
        public BackgroundType backgroundType;
        public bool enableMobileMode = false;

        bool dynamicUpdateEnabled;

        public enum BackgroundType
        {
            BASIC,
            ADVANCED
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
                UpdateBackground();
            }

            if (backgroundObject == null && backgroundType == BackgroundType.BASIC)
                backgroundObject = gameObject.GetComponent<Image>();

            else if (backgroundVideo == null && backgroundType == BackgroundType.ADVANCED)
            {
                backgroundVideo = gameObject.GetComponent<VideoPlayer>();
                backgroundVideoImage = gameObject.GetComponent<RawImage>();
            }
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
                    UpdateBackground();
            }
        }

        void UpdateBackground()
        {
            try
            {
                if (enableMobileMode == true)
                {
                    if (backgroundType == BackgroundType.BASIC)
                    {
                        backgroundObject.enabled = true;
                        backgroundObject.sprite = UIManagerAsset.backgroundImage;
                        backgroundObject.color = UIManagerAsset.backgroundColorTint;
                        backgroundObject.preserveAspect = UIManagerAsset.backgroundPreserveAspect;
                    }

                    if (backgroundType == BackgroundType.ADVANCED)
                    {
                        backgroundVideo.enabled = false;
                        backgroundVideoImage.enabled = false;
                    }
                }

                else
                {
                    if (UIManagerAsset.backgroundType == UIManager.BackgroundType.ADVANCED && backgroundType == BackgroundType.ADVANCED)
                    {
                        backgroundVideo.enabled = true;
                        backgroundVideoImage.enabled = true;
                        backgroundVideo.clip = UIManagerAsset.backgroundVideo;
                        backgroundVideoImage.color = UIManagerAsset.backgroundColorTint;
                        backgroundVideo.playbackSpeed = UIManagerAsset.backgroundSpeed;
                    }

                    else if (UIManagerAsset.backgroundType == UIManager.BackgroundType.BASIC && backgroundType == BackgroundType.BASIC)
                    {
                        backgroundObject.enabled = true;
                        backgroundObject.sprite = UIManagerAsset.backgroundImage;
                        backgroundObject.color = UIManagerAsset.backgroundColorTint;
                        backgroundObject.preserveAspect = UIManagerAsset.backgroundPreserveAspect;
                    }

                    else if (UIManagerAsset.backgroundType == UIManager.BackgroundType.BASIC && backgroundType == BackgroundType.ADVANCED)
                    {
                        backgroundVideo.enabled = false;
                        backgroundVideoImage.enabled = false;
                    }

                    else if (UIManagerAsset.backgroundType == UIManager.BackgroundType.ADVANCED && backgroundType == BackgroundType.BASIC)
                        backgroundObject.enabled = false;
                }
            }

            catch { }
        }
    }
}