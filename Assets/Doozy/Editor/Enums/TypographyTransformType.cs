// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Editor.Enums
{
	/// <summary> Text case types </summary>
	public enum TypographyCase
	{
		/// <summary> Sentence case <para/> Capitalize the first letter of a sentence and leave all other letters as lowercase </summary>
		SentenceCase,
		
		/// <summary> lowercase <para/> Exclude capital letters from text </summary>
		Lowercase,
		
		/// <summary> Uppercase <para/> Capitalize all of the letters </summary>
		Uppercase,
		
		/// <summary> Capitalize Each Word <para/> Capitalize the first letter of each word and leave the other letters lowercase </summary>
		CapitalizeEachWord,
		
		/// <summary> tOGGLE cASE <para/> Shift between two case views (for example, to shift between Capitalize Each Word and the opposite, cAPITALIZE eACH wORD)
		/// </summary>
		ToggleCase
	}
}