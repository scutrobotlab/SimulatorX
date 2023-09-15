// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Common
{
    [Serializable]
    public class CategoryNameItem
    {
        public const string k_DefaultCategory = "None";
        public const string k_DefaultName = "None";

        [SerializeField] private string Category;
        public string category => Category;

        [SerializeField] private string Name;
        public string name => Name;

        public CategoryNameItem()
        {
            Category = k_DefaultCategory;
            Name = k_DefaultName;
        }
        
        public CategoryNameItem(string category)
        {
            Category = category;
            Name = k_DefaultName;
        }
        
        public CategoryNameItem(string category, string name, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            Category = CleanString(category, removeWhitespaces, removeSpecialCharacters);
            Name = CleanString(name, removeWhitespaces, removeSpecialCharacters);
        }

        /// <summary> Set a new Category value </summary>
        /// <param name="newCategory"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        public (bool, string) SetCategory(string newCategory, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            if (newCategory.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty())
                return (false, $"Invalid '{nameof(newCategory)}'. It cannot be null or empty or contain special characters");
            Category = CleanString(newCategory, removeWhitespaces, removeSpecialCharacters);
            return (true, $"'{nameof(Category)}' renamed to: {Category}");
        }

        /// <summary> Set a new Name value </summary>
        /// <param name="newName"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        public (bool, string) SetName(string newName, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            if (newName.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty())
                return (false, $"Invalid '{nameof(newName)}'. It cannot be null or empty or contain special characters");
            Name = CleanString(newName, removeWhitespaces, removeSpecialCharacters);
            return (true, $"'{nameof(Name)}' renamed to: {Name}");
        }

        /// <summary> Cleans the string by removing any empty spaces (at the start of the string and/or the end of the string) </summary>
        /// <param name="value"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        public static string CleanString(string value, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            if (removeWhitespaces) value = value.RemoveWhitespaces();
            if (removeSpecialCharacters) value = value.RemoveAllSpecialCharacters();
            return value.Trim();
        }
    }
}
