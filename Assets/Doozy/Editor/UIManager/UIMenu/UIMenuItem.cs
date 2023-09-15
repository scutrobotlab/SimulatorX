// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.UIMenu
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/UI Menu/Menu Item"
        )
    ]
    public class UIMenuItem : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "Menu Item";

        public string cleanAssetName => $"{DEFAULT_ASSET_FILENAME} - {PrefabTypeName} - {PrefabCategory} - {PrefabName}";

        [SerializeField] private GameObject Prefab;
        public GameObject prefab => Prefab;

        [SerializeField] private UIPrefabType PrefabType;
        public UIPrefabType prefabType
        {
            get => PrefabType;
            set
            {
                PrefabType = value;
                if (PrefabType != UIPrefabType.Custom)
                    PrefabTypeName = value.ToString();
            }
        }

        [SerializeField] private string PrefabTypeName;
        public string prefabTypeName
        {
            get
            {
                if (PrefabType == UIPrefabType.Custom)
                    return PrefabTypeName;
                return PrefabType.ToString();
            }
            set
            {
                PrefabType = UIPrefabType.Custom;
                PrefabTypeName = value;
            }
        }

        [SerializeField] private string PrefabCategory;
        public string prefabCategory => PrefabCategory;

        [SerializeField] private string PrefabName;
        public string prefabName
        {
            get => PrefabName;
            set => PrefabName = value;
        }
        
        [SerializeField] private PrefabInstantiateMode InstantiateMode;
        public PrefabInstantiateMode instantiateMode => InstantiateMode;

        [SerializeField] private bool LockInstantiateMode;
        public bool lockInstantiateMode => LockInstantiateMode;

        [SerializeField] private bool Colorize = true;
        public bool colorize => Colorize;

        [SerializeField] private List<string> Tags;
        public List<string> tags => Tags;

        [SerializeField] private string InfoTag;
        public string infoTag => InfoTag;

        [SerializeField] private List<Texture2D> Icon;
        public List<Texture2D> icon => Icon;

        public string cleanPrefabTypeName => PrefabType != UIPrefabType.Custom ? PrefabType.ToString() : PrefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
        public string cleanPrefabCategory => PrefabCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
        public string cleanPrefabName => PrefabName.RemoveWhitespaces().RemoveAllSpecialCharacters();

        public bool isValid => hasPrefab && hasPrefabType && hasCategory && hasName;
        public bool hasPrefabType => PrefabType != UIPrefabType.Custom || !prefabTypeName.IsNullOrEmpty();
        public bool hasPrefab => Prefab != null;
        public bool hasCategory => !PrefabCategory.IsNullOrEmpty();
        public bool hasName => !PrefabName.IsNullOrEmpty();
        public bool hasIcon => Icon != null && Icon.Where(item => item != null).ToList().Count == 1;
        public bool hasAnimatedIcon => Icon != null && Icon.Where(item => item != null).ToList().Count > 1;
        public bool hasInfoTag => !InfoTag.IsNullOrEmpty();

        public UIMenuItem()
        {
            prefabType = UIPrefabType.Component;
        }

        public UIMenuItem Validate()
        {
            if (name.Equals(cleanAssetName))
                return this;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), cleanAssetName);
            EditorUtility.SetDirty(this);

            return this;
        }

        public UIMenuItem SortSpritesAz()
        {
            Icon = Icon.OrderBy(item => item.name).ToList();
            return this;
        }

        public UIMenuItem SortSpritesZa()
        {
            Icon = Icon.OrderByDescending(item => item.name).ToList();
            return this;
        }

        public UIMenuItem SortTagsAz()
        {
            Tags = Tags.OrderBy(tag => tag).ToList();
            return this;
        }

        public UIMenuItem SortTagsZa()
        {
            Tags = Tags.OrderByDescending(tag => tag).ToList();
            return this;
        }

        public void AddToScene()
        {
            UIMenuUtils.AddToScene(this);
        }
    }
}
