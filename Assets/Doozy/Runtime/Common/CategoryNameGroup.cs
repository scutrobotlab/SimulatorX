// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Common
{
    [Serializable]
    public class CategoryNameGroup<T> where T : CategoryNameItem, new()
    {
        public static string defaultCategory => CategoryNameItem.k_DefaultCategory;
        public static string defaultName => CategoryNameItem.k_DefaultName;

        [SerializeField] private List<T> Items;
        public List<T> items => Items ??= new List<T> { new T() };
        public bool isEmpty
        {
            get
            {
                if (!ContainsCategory(defaultCategory))
                {
                    items.Add((T)new CategoryNameItem(defaultCategory, defaultName));
                    CleanDatabase();
                }

                return items.Count < 2;
            }
        }

        #region Category

        public bool ContainsCategory(string category) =>
            items.Any(data => data.category.Equals(CleanString(category)));

        public (bool, string) CanAddCategory(string category)
        {
            category = CleanString(category);

            return category.IsNullOrEmpty()
                ? (false, $"Invalid '{nameof(category)}'. It cannot be null or empty or contain special characters")
                : ContainsCategory(category)
                    ? (false, $"'{category}' already exists")
                    : (true, $"Can add the '{category}' category");

        }

        public bool AddCategory(string category)
        {
            bool canAddCategory;
            (canAddCategory, _) = CanAddCategory(category);
            if (!canAddCategory) return false;
            items.Add((T)new CategoryNameItem(category));
            CleanDatabase();
            return true;
        }

        public (bool, string) CanRemoveCategory(string category) =>
            category.Equals(defaultCategory)
                ? (false, $"Cannot remove the '{category}' category")
                : !ContainsCategory(category)
                    ? (false, $"The '{category}' category does not exist")
                    : (true, $"Can remove the '{category}' category");

        public bool RemoveCategory(string category)
        {
            bool canRemoveCategory;
            (canRemoveCategory, _) = CanRemoveCategory(category);
            if (!canRemoveCategory) return false;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].category.Equals(category))
                    items.RemoveAt(i);
            }
            return true;
        }

        /// <summary> Get all the category names, sorted alphabetically </summary>
        public IEnumerable<string> GetCategories()
        {
            CleanDatabase();
            var set = new HashSet<string>();
            // set.Add(defaultCategory); //add default
            foreach (T data in items)
                set.Add(data.category);
            return set;
        }

        #endregion

        #region Name

        public bool ContainsName(string category, string name) =>
            items.Any(data => data.category.Equals(CleanString(category)) & data.name.Equals(CleanString(name)));

        public (bool, string) CanAddName(string category, string name)
        {
            category = CleanString(category);
            name = CleanString(name);

            return category.IsNullOrEmpty()
                ? (false, $"Invalid '{nameof(category)}'. It cannot be null or empty or contain special characters")
                : category.Equals(defaultCategory)
                    ? (false, $"Cannot add anything to the '{category}' category")
                    : name.IsNullOrEmpty()
                        ? (false, $"Invalid '{nameof(name)}'. It cannot be null or empty or contain special characters")
                        : ContainsName(category, name)
                            ? (false, $"The '{name}' name already exists in the '{category}' category")
                            : (true, $"Can add the '{name}' name to the '{category}' category");

        }

        public bool AddName(string category, string name)
        {
            bool canAddName;
            (canAddName, _) = CanAddName(category, name);
            if (!canAddName) return false;
            items.Add((T)new CategoryNameItem(category, name));
            CleanDatabase();
            return true;
        }

        public (bool, string) CanRemoveName(string category, string name) =>
            category.Equals(defaultCategory)
                ? (false, $"Cannot remove anything from the '{category}' category")
                : name.Equals(defaultName)
                    ? (false, $"Cannot remove '{name}'")
                    : !ContainsName(category, name)
                        ? (false, $"The name '{name}' was not found in the '{category}' category")
                        : (true, $"Can remove the '{name}' from the '{category}' category");

        public bool RemoveName(string category, string name)
        {
            bool canRemoveName;
            (canRemoveName, _) = CanRemoveName(category, name);
            if (!canRemoveName) return false;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (!items[i].category.Equals(category))
                    continue;

                if (!items[i].name.Equals(name))
                    continue;

                items.RemoveAt(i);
                break;
            }

            return true;
        }

        /// <summary> Get data for the given category and name. Returns null if not found </summary>
        /// <param name="category"> Target category </param>
        /// <param name="name"> Target name </param>
        public T Get(string category, string name) =>
            items.Where(data => data.category.Equals(category)).FirstOrDefault(data => data.name.Equals(name));

        /// <summary> Get all the names inside the given category </summary>
        /// <param name="category"> Target category </param>
        public IEnumerable<string> GetNames(string category)
        {
            CleanDatabase();
            var set = new HashSet<string>();
            //set.Add(defaultName); //add default name
            foreach (T data in items.Where(data => data.category.Equals(category)))
                set.Add(data.name);
            return set;
        }

        #endregion

        public void ClearDatabase()
        {
            items.Clear();
            items.Add(new T()); //add the default values
        }

        /// <summary> Remove null or empty entries and sort the group by category and then by name </summary>
        public void CleanDatabase()
        {
            //Remove null or empty entries - this should not be an issue due to the checks put in place when adding data to the group
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] == null)
                {
                    items.RemoveAt(i);
                    continue;
                }

                if (items[i].category.Trim().IsNullOrEmpty() | items[i].name.Trim().IsNullOrEmpty())
                    items.RemoveAt(i);
            }

            //Sort group
            Items = items
                .OrderBy(data => data.category)
                .ThenBy(data => data.name)
                .ToList();
        }

        /// <summary> Cleans the string by removing any empty spaces (at the start of the string and/or the end of the string) </summary>
        /// <param name="value"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        public static string CleanString(string value, bool removeWhitespaces = true, bool removeSpecialCharacters = true) =>
            CategoryNameItem.CleanString(value, removeWhitespaces, removeSpecialCharacters);
    }
}
