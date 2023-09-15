// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Doozy.Runtime.Colors.Models
{
   /// <summary>
    /// The RGB color model is an additive color model in which red, green and blue light are added together in various ways to reproduce a broad array of colors.
    /// </summary>
    [Serializable]
    public struct RGB
    {
        /// <summary> Construct a new RGB struct </summary>
        /// <param name="r"> Red - r ∊ [0, 1] </param>
        /// <param name="g"> Green - g ∊ [0, 1] </param>
        /// <param name="b"> Blue - b ∊ [0, 1] </param>
        public RGB(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        /// <summary> Red - r ∊ [0, 1] </summary>
        public float r;
        
        /// <summary> Green - g ∊ [0, 1] </summary>
        public float g;
        
        /// <summary> Blue - b ∊ [0, 1] </summary>
        public float b;

        public RGB Copy() =>
            new RGB(r, g, b);

        public Color ToColor(float alpha = 1) =>
            new Color(r, g, b, Mathf.Clamp(alpha, 0, 1));

        public HSL ToHSL() =>
            ColorUtils.RGBtoHSL(this);

        public HSV ToHSV() =>
            ColorUtils.RGBtoHSV(this);

        public RGB Validate()
        {
            r = ValidateColor(r, R.MIN, R.MAX);
            g = ValidateColor(g, G.MIN, G.MAX);
            b = ValidateColor(b, B.MIN, B.MAX);
            return this;
        }

        private float ValidateColor(float value, float min, float max) =>
            Mathf.Max(min, Mathf.Min(max, value));

        public Vector3 Factorize() =>
            new Vector3
            {
                x = FactorizeColor(r, R.MIN, R.MAX, R.F),
                y = FactorizeColor(g, G.MIN, G.MAX, G.F),
                z = FactorizeColor(b, B.MIN, B.MAX, B.F)
            };

        private int FactorizeColor(float value, float min, float max, float f) =>
            (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));

        public string ToString(bool factorize = false) =>
            factorize 
                ? $"rgb({Factorize().x}, {Factorize().y}, {Factorize().z})"
                : $"rgb({r}, {g}, {b})";

        /// <summary>
        /// A hex triplet is a six-digit, three-byte hexadecimal number used in HTML, CSS, SVG, and other computing applications to represent colors.
        /// <para/>The bytes represent the red, green and blue components of the color.
        /// <para/>One byte represents a number in the range 00 to FF (in hexadecimal notation), or 0 to 255 in decimal notation.
        /// </summary>
        public string ToHEX(bool addHashTag = true) =>
            (addHashTag ? "#" : "") + ColorUtility.ToHtmlStringRGB(ToColor(1));

        /// <summary> Red </summary>
        public struct R
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 255;
        }
        
        /// <summary> Green </summary>
        public struct G
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 255;
        }
        
        /// <summary> Blue </summary>
        public struct B
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 255;
        }
    }

}