// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.UIManager.Automation.Generators;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.UIManager.UIMenu
{
    [Serializable]
    public class UIMenuItemsDatabase : SingletonEditorScriptableObject<UIMenuItemsDatabase>, IUpdateCallback
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

        [SerializeField] private string DatabaseName = "Prefabs Menu";
        public string databaseName => DatabaseName;

        [SerializeField] private string DatabaseDescription = $"Collection of all the menu items that make up the Prefabs Menu";
        public string databaseDescription => DatabaseDescription;


        [SerializeField] private List<UIMenuItem> Database;
        public List<UIMenuItem> database
        {
            get
            {
                if (Database == null) Database = new List<UIMenuItem>();
                RemoveNullEntries();
                return Database;
            }
        }

        public bool isEmpty => database.Count == 0;

        private void RemoveNullEntries() =>
            Database =
                Database
                    .Where(item => item != null && item.isValid)
                    .ToList();

        public void RefreshDatabaseItem(UIMenuItem item, bool saveAssets = false, bool refreshAssetDatabase = false, bool runHelperClassGenerator = false)
        {
            if (item == null || !item.isValid) return;
            string title = "Prefabs Menu Database";
            string info = $"Refreshing '{item.name}'";

            EditorUtility.DisplayProgressBar(title, info, 0.1f);
            //VALIDATE item - make sure its setup is correct
            item.Validate();

            //CHECK IF the item exists in the database
            if (instance.database.Contains(item))
            {
                //---ITEM FOUND---

                //CHECK IF regenerate database helper class is needed
                if (!runHelperClassGenerator)
                {
                    EditorUtility.ClearProgressBar();
                    //STOP
                    return;
                }

                //REGENERATE database helper class
                GenerateDatabaseHelperClass(saveAssets, refreshAssetDatabase);

                EditorUtility.ClearProgressBar();
                //STOP
                return;
            }

            EditorUtility.DisplayProgressBar(title, info, 0.4f);

            //NULL CHECK - if the item didn't delete itself due to bad setup (items are configured, in validate, to self-move-to-trash if they are invalid)
            if (item == null)
            {
                EditorUtility.ClearProgressBar();
                //STOP
                return;
            }

            //MARK DIRTY - the item
            EditorUtility.SetDirty(item);

            //MARK DIRTY - the database
            EditorUtility.SetDirty(instance);

            //SAVE ITEM PATH - get it from the asset
            string itemPath = AssetDatabase.GetAssetPath(item);

            //ADD ITEM to DATABASE - item passed all the tests thus it gets added to the database
            instance.database.Add(item);

            Debugger.Log
            (
                $"'{item.name}' Prefab Menu Item was added to the Prefab Menu Database" +
                "\n" +
                $"Item Path: {itemPath}"
            );

            EditorUtility.DisplayProgressBar(title, info, 0.8f);
            //CHECK IF - the system should re-generate the database helper class
            if (runHelperClassGenerator)
            {
                GenerateDatabaseHelperClass(saveAssets, refreshAssetDatabase);

                EditorUtility.ClearProgressBar();
                //STOP
                return;
            }

            EditorUtility.DisplayProgressBar(title, info, 0.9f);
            //SAVE & REFRESH ASSETS DATABASE if required
            SaveAndRefreshAssetDatabase(saveAssets, refreshAssetDatabase);
            EditorUtility.ClearProgressBar();
        }

        public void RefreshDatabase(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            //CLEAR DATABASE
            instance.database.Clear();

            //FIND the GUIDs for all ScriptableObjects of the given type
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UIMenuItem)}");

            //PROCESS ALL FOUND ASSETS (validate & add to database) 
            foreach (string guid in guids)
                RefreshDatabaseItem(AssetDatabase.LoadAssetAtPath<UIMenuItem>(AssetDatabase.GUIDToAssetPath(guid)));

            //MARK DATABASE as DIRTY
            EditorUtility.SetDirty(instance);

            //GENERATE DATABASE HELPER CLASS
            GenerateDatabaseHelperClass(saveAssets, refreshAssetDatabase);
        }

        private void GenerateDatabaseHelperClass(bool saveAssets, bool refreshAssetDatabase) =>
            UIMenuContextMenuGenerator.Run(saveAssets, refreshAssetDatabase);

        public static UIMenuItem GetMenuItem(string prefabTypeName, string prefabCategory, string prefabName)
        {
            instance.RemoveNullEntries();

            string cleanPrefabTypeName = prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            string cleanPrefabCategory = prefabCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
            string cleanPrefabName = prefabName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            foreach (UIMenuItem item in instance.database
                .Where(item => item.cleanPrefabTypeName.Equals(cleanPrefabTypeName))
                .Where(item => item.cleanPrefabCategory.Equals(cleanPrefabCategory))
                .Where(item => item.cleanPrefabName.Equals(cleanPrefabName)))
            {
                return item;
            }

            Debugger.LogWarning($"The '{prefabTypeName} - {prefabCategory} - {prefabName}' Prefab Menu Item was not found! Returned null.");

            return null;
        }

        public static List<UIMenuItem> GetCategoryMenuItems(string prefabTypeName, string prefabCategory)
        {
            instance.RemoveNullEntries();

            string cleanPrefabTypeName = prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            string cleanPrefabCategory = prefabCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();

            return
                instance.database
                    .Where(item => item.cleanPrefabTypeName.Equals(cleanPrefabTypeName))
                    .Where(item => item.cleanPrefabCategory.Equals(cleanPrefabCategory))
                    .OrderBy(item => item.cleanPrefabName)
                    .ToList();
        }

        private static void SaveAndRefreshAssetDatabase(bool save, bool refresh)
        {
            if (save)
                AssetDatabase.SaveAssets();

            if (refresh)
                AssetDatabase.Refresh();
        }

        public static List<string> GetPrefabTypes()
        {
            var list =
                instance.database
                    .Select(item => item.prefabTypeName)
                    .Distinct()
                    .ToList();
            list.Sort();
            
            //custom sort
            // - Component
            // - Container
            // - Content
            // - Layout
            // - Misc
            // - anything else
            {
                if (list.Contains("Misc"))
                {
                    list.Remove("Misc");
                    list.Insert(0, "Misc");
                }
                
                if (list.Contains("Layout"))
                {
                    list.Remove("Layout");
                    list.Insert(0, "Layout");
                }
                
                if (list.Contains("Content"))
                {
                    list.Remove("Content");
                    list.Insert(0, "Content");
                }

                if (list.Contains("Container"))
                {
                    list.Remove("Container");
                    list.Insert(0, "Container");
                }

                if (list.Contains("Component"))
                {
                    list.Remove("Component");
                    list.Insert(0, "Component");
                }
            }
            
            return list;
        }

        public static List<string> GetCategories(string prefabTypeName)
        {
            string cleanPrefabTypeName = prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            var list =
                instance.database
                    .Where(item => item.cleanPrefabTypeName.Equals(cleanPrefabTypeName))
                    .Select(item => item.prefabCategory)
                    .Distinct()
                    .ToList();
            list.Sort();
            return list;
        }

        public static List<string> GetPrefabNames(string prefabTypeName, string prefabCategory)
        {
            string cleanPrefabTypeName = prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            string cleanPrefabCategory = prefabCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();

            var list =
                instance.database
                    .Where(item => item.cleanPrefabTypeName.Equals(cleanPrefabTypeName))
                    .Where(item => item.cleanPrefabCategory.Equals(cleanPrefabCategory))
                    .Select(item => item.prefabName)
                    .Distinct()
                    .ToList();
            list.Sort();
            return list;
        }
    }
}
