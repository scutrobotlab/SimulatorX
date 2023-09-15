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

namespace Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/MicroAnimation Group"
        )
    ]
    public class EditorDataMicroAnimationGroup : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_MicroAnimationGroup";

        [SerializeField] private string GroupCategory;
        internal string groupCategory => GroupCategory;

        [SerializeField] private string GroupName;
        internal string groupName => GroupName;

        [SerializeField] private List<EditorMicroAnimationInfo> MicroAnimations = new List<EditorMicroAnimationInfo>();
        internal List<EditorMicroAnimationInfo> microAnimations => MicroAnimations;

        internal void AddNewItem() =>
            MicroAnimations.Insert(0, new EditorMicroAnimationInfo());

        internal void RemoveNullEntries() =>
            MicroAnimations = MicroAnimations.Where(item => item != null).ToList();

        internal void RemoveDuplicates() =>
            MicroAnimations = MicroAnimations.GroupBy(item => item.AnimationName).Select(item => item.First()).ToList();

        internal void SortByAnimationName() =>
            MicroAnimations = MicroAnimations.OrderBy(item => item.AnimationName).ToList();

        internal List<Texture2D> GetTextures(string animationName)
        {
            string cleanName = animationName.RemoveAllSpecialCharacters().RemoveWhitespaces();

            MicroAnimations = MicroAnimations.Where(item => item != null).ToList();

            // foreach (EditorMicroAnimationInfo animationInfo in MicroAnimations.Where(item => item.AnimationName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            foreach (EditorMicroAnimationInfo animationInfo in MicroAnimations.Where(item => item.AnimationName.Equals(cleanName)))
                return animationInfo.Textures.ToList();

            Debug.LogWarning($"MicroAnimation '{animationName}' not found! Returned null.");

            return null;
        }

        public void LoadAnimationsFromFolders(bool saveAssets = true)
        {
            EditorUtility.ClearProgressBar();
            string title = "Loading Animations";
            string info = string.Empty;
            
            
            MicroAnimations.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] subFolderPaths = Directory.GetDirectories(assetParentFolderPath);

            int subfoldersCount = subFolderPaths.Length;
            int processedFolders = 0;
            foreach (string subFolderPath in subFolderPaths)
            {
                float progress = 0.8f * Mathf.Lerp(0, subfoldersCount, processedFolders / (float) subfoldersCount);
                EditorUtility.DisplayProgressBar(title, subFolderPath, progress);
                processedFolders++;
                
                string[] files = Directory.GetFiles(subFolderPath, "*.png", SearchOption.TopDirectoryOnly);
                if (files.Length == 0) continue;
                TextureUtils.SetTextureSettingsToGUI(files, false);
                MicroAnimations.Add(
                    new EditorMicroAnimationInfo
                    {
                        Textures = TextureUtils.GetTextures2D(subFolderPath)
                    });
            }

            EditorUtility.DisplayProgressBar(title, info, 0.9f);

            if (MicroAnimations.Count == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                
                EditorUtility.ClearProgressBar();
                //STOP
                return;
            }

            Validate(saveAssets);

            Debugger.Log($"Loaded [{groupCategory}][{groupName}] with {microAnimations.Count} micro-animations");
            
            EditorUtility.ClearProgressBar();
        }

        internal void Validate(bool saveAssets = true)
        {
            string path = AssetDatabase.GetAssetPath(this);
            string[] splitPath = path.Split('/');
            string folderName = splitPath[splitPath.Length - 2];

            GroupCategory = GroupCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
            GroupName = folderName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            AssetDatabase.RenameAsset(path, $"{DEFAULT_ASSET_FILENAME}_{GroupName}_{groupCategory}");

            if (MicroAnimations == null) 
                MicroAnimations = new List<EditorMicroAnimationInfo>();
            
            MicroAnimations = MicroAnimations.Where(item => item != null).ToList();
            
            foreach (EditorMicroAnimationInfo animationInfo in MicroAnimations)
                animationInfo.Textures = animationInfo.Textures.Where(item => item != null).ToList();
            
            MicroAnimations = MicroAnimations.Where(item => item.Textures != null && item.Textures.Count > 0).ToList();

            foreach (EditorMicroAnimationInfo animationInfo in MicroAnimations)
            {
                string[] firstTextureSplitPath = AssetDatabase.GetAssetPath(animationInfo.Textures[0]).Split('/');
                animationInfo.AnimationName = firstTextureSplitPath[firstTextureSplitPath.Length - 2];
                animationInfo.ValidateName();
            }

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }

    }
}
