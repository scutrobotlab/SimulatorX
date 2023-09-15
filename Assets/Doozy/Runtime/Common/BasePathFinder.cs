// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace Doozy.Runtime.Common
{
	[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public class BasePathFinder<T> : ScriptableObject where T : ScriptableObject
	{
		private static bool debugMode => false;
		private static string s_foundPath = string.Empty;

		// ReSharper disable once MemberCanBeProtected.Global
		public static string path
		{
			get
			{
#if UNITY_EDITOR
				if (!string.IsNullOrEmpty(s_foundPath))
				{
					if (debugMode) Debug.Log($"Saved Path: {s_foundPath}");
					return s_foundPath;
				}

				T obj = CreateInstance<T>();
				var s = UnityEditor.MonoScript.FromScriptableObject(obj);
				string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
				DestroyImmediate(obj);
				var fileInfo = new FileInfo(assetPath);
				DirectoryInfo baseDir = fileInfo.Directory;
				Debug.Assert(baseDir != null, nameof(baseDir) + " != null");
				string baseDirPath = CleanPath(baseDir.ToString());
				int index = baseDirPath.IndexOf("Assets/", StringComparison.Ordinal);
				Assert.IsTrue(index >= 0);
				baseDirPath = baseDirPath.Substring(index);
				s_foundPath = baseDirPath;
				if (debugMode) Debug.Log($"Found Path: {s_foundPath}");
				return baseDirPath;
#else
                return "Path cannot be returned outside the Unity Editor";
#endif
			}
		}

		/// <summary> Replace any back slashes '\' with forward slashes '/' and returns the path string </summary>
		private static string CleanPath(string rawPath) =>
			rawPath.Replace('\\', '/');
	}
}