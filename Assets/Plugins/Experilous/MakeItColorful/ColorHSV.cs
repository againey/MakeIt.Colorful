/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// A color struct for storing and maniputing colors in the HSV (hue, saturation, and value) color space.
	/// </summary>
	[Serializable] public struct ColorHSV
	{
		/// <summary>
		/// The color's hue, in the range [0, 1).
		/// </summary>
		/// <remarks>Note that hue is a circular range, conceptually wrapping around back to 0 after reaching a value of 1.</remarks>
		public float h;

		/// <summary>
		/// The color's saturation, in the range [0, 1].
		/// </summary>
		public float s;

		/// <summary>
		/// The color's value, in the range [0, 1].
		/// </summary>
		public float v;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given hue, saturation, and value, assuming an opacity of 1.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="s">The color's saturation.</param>
		/// <param name="v">The color's value.</param>
		public ColorHSV(float h, float s, float v)
		{
			this.h = h;
			this.s = s;
			this.v = v;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given hue, saturation, value, and opacity.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="s">The color's saturation.</param>
		/// <param name="v">The color's value.</param>
		/// <param name="a">The color's opacity.</param>
		public ColorHSV(float h, float s, float v, float a)
		{
			this.h = h;
			this.s = s;
			this.v = v;
			this.a = a;
		}

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSV.</param>
		public ColorHSV(Color rgb)
		{
			this = FromRGB(rgb);
		}

		/// <summary>
		/// Converts the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromRGB(Color rgb)
		{
			ColorHSV hsv;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			float chroma = max - min;

			if (chroma > 0f)
			{
				if (rgb.r == max)
				{
					hsv.h = Mathf.Repeat((rgb.g - rgb.b) / chroma, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hsv.h = ((rgb.b - rgb.r) / chroma + 2f) / 6f;
				}
				else
				{
					hsv.h = ((rgb.r - rgb.g) / chroma + 4f) / 6f;
				}

				hsv.s = chroma / max;
			}
			else
			{
				hsv.h = 0f;
				hsv.s = 0f;
			}

			hsv.v = max;
			hsv.a = rgb.a;

			return hsv;
		}

		/// <summary>
		/// Converts the HSV color to the RGB color space.
		/// </summary>
		/// <returns>The RGB representation of the color.</returns>
		public Color ToRGB()
		{
			float chroma = v * s;
			float min = v - chroma;
			Color rgb = new Color(min, min, min, a);
			if (chroma > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					rgb.r += chroma;
					rgb.g += chroma * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					rgb.g += chroma;
					rgb.r += chroma * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					rgb.g += chroma;
					rgb.b += chroma * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					rgb.b += chroma;
					rgb.g += chroma * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					rgb.b += chroma;
					rgb.r += chroma * (scaledHue - 4f);
				}
				else
				{
					rgb.r += chroma;
					rgb.b += chroma * (6f - scaledHue);
				}
			}
			return rgb;
		}

		/// <summary>
		/// Converts the given HCV color to the HSV color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCV(ColorHCV hcv)
		{
			return hcv.ToHSV();
		}

		/// <summary>
		/// Converts the HSV color to the HCV color space.
		/// </summary>
		/// <returns>The HCV representation of the color.</returns>
		public ColorHCV ToHCV()
		{
			return new ColorHCV(h, s * v, v, a);
		}

		/// <summary>
		/// Converts the given HSV color to the RGB color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static explicit operator Color(ColorHSV hsv)
		{
			return hsv.ToRGB();
		}

		/// <summary>
		/// Converts the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(Color rgb)
		{
			return FromRGB(rgb);
		}

		/// <summary>
		/// Converts the given HSV color to the HCV color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorHSV hsv)
		{
			return hsv.ToHCV();
		}

		/// <summary>
		/// Provides access to the four color channels using a numeric zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index for accessing hue (0), saturation (1), value (2), or opacity (3).</param>
		/// <returns>The color channel corresponding to the channel index specified.</returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return h;
					case 1: return s;
					case 2: return v;
					case 3: return a;
					default: throw new ArgumentOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0: h = value; break;
					case 1: s = value; break;
					case 2: v = value; break;
					case 3: a = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <remarks>
		/// <para>Because hue is a circular range, interpolation between two hue values is a little bit different than
		/// ordinary non-circular values.  Instead of doing a straight mathematical linear interpolation, the distance
		/// from the first hue to the second is checked in both the forward and backward directions, and the interpolation
		/// is performed in whichever direction is shortest.  If the two hues are exact polar opposites and thus equally
		/// far in both directions, the interpolation is always TODO currently not what I want.</para>
		/// </remarks>
		/// <seealso cref="LerpUnclamped(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpForward(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpBackward(ColorHSV, ColorHSV, float)"/>
		public static ColorHSV Lerp(ColorHSV a, ColorHSV b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Typically within the range [0, 1], but does not need to be, and will not be clamped.</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <remarks>
		/// <para>Because hue is a circular range, interpolation between two hue values is a little bit different than
		/// ordinary non-circular values.  Instead of doing a straight mathematical linear interpolation, the distance
		/// from the first hue to the second is checked in both the forward and backward directions, and the interpolation
		/// is performed in whichever direction is shortest.  If the two hues are exact polar opposites and thus equally
		/// far in both directions, the interpolation is always TODO currently not what I want.</para>
		/// <para>When specifying a <paramref name="t"/> value outside the range [0, 1], the resulting color may no longer be
		/// within the valid range of the color space, even if the two original colors are within the valid range.</para>
		/// </remarks>
		/// <seealso cref="Lerp(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSV, ColorHSV, float)"/>
		public static ColorHSV LerpUnclamped(ColorHSV a, ColorHSV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHSV(
				hueDelta <= 0.5f
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t)
							: Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always increasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpForwardUnclamped(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="Lerp(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpBackward(ColorHSV, ColorHSV, float)"/>
		public static ColorHSV LerpForward(ColorHSV a, ColorHSV b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always increasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Typically within the range [0, 1], but does not need to be, and will not be clamped.</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <remarks>
		/// <para>When specifying a <paramref name="t"/> value outside the range [0, 1], the resulting color may no longer be
		/// within the valid range of the color space, even if the two original colors are within the valid range.</para>
		/// </remarks>
		/// <seealso cref="LerpForward(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSV, ColorHSV, float)"/>
		public static ColorHSV LerpForwardUnclamped(ColorHSV a, ColorHSV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSV(
				hueA <= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always decreasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="Lerp(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpForward(ColorHSV, ColorHSV, float)"/>
		public static ColorHSV LerpBackward(ColorHSV a, ColorHSV b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always decreasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Typically within the range [0, 1], but does not need to be, and will not be clamped.</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <remarks>
		/// <para>When specifying a <paramref name="t"/> value outside the range [0, 1], the resulting color may no longer be
		/// within the valid range of the color space, even if the two original colors are within the valid range.</para>
		/// </remarks>
		/// <seealso cref="LerpBackward(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHSV, ColorHSV, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHSV, ColorHSV, float)"/>
		public static ColorHSV LerpBackwardUnclamped(ColorHSV a, ColorHSV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSV(
				hueA >= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Adds the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be added.</param>
		/// <param name="b">The second color whose channels are to be added.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color plus the corresponding value of the second channel.</returns>
		public static ColorHSV operator +(ColorHSV a, ColorHSV b)
		{
			return new ColorHSV(Mathf.Repeat(a.h + b.h, 1f), a.s + b.s, a.v + b.v, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorHSV operator -(ColorHSV a, ColorHSV b)
		{
			return new ColorHSV(Mathf.Repeat(a.h - b.h, 1f), a.s - b.s, a.v - b.v, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The value by which to multiply the color's channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHSV operator *(float b, ColorHSV a)
		{
			return new ColorHSV(Mathf.Repeat(a.h * b, 1f), a.s * b, a.v * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHSV operator *(ColorHSV a, float b)
		{
			return new ColorHSV(Mathf.Repeat(a.h * b, 1f), a.s * b, a.v * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorHSV operator *(ColorHSV a, ColorHSV b)
		{
			return new ColorHSV(Mathf.Repeat(a.h * b.h, 1f), a.s * b.s, a.v * b.v, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorHSV operator /(ColorHSV a, float b)
		{
			return new ColorHSV(Mathf.Repeat(a.h / b, 1f), a.s / b, a.v / b, a.a / b);
		}

		/// <summary>
		/// Checks if the color is equal to a specified color.
		/// </summary>
		/// <param name="other">The other color to which the color is to be compared.</param>
		/// <returns>Returns true if both colors are equal, false otherwise.</returns>
		/// <remarks>This function checks for perfect bitwise equality.  If any of the channels differ by even the smallest amount,
		/// or if the two hue values are equivalent but one or both are outside of the normal range of [0, 1), then this function
		/// will return false.</remarks>
		public override bool Equals(object other)
		{
			return other is ColorHSV && this == (ColorHSV)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// or if the hue value changes to a value which is equivalent due to the circular nature of hue's range but are nonetheless
		/// distinct values, then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return h.GetHashCode() ^ s.GetHashCode() ^ v.GetHashCode() ^ a.GetHashCode();
		}

		/// <summary>
		/// Checks if the two colors are equal to each other.
		/// </summary>
		/// <param name="lhs">The first color compare for equality.</param>
		/// <param name="rhs">The second color compare for equality.</param>
		/// <returns>Returns true if both colors are equal, false otherwise.</returns>
		/// <remarks>This function checks for perfect bitwise equality.  If any of the channels differ by even the smallest amount,
		/// or if the two hue values are equivalent but one or both are outside of the normal range of [0, 1), then this function
		/// will return false.</remarks>
		public static bool operator ==(ColorHSV lhs, ColorHSV rhs)
		{
			return lhs.h == rhs.h && lhs.s == rhs.s && lhs.v == rhs.v && lhs.a == rhs.a;
		}

		/// <summary>
		/// Checks if the two colors are not equal to each other.
		/// </summary>
		/// <param name="lhs">The first color compare for inequality.</param>
		/// <param name="rhs">The second color compare for inequality.</param>
		/// <returns>Returns true if the two colors are not equal, false if they are equal.</returns>
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels differ by even the smallest amount,
		/// or if the two hue values are equivalent but one or both are outside of the normal range of [0, 1), then this function
		/// will return true.</remarks>
		public static bool operator !=(ColorHSV lhs, ColorHSV rhs)
		{
			return lhs.h != rhs.h || lhs.s != rhs.s || lhs.v != rhs.v || lhs.a != rhs.a;
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, saturation as y, value as z, and opacity as w.
		/// </summary>
		/// <param name="hsv">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HSV color.</returns>
		public static implicit operator Vector4(ColorHSV hsv)
		{
			return new Vector4(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HSV color, with x as hue, y as saturation, z as value, and and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HSV color.</param>
		/// <returns>The HSV color converted from the provided vector.</returns>
		public static implicit operator ColorHSV(Vector4 v)
		{
			return new ColorHSV(v.x, v.y, v.z, v.w);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("HSVA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, v, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("HSVA({0}, {1}, {2}, {3})", h.ToString(format), s.ToString(format), v.ToString(format), a.ToString(format));
		}
	}
}
