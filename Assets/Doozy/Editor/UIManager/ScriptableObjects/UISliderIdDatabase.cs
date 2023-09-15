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
    public class UISliderIdDatabase : SingletonEditorScriptableObject<UISliderIdDatabase>, IUpdateCallback
    {
        #region IUpdateCallback

        public UnityAction onUpdateCallback { get; set; }

        public void SetOnUpdateCallback(UnityAction callback) =>
            onUpdateCallback = callback;

        public void AddOnUpdateCallback(UnityAction callback) =>
            onUpdateCallback += callback;

        public void RemoveOnUpdateCallback(UnityAction callback) =>
            onUpdateCallback -= callback;

        public void ClearOnUpdateCallback() =>
            onUpdateCallback = null;

        #endregion

        [SerializeField] private UISliderIdDataGroup Database;
        public UISliderIdDataGroup database => Database ??= new UISliderIdDataGroup();

        public UISliderIdDatabase() => Database = new UISliderIdDataGroup();

        public (bool, string) CanImportRoamingDatabases(List<UISliderIdRoamingDatabase> roamingDatabases)
        {
            roamingDatabases ??= new List<UISliderIdRoamingDatabase>(); //avoid null
            roamingDatabases = roamingDatabases.Where(d => d != null).ToList(); //remove null entries
            return
                roamingDatabases.Count == 0 
                    ? (false, $"Select at least 1 (one) valid database for import.") 
                    : (true, $"Settings are valid for import");

        }

        public bool ImportRoamingDatabases(List<ScriptableObject> databases)
        {
            var roamingDatabases = databases.Cast<UISliderIdRoamingDatabase>().ToList();
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
            foreach (UISliderIdRoamingDatabase roamingDatabase in roamingDatabases)
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
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UISliderIdRoamingDatabase)}");
            if (guids == null || guids.Length == 0)
            {
                EditorUtility.DisplayDialog("Import Databases", $"No {nameof(UISliderIdRoamingDatabase)} databases were found", "Ok");
                return false;
            }
            float numberOfDatabasesFound = guids.Length;
            int counter = 0;
            foreach (string guid in guids)
            {
                counter++;
                UISliderIdRoamingDatabase asset = AssetDatabase.LoadAssetAtPath<UISliderIdRoamingDatabase>(AssetDatabase.GUIDToAssetPath(guid));
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
            UISliderIdRoamingDatabase roamingDatabase = CreateInstance<UISliderIdRoamingDatabase>();
            roamingDatabase.databaseName = databaseName;
            roamingDatabase.database.ClearDatabase();
            string savePath = EditorUtility.SaveFilePanelInProject("Export Roaming Database", $"{databaseName}_{nameof(UISliderIdRoamingDatabase)}", "asset", "Save to");
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
