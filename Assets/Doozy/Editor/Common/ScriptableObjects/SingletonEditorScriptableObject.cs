// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Common.ScriptableObjects
{
    public abstract class SingletonEditorScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static string dataFolderPath => $"{EditorPath.path}/Data";
        private static string dataFileName => $"{typeof(T).Name}.asset";
        private static string dataFilePath => $"{dataFolderPath}/{dataFileName}";

        private static T s_instance;

        public static T instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<T>(dataFilePath);
                if (s_instance != null) return s_instance;
                s_instance = CreateInstance<T>();
                AssetDatabase.CreateAsset(s_instance, dataFilePath);
                return s_instance;
            }
        }

        public static void Restore()
        {
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
        }
        
        public static void UndoRecord(string message)
        {
            Undo.RecordObject(instance, message);
            EditorUtility.SetDirty(instance);
        }

        public static void Save(bool refreshAssetDatabase = false)
        {
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
            if (refreshAssetDatabase) AssetDatabase.Refresh();
        }
    }
}
