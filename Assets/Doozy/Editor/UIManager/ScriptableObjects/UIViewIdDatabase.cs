// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.UIManager.ScriptableObjects
{
    [Serializable]
    public class UIViewIdDatabase : SingletonEditorScriptableObject<UIViewIdDatabase>, IUpdateCallback
    {
        [SerializeField] private UIViewIdDataGroup Database;
        public UIViewIdDataGroup database => Database ??= new UIViewIdDataGroup();

        public UnityAction onUpdateCallback { get; set; }

        public void SetOnUpdateCallback(UnityAction callback) =>
            onUpdateCallback = callback;
        
        public void AddOnUpdateCallback(UnityAction callback) =>
            onUpdateCallback += callback;

        public void RemoveOnUpdateCallback(UnityAction callback) =>
            onUpdateCallback -= callback;

        public void ClearOnUpdateCallback() =>
            onUpdateCallback = null;
        
        public UIViewIdDatabase() => Database = new UIViewIdDataGroup();
        
        public (bool, string) CanImportRoamingDatabases(List<UIViewIdRoamingDatabase> roamingDatabases)
        {
            roamingDatabases = roamingDatabases ?? new List<UIViewIdRoamingDatabase>(); //avoid null
            roamingDatabases = roamingDatabases.Where(d => d != null).ToList(); //remove null entries
            if (roamingDatabases.Count == 0) return (false, $"Select at least 1 (one) valid database for import.");
            return (true, $"Settings are valid for import");

        }

        public bool ImportRoamingDatabases(List<ScriptableObject> databases)
        {
            var roamingDatabases = databases.Cast<UIViewIdRoamingDatabase>().ToList();
            bool canImport;
            string message;
            (canImport, message) = CanImportRoamingDatabases(roamingDatabases);
            if (!canImport)
            {
                EditorUtility.DisplayDialog("Import Database", message, "Ok");
                return false;
            }
            int numberOfDatabases = databases.Count;
            int counter = 0;
            Undo.RecordObject(instance, "Import");
            foreach (UIViewIdRoamingDatabase roamingDatabase in roamingDatabases)
            {
                counter++;
                EditorUtility.DisplayProgressBar("Importing Roaming Database", $"{roamingDatabase.name}", Mathf.Clamp01(counter / (float)numberOfDatabases));
                foreach (CategoryNameItem item in roamingDatabase.database.items)
                    database.AddName(item.category, item.name);
            }
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            Debug.Log($"Imported {numberOfDatabases} Databases");
            return true;
        }

        public bool ImportAllRoamingDatabasesFoundInProject()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UIViewIdRoamingDatabase)}");
            if (guids == null || guids.Length == 0)
            {
                EditorUtility.DisplayDialog("Import Databases", $"No {nameof(UIViewIdRoamingDatabase)} databases were found", "Ok");
                return false;
            }
            float numberOfDatabasesFound = guids.Length;
            int counter = 0;
            foreach (string guid in guids)
            {
                counter++;
                UIViewIdRoamingDatabase asset = AssetDatabase.LoadAssetAtPath<UIViewIdRoamingDatabase>(AssetDatabase.GUIDToAssetPath(guid));
                EditorUtility.DisplayProgressBar("Importing Roaming Database", $"{asset.name}", Mathf.Clamp01(counter / numberOfDatabasesFound));
                foreach (CategoryNameItem item in asset.database.items)
                    instance.database.AddName(item.category, item.name);
            }
            instance.database.CleanDatabase();
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            return true;
        }

        public (bool, string) CanExportRoamingDatabase(string databaseName, List<string> selectedCategories)
        {
            databaseName = databaseName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            if (databaseName.IsNullOrEmpty()) return (false, $"Invalid {nameof(databaseName)}. Database name cannot be null or empty.");
            if (selectedCategories == null || selectedCategories.Count == 0) return (false, $"Select at least 1 (one) category for export.");
            if (database == null) return (false, $"Invalid {nameof(database)}. Source cannot be null.");
            return (true, $"Settings are valid for export");
        }

        public bool ExportRoamingDatabase(string databaseName, List<string> selectedCategories)
        {
            bool canExport;
            string message;
            (canExport, message) = CanExportRoamingDatabase(databaseName, selectedCategories);
            if (!canExport)
            {
                EditorUtility.DisplayDialog("Export Database", message, "Ok");
                return false;
            }
            UIViewIdRoamingDatabase roamingDatabase = CreateInstance<UIViewIdRoamingDatabase>();
            roamingDatabase.databaseName = databaseName;
            roamingDatabase.database.ClearDatabase();
            string savePath = EditorUtility.SaveFilePanelInProject("Export Roaming Database", $"{databaseName}_{nameof(UIViewIdRoamingDatabase)}", "asset", "Save to");
            if (savePath.IsNullOrEmpty()) return false;
            foreach (string category in selectedCategories)
            {
                if (category.IsNullOrEmpty() || category.Equals(CategoryNameItem.k_DefaultCategory))
                    continue;
                foreach (CategoryNameItem item in database.items.Where(item => item.category.Equals(category)))
                    roamingDatabase.database.AddName(item.category, item.name);
            }
            EditorUtility.SetDirty(roamingDatabase);
            AssetDatabase.CreateAsset(roamingDatabase, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"'{databaseName}' Database saved at: {savePath}");
            EditorGUIUtility.PingObject(roamingDatabase);
            return true;
        }
    }
}
