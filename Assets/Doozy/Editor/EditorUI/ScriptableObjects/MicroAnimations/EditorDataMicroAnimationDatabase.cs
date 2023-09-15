// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI.Automation.Generators;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations
{
    public class EditorDataMicroAnimationDatabase : SingletonEditorScriptableObject<EditorDataMicroAnimationDatabase>, IEditorDataDatabase
    {
        [SerializeField] private string DatabaseName = "Editor MicroAnimations";
        public string databaseName => DatabaseName;

        [SerializeField] private string DatabaseDescription = "Collection of MicroAnimations used in the Editor";
        public string databaseDescription => DatabaseDescription;

        public List<EditorDataMicroAnimationGroup> Database =
            new List<EditorDataMicroAnimationGroup>();

        private void RemoveNullEntries() =>
            Database = Database.Where(item => item != null).ToList();

        public void RefreshDatabaseItem(EditorDataMicroAnimationGroup item, bool saveAssets = false, bool refreshAssetDatabase = false, bool runHelperClassGenerator = false)
        {
            string title = "Editor Micro-Animations Database";
            string info = $"Refreshing '{item.groupCategory} - {item.groupName}'";

            EditorUtility.DisplayProgressBar(title, info, 0.1f);
            //VALIDATE item - make sure its setup is correct
            item.Validate();
            
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
            //LOAD ALL animations found in the sub-folders of the same folder as the ITEM
            item.LoadAnimationsFromFolders(false);

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
            instance.Database.Add(item);

            Debugger.Log
            (
                $"'{item.groupCategory} > {item.groupName}' Micro-Animation Group ({item.microAnimations.Count} animations) was added to the Editor Micro-Animation Database" +
                "\n" +
                $"Group Path: {itemPath}"
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
            instance.Database.Clear();

            //FIND the GUIDs for all ScriptableObjects of the given type
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataMicroAnimationGroup)}");

            //PROCESS ALL FOUND ASSETS (validate & add to database) 
            foreach (string guid in guids)
                RefreshDatabaseItem(AssetDatabase.LoadAssetAtPath<EditorDataMicroAnimationGroup>(AssetDatabase.GUIDToAssetPath(guid)));

            //MARK DATABASE as DIRTY
            EditorUtility.SetDirty(instance);

            //GENERATE DATABASE HELPER CLASS
            GenerateDatabaseHelperClass(saveAssets, refreshAssetDatabase);
        }

        private void GenerateDatabaseHelperClass(bool saveAssets, bool refreshAssetDatabase) =>
            EditorMicroAnimationsGenerator.Run(saveAssets, refreshAssetDatabase);

        internal static EditorDataMicroAnimationGroup GetMicroAnimationGroup(string groupCategory, string groupName)
        {
            instance.RemoveNullEntries();

            string cleanCategoryName = groupCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
            string cleanGroupName = groupName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            foreach (EditorDataMicroAnimationGroup animationGroup in instance.Database
                // .Where(item => item.groupCategory.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanCategoryName))
                .Where(item => item.groupCategory.Equals(cleanCategoryName))
                // .Where(item => item.groupName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanGroupName)))
                .Where(item => item.groupName.Equals(cleanGroupName)))
            {
                return animationGroup;
            }

            Debugger.LogWarning($"MicroAnimation Group '{groupName}' not found in the '{groupCategory}' category! Returned null.");

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
