// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.Common.ScriptableObjects
{
    public class SingletonRuntimeScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static string fileName => $"{typeof(T).Name}";
        private static string assetFileName => $"{fileName}.asset";
        private static string assetFolderPath => $"{RuntimePath.path}/Data/Resources/";
        private static string assetFilePath => $"{assetFolderPath}/{assetFileName}";

        [ClearOnReload]
        private static T s_instance;

        public static T instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = Resources.Load<T>(fileName);
                if (s_instance != null) return s_instance;
                #if UNITY_EDITOR
                {
                    s_instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetFilePath);
                    if (s_instance != null) return s_instance;
                }
                #endif
                s_instance = CreateInstance<T>();
                #if UNITY_EDITOR
                {
                    UnityEditor.AssetDatabase.CreateAsset(s_instance, assetFilePath);
                }
                #endif
                return s_instance;
            }
        }
        
        #if UNITY_EDITOR
        
        public static void Restore()
        {
            UnityEditor.EditorUtility.SetDirty(instance);
        }
        
        public static void UndoRecord(string message)
        {
            UnityEditor.Undo.RecordObject(instance, message);
            UnityEditor.EditorUtility.SetDirty(instance);
        }

        public static void Save(bool refreshAssetDatabase = false)
        {
            UnityEditor.EditorUtility.SetDirty(instance);
            UnityEditor.AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) UnityEditor.AssetDatabase.Refresh();
        }
        
        #endif
    }
}
