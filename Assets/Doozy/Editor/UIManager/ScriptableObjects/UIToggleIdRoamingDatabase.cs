// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.ScriptableObjects
{
    [Serializable]
    public class UIToggleIdRoamingDatabase: ScriptableObject
    {
        [SerializeField] private string Name;
        public string databaseName
        {
            get => Name;
            internal set => Name = value.RemoveWhitespaces().RemoveAllSpecialCharacters();
        }

        [SerializeField] private UIToggleIdDataGroup Database;
        public UIToggleIdDataGroup database => Database ??= new UIToggleIdDataGroup();
        
        public void Validate()
        {
            string initialName = Name;
            string initialFilename = name;
            databaseName = Name;
            name = $"{Name}_{nameof(UIToggleIdRoamingDatabase)}";

            if(initialName.Equals(Name) & initialFilename.Equals(name))
                return;
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
