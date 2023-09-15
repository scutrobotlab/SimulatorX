using UnityEngine;
using UnityEngine.Video;
using TMPro;

namespace Michsky.UI.Shift
{
    [CreateAssetMenu(fileName = "New UI Manager", menuName = "Shift UI/New UI Manager")]
    public class UIManager : ScriptableObject
    {
        [HideInInspector] public bool enableDynamicUpdate = true;
        [HideInInspector] public bool enableExtendedColorPicker = true;
        [HideInInspector] public bool editorHints = true;

        // [Header("BACKGROUND")]
        public Color backgroundColorTint = new Color(255, 255, 255, 255);
        public BackgroundType backgroundType;
        public Sprite backgroundImage;
        public VideoClip backgroundVideo;
        public bool backgroundPreserveAspect;
        [Range(0.1f, 5)] public float backgroundSpeed = 1;

        // [Header("COLORS")]
        public Color primaryColor = new Color(255, 255, 255, 255);
        public Color secondaryColor = new Color(255, 255, 255, 255);
        public Color primaryReversed = new Color(255, 255, 255, 255);
        public Color negativeColor = new Color(255, 255, 255, 255);
        public Color backgroundColor = new Color(255, 255, 255, 255);

        // [Header("FONTS")]
        public TMP_FontAsset lightFont;
        public TMP_FontAsset regularFont;
        public TMP_FontAsset mediumFont;
        public TMP_FontAsset semiBoldFont;
        public TMP_FontAsset boldFont;

        // [Header("LOGO")]
        public Sprite gameLogo;
        public Color logoColor = new Color(255, 255, 255, 255);

        // [Header("PARTICLES")]
        public Color particleColor = new Color(255, 255, 255, 255);

        // [Header("SOUNDS")]
        public AudioClip backgroundMusic;
        public AudioClip hoverSound;
        public AudioClip clickSound;

        public enum ButtonThemeType
        {
            BASIC,
            CUSTOM
        }

        public enum BackgroundType
        {
            BASIC,
            ADVANCED
        }
    }
}