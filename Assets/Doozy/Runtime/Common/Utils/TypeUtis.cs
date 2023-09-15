// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common.Utils
{
	public static class TypeUtils
	{
		public static T CastObject<T>(object input) =>
			(T) input;

		public static T ConvertObject<T>(object input) =>
			(T) Convert.ChangeType(input, typeof(T));

		public static IEnumerable<Type> GetDerivedTypesOfType(Type type) =>
			from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
			from assemblyType in domainAssembly.GetTypes()
			where type.IsAssignableFrom(assemblyType)
			where assemblyType.IsSubclassOf(type) && !assemblyType.IsAbstract
			select assemblyType;
	}
}