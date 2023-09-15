// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Common.Extensions
{
	public static class DoubleExtensions
	{
		/// <summary> Round number with the given number of decimals </summary>
		/// <param name="target"> Target number </param>
		/// <param name="decimals"> Number of decimals </param>
		public static double Round(this double target, int decimals = 1) =>
			Math.Round(target, decimals);
	}
}