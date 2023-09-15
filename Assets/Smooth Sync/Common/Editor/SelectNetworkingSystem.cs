#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smooth
{
    public class SelectNetworkingSystem : EditorWindow
    {
        enum NetworkingSystem { UNET, MIRROR, MLAPI, PUN, PUN2, NONE }
        string[] networkingSystemStrings = { "UNet", "Mirror", "MLAPI", "Netcode for GameObjects", "PUN", "PUN2", "None" };
        string[] packageStrings = { "UNet", "Mirror", "MLAPI", "NetcodeForGameObjects", "PUN", "PUN2", "None" };
        string[] inconsistentlyNamedExampleScenes = { "SmoothSyncExample", "SmoothSyncMirrorExample", "SmoothSyncMLAPIExample", "SmoothSyncNetcodeExample", "SmoothSyncExamplePUN", "SmoothSyncExampleScenePUN2" };
        NetworkingSystem networkingSystem;

        GUIStyle headerStyle = new GUIStyle();
        GUIStyle titleStyle = new GUIStyle();
        GUIStyle logoStyle = new GUIStyle();
        GUIStyle bodyStyle = new GUIStyle();
        GUIStyle buttonSectionStyle = new GUIStyle();
        GUIStyle labelStyle;
        GUIStyle welcomeTextStyle;
        GUIStyle networkingSystemOptionsStyle;
        GUIStyle buttonStyle;

        Texture2D logo, bg;

        string titleText = "Smooth Sync Setup";
        string welcomeText = "Welcome!\n\nSmooth Sync supports many different networking systems. To get started, please select your networking system below. Don't worry, you can change your selection any time.\n";

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

        [MenuItem("Window/Smooth Sync Setup")]
        public static void SetNetworkingSystem()
        {
            var window = GetWindow<SelectNetworkingSystem>();
            window.titleContent = new GUIContent("Smooth Setup");
            window.minSize = new Vector2(820, 410);
            window.maxSize = new Vector2(820, 410);
            window.Show();
        }

        void OnGUI()
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            welcomeTextStyle = new GUIStyle(EditorStyles.label);
            networkingSystemOptionsStyle = new GUIStyle(EditorStyles.radioButton);
            buttonStyle = new GUIStyle(EditorStyles.miniButton);
            
            bodyStyle.padding = new RectOffset(12, 12, 12, 12);
            
            welcomeTextStyle.wordWrap = true;
            welcomeTextStyle.fontSize = 14;
            
            labelStyle.fontSize = 14;
            labelStyle.padding = new RectOffset(0, 0, 0, 5);
            
            networkingSystemOptionsStyle.fontSize = 14;
            networkingSystemOptionsStyle.padding = new RectOffset(21, 0, 0, 0);
            networkingSystemOptionsStyle.margin = new RectOffset(0, 0, 5, 0);
            
            buttonSectionStyle.padding = new RectOffset(0, 0, 22, 0);

            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.padding = new RectOffset(0, 0, 4, 4);
            buttonStyle.fixedHeight = 0;
            buttonStyle.stretchHeight = true;

            DrawHeader();

            EditorGUILayout.BeginVertical(bodyStyle);

            // Welcome text
            EditorGUILayout.LabelField(welcomeText, welcomeTextStyle);

            // Networking System
            GUILayout.Label("Networking System", labelStyle);
            networkingSystem = (NetworkingSystem)GUILayout.SelectionGrid((int)networkingSystem, networkingSystemStrings, 1, networkingSystemOptionsStyle);

            // Button
            EditorGUILayout.BeginHorizontal(buttonSectionStyle);
            if (GUILayout.Button("Setup", buttonStyle))
            {
                Setup(networkingSystem);
                Close();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void Setup(NetworkingSystem networkingSystem)
        {
            if (networkingSystem != NetworkingSystem.NONE)
            {
                ImportPackageForNetworkingSystem(networkingSystem);
                SelectExampleScene(networkingSystem);
            }
        }

        void ImportPackageForNetworkingSystem(NetworkingSystem networkingSystem)
        {
            string  networkingSystemString = packageStrings[(int)networkingSystem];
            string packageName = "SmoothSync";
            packageName += networkingSystemString;
            packageName += ".unitypackage";
            AssetDatabase.ImportPackage("Assets/Smooth Sync/" + packageName, false);
        }

        void SelectExampleScene(NetworkingSystem networkingSystem)
        {
            string networkingSystemString = networkingSystemStrings[(int)networkingSystem];
            string exampleSceneName = inconsistentlyNamedExampleScenes[(int)networkingSystem];
            string exampleScenePath = "Assets/Smooth Sync/";
            exampleScenePath += networkingSystemString;
            exampleScenePath += "/Smooth Sync Example Scene/";
            exampleScenePath += exampleSceneName + ".unity";
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(exampleScenePath);
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
#endif