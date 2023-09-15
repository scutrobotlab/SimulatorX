// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Diagnostics.CodeAnalysis;
using Doozy.Runtime.Colors.Models;
using UnityEngine;

namespace Doozy.Runtime.Colors
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public static class ColorExtensions
	{
		
		public static Color From256(this Color target, int r, int g, int b, float a = 1f) =>
			new Color(r / 256f, g / 256f, b / 256f, a);

		/// <summary> Get the maximum value of this color's RGB values </summary>
		public static float MaxRGB(this Color target) =>
			Mathf.Max(target.r, target.g, target.b);

		/// <summary> Get the minimum value of this color's RGB values </summary>
		public static float MinRGB(this Color target) =>
			Mathf.Min(target.r, target.g, target.b);

		/// <summary>
		/// Luminosity (also called brightness, lightness or luminance) stands for the intensity of the energy output of a visible light source.
		/// <para/> It basically tells how light a colour is and is measured on the following scale: L = [0, 1]
		/// <para/> L = (1 / 2) x (Max(RGB) + Min(RGB))
		/// </summary>
		public static float Luminosity(this Color target) =>
			// (target.MaxRGB() + target.MinRGB()) / 2;
			(float) Math.Round(target.GetHSLLightness(), 2);

		/// <summary>
		/// Saturation is an expression for the relative bandwidth of the visible output from a light source.
		/// <para/> As saturation increases, colours appear more pure. As saturation decreases, colours appear more washed-out.
		/// <para/> It is measured on the following scale: S = [0, 1]
		/// </summary>
		public static float Saturation(this Color target) =>
			// target.Luminosity() < 1 ? (target.MaxRGB() - target.MinRGB()) / (1 - Mathf.Abs(target.Luminosity() * 2 - 1)) : 0;
			(float) Math.Round(target.GetHSLSaturation(), 2);

		public static float Hue(this Color target, bool factorize = false) =>
			ColorUtils.Hue(target.r, target.g, target.b, factorize);

		/// <summary> Get the alpha value of the color </summary>
		/// <param name="target"> Target color </param>
		public static float Alpha(this Color target) =>
			target.a;

		/// <summary> Get a new Color with the given alpha value </summary>
		/// <param name="target"> Target color </param>
		/// <param name="value"> Target alpha value</param>
		public static Color WithAlpha(this Color target, float value) =>
			new Color(target.r, target.g, target.b, Mathf.Clamp01(value));

		#region HSL

		#region H - Hue

		/// <summary> Get HSL hue by converting to HSL and returning the value </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="factorize"> TRUE returns a value between 0 and 360 <para/> FALSE returns a value between 0 and 1 </param>
		public static float GetHSLHue(this Color target, bool factorize = false) =>
			factorize ? Mathf.Round(target.ToHSL().h * HSL.H.F) : target.ToHSL().h;

		/// <summary> Set HSL hue by converting to HSL, setting the new value (between 0 and 1), and returning the new Color </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="hue"> Hue value from 0 to 1 </param>
		public static Color SetHSLHue(this Color target, float hue)
		{
			var hsl = target.ToHSL();
			hsl.h = Mathf.Clamp01(hue);
			return hsl.ToColor();
		}
		
		/// <summary> Set HSL hue by converting to HSL, setting the new value (between 0 and 360), and returning the new Color </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="hue"> Hue value from 0 to 360 </param>
		public static Color SetHSLHue(this Color target, int hue)
		{
			var hsl = target.ToHSL();
			hsl.h = Mathf.Clamp(hue/HSL.H.F, 0, HSL.H.F);
			return hsl.ToColor();
		}

		#endregion

		#region S - Saturation

		/// <summary> Get HSL saturation by converting to HSL and returning the value </summary>
		/// <param name="target"> Target Color </param>
		public static float GetHSLSaturation(this Color target) =>
			target.ToHSL().s;

		/// <summary> Set HSL saturation by converting to HSL, setting the new value, and returning the new Color </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="saturation"> Saturation value from 0 to 1 </param>
		public static Color SetHSLSaturation(this Color target, float saturation)
		{
			var hsl = target.ToHSL();
			hsl.s = Mathf.Clamp01(saturation);
			return hsl.ToColor();
		}

		#endregion

		#region L - Lightness

		/// <summary> Get HSL lightness by converting to HSL and returning the value </summary>
		/// <param name="target"> Target Color </param>
		public static float GetHSLLightness(this Color target) =>
			target.ToHSL().l;

		/// <summary>
		/// Set HSL lightness by converting to HSL, setting the new value (between 0 and 1), and returning the new Color
		/// <para/> To shade: lower the Lightness
		/// <para/> To tint: increase the Lightness
		/// <para/> 50% means an unaltered Hue = 0.5f
		/// <para/> over 50% means the Hue is lighter (tint) = over 0.5f
		/// <para/> below 50% means the Hue is darker (shade) = below 0.5f
		/// </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="lightness"> Lightness value from 0 to 1
		/// <para/> To shade: lower the Lightness
		/// <para/> To tint: increase the Lightness
		/// <para/> 50% means an unaltered Hue = 0.5f
		/// <para/> over 50% means the Hue is lighter (tint) = over 0.5f
		/// <para/> below 50% means the Hue is darker (shade) = below 0.5f
		/// </param>
		public static Color SetHSLLightness(this Color target, float lightness)
		{
			var hsl = target.ToHSL();
			hsl.l = Mathf.Clamp01(lightness);
			return hsl.ToColor();
		}

		#endregion

		#endregion

		#region HSV

		#region H - Hue

		/// <summary> Get HSV hue by converting to HSV and returning the value </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="factorize"> TRUE returns a value between 0 and 360 <para/> FALSE returns a value between 0 and 1 </param>
		public static float GetHSVHue(this Color target, bool factorize = false) =>
			factorize ? Mathf.Round(target.ToHSV().h * HSV.H.F) : target.ToHSV().h;

		/// <summary> Set HSV hue by converting to HSV, setting the new value (between 0 and 1), and returning the new Color </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="hue"> Hue value from 0 to 1 </param>
		public static Color SetHSVHue(this Color target, float hue)
		{
			var hsv = target.ToHSV();
			hsv.h = Mathf.Clamp01(hue);
			return hsv.ToColor();
		}
		
		/// <summary> Set HSV hue by converting to HSV, setting the new value (between 0 and 360), and returning the new Color </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="hue"> Hue value from 0 to 360 </param>
		public static Color SetHSVHue(this Color target, int hue)
		{
			var hsv = target.ToHSV();
			hsv.h = Mathf.Clamp(hue/HSV.H.F, 0, HSV.H.F);
			return hsv.ToColor();
		}

		#endregion

		#region S - Saturation

		/// <summary> Get HSV saturation by converting to HSV and returning the value </summary>
		/// <param name="target"> Target Color </param>
		public static float GetHSVSaturation(this Color target) =>
			target.ToHSV().s;

		/// <summary>
		/// Set HSV saturation by converting to HSV, setting the new value, and returning the new Color
		/// <para/> To shade: increase the Saturation
		/// <para/> To tint: lower the Saturation
		/// </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="saturation">
		/// Saturation value from 0 to 1
		/// <para/> To shade: increase the Saturation
		/// <para/> To tint: lower the Saturation
		/// </param>
		public static Color SetHSVSaturation(this Color target, float saturation)
		{
			var hsv = target.ToHSV();
			hsv.s = Mathf.Clamp01(saturation);
			return hsv.ToColor();
		}

		#endregion

		#region V - Value / Brightness

		/// <summary> Get HSV value (brightness) by converting to HSV and returning the value </summary>
		/// <param name="target"> Target Color </param>
		public static float GetHSVValue(this Color target) =>
			target.ToHSV().v;

		/// <summary>
		/// Set HSV value (brightness) by converting to HSV, setting the new value, and returning the new Color
		/// <para/> To shade: lower the Value (brightness)
		/// <para/> To tint: increase the Value (brightness)
		/// </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="value"> Value (brightness) from 0 to 1
		/// <para/> To shade: lower the Value (brightness)
		/// <para/> To tint: increase the Value (brightness)
		/// </param>
		public static Color SetHSVValue(this Color target, float value)
		{
			var hsv = target.ToHSV();
			hsv.v = Mathf.Clamp01(value);
			return hsv.ToColor();
		}

		#endregion

		#endregion

		/// <summary>
		/// Get a darker variant of the color
		/// <para/> A shade is produced by "darkening" a hue or "adding black" 
		/// </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="factor"> Shade factor (0.1 is equivalent of adding +10% black) </param>
		public static Color WithRGBShade(this Color target, float factor)
		{
			target.r *= 1 - factor;
			target.g *= 1 - factor;
			target.b *= 1 - factor;
			return target;
		}

		/// <summary>
		/// Get a lighter variant of the color
		/// <para/> A tint is produced by "lightning" a hue or "adding white"
		/// </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="factor"> Tint factor (0.1 is equivalent of adding +10% white) </param>
		/// <returns></returns>
		public static Color WithRGBTint(this Color target, float factor)
		{
			target.r += (1 - target.r) * factor;
			target.g += (1 - target.g) * factor;
			target.b += (1 - target.b) * factor;
			return target;
		}

		/// <summary>
		/// Get a mix of the two colors (takes other color's alpha value into consideration)
		/// <para/> Layering the target color with the other color
		/// </summary>
		/// <param name="target"> Target Color </param>
		/// <param name="other"> Other Color </param>
		public static Color MixedWithColor(this Color target, Color other) =>
			new Color(target.r + (other.r - target.r) * other.a,
			          target.g + (other.g - target.g) * other.a, 
			          target.b + (other.b - target.b) * other.a);
		
		
		public static HSL ToHSL(this Color target) =>
			ColorUtils.COLORtoHSL(target);

		public static HSV ToHSV(this Color target) =>
			ColorUtils.COLORtoHSV(target);

		public static RGB ToRGB(this Color target) =>
			ColorUtils.COLORtoRGB(target);

	}
}