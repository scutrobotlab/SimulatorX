// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI.Automation.Generators;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Colors
{
    [Serializable]
    public class EditorDataColorDatabase : SingletonEditorScriptableObject<EditorDataColorDatabase>, IEditorDataDatabase
    {
        
        
        [SerializeField] private string DatabaseName = "Editor Colors";
        public string databaseName => DatabaseName;

        [SerializeField] private string DatabaseDescription = "Collections of Colors used in the Editor";
        public string databaseDescription => DatabaseDescription;

        public List<EditorDataColorPalette> Database =
            new List<EditorDataColorPalette>();

        private void RemoveNullEntries() =>
            Database = Database.Where(item => item != null).ToList();

        public void RefreshDatabaseItem(EditorDataColorPalette item, bool saveAssets = false, bool refreshAssetDatabase = false, bool runHelperClassGenerator = false)
        {
            string title = "Editor Colors Database";
            string info = $"Refreshing '{item.paletteName}'";

            EditorUtility.DisplayProgressBar(title, info, 0.1f);
            //CHECK IF the item exists in the database
            if (instance.Database.Contains(item))
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
            //VALIDATE item - make sure its setup is correct
            item.Validate();
            
            //NULL CHECK - if the item didn't delete itself due to bad setup (items are configured, in validate, to self-move-to-trash if they are invalid)
            if (item == null) return;

            //MARK DIRTY - the item
            EditorUtility.SetDirty(item);

            //MARK DIRTY - the database
            EditorUtility.SetDirty(instance);

            //SAVE ITEM PATH - get it from the asset
            string itemPath = AssetDatabase.GetAssetPath(item);

            EditorUtility.DisplayProgressBar(title, info, 0.6f);
            //CHECK if the item has a name - DO NOT ADD nameless items to the database as there is no way to find them after the fact and the generated database helper class will give errors
            if (item.paletteName.IsNullOrEmpty())
            {
                Debugger.LogWarning(
                    "'Cannot add an unnamed Color Palette to the Editor Colors Database." +
                    $"\nSkipped Palette Path: {itemPath}");

                //SAVE & REFRESH ASSETS DATABASE if required
                SaveAndRefreshAssetDatabase(saveAssets, refreshAssetDatabase);
                
                EditorUtility.ClearProgressBar();
                //STOP
                return;
            }

            EditorUtility.DisplayProgressBar(title, info, 0.7f);
            //CHECK FOR DUPLICATES - check if the database doesn't already contain an item with the same name
            EditorDataColorPalette foundItem = GetColorPalette(item.paletteName, true);
            if (foundItem != null)
            {
                Debugger.LogWarning(
                    $"'Cannot add another Color Palette named {foundItem.paletteName}, as one already exists in the Editor Colors Database." +
                    $"\nRegistered Palette Path: {AssetDatabase.GetAssetPath(foundItem)}" +
                    $"\nSkipped Palette Path: {itemPath}");

                //SAVE & REFRESH ASSETS DATABASE if required
                SaveAndRefreshAssetDatabase(saveAssets, refreshAssetDatabase);
                
                EditorUtility.ClearProgressBar();
                //STOP
                return;
            }

            //ADD ITEM to DATABASE - item passed all the tests thus it gets added to the database
            instance.Database.Add(item);

            Debugger.Log(
                $"'{item.paletteName}' Color Palette ({item.colors.Count} colors) was added to the Editor Colors Database." +
                $"\n Palette Path: {itemPath}");

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
            instance.Database.Clear();

            //CREATE DEFAULT PALETTE
            EditorDataColorPalette.RegenerateDefaultColorPalette();

            //FIND the GUIDs for all ScriptableObjects of the given type
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataColorPalette)}");

            //PROCESS ALL FOUND ASSETS (validate & add to database) 
            foreach (string guid in guids)
                RefreshDatabaseItem(AssetDatabase.LoadAssetAtPath<EditorDataColorPalette>(AssetDatabase.GUIDToAssetPath(guid)));

            //MARK DATABASE as DIRTY
            EditorUtility.SetDirty(instance);

            //GENERATE DATABASE HELPER CLASS
            GenerateDatabaseHelperClass(saveAssets, refreshAssetDatabase);
        }

        private void GenerateDatabaseHelperClass(bool saveAssets, bool refreshAssetDatabase) =>
            EditorColorsGenerator.Run(saveAssets, refreshAssetDatabase);

        internal static EditorDataColorPalette GetColorPalette(string paletteName, bool silent = false)
        {
            //clean database null entries
            instance.RemoveNullEntries();

            //remove whitespaces and any special characters from the name
            string cleanName = paletteName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            //search for the item with the 'cleanName'
            // foreach (EditorDataColorPalette palette in instance.Database.Where(item => item.paletteName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            foreach (EditorDataColorPalette palette in instance.Database.Where(item => item.paletteName.Equals(cleanName)))
                return palette;

            if (!silent) Debug.LogWarning($"Color Palette '{paletteName}' not found! Returned null");

            //return NULL <<< no item with the 'cleanName' was found in the database
            return null;
        }

        private static void SaveAndRefreshAssetDatabase(bool save, bool refresh)
        {
            if (save)
                AssetDatabase.SaveAssets();

            if (refresh)
                AssetDatabase.Refresh();
        }
    }
}
