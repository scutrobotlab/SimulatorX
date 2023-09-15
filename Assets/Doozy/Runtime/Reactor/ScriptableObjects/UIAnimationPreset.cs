// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.ScriptableObjects.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects
{
    // [
    //     CreateAssetMenu
    //     (
    //         fileName = DEFAULT_ASSET_FILENAME,
    //         menuName = "Doozy/Reactor/UIAnimation Preset"
    //     )
    // ]
    [Serializable]
    public class UIAnimationPreset : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "AnimationPreset";
        private static string dataFolderPath => $"{RuntimePath.path}/Data/UIAnimationPresets";
        private string dataFileName => DataFileName(animationType, category, presetName);
        private string dataFilePath => $"{dataFolderPath}/{dataFileName}";

        private const string INITIAL_CATEGORY_NAME = "PresetCategory";
        private const string INITIAL_PRESET_NAME = "PresetName";

        public UIAnimationType animationType => Settings.animationType;

        [SerializeField] private string Category = INITIAL_CATEGORY_NAME;
        public string category
        {
            get => Category;
            set => UpdateCategory(value);
        }

        [SerializeField] private string PresetName = INITIAL_PRESET_NAME;
        public string presetName
        {
            get => PresetName;
            set => UpdatePresetName(value);
        }

        [SerializeField] private UIAnimationSettings Settings;
        public UIAnimationSettings settings => Settings;

        public UIAnimationPreset() : this(UIAnimationType.Custom) {}

        public UIAnimationPreset(UIAnimationType animationType, string category = INITIAL_CATEGORY_NAME, string presetName = INITIAL_PRESET_NAME)
        {
            Settings = new UIAnimationSettings(animationType);
            this.category = category;
            this.presetName = presetName;
        }

        public UIAnimationPreset(UIAnimation source, string category = INITIAL_CATEGORY_NAME, string presetName = INITIAL_PRESET_NAME)
        {
            Settings = new UIAnimationSettings(source);
            this.category = category;
            this.presetName = presetName;
        }

        public void CleanCategory() =>
            category = category;

        public void CleanPresetName() =>
            presetName = presetName;

        public static string CleanString(string value) =>
            value.RemoveWhitespaces().RemoveAllSpecialCharacters();

        private void UpdateCategory(string value, bool updateAssetFileName = false)
        {
            value = CleanString(value);
            if (value.IsNullOrEmpty()) return;
            Category = value;
            if (updateAssetFileName) UpdateAssetFileName();
        }

        private void UpdatePresetName(string value, bool updateAssetFileName = false)
        {
            value = CleanString(value);
            if (value.IsNullOrEmpty()) return;
            PresetName = value;
            if (updateAssetFileName) UpdateAssetFileName();
        }

        private void UpdateAssetFileName()
        {
            #if UNITY_EDITOR
            name = dataFileName;
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        public UIAnimationPreset RenamePreset(string newCategory, string newPresetName)
        {
            category = newCategory;
            presetName = newPresetName;
            UpdateAssetFileName();
            return this;
        }

        public UIAnimationPreset SetCategory(string value)
        {
            category = value;
            return this;
        }

        public UIAnimationPreset SetPresetName(string value)
        {
            presetName = value;
            return this;
        }

        public UIAnimationPreset GetAnimationSettings(UIAnimation source)
        {
            Settings.GetAnimationSettings(source);
            return this;
        }

        public UIAnimationPreset SetAnimationSettings(UIAnimation target)
        {
            Settings.SetAnimationSettings(target);
            return this;
        }

        public static UIAnimationPreset NewPreset(UIAnimation source, string category, string presetName, string path = "")
        {
            #if UNITY_EDITOR
            {
                bool canAddPreset;
                string message;
                (canAddPreset, message) = UIAnimationPresetDatabase.instance.CanAddPreset(source.animationType, category, presetName);
                if (!canAddPreset)
                {
                    Debug.Log(message);
                    return null;
                }

                UIAnimationPreset preset = CreateInstance<UIAnimationPreset>();
                preset.Settings = new UIAnimationSettings(source);
                preset.category = category;
                preset.presetName = presetName;
                UnityEditor.AssetDatabase.CreateAsset(preset, path.IsNullOrEmpty() ? preset.dataFilePath : path);
                UIAnimationPresetDatabase.instance.AddPreset(preset);
                return preset;
            }
            #else
            {
                Debug.LogWarning($"Unable to execute. A new preset can only be created in the Unity Editor");
                return null;
            }
            #endif
        }

        internal static UIAnimationPreset NewDefaultPreset(UIAnimationType animationType)
        {
            #if UNITY_EDITOR
            {
                UIAnimationPreset preset = CreateInstance<UIAnimationPreset>();
                preset.Settings = new UIAnimationSettings(animationType);
                preset.category = UIAnimationPresetGroup.defaultCategoryName;
                preset.presetName = UIAnimationPresetGroup.defaultPresetName;

                switch (animationType)
                {
                    case UIAnimationType.Show:
                        preset.settings.MoveEnabled = true;
                        preset.settings.MoveFromReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveFromDirection = MoveDirection.Left;
                        preset.settings.MoveToDirection = MoveDirection.CustomPosition;
                        break;
                    case UIAnimationType.Hide:
                        preset.settings.MoveEnabled = true;
                        preset.settings.MoveFromReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveFromDirection = MoveDirection.CustomPosition;
                        preset.settings.MoveToDirection = MoveDirection.Right;
                        break;
                    case UIAnimationType.Loop:
                        preset.settings.MoveEnabled = true;
                        preset.settings.MoveFromReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveFromOffset = new Vector3(0, -5, 0);
                        preset.settings.MoveToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveToOffset = new Vector3(0, 5, 0);
                        preset.settings.MoveFromDirection = MoveDirection.CustomPosition;
                        preset.settings.MoveToDirection = MoveDirection.CustomPosition;
                        preset.settings.MoveReactionSettings.loops = -1;
                        preset.settings.MoveReactionSettings.playMode = PlayMode.PingPong;
                        preset.settings.MoveReactionSettings.ease = Ease.InOutSine;
                        break;
                    case UIAnimationType.Custom:
                        preset.settings.MoveEnabled = true;
                        preset.settings.MoveFromReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveFromOffset = new Vector3(-20, 0, 0);
                        preset.settings.MoveToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveToOffset = new Vector3(20, 0, 0);
                        preset.settings.MoveFromDirection = MoveDirection.CustomPosition;
                        preset.settings.MoveToDirection = MoveDirection.CustomPosition;
                        break;
                    case UIAnimationType.Button:
                        const float buttonDuration = 0.3f;
                        preset.settings.ScaleEnabled = true;
                        preset.settings.ScaleFromReferenceValue = ReferenceValue.StartValue;
                        preset.settings.ScaleToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.ScaleToOffset = new Vector3(-0.2f, -0.2f, 0f);
                        preset.settings.ScaleReactionSettings.duration = buttonDuration;
                        preset.settings.ScaleReactionSettings.playMode = PlayMode.PingPong;
                        break;
                    case UIAnimationType.State:
                        const float stateDuration = 0.3f;
                        preset.settings.MoveEnabled = true;
                        preset.settings.MoveFromReferenceValue = ReferenceValue.CurrentValue;
                        preset.settings.MoveToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveToOffset = new Vector3(0, 5, 0);
                        preset.settings.MoveReactionSettings.duration = stateDuration;
                        preset.settings.MoveReactionSettings.ease = Ease.OutBack;
                        break;
                    case UIAnimationType.Reset:
                        const float resetDuration = 0.3f;
                        preset.settings.MoveEnabled = true;
                        preset.settings.MoveFromReferenceValue = ReferenceValue.CurrentValue;
                        preset.settings.MoveToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.MoveReactionSettings.duration = resetDuration;

                        preset.settings.RotateEnabled = true;
                        preset.settings.RotateFromReferenceValue = ReferenceValue.CurrentValue;
                        preset.settings.RotateToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.RotateReactionSettings.duration = resetDuration;

                        preset.settings.ScaleEnabled = true;
                        preset.settings.ScaleFromReferenceValue = ReferenceValue.CurrentValue;
                        preset.settings.ScaleToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.ScaleReactionSettings.duration = resetDuration;

                        preset.settings.FadeEnabled = true;
                        preset.settings.FadeFromReferenceValue = ReferenceValue.CurrentValue;
                        preset.settings.FadeToReferenceValue = ReferenceValue.StartValue;
                        preset.settings.FadeReactionSettings.duration = resetDuration;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(animationType), animationType, null);
                }

                UnityEditor.AssetDatabase.CreateAsset(preset, preset.dataFilePath);
                return preset;
            }
            #else
            {
                Debug.LogWarning($"Unable to execute. A new preset can only be created in the Unity Editor");
                return null;
            }
            #endif
        }

        /// <summary> {animationType}_{category}_{presetName}.asset </summary>
        public static string DataFileName(UIAnimationType animationType, string category, string presetName, string extension = ".asset") =>
            $"{animationType}_{category}_{presetName}{extension}";
    }
}
