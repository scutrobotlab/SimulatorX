// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common
{
	public interface IListDatabase<TKey, TValue>
	{
		void Add(TKey key);
		void Add(TKey key, TValue value);

		void Clear();

		bool ContainsKey(TKey key);
		bool ContainsValue(TKey key, TValue value);
		bool ContainsValue(TValue value);

		int CountKeys();
		int CountValues(TKey key);

		List<TValue> GetValues(TKey key);
		List<TKey> GetKeys();
		
		void Remove(TKey key);
		void Remove(TKey key, TValue value, bool deleteEmptyKey = true);
		void Remove(TValue value, bool deleteEmptyKey = true);

		void Validate(bool deleteEmptyKeys = true);
	}
}