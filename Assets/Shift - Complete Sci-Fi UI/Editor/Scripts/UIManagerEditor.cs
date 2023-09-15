using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace Michsky.UI.Shift
{
    [CustomEditor(typeof(UIManager))]
    [System.Serializable]
    public class UIManagerEditor : Editor
    {
        GUISkin customSkin;
        protected static string buildID = "B16-20211215";
        protected static float foldoutItemSpace = 2;
        protected static float foldoutTopSpace = 5;
        protected static float foldoutBottomSpace = 2;

        protected static bool showBackground = false;
        protected static bool showColors = false;
        protected static bool showFonts = false;
        protected static bool showLogo = false;
        protected static bool showParticle = false;
        protected static bool showSounds = false;

        void OnEnable()
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = (GUISkin)Resources.Load("Editor\\Shift UI Skin Dark"); }
            else { customSkin = (GUISkin)Resources.Load("Editor\\Shift UI Skin Light"); }
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor variables are missing. You can manually fix this by deleting " +
                    "Shift UI > Resources folder and then re-import the package. \n\nIf you're still seeing this " +
                    "dialog even after the re-import, contact me with this ID: " + buildID, MessageType.Error);

                if (GUILayout.Button("Contact")) { Email(); }
                return;
            }

            // Foldout style
            GUIStyle foldoutStyle = customSkin.FindStyle("UIM Foldout");

            // UIM Header
            ShiftUIEditorHandler.DrawHeader(customSkin, "UIM Header", 8);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Background
            var backgroundType = serializedObject.FindProperty("backgroundType");
            var backgroundImage = serializedObject.FindProperty("backgroundImage");
            var backgroundPreserveAspect = serializedObject.FindProperty("backgroundPreserveAspect");
            var backgroundVideo = serializedObject.FindProperty("backgroundVideo");
            var backgroundSpeed = serializedObject.FindProperty("backgroundSpeed");
            var backgroundColorTint = serializedObject.FindProperty("backgroundColorTint");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showBackground = EditorGUILayout.Foldout(showBackground, "Background", true, foldoutStyle);
            showBackground = GUILayout.Toggle(showBackground, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);


            if (showBackground && backgroundType.enumValueIndex == 0)
            {
                ShiftUIEditorHandler.DrawProperty(backgroundType, customSkin, "Background Type");
                ShiftUIEditorHandler.DrawProperty(backgroundImage, customSkin, "Background Image");
                ShiftUIEditorHandler.DrawProperty(backgroundColorTint, customSkin, "Color Tint");
                ShiftUIEditorHandler.DrawProperty(backgroundPreserveAspect, customSkin, "Preserve Aspect");
            }

            if (showBackground && backgroundType.enumValueIndex == 1)
            {
                ShiftUIEditorHandler.DrawProperty(backgroundType, customSkin, "Background Type");
                ShiftUIEditorHandler.DrawProperty(backgroundVideo, customSkin, "Background Video");
                ShiftUIEditorHandler.DrawProperty(backgroundColorTint, customSkin, "Color Tint");
                ShiftUIEditorHandler.DrawProperty(backgroundSpeed, customSkin, "Animation Speed");
                EditorGUILayout.HelpBox("Video Player will be used for background on Advanced mode.", MessageType.Info);
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Colors       
            var primaryColor = serializedObject.FindProperty("primaryColor");
            var secondaryColor = serializedObject.FindProperty("secondaryColor");
            var primaryReversed = serializedObject.FindProperty("primaryReversed");
            var negativeColor = serializedObject.FindProperty("negativeColor");
            var backgroundColor = serializedObject.FindProperty("backgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showColors = EditorGUILayout.Foldout(showColors, "Colors", true, foldoutStyle);
            showColors = GUILayout.Toggle(showColors, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showColors)
            {
                ShiftUIEditorHandler.DrawProperty(primaryColor, customSkin, "Primary");
                ShiftUIEditorHandler.DrawProperty(secondaryColor, customSkin, "Secondary");
                ShiftUIEditorHandler.DrawProperty(primaryReversed, customSkin, "Primary Reversed");
                ShiftUIEditorHandler.DrawProperty(negativeColor, customSkin, "Negative");
                ShiftUIEditorHandler.DrawProperty(backgroundColor, customSkin, "Background");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Fonts
            var lightFont = serializedObject.FindProperty("lightFont");
            var regularFont = serializedObject.FindProperty("regularFont");
            var mediumFont = serializedObject.FindProperty("mediumFont");
            var semiBoldFont = serializedObject.FindProperty("semiBoldFont");
            var boldFont = serializedObject.FindProperty("boldFont");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showFonts = EditorGUILayout.Foldout(showFonts, "Fonts", true, foldoutStyle);
            showFonts = GUILayout.Toggle(showFonts, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showFonts)
            {
                ShiftUIEditorHandler.DrawProperty(lightFont, customSkin, "Light");
                ShiftUIEditorHandler.DrawProperty(regularFont, customSkin, "Regular");
                ShiftUIEditorHandler.DrawProperty(mediumFont, customSkin, "Medium");
                ShiftUIEditorHandler.DrawProperty(semiBoldFont, customSkin, "Semibold");
                ShiftUIEditorHandler.DrawProperty(boldFont, customSkin, "Bold");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Logo
            var gameLogo = serializedObject.FindProperty("gameLogo");
            var logoColor = serializedObject.FindProperty("logoColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showLogo = EditorGUILayout.Foldout(showLogo, "Logo", true, foldoutStyle);
            showLogo = GUILayout.Toggle(showLogo, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showLogo)
            {
                ShiftUIEditorHandler.DrawProperty(gameLogo, customSkin, "Game Logo");
                ShiftUIEditorHandler.DrawProperty(logoColor, customSkin, "Logo Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Particles
            var particleColor = serializedObject.FindProperty("particleColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showParticle = EditorGUILayout.Foldout(showParticle, "UI Particles", true, foldoutStyle);
            showParticle = GUILayout.Toggle(showParticle, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showParticle)
            {
                ShiftUIEditorHandler.DrawProperty(particleColor, customSkin, "Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Sounds
            var backgroundMusic = serializedObject.FindProperty("backgroundMusic");
            var hoverSound = serializedObject.FindProperty("hoverSound");
            var clickSound = serializedObject.FindProperty("clickSound");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showSounds = EditorGUILayout.Foldout(showSounds, "Sounds", true, foldoutStyle);
            showSounds = GUILayout.Toggle(showSounds, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showSounds)
            {
                ShiftUIEditorHandler.DrawProperty(backgroundMusic, customSkin, "Background Music");
                ShiftUIEditorHandler.DrawProperty(hoverSound, customSkin, "Hover SFX");
                ShiftUIEditorHandler.DrawProperty(clickSound, customSkin, "Click SFX");
            }

            // Settings
            GUILayout.EndVertical();
            ShiftUIEditorHandler.DrawHeader(customSkin, "Options Header", 14);

            var enableDynamicUpdate = serializedObject.FindProperty("enableDynamicUpdate");
            enableDynamicUpdate.boolValue = ShiftUIEditorHandler.DrawToggle(enableDynamicUpdate.boolValue, customSkin, "Update Values");

            var enableExtendedColorPicker = serializedObject.FindProperty("enableExtendedColorPicker");
            enableExtendedColorPicker.boolValue = ShiftUIEditorHandler.DrawToggle(enableExtendedColorPicker.boolValue, customSkin, "Extended Color Picker");

            if (enableExtendedColorPicker.boolValue == true) { EditorPrefs.SetInt("ShiftUIManager.EnableExtendedColorPicker", 1); }
            else { EditorPrefs.SetInt("ShiftUIManager.EnableExtendedColorPicker", 0); }

            var editorHints = serializedObject.FindProperty("editorHints");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(-3);
            editorHints.boolValue = ShiftUIEditorHandler.DrawTogglePlain(editorHints.boolValue, customSkin, "UI Manager Hints");
            GUILayout.Space(3);

            if (editorHints.boolValue == true)
            {
                EditorGUILayout.HelpBox("These values are universal and affect all objects containing 'UI Manager' component.", MessageType.Info);
                EditorGUILayout.HelpBox("If want to assign unique values, remove 'UI Manager' component from the object ", MessageType.Info);
                EditorGUILayout.HelpBox("Navigate to 'Tools > Shift UI > Show UI Manager' to open this window quickly.", MessageType.Info);
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            Repaint();

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Reset to defaults")))
                ResetToDefaults();

            GUILayout.EndHorizontal();

            // Support
            ShiftUIEditorHandler.DrawHeader(customSkin, "Support Header", 14);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Need help? Contact me via:", customSkin.FindStyle("Text"));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Discord", customSkin.button)) { Discord(); }
            if (GUILayout.Button("Twitter", customSkin.button)) { Twitter(); }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("E-mail", customSkin.button)) { Email(); }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ID: " + buildID);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
        }

        void Discord() { Application.OpenURL("https://discord.gg/VXpHyUt"); }
        void Email() { Application.OpenURL("https://www.michsky.com/contact/"); }
        void Twitter() { Application.OpenURL("https://www.twitter.com/michskyHQ"); }

        void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("Reset to defaults", "Are you sure you want to reset UI Manager values to default?", "Yes", "Cancel"))
            {
                try
                {
                    Preset defaultPreset = Resources.Load<Preset>("Shift UI Presets/Default");
                    defaultPreset.ApplyTo(Resources.Load("Shift UI Manager"));
                    Selection.activeObject = null;
                    Debug.Log("<b>[Shift UI Manager]</b> Resetting is successful.");
                }

                catch { Debug.LogWarning("<b>[UI Manager]</b> Resetting failed. Default preset seems to be missing"); }
            }
        }
    }
}