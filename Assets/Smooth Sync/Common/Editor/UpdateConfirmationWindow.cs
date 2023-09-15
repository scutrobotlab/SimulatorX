using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smooth
{
    public class UpdateConfirmationWindow : EditorWindow
    {
        GUIStyle headerStyle = new GUIStyle();
        GUIStyle titleStyle = new GUIStyle();
        GUIStyle logoStyle = new GUIStyle();
        GUIStyle bodyStyle = new GUIStyle();
        GUIStyle buttonSectionStyle = new GUIStyle();
        GUIStyle welcomeTextStyle;
        GUIStyle buttonStyle;
        Texture2D logo, bg;
        List<string> installedNetworkingSystems = new List<string>();

        string titleText = "Smooth Sync Update";
        string welcomeText = "Update detected!\n\nWould you like to automatically import the new package for your networking system?. This will overwrite any changes you've made to the SmoothSync files or example scenes.\n";

        void OnEnable()
        {
            if (logo == null)
            {
                string[] paths = AssetDatabase.FindAssets("whale_256 t:Texture2D");
                if (paths != null && paths.Length > 0)
                {
                    logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(paths[0]));
                }
            }
            if (bg == null)
            {
                string[] paths = AssetDatabase.FindAssets("Noble Setup Title Background t:Texture2D");
                if (paths != null && paths.Length > 0)
                {
                    bg = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(paths[0]));
                }
            }
        }

        public static void ShowUpdateConfirmation(List<string> validSystems)
        {
            var window = GetWindow<UpdateConfirmationWindow>();
            window.titleContent = new GUIContent("Smooth Update");
            window.minSize = new Vector2(820, 390);
            window.maxSize = new Vector2(820, 390);
            window.installedNetworkingSystems = validSystems;
            window.Show();
        }

        void OnGUI()
        {
            welcomeTextStyle = new GUIStyle(EditorStyles.label);
            welcomeTextStyle.wordWrap = true;
            welcomeTextStyle.fontSize = 14;

            buttonSectionStyle.padding = new RectOffset(0, 0, 22, 0);

            buttonStyle = new GUIStyle(EditorStyles.miniButton);
            bodyStyle.padding = new RectOffset(12, 12, 12, 12);
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.padding = new RectOffset(0, 0, 4, 4);
            buttonStyle.fixedHeight = 0;
            buttonStyle.stretchHeight = true;

            DrawHeader();

            EditorGUILayout.BeginVertical(bodyStyle);

            // Welcome text
            EditorGUILayout.LabelField(welcomeText, welcomeTextStyle);

            // Button
            EditorGUILayout.BeginHorizontal(buttonSectionStyle);
            if (GUILayout.Button("No thanks", buttonStyle))
            {
                Close();
            }
            if (GUILayout.Button("Yes please", buttonStyle))
            {
                foreach (string system in installedNetworkingSystems)
                {
                    AssetDatabase.ImportPackage("Assets/Smooth Sync/SmoothSync" + system + ".unitypackage", false);
                }
                Close();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void DrawHeader()
        {
            headerStyle.normal.background = bg;
            headerStyle.fixedHeight = 80;

            titleStyle.fontSize = 26;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.padding = new RectOffset(15, 15, 25, 10);

            logoStyle.fixedWidth = 70;
            logoStyle.margin = new RectOffset(0, 15, 7, 7);

            EditorGUILayout.BeginHorizontal(headerStyle);

            GUILayout.Label(titleText, titleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(logo, logoStyle);

            EditorGUILayout.EndHorizontal();
        }
    }
}
