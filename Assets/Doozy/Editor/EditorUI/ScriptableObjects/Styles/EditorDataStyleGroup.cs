// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Styles
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/Style Group (USS)"
        )
    ]
    public class EditorDataStyleGroup : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_StyleGroup";

        [SerializeField] private string GroupName;
        internal string groupName => GroupName;

        [SerializeField] private List<EditorStyleInfo> Styles = new List<EditorStyleInfo>();
        internal List<EditorStyleInfo> styles => Styles;

        internal void AddNewItem() =>
            Styles.Insert(0, new EditorStyleInfo());
        
        internal void SortByFileName() =>
            Styles = Styles.OrderBy(item => item.UssReference.name).ToList();
        
        internal StyleSheet GetStyleSheet(string styleName)
        {
            string cleanName = styleName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            Styles = Styles.Where(item => item != null && item.UssReference != null).ToList();
            
            foreach (EditorStyleInfo styleInfo in Styles.Where(item => item.UssReference.name.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
                return styleInfo.UssReference;

            Debug.LogWarning($"USS Style '{styleName}' not found! Returned null");
            return null;
        }

        public void LoadUssReferencesFromFolder(bool saveAssets = true)
        {
            Styles.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.uss", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            foreach (string filePath in files)
            {
                StyleSheet reference = AssetDatabase.LoadAssetAtPath<StyleSheet>(filePath);
                if (reference == null) continue;
                Styles.Add(new EditorStyleInfo { UssReference = reference });
            }

            Validate(saveAssets);

            Debugger.Log($"Found the '{groupName}' USS Style Group ({Styles.Count} style sheets)");
        }

        internal void Validate(bool saveAssets = true)
        {
            Styles = Styles.Where(item => item != null && item.UssReference != null).ToList();
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (Styles == null || Styles.Count == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }
            
            string tempName = GroupName;
            GroupName = GroupName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            if (tempName != GroupName)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }

            AssetDatabase.RenameAsset(assetPath, $"{DEFAULT_ASSET_FILENAME}_{GroupName}");

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();

        }
    }
}
