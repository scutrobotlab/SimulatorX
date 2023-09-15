// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Reactor.ScriptableObjects.Internal
{
    [Serializable]
    public class UIAnimationPresetGroup
    {
        public static string defaultCategoryName => "Default";
        public static string defaultPresetName => "Default";

        [SerializeField] private UIAnimationType GroupAnimationType;

        [SerializeField] private List<UIAnimationPreset> Presets = new List<UIAnimationPreset>();
        public List<UIAnimationPreset> presets => Presets;

        [SerializeField] private List<string> CategoryNames = new List<string>();
        public List<string> categoryNames => CategoryNames;

        [SerializeField] private List<PresetCategory> PresetCategories = new List<PresetCategory>();
        public List<PresetCategory> presetCategories => PresetCategories;


        public UIAnimationPresetGroup(UIAnimationType animationType) =>
            GroupAnimationType = animationType;

        public UIAnimationPresetGroup AddPreset(UIAnimationPreset preset, bool validate = false, bool sort = false, bool allowDefaultPresets = false)
        {
            bool canAddPreset;
            string message;
            (canAddPreset, message) = CanAddPreset(preset, allowDefaultPresets);
            if (!canAddPreset)
            {
                Debug.Log(message);
                return this;
            }

            Presets.Add(preset);
            PresetCategory category = GetCategory(preset.category);
            if (category != null)
            {
                category.AddName(preset.presetName);
            }
            else
            {
                PresetCategories.Add(new PresetCategory(preset.category).AddName(preset.presetName));
                CategoryNames.Add(preset.category);
            }

            if (validate) Validate();
            if (sort) Sort();
            return this;
        }

        private bool ContainsCategory(string categoryName) =>
            PresetCategories.Any(c => c.Category.Equals(categoryName.RemoveAllSpecialCharacters().RemoveWhitespaces()));

        public PresetCategory GetCategory(string categoryName) =>
            PresetCategories.FirstOrDefault(presetCategory => presetCategory.Category.Equals(categoryName));

        public List<string> GetPresetNames(string categoryName)
        {
            foreach (PresetCategory presetCategory in PresetCategories)
            {
                if (presetCategory.Category.Equals(categoryName))
                    return presetCategory.Names;
            }
            return null;
        }

        internal void AddDefaultPreset()
        {
            if (Contains(defaultCategoryName, defaultPresetName)) return;
            Presets.Add(UIAnimationPreset.NewDefaultPreset(GroupAnimationType));
            PresetCategories.Add(new PresetCategory(defaultCategoryName).AddName(defaultPresetName));
            CategoryNames.Add(defaultCategoryName);
            Validate(false);
            Sort();
        }

        public void RemoveCategory(string category)
        {
            PresetCategories = PresetCategories.Where(pc => pc.Category != category).ToList();
            CategoryNames.Remove(category);
        }

        public bool RemovePreset(UIAnimationPreset preset)
        {
            if (preset == null) return false;
            if (!Contains(preset)) return false;
            Presets.Remove(preset);
            if (!ContainsCategory(preset.category)) return true;
            PresetCategory presetCategory = GetCategory(preset.category);
            presetCategory.Names.Remove(preset.presetName);
            if (presetCategory.Names.Count == 0) RemoveCategory(presetCategory.Category);
            return true;
        }

        public bool RemovePreset(string category, string presetName) =>
            RemovePreset(GetPreset(category, presetName));

        public UIAnimationPreset GetPreset(string category, string presetName) =>
            Presets
                .Where(p => p.category.Equals(category))
                .FirstOrDefault(p => p.presetName.Equals(presetName));

        public UIAnimationPresetGroup Clear()
        {
            Presets.Clear();
            PresetCategories.Clear();
            CategoryNames.Clear();
            return this;
        }

        public bool Contains(UIAnimationPreset preset) =>
            Presets.Contains(preset);

        public bool Contains(string category, string presetName) =>
            GetPreset(category, presetName) != null;

        public (bool, string) CanAddPreset(UIAnimationPreset preset, bool allowDefaultPresets = false)
        {
            if (preset == null) return (false, $"{nameof(preset)} is null");
            preset.CleanCategory();
            preset.CleanPresetName();
            if (Presets.Contains(preset)) return (false, $"Preset already exists in the database");
            return CanAddPreset(preset.animationType, preset.category, preset.presetName, allowDefaultPresets);
        }

        public (bool, string) CanAddPreset(UIAnimationType animationType, string category, string presetName, bool allowDefaultPresets = false)
        {
            category = UIAnimationPreset.CleanString(category);
            presetName = UIAnimationPreset.CleanString(presetName);

            if (animationType != GroupAnimationType) return (false, $"Preset AnimationType: '{animationType}' is different than the Preset Group AnimationType: '{GroupAnimationType}'");
            if (category.IsNullOrEmpty()) return (false, $"Category cannot be null or empty");
            if (presetName.IsNullOrEmpty()) return (false, $"Preset name cannot be null or empty");

            if (!allowDefaultPresets)
            {
                if (category.Equals(defaultCategoryName)) return (false, $"Cannot add any presets to the '{defaultCategoryName}' category");
                if (presetName.Equals(defaultPresetName)) return (false, $"Cannot use '{defaultPresetName}' as a preset name");
            }

            foreach (UIAnimationPreset p in Presets)
            {
                if (!p.category.Equals(category)) continue;
                if (!p.presetName.Equals(presetName)) continue;
                return (false, $"Another preset with the '{presetName}' name already exists in the '{category}' category. Change the preset name and/or the category and try again.");
            }
            return (true, "Preset can be added to this group");
        }

        public UIAnimationPresetGroup Validate(bool addDefaultPreset = true)
        {
            if (addDefaultPreset) AddDefaultPreset();

            Presets = Presets.Distinct().Where(p => p != null).ToList();
            PresetCategories = PresetCategories.Where(pc => pc.Names.Count > 0).ToList();
            CategoryNames.Clear();
            foreach (PresetCategory presetCategory in PresetCategories)
                CategoryNames.Add(presetCategory.Category);

            return this;
        }
        public UIAnimationPresetGroup Sort()
        {
            Presets = Presets.OrderBy(p => p.category).ThenBy(p => p.presetName).ToList();

            foreach (PresetCategory presetCategory in presetCategories)
                presetCategory.Names.Sort();

            CategoryNames.Sort();

            return this;
        }

    }
}
