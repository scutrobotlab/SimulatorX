// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Internal
{
    internal class ReactionDictionary<T>
    {
        private Dictionary<T, HashSet<Reaction>> dictionary { get; set; }

        private bool initialized { get; set; }

        private void Initialize()
        {
            if (dictionary == null) initialized = false;
            if (initialized) return;
            dictionary = new Dictionary<T, HashSet<Reaction>>();
            initialized = true;
        }

        internal void Validate()
        {
            Initialize();
            RemoveEmptyKeys();
        }

        internal List<Reaction> GetReactions(T targetObject)
        {
            Initialize();
            return targetObject != null && dictionary.ContainsKey(targetObject)
                ? dictionary[targetObject].ToList()
                : new List<Reaction>();
        }

        internal void AddReaction(T key, Reaction value)
        {
            Initialize();
            if (key == null || value == null) return;
            if (dictionary.ContainsKey(key))
            {
                if (dictionary[key] == null)
                {
                    dictionary[key] = new HashSet<Reaction> { value };
                    return;
                }
                dictionary[key].Add(value);
                return;
            }
            dictionary.Add(key, new HashSet<Reaction> { value });
        }

        internal void RemoveReaction(T key, Reaction value)
        {
            Initialize();
            if (key == null) return;
            if (!dictionary.ContainsKey(key)) return;
            HashSet<Reaction> hashSet = dictionary[key];
            hashSet.Remove(null);
            hashSet.Remove(value);
            if (hashSet.Count != 0) return;
            dictionary.Remove(key);
        }

        internal void RemoveReaction(Reaction value)
        {
            Initialize();
            foreach (T key in dictionary.Keys)
                dictionary[key].Remove(value);
            RemoveEmptyKeys();
        }

        private void RemoveEmptyKeys()
        {
            var emptyKeys = dictionary.Keys.Where(key => dictionary[key] == null || dictionary[key].Count == 0).ToList();
            foreach (T key in emptyKeys)
                dictionary.Remove(key);
        }
    }
}
