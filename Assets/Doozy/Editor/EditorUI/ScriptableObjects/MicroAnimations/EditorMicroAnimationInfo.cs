// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations
{
	[Serializable]
	public class EditorMicroAnimationInfo
	{
		public string AnimationName;
		public List<Texture2D> Textures;
		
		public void ValidateName() =>
			AnimationName = AnimationName.RemoveWhitespaces().RemoveAllSpecialCharacters();
	}
}