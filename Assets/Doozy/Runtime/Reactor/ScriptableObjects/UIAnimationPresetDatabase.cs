// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.ScriptableObjects.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects
{
    [Serializable]
    public class UIAnimationPresetDatabase : ScriptableObject
    {
        private static string fileName => $"{nameof(UIAnimationPresetDatabase)}";
        private static string assetFileName => $"{fileName}.asset";
        // private static string assetFolderPath => $"{RuntimePath.path}/_Data/Resources/";
        private static string assetFolderPath => $"{RuntimePath.path}/Data";
        private static string assetFilePath => $"{assetFolderPath}/{assetFileName}";
        
        private static UIAnimationPresetDatabase s_instance;

        public static UIAnimationPresetDatabase instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                // s_instance = Resources.Load<UIAnimationPresetDatabase>(fileName);
                // if (s_instance != null) return s_instance;
                #if UNITY_EDITOR
                {
                    s_instance = UnityEditor.AssetDatabase.LoadAssetAtPath<UIAnimationPresetDatabase>(assetFilePath);
                    if (s_instance != null) return s_instance;
                }
                #endif
                s_instance = CreateInstance<UIAnimationPresetDatabase>();
                #if UNITY_EDITOR
                {
                    UnityEditor.AssetDatabase.CreateAsset(s_instance, assetFilePath);
                }
                #endif
                return s_instance;
            }
        }
        
        public static string defaultCategoryName => UIAnimationPresetGroup.defaultCategoryName;
        public static string defaultPresetName => UIAnimationPresetGroup.defaultPresetName;

        [SerializeField] private UIAnimationPresetGroup ShowPresets = new UIAnimationPresetGroup(UIAnimationType.Show);
        public UIAnimationPresetGroup showPresets => ShowPresets;

        [SerializeField] private UIAnimationPresetGroup HidePresets = new UIAnimationPresetGroup(UIAnimationType.Hide);
        public UIAnimationPresetGroup hidePresets => HidePresets;
        
        [SerializeField] private UIAnimationPresetGroup LoopPresets = new UIAnimationPresetGroup(UIAnimationType.Loop);
        public UIAnimationPresetGroup loopPresets => LoopPresets;
        
        [SerializeField] private UIAnimationPresetGroup ButtonPresets = new UIAnimationPresetGroup(UIAnimationType.Button);
        public UIAnimationPresetGroup buttonPresets => ButtonPresets;
        
        [SerializeField] private UIAnimationPresetGroup StatePresets = new UIAnimationPresetGroup(UIAnimationType.State);
        public UIAnimationPresetGroup statePresets => StatePresets;
        
        [SerializeField] private UIAnimationPresetGroup ResetPresets = new UIAnimationPresetGroup(UIAnimationType.Reset);
        public UIAnimationPresetGroup resetPresets => ResetPresets;
        
        [SerializeField] private UIAnimationPresetGroup CustomPresets = new UIAnimationPresetGroup(UIAnimationType.Custom);
        public UIAnimationPresetGroup customPresets => CustomPresets;


        public UIAnimationPresetGroup GetPresetGroup(UIAnimationType animationType)
        {
            switch (animationType)
            {
                case UIAnimationType.Show: return showPresets;
                case UIAnimationType.Hide: return hidePresets;
                case UIAnimationType.Loop: return loopPresets;
                case UIAnimationType.Button: return buttonPresets;
                case UIAnimationType.State: return statePresets;
                case UIAnimationType.Reset: return resetPresets;
                case UIAnimationType.Custom: return customPresets;
                default: throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
            }
        }

        public void RefreshDatabase(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            #if UNITY_EDITOR
            {
                const string title = "Reactor - Refreshing UIAnimation Presets";
                UnityEditor.EditorUtility.DisplayProgressBar(title, "Initializing...", 0f);
                showPresets.Clear();
                hidePresets.Clear();
                loopPresets.Clear();
                buttonPresets.Clear();
                statePresets.Clear();
                resetPresets.Clear();
                customPresets.Clear();

                UnityEditor.EditorUtility.DisplayProgressBar(title, "Searching for preset files...", 0.1f);
                //FIND the GUIDs for all ScriptableObjects of the given type
                string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(UIAnimationPreset)}");

                //PROCESS ALL FOUND ASSETS (validate & add) 
                int foundPresetsCount = guids.Length;
                for (int i = 0; i < foundPresetsCount; i++)
                {
                    string guid = guids[i];
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    UnityEditor.EditorUtility.DisplayProgressBar(title, $"{assetPath}", 0.2f + 0.6f * ((i + 1f)/foundPresetsCount));
                    AddPreset(UnityEditor.AssetDatabase.LoadAssetAtPath<UIAnimationPreset>(assetPath), false, true);
                }

                UnityEditor.EditorUtility.DisplayProgressBar(title, "Validating...", 0.8f);
                Validate();
                UnityEditor.EditorUtility.DisplayProgressBar(title, "Sorting...", 0.9f);
                Sort();

                //MARK DATABASE as DIRTY
                UnityEditor.EditorUtility.SetDirty(instance);

                UnityEditor.EditorUtility.DisplayProgressBar(title, "Saving...", 1f);
                if (saveAssets) UnityEditor.AssetDatabase.SaveAssets();
                if (refreshAssetDatabase) UnityEditor.AssetDatabase.Refresh();
                UnityEditor.EditorUtility.ClearProgressBar();
            }
            #endif
        }

        public UIAnimationPresetDatabase Validate()
        {
            ShowPresets.Validate();
            HidePresets.Validate();
            LoopPresets.Validate();
            ButtonPresets.Validate();
            StatePresets.Validate();
            ResetPresets.Validate();
            CustomPresets.Validate();
            return this;
        }

        public UIAnimationPresetDatabase Sort()
        {
            ShowPresets.Sort();
            HidePresets.Sort();
            LoopPresets.Sort();
            ButtonPresets.Sort();
            StatePresets.Sort();
            ResetPresets.Sort();
            CustomPresets.Sort();
            return this;
        }

        public (bool, string) CanAddPreset(UIAnimationType animationType, string category, string presetName) =>
            GetPresetGroup(animationType).CanAddPreset(animationType, category, presetName);

        public void AddPreset(UIAnimationPreset preset, bool saveAssets = true, bool allowDefaultPresets = false)
        {
            if (preset == null) return;
            if (ContainsPreset(preset)) return;
            if (!ValidatePreset(preset)) return;
            GetPresetGroup(preset.animationType).AddPreset(preset, false, false, allowDefaultPresets);
            #if UNITY_EDITOR
            {
                UnityEditor.EditorUtility.SetDirty(this);
                if (saveAssets) UnityEditor.AssetDatabase.SaveAssets();
            }
            #endif
        }

        public UIAnimationPreset GetDefaultPreset(UIAnimationType animationType) =>
            GetPresetGroup(animationType).GetPreset(defaultCategoryName, defaultPresetName);
        
        public UIAnimationPreset GetPreset(UIAnimationType animationType, string category, string presetName) =>
            GetPresetGroup(animationType).GetPreset(category, presetName);

        public bool ContainsPreset(UIAnimationType animationType, string category, string presetName) =>
            GetPresetGroup(animationType).Contains(category, presetName);

        public bool ContainsPreset(UIAnimationPreset preset) =>
            preset != null && GetPresetGroup(preset.animationType).Contains(preset);

        public bool RemovePreset(UIAnimationType animationType, string category, string presetName) =>
            GetPresetGroup(animationType).RemovePreset(category, presetName);

        public bool RemovePreset(UIAnimationPreset preset) =>
            preset != null && GetPresetGroup(preset.animationType).RemovePreset(preset);

        private bool ValidatePreset(UIAnimationPreset preset)
        {
            if (!(preset.category.IsNullOrEmpty() | preset.presetName.IsNullOrEmpty()))
                return true;
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.MoveAssetToTrash(UnityEditor.AssetDatabase.GetAssetPath(preset));
            #endif
            return false;
        }
    }
}
