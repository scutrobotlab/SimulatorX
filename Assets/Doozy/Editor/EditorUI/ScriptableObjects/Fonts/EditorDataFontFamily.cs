// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Enums;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.Fonts
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/Font Family"
        )
    ]
    public class EditorDataFontFamily : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_FontFamily";

        [SerializeField] private string FontName;
        internal string fontName => FontName;

        [SerializeField] private List<EditorFontInfo> Fonts = new List<EditorFontInfo>();
        internal List<EditorFontInfo> fonts => Fonts;

        internal void SortFontsByWeight() =>
            Fonts = Fonts.OrderBy(fi => (int)fi.Weight).ToList();
        
        internal Font GetFont(int weightValue) =>
            GetFont((GenericFontWeight)weightValue);

        private Font GetFont(GenericFontWeight weight)
        {
            Fonts = Fonts.Where(item => item != null && item.FontReference != null).ToList();
            
            foreach (EditorFontInfo fontInfo in Fonts.Where(fi => fi.Weight == weight))
                return fontInfo.FontReference;
            
            Debug.LogWarning($"Font '{fontName}-{weight}' not found! Returned default Unity font");
            
            return DesignUtils.unityDefaultFont;
        }

        public void LoadFontsFromFolder(bool saveAssets = true)
        {
            Fonts.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.ttf", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            foreach (string filePath in files)
            {
                Font reference = AssetDatabase.LoadAssetAtPath<Font>(filePath);
                if (reference == null) continue;
                Fonts.Add(new EditorFontInfo { FontReference = reference });
            }

            EditorUtility.SetDirty(this);
            Validate(saveAssets);

            string weights =
                fonts
                    .Aggregate(string.Empty, (current, fontInfo) => current + $"{fontInfo.Weight}, ")
                    .TrimEnd(' ', ',');

            Debugger.Log($"Found the '{FontName}' Font ({fonts.Count} weights: {weights})");
        }

        internal void Validate(bool saveAssets = true)
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            string[] splitPath = assetPath.Split('/');
            string assetParentFolderName = splitPath[splitPath.Length - 2];

            FontName = assetParentFolderName.RemoveAllSpecialCharacters().RemoveWhitespaces();
            AssetDatabase.RenameAsset(assetPath, $"{DEFAULT_ASSET_FILENAME}_{FontName}");

            if (Fonts == null) Fonts = new List<EditorFontInfo>();
            Fonts = Fonts.Where(fi => fi != null && fi.FontReference != null).ToList();
            foreach (EditorFontInfo fontInfo in fonts)
            {
                GenericFontWeight weight = GenericFontWeight.Regular;
                foreach (GenericFontWeight style in Enum.GetValues(typeof(GenericFontWeight)))
                {
                    if (!fontInfo.FontReference.name.Contains($"-{style}")) continue;
                    weight = style;
                    break;
                }
                fontInfo.Weight = weight;
            }

            SortFontsByWeight();

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }
    }
}
