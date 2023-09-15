// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Common
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ListDatabase<TKey, TValue> : IListDatabase<TKey, TValue>
	{
		private readonly Type m_keyType;
		private readonly Type m_valueType;
		public Dictionary<TKey, List<TValue>> Database { get; }

		public Dictionary<TKey, List<TValue>>.KeyCollection Keys => Database.Keys;
		public Dictionary<TKey, List<TValue>>.ValueCollection Values => Database.Values;
		
		public ListDatabase()
		{
			m_keyType = typeof(TKey);
			m_valueType = typeof(TValue);
			Database = new Dictionary<TKey, List<TValue>>();
		}

		public void Add(TKey key)
		{
			if (Database.ContainsKey(key)) return;
			Database.Add(key, new List<TValue>());
		}

		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				if (ContainsValue(key, value))
					return;
				if (Database[key] == null)
					Database[key] = new List<TValue>();
				Database[key].Add(value);
				return;
			}

			Database.Add(key, new List<TValue> {value});
		}

		public void Clear()
		{
			Database.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return Database.ContainsKey(key);
		}

		public bool ContainsValue(TKey key, TValue value)
		{
			return ContainsKey(key) && Database[key].Contains(value);
		}

		public bool ContainsValue(TValue value)
		{
			return Database.Keys.Any(key => Database[key].Contains(value));
		}

		public int CountKeys()
		{
			return Database.Keys.Count;
		}

		public int CountValues(TKey key)
		{
			return Database.ContainsKey(key) 
				       ? Database[key].Count
				       : 0;
		}

		public List<TKey> GetKeys()
		{
			return Keys.ToList();
		}

		public List<TValue> GetValues(TKey key)
		{
			return key == null || !ContainsKey(key) || Database[key] == null
				       ? new List<TValue>()
				       : Database[key].ToList();
		}

		public void Remove(TKey key)
		{
			if (!ContainsKey(key)) return;
			Database.Remove(key);
		}

		public void Remove(TKey key, TValue value, bool deleteEmptyKey = true)
		{
			if (!ContainsValue(key, value)) return;
			Database[key].Remove(value);
			if (!deleteEmptyKey) return;
			if (Database[key].Count == 0) Database.Remove(key);
		}

		public void Remove(TValue value, bool deleteEmptyKey = true)
		{
			if (!ContainsValue(value)) return;
			var keysToRemove = new List<TKey>();
			foreach (TKey key in Database.Keys.Where(key => Database[key].Contains(value)))
			{
				Database[key].Remove(value);
				if (!deleteEmptyKey) continue;
				if (Database[key].Count == 0) keysToRemove.Add(key);
			}

			if (!deleteEmptyKey) return;
			foreach (TKey key in keysToRemove) Database.Remove(key);
		}

		public void Validate(bool deleteEmptyKeys = true)
		{
			var keysToRemove = new List<TKey>();
			foreach (TKey key in Database.Keys)
			{
				for (int i = Database[key].Count - 1; i >= 0; i--)
				{
					TValue value = Database[key][i];
					if (value != null) continue;
					Database[key].RemoveAt(i);
				}

				if (!deleteEmptyKeys) continue;
				if (Database[key].Count == 0) keysToRemove.Add(key);
			}

			if (!deleteEmptyKeys) return;
			foreach (TKey key in keysToRemove) Database.Remove(key);
		}
	}
}