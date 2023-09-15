// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable ConvertToAutoProperty

namespace Doozy.Runtime.UIElements
{
	/// <summary> Representation of a rectangle edge left, top, right and bottom values </summary>
	public struct EdgeValues : IEquatable<EdgeValues>
	{
		/// <summary> Left edge value </summary>
		public float Left;

		/// <summary> Top edge value </summary>
		public float Top;

		/// <summary> Right edge value </summary>
		public float Right;

		/// <summary> Bottom edge value </summary>
		public float Bottom;

		/// <summary> Creates a new edge values set with the given left, top, right, bottom values </summary>
		/// <param name="left"> Left edge value </param>
		/// <param name="top"> Top edge value </param>
		/// <param name="right"> Right edge value </param>
		/// <param name="bottom"> Bottom edge value </param>
		public EdgeValues(float left, float top, float right, float bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		/// <summary> Set left, top, right and bottom values of an existing edge values set </summary>
		/// <param name="newLeft"> New 'left' value </param>
		/// <param name="newTop"> New 'top' value </param>
		/// <param name="newRight"> New 'right' value </param>
		/// <param name="newBottom"> New 'bottom' value </param>
		public EdgeValues Set(float newLeft, float newTop, float newRight, float newBottom)
		{
			Left = newLeft;
			Top = newTop;
			Right = newRight;
			Bottom = newBottom;
			return this;
		}

		/// <summary> Set the 'left' value of an existing edge values set </summary>
		/// <param name="newLeft"> New left value </param>
		public EdgeValues SetLeft(float newLeft)
		{
			Left = newLeft;
			return this;
		}

		/// <summary> Set the 'top' value of an existing edge values set </summary>
		/// <param name="newTop"> New top value </param>
		public EdgeValues SetTop(float newTop)
		{
			Top = newTop;
			return this;
		}

		/// <summary> Set the 'right' value of an existing edge values set </summary>
		/// <param name="newRight"> New right value </param>
		public EdgeValues SetRight(float newRight)
		{
			Right = newRight;
			return this;
		}

		/// <summary> Set the 'bottom' value of an existing edge values set </summary>
		/// <param name="newBottom"> New 'bottom' value </param>
		public EdgeValues SetBottom(float newBottom)
		{
			Bottom = newBottom;
			return this;
		}

		private static readonly EdgeValues ZeroValues = new EdgeValues(0f, 0f, 0f, 0f);
		private static readonly EdgeValues OneValues = new EdgeValues(1f, 1f, 1f, 1f);
		private static readonly EdgeValues OneLeftRightValues = new EdgeValues(1f, 0f, 1f, 0f);
		private static readonly EdgeValues OneTopBottomValues = new EdgeValues(0f, 1f, 0f, 1f);
		private static readonly EdgeValues TwoValues = new EdgeValues(2f, 2f, 2f, 2f);
		private static readonly EdgeValues TwoLeftRightValues = new EdgeValues(2f, 0f, 2f, 0f);
		private static readonly EdgeValues TwoTopBottomValues = new EdgeValues(0f, 2f, 0f, 2f);
		private static readonly EdgeValues ThreeValues = new EdgeValues(3f, 3f, 3f, 3f);
		private static readonly EdgeValues ThreeLeftRightValues = new EdgeValues(3f, 0f, 3f, 0f);
		private static readonly EdgeValues ThreeTopBottomValues = new EdgeValues(0f, 3f, 0f, 3f);
		private static readonly EdgeValues FourValues = new EdgeValues(4f, 4f, 4f, 4f);
		private static readonly EdgeValues FourLeftRightValues = new EdgeValues(4f, 0f, 4f, 0f);
		private static readonly EdgeValues FourTopBottomValues = new EdgeValues(0f, 4f, 0f, 4f);


		/// <summary> Shorthand for writing EdgeValues(0f, 0f, 0f, 0f) </summary>
		public static EdgeValues zero => ZeroValues;

		/// <summary> Shorthand for writing EdgeValues(1f, 1f, 1f, 1f) </summary>
		public static EdgeValues one => OneValues;

		/// <summary> Shorthand for writing EdgeValues(1f, 0f, 1f, 0f) </summary>
		public static EdgeValues oneLeftRight => OneLeftRightValues;

		/// <summary> Shorthand for writing EdgeValues(0f, 1f, 0f, 1f) </summary>
		public static EdgeValues oneTopBottom => OneTopBottomValues;

		/// <summary> Shorthand for writing EdgeValues(2f, 2f, 2f, 2f) </summary>
		public static EdgeValues two => TwoValues;

		/// <summary> Shorthand for writing EdgeValues(2f, 0f, 2f, 0f) </summary>
		public static EdgeValues twoLeftRight => TwoLeftRightValues;

		/// <summary> Shorthand for writing EdgeValues(0f, 2f, 0f, 2f) </summary>
		public static EdgeValues twoTopBottom => TwoTopBottomValues;

		/// <summary> Shorthand for writing EdgeValues(3f, 3f, 3f, 3f) </summary>
		public static EdgeValues three => ThreeValues;

		/// <summary> Shorthand for writing EdgeValues(3f, 0f, 3f, 0f) </summary>
		public static EdgeValues threeLeftRight => ThreeLeftRightValues;

		/// <summary> Shorthand for writing EdgeValues(0f, 3f, 0f, 3f) </summary>
		public static EdgeValues threeTopBottom => ThreeTopBottomValues;

		/// <summary> Shorthand for writing EdgeValues(4f, 4f, 4f, 4f) </summary>
		public static EdgeValues four => FourValues;

		/// <summary> Shorthand for writing EdgeValues(4f, 0f, 4f, 0f) </summary>
		public static EdgeValues fourLeftRight => FourLeftRightValues;

		/// <summary> Shorthand for writing EdgeValues(0f, 4f, 0f, 4f) </summary>
		public static EdgeValues fourTopBottom => FourTopBottomValues;

		public bool Equals(EdgeValues other) => Left == (double) other.Left && Top == (double) other.Top && Right == (double) other.Right && Bottom == (double) other.Bottom;
		public override bool Equals(object obj) => obj is EdgeValues other && Equals(other);

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Left.GetHashCode();
				hashCode = (hashCode * 397) ^ Top.GetHashCode();
				hashCode = (hashCode * 397) ^ Right.GetHashCode();
				hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
				return hashCode;
			}
		}

		public static EdgeValues operator +(EdgeValues a, EdgeValues b) => new EdgeValues(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);

		public static EdgeValues operator -(EdgeValues a, EdgeValues b) => new EdgeValues(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);

		public static EdgeValues operator -(EdgeValues a) => new EdgeValues(-a.Left, -a.Top, -a.Right, -a.Bottom);

		public static EdgeValues operator *(EdgeValues a, float value) => new EdgeValues(a.Left * value, a.Top * value, a.Right * value, a.Bottom * value);

		public static EdgeValues operator *(float value, EdgeValues a) => new EdgeValues(a.Left * value, a.Top * value, a.Right * value, a.Bottom * value);

		public static EdgeValues operator *(EdgeValues a, int value) => new EdgeValues(a.Left * value, a.Top * value, a.Right * value, a.Bottom * value);

		public static EdgeValues operator *(int value, EdgeValues a) => new EdgeValues(a.Left * value, a.Top * value, a.Right * value, a.Bottom * value);

		public static EdgeValues operator /(EdgeValues a, float value) => new EdgeValues(a.Left / value, a.Top / value, a.Right / value, a.Bottom / value);

		public static EdgeValues operator /(EdgeValues a, int value) => new EdgeValues(a.Left / value, a.Top / value, a.Right / value, a.Bottom / value);

		public static bool operator ==(EdgeValues a, EdgeValues b)
		{
			float left = a.Left - b.Left;
			float top = a.Top - b.Top;
			float right = a.Right - b.Right;
			float bottom = a.Bottom - b.Bottom;
			return (double) left * (double) left +
			       (double) top * (double) top +
			       (double) right * (double) right +
			       (double) bottom * (double) bottom < 9.99999943962493E-11;
		}

		public static bool operator !=(EdgeValues a, EdgeValues b) => !(a == b);

		/// <summary> Returns a nicely formatted string for this values set </summary>
		public override string ToString() => $"({Left}, {Top}, {Right}, {Bottom})";

		/// <summary> Returns a nicely formatted string for this values set </summary>
		/// <param name="verbose"> Show values names </param>
		public string ToString(bool verbose) => verbose ? $"(left: {Left}, top: {Top}, right: {Right}, bottom: {Bottom})" : $"({Left}, {Top}, {Right}, {Bottom})";
	}
}