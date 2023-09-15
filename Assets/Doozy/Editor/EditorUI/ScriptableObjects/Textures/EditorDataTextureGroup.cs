// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Textures
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/Texture Group"
        )
    ]
    public class EditorDataTextureGroup : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_TextureGroup";

        [SerializeField] private string GroupCategory;
        internal string groupCategory => GroupCategory;
        
        [SerializeField] private string GroupName;
        internal string groupName => GroupName;

        [SerializeField] private string RemovePrefixFromTextureName;
        internal string removePrefixFromTextureName => RemovePrefixFromTextureName;
        
        [SerializeField]
        private List<EditorTextureInfo> Textures = new List<EditorTextureInfo>();
        internal List<EditorTextureInfo> textures => Textures;

        internal void AddNewItem() =>
            Textures.Insert(0, new EditorTextureInfo());

        internal void RemoveNullEntries() =>
            Textures = Textures.Where(item => item != null && item.TextureReference != null).ToList();

        internal void SortByFileName() =>
            Textures = Textures.OrderBy(item => item.TextureReference.name).ToList();

        internal void RemoveDuplicates() =>
            Textures = Textures.GroupBy(item => item.TextureReference).Select(item => item.First()).ToList();

        internal Texture2D GetTexture(string textureName)
        {
            string cleanName = textureName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            
            // foreach (EditorTextureInfo textureInfo in textures.Where(ti => ti.TextureName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            foreach (EditorTextureInfo textureInfo in textures.Where(ti => ti.TextureName.Equals(cleanName)))
                return textureInfo.TextureReference;
            
            Debug.LogWarning($"Texture '{textureName}' not found! Returned null.");
            
            return null;
        }

        public void LoadTexturesFromFolder(bool saveAssets = true)
        {
            Textures.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.png", SearchOption.TopDirectoryOnly);
            
            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }
            
            TextureUtils.SetTextureSettingsToGUI(files);
            List<Texture2D> textures2D = TextureUtils.GetTextures2D(assetParentFolderPath);
            foreach (Texture2D texture2D in textures2D)
                Textures.Add(new EditorTextureInfo
                {
                    TextureReference = texture2D
                });

            Validate(saveAssets);

            Debugger.Log($"Found the '{groupCategory} > {groupName}' Texture Group ({textures.Count} textures)");
        }

        internal void Validate(bool saveAssets = true)
        {
            string path = AssetDatabase.GetAssetPath(this);
            string[] splitPath = path.Split('/');
            string folderName = splitPath[splitPath.Length - 2];

            GroupCategory = GroupCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
            GroupName = folderName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            
            AssetDatabase.RenameAsset(path, $"{DEFAULT_ASSET_FILENAME}_{GroupName}_{groupCategory}");

            Textures = Textures ?? new List<EditorTextureInfo>();
            
            RemoveNullEntries();
            RemoveDuplicates();
            
            foreach (EditorTextureInfo textureInfo in Textures)
            {
                string textureFileName = textureInfo.TextureReference.name;
                if (!RemovePrefixFromTextureName.IsNullOrEmpty())
                    textureFileName = textureFileName.Replace(RemovePrefixFromTextureName, "");
                textureInfo.TextureName = textureFileName;
                textureInfo.ValidateName();
            }

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }

    }


}
