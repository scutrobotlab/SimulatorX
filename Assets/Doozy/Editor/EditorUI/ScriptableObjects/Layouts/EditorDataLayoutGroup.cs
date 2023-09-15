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

namespace Doozy.Editor.EditorUI.ScriptableObjects.Layouts
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/Layout Group (UXML)"
        )
    ]
    public class EditorDataLayoutGroup : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_LayoutGroup";

        [SerializeField] private string GroupName;
        internal string groupName => GroupName;

        [SerializeField] private List<EditorLayoutInfo> Layouts = new List<EditorLayoutInfo>();
        internal List<EditorLayoutInfo> layouts => Layouts;

        internal void AddNewItem() =>
            Layouts.Insert(0, new EditorLayoutInfo());

        internal void SortByFileName()
        {
            Layouts = Layouts.Where(item => item != null && item.UxmlReference != null).ToList();
            Layouts = Layouts.OrderBy(item => item.UxmlReference.name).ToList();
        }

        internal VisualTreeAsset GetVisualTreeAsset(string layoutName)
        {
            string cleanName = layoutName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            Layouts = Layouts.Where(item => item != null && item.UxmlReference != null).ToList();

            foreach (EditorLayoutInfo layoutInfo in Layouts.Where(item => item.UxmlReference.name.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
                return layoutInfo.UxmlReference;

            Debug.LogWarning($"UXML Layout '{layoutName}' not found! Returned null");
            return null;
        }

        public void LoadUxmlReferencesFromFolder(bool saveAssets = true)
        {
            Layouts.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.uxml", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            foreach (string filePath in files)
            {
                VisualTreeAsset reference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(filePath);
                if (reference == null) continue;
                Layouts.Add(new EditorLayoutInfo { UxmlReference = reference });
            }

            Validate(saveAssets);

            Debugger.Log($"Found the '{groupName}' UXML Layout Group ({Layouts.Count} layouts)");
        }

        internal void Validate(bool saveAssets = true)
        {
            Layouts = Layouts.Where(item => item != null && item.UxmlReference != null).ToList();
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (Layouts == null || Layouts.Count == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }
            
            GroupName = GroupName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            AssetDatabase.RenameAsset(assetPath, $"{DEFAULT_ASSET_FILENAME}_{GroupName}");

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }
    }
}
