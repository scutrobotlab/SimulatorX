// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine.UIElements;
namespace Doozy.Runtime.UIElements.Extensions
{
	public static class TextElementExtensions
	{
		/// <summary> Set the text associated with the element </summary>
		/// <param name="target"> Target TextElement </param>
		/// <param name="text"> Text value </param>
		public static T SetText<T>(this T target, string text) where T : TextElement
		{
			target.text = text;
			return target;
		}

		/// <summary> Get the text associated with the element </summary>
		/// <param name="target"> Target TextElement </param>
		public static string GetText<T>(this T target) where T : TextElement => target.text;
	}
}