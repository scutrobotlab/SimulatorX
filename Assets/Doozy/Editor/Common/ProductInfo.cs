// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Common
{
    #if DOOZY_42
	[CreateAssetMenu(menuName = "42/Product Info", fileName = "ProductInfo", order = 0)]
    #endif
    [Serializable]
    public class ProductInfo : ScriptableObject
    {
        public string Category;
        
        [Space(5)]
        public string Name;
        
        [Space(5)]
        public int Major;
        public int Minor;
        public int RevisionVersion;
        
        [HideInInspector]
        public int BuildVersion;

        public Version version => new Version(Major, Minor, BuildVersion, RevisionVersion);

        /// <summary> Construct a new ProductInfo with the given values </summary>
        /// <param name="category"> Product category </param>
        /// <param name="productName"> Name of the product </param>
        /// <param name="major"> Major Version number: Incompatible API changes </param>
        /// <param name="minor"> Minor Version number: Functionality added in a backwards compatible manner </param>
        /// <param name="revision"> Patch Version number: Backwards compatible bug fixes </param>
        public ProductInfo(string category, string productName, int major, int minor, int revision)
        {
            Category = category;
            Name = productName;
            Major = major;
            Minor = minor;
            RevisionVersion = revision;
            BuildVersion = 1;
        }

        /// <summary> Product category without any whitespaces </summary>
        public string safeCategory => Category.Replace(" ", "");

        /// <summary> Name of the product without any whitespaces </summary>
        public string safeName => Name.Replace(" ", "");

        /// <summary>
        /// Name of the product (with whitespaces) and its version
        /// <para/> {Name} {Major}.{Minor}.{Patch}
        /// </summary>
        public string nameAndVersion => $"{Name} {Major}.{Minor}.{RevisionVersion}";

        /// <summary>
        /// Product category, name of the product (with whitespaces) and its version
        /// <para/> {Category} {Name} {Major}.{Minor}.{Patch}
        /// </summary>
        public string categoryAndNameAndVersion => $"{Category} {nameAndVersion}";

        public override string ToString() => nameAndVersion;

        /// <summary> Search the project for all the ProductDetails assets with the given category </summary>
        /// <param name="category"> Product category to search for </param>
        public static List<ProductInfo> GetProductCategory(string category)
        {
            var list = new List<ProductInfo>();
            #if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ProductInfo)}");
            if (guids == null)
            {
                Debug.Log($"No {nameof(ProductInfo)} asset was found in this project");
                return list;
            }

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ProductInfo asset = AssetDatabase.LoadAssetAtPath<ProductInfo>(assetPath);
                if (asset == null || list.Contains(asset) || !asset.Category.Equals(category)) continue;
                list.Add(asset);
            }

            return list;
            #else
			Debugger.Log("Cannot get this information outside the Unity Editor");
			return list;
            #endif
        }

        /// <summary> Search the project for all the ProductDetails assets and get the first one with the given category and productName </summary>
        /// <param name="category"> Product category to search for </param>
        /// <param name="productName"> Name of the product to search for </param>
        public static ProductInfo Get(string category, string productName)
        {
            #if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ProductInfo)}");
            if (guids != null)
                return guids.Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<ProductInfo>)
                    .FirstOrDefault(asset => asset != null && asset.Category.Equals(category) && asset.Name.Equals(productName));
            Debug.Log($"No {nameof(ProductInfo)} asset was found in this project");
            return null;

            #else
			Debugger.Log("Cannot get this information outside the Unity Editor");
			return null;
            #endif
        }

        /// <summary> Search the project for all the ProductDetails assets and get the first one with the given productName </summary>
        /// <param name="productName"> Name of the product to search for </param>
        public static ProductInfo Get(string productName)
        {
            #if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ProductInfo)}");
            if (guids != null)
                return guids.Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<ProductInfo>)
                    .Where(asset => asset != null)
                    .FirstOrDefault(asset => asset.Name.Equals(productName));
            Debug.Log($"No {nameof(ProductInfo)} asset was found in this project");
            return null;

            #else
			Debugger.Log("Cannot get this information outside the Unity Editor");
			return null;
            #endif
        }
    }
}
