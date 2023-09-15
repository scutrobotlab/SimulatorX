using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Shift
{
    public class ToolsMenu : Editor
    {
        static string objectPath;

        static void GetObjectPath()
        {
            objectPath = AssetDatabase.GetAssetPath(Resources.Load("Shift UI Manager"));
            objectPath = objectPath.Replace("Resources/Shift UI Manager.asset", "").Trim();
            objectPath = objectPath + "Prefabs/";
        }

        static void MakeSceneDirty(GameObject source, string sourceName)
        {
            if (Application.isPlaying == false)
            {
                Undo.RegisterCreatedObjectUndo(source, sourceName);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        static void ShowErrorDialog()
        {
            EditorUtility.DisplayDialog("Shift UI", "Cannot create the object due to missing manager file. " +
                    "Make sure you have 'Shift UI Manager' file in Shift UI > Resources folder.", "Okay");
        }

        static void UpdateCustomEditorPath()
        {
            string mainPath = AssetDatabase.GetAssetPath(Resources.Load("Shift UI Manager"));
            mainPath = mainPath.Replace("Resources/Shift UI Manager.asset", "").Trim();
            string darkPath = mainPath + "Editor/Shift UI Skin Dark.guiskin";
            string lightPath = mainPath + "Editor/Shift UI Skin Light.guiskin";

            EditorPrefs.SetString("ShiftUI.CustomEditorDark", darkPath);
            EditorPrefs.SetString("ShiftUI.CustomEditorLight", lightPath);
        }

        static void CreateObject(string resourcePath)
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + resourcePath + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;

                try
                {
                    if (Selection.activeGameObject == null)
                    {
                        var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                        clone.transform.SetParent(canvas.transform, false);
                    }

                    else { clone.transform.SetParent(Selection.activeGameObject.transform, false); }

                    clone.name = clone.name.Replace("(Clone)", "").Trim();
                    MakeSceneDirty(clone, clone.name);
                }

                catch
                {
                    CreateCanvas();
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    clone.transform.SetParent(canvas.transform, false);
                    clone.name = clone.name.Replace("(Clone)", "").Trim();
                    MakeSceneDirty(clone, clone.name);
                }

                Selection.activeObject = clone;
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("GameObject/Shift UI/Canvas", false, -1)]
        static void CreateCanvas()
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + "Other/Canvas" + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Selection.activeObject = clone;
                MakeSceneDirty(clone, clone.name);
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("Tools/Shift UI/Show UI Manager")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("Shift UI Manager");

            if (Selection.activeObject == null)
                Debug.Log("Can't find a file named 'Shift UI Manager'. Make sure you have 'Shift UI Manager' file in Resources folder.");
        }

        [MenuItem("GameObject/Shift UI/Buttons/Chapter Button", false, 0)]
        static void BCB() { CreateObject("Button/Chapter Button"); }

        [MenuItem("GameObject/Shift UI/Buttons/Icon Button", false, 0)]
        static void BIB() { CreateObject("Button/Icon Button"); }

        [MenuItem("GameObject/Shift UI/Buttons/Main Button", false, 0)]
        static void BFMB() { CreateObject("Button/Main Button"); }

        [MenuItem("GameObject/Shift UI/Buttons/Spotlight Button", false, 0)]
        static void BSB() { CreateObject("Button/Spotlight Button"); }

        [MenuItem("GameObject/Shift UI/Dropdown/Standard", false, 0)]
        static void DST() { CreateObject("Dropdown/Dropdown"); }

        [MenuItem("GameObject/Shift UI/Horizontal Selector/Standard", false, 0)]
        static void HSHS() { CreateObject("Horizontal Selector/Horizontal Selector"); }

        [MenuItem("GameObject/Shift UI/Input Field/Standard (Left Aligned)", false, 0)]
        static void IFSLA() { CreateObject("Input Field/Standard (Left Aligned)"); }

        [MenuItem("GameObject/Shift UI/Input Field/Standard (Middle Aligned)", false, 0)]
        static void IFSMA() { CreateObject("Input Field/Standard (Middle Aligned)"); }

        [MenuItem("GameObject/Shift UI/Loaders/Default Loader", false, 0)]
        static void SPST() { CreateObject("Loader/Loading"); }

        [MenuItem("GameObject/Shift UI/Scrollbar/Standard", false, 0)]
        static void SBSB() { CreateObject("Scrollbar/Scrollbar"); }

        [MenuItem("GameObject/Shift UI/Slider/Standard", false, 0)]
        static void SLSL() { CreateObject("Slider/Slider"); }

        [MenuItem("GameObject/Shift UI/Switch/Standard", false, 0)]
        static void SWSW() { CreateObject("Switch/Switch"); }
    }
}