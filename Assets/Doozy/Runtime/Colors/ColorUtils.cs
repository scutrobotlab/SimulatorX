// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//https://stackoverflow.com/questions/6615002/given-an-rgb-value-how-do-i-create-a-tint-or-shade
//https://en.wikipedia.org/wiki/HSL_and_HSV
//https://github.com/edelstone/tints-and-shades
//https://medium.com/@donatbalipapp/colours-maths-90346fb5abda
//https://rip94550.wordpress.com/2009/02/26/color-hsb-and-tint-tone-shade/
//https://en.wikipedia.org/wiki/Color_space
//https://www.easyrgb.com/en/math.php
//https://www.cs.rit.edu/~ncs/color/t_convert.html
//https://www.programmingalgorithms.com/algorithm/rgb-to-hsv/

using System;
using Doozy.Runtime.Colors.Models;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Doozy.Runtime.Colors
{
#pragma warning disable 0219
	public static class ColorUtils
	{
		#region Hue

		/// <summary> Get the HUE from RGB values </summary>
		/// <param name="r"> Red </param>
		/// <param name="g"> Green </param>
		/// <param name="b"> Blue </param>
		/// <param name="factorize"> TRUE returns a value between 0 and 360 <para/> FALSE returns a value between 0 and 1 </param>
		public static float Hue(float r, float g, float b, bool factorize = false)
		{
			if (r == g && g == b) return 0;
			float hue = 0;
			if (r >= g && g >= b) hue = 60 * ((g - b) / (r - b));         //(A) If R ≥ G ≥ B  |  H = 60° x [(G-B)/(R-B)]
			else if (g > r && r >= b) hue = 60 * (2 - (r - b) / (g - b)); //(B) If G > R ≥ B  |  H = 60° x [2 - (R-B)/(G-B)]
			else if (g >= b && b > r) hue = 60 * (2 + (b - r) / (g - r)); //(C) If G ≥ B > R  |  H = 60° x [2 + (B-R)/(G-R)]
			else if (b > g && g > r) hue = 60 * (4 - (g - r) / (b - r));  //(D) If B > G > R  |  H = 60° x [4 - (G-R)/(B-R)]
			else if (b > r && r >= g) hue = 60 * (4 + (r - g) / (b - g)); //(E) If B > R ≥ G  |  H = 60° x [4 + (R-G)/(B-G)]
			else if (r >= b && b > g) hue = 60 * (6 - (b - g) / (r - g)); //(F) If R ≥ B > G  |  H = 60° x [6 - (B-G)/(R-G)]

			if (factorize) Mathf.RoundToInt(hue);
			return (float) Math.Round(hue / 360, 2);

			// if (factorize) return hue;                              
			// return hue / 360;                                       
		}

		/// <summary> Get the HUE value from RBG </summary>
		/// <param name="value"></param>
		/// <param name="factorize"> </param>
		public static float RGBtoHUE(RGB value, bool factorize = false) =>
			Hue(value.r, value.g, value.b, factorize);

		/// <summary> Convert pure HUE to RGB </summary>
		/// <param name="value"> HUE value </param>
		public static RGB HUEtoRGB(float value)
		{
			float R = Mathf.Abs(value * 6 - 3) - 1;
			float G = 2 - Mathf.Abs(value * 6 - 2);
			float B = 2 - Mathf.Abs(value * 6 - 4);
			return new RGB(R, G, B);
		}

		#endregion

		#region Color

		/// <summary> Convert RGB to Color </summary>
		/// <param name="value"> RGB value </param>
		public static Color RGBtoCOLOR(RGB value) =>
			new Color(value.r, value.g, value.g);

		/// <summary> Convert HSL to Color </summary>
		/// <param name="value"> HSL value </param>
		public static Color HSLtoCOLOR(HSL value) =>
			RGBtoCOLOR(value.ToRGB());

		/// <summary> Convert HSV to Color </summary>
		/// <param name="value"> HSV value </param>
		public static Color HSVtoCOLOR(HSV value) =>
			RGBtoCOLOR(value.ToRGB());

		#endregion

		#region HSL

		/// <summary> Convert R G B values to HSL </summary>
		/// <param name="r"> Red </param>
		/// <param name="g"> Green </param>
		/// <param name="b"> Blue </param>
		/// <returns></returns>
		public static HSL RGBtoHSL(float r, float g, float b)
		{
			//http://www.rapidtables.com/convert/color/rgb-to-hsl.htm
			float Cmax = Mathf.Max(r, g, b);
			float Cmin = Mathf.Min(r, g, b);
			float delta = Cmax - Cmin;
			float H = 0;
			float S = 0;
			float L = (Cmax + Cmin) / 2;
			if (delta == 0) return new HSL(H, S, L).Validate();
			H = Hue(r, g, b);
			S = L < 0.5f
				    ? delta / (Cmax + Cmin)
				    : delta / (2 - Cmax - Cmin);
			return new HSL(H, S, L).Validate();
		}

		/// <summary> Convert RGB to HSL </summary>
		/// <param name="value"> RGB value </param>
		public static HSL RGBtoHSL(RGB value) =>
			RGBtoHSL(value.r, value.g, value.b);

		/// <summary> Convert Color to HSL </summary>
		/// <param name="color"> Color value </param>
		public static HSL COLORtoHSL(Color color) =>
			RGBtoHSL(color.r, color.g, color.b);

		#endregion

		#region HSV / HSB

		/// <summary> Convert R G B values to HSV </summary>
		/// <param name="r"> Red </param>
		/// <param name="g"> Green </param>
		/// <param name="b"> Blue </param>
		public static HSV RGBtoHSV(float r, float g, float b)
		{
			//http://www.rapidtables.com/convert/color/rgb-to-hsv.htm //http://www.easyrgb.com/en/math.php#text20
			float Cmax = Mathf.Max(r, g, b);
			float Cmin = Mathf.Min(r, g, b);
			float delta = Cmax - Cmin;
			float H = 0;
			float S = 0;
			float V = Cmax;
			if (delta == 0) return new HSV(H, S, V).Validate();
			H = Hue(r, g, b);
			S = delta / Cmax;
			return new HSV(H, S, V).Validate();
		}

		/// <summary> Convert RGB to HSV </summary>
		/// <param name="value"> RGB value </param>
		public static HSV RGBtoHSV(RGB value) =>
			RGBtoHSV(value.r, value.g, value.g);

		/// <summary> Convert Color to HSV </summary>
		/// <param name="color"> Color value </param>
		public static HSV COLORtoHSV(Color color) =>
			RGBtoHSV(color.r, color.g, color.b);

		#endregion

		#region RGB

		/// <summary> Convert Color to RGB </summary>
		/// <param name="color"> Color value </param>
		public static RGB COLORtoRGB(Color color) =>
			new RGB(color.r, color.g, color.b);

		/// <summary> Convert HSL to RGB </summary>
		/// <param name="value"> HSL value </param>
		public static RGB HSLtoRGB(HSL value)
		{
			//http://www.rapidtables.com/convert/color/hsl-to-rgb.htm
			HSL hsl = new HSL(value.h, value.s, value.l).Validate();
			float H = hsl.Factorize().x;
			float S = hsl.s;
			float L = hsl.l;
			float C = (1 - Mathf.Abs(2 * L - 1)) * S;
			float X = C * (1 - Mathf.Abs(H / 60 % 2 - 1));
			float m = L - C / 2;
			float r = 0, g = 0, b = 0;
			if (0 <= H && H < 60)
			{
				r = C;
				g = X;
				b = 0;
			}
			else if (60 <= H && H < 120)
			{
				r = X;
				g = C;
				b = 0;
			}
			else if (120 <= H && H < 180)
			{
				r = 0;
				g = C;
				b = X;
			}
			else if (180 <= H && H < 240)
			{
				r = 0;
				g = X;
				b = C;
			}
			else if (240 <= H && H < 300)
			{
				r = X;
				g = 0;
				b = C;
			}
			else if (300 <= H && H < 360)
			{
				r = C;
				g = 0;
				b = X;
			}

			return new RGB(r + m, g + m, b + m).Validate();
		}

		/// <summary> Convert HSV to RGB </summary>
		/// <param name="value"> HSV value </param>
		public static RGB HSVtoRGB(HSV value) //http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
		{
			var hsv = new HSV(value.h, value.s, value.v);

			float H = hsv.Factorize().x;
			float S = hsv.s;
			float V = hsv.v;

			float C = V * S;
			float X = C * (1 - Mathf.Abs(H / 60 % 2 - 1));
			float m = V - C;

			float r = 0, g = 0, b = 0;

			if (0 <= H && H < 60)
			{
				r = C;
				g = X;
				b = 0;
			}
			else if (60 <= H && H < 120)
			{
				r = X;
				g = C;
				b = 0;
			}
			else if (120 <= H && H < 180)
			{
				r = 0;
				g = C;
				b = X;
			}
			else if (180 <= H && H < 240)
			{
				r = 0;
				g = X;
				b = C;
			}
			else if (240 <= H && H < 300)
			{
				r = X;
				g = 0;
				b = C;
			}
			else if (300 <= H && H < 360)
			{
				r = C;
				g = 0;
				b = X;
			}

			return new RGB(r + m, g + m, b + m);
		}

		#endregion
	}
#pragma warning restore 0219
}