/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// A color struct for storing and maniputing colors in the HCL (hue, chroma, and lightness) color space.
	/// </summary>
	[Serializable] public struct ColorHCL
	{
		#region Fields and Direct Constructors

		/// <summary>
		/// The color's hue, in the range [0, 1).
		/// </summary>
		/// <remarks>Note that hue is a circular range, conceptually wrapping around back to 0 after reaching a value of 1.</remarks>
		public float h;

		/// <summary>
		/// The color's chroma, in the range [0, 1].
		/// </summary>
		public float c;

		/// <summary>
		/// The color's lightness, in the range [0, 1].
		/// </summary>
		public float l;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given hue, chroma, and lightness, assuming an opacity of 1.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="c">The color's chroma.</param>
		/// <param name="l">The color's value.</param>
		public ColorHCL(float h, float c, float l)
		{
			this.h = h;
			this.c = c;
			this.l = l;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given hue, chroma, lightness, and opacity.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="c">The color's chroma.</param>
		/// <param name="l">The color's lightness.</param>
		/// <param name="a">The color's opacity.</param>
		public ColorHCL(float h, float c, float l, float a)
		{
			this.h = h;
			this.c = c;
			this.l = l;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HCL color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HCL.</param>
		public ColorHCL(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HCL color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HCL color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HCL.</param>
		/// <param name="g">The green component of the RGB color to convert to HCL.</param>
		/// <param name="b">The blue component of the RGB color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the HCL color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HCL.</param>
		/// <param name="g">The green component of the RGB color to convert to HCL.</param>
		/// <param name="b">The blue component of the RGB color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromRGB(float r, float g, float b, float a)
		{
			ColorHCL hcl;
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);

			hcl.c = max - min;
			hcl.h = Detail.HueUtility.FromRGB(r, g, b, max, hcl.c);
			hcl.l = (max + min) * 0.5f;
			hcl.a = a;

			return hcl;
		}

		/// <summary>
		/// Converts the given HCL color to the RGB color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static explicit operator Color(ColorHCL hcl)
		{
			float min = hcl.l - hcl.c * 0.5f;
			return (hcl.c > 0f) ? Detail.HueUtility.ToRGB(hcl.h, hcl.c, min, hcl.a) : new Color(min, min, min, hcl.a);
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the HCL color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HCL.</param>
		public ColorHCL(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HCL color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HCL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HCL.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HCL.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the HCL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HCL.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HCL.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromCMY(float c, float m, float y, float a)
		{
			ColorHCL hcl;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			hcl.c = max - min;
			hcl.h = Detail.HueUtility.FromCMY(c, m, y, min, hcl.c);
			hcl.l = 1f - (max + min) * 0.5f;
			hcl.a = a;

			return hcl;
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the HCL color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HCL.</param>
		public ColorHCL(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCL color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HCL.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HCL.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HCL.</param>
		/// <param name="k">The key component of the CMYK color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HCL.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HCL.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HCL.</param>
		/// <param name="k">The key component of the CMYK color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromCMYK(float c, float m, float y, float k, float a)
		{
			if (k >= 1f) return new ColorHCL(0f, 0f, 0f, a);

			float kInv = 1f - k;

			ColorHCL hcl;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			hcl.c = max - min;
			hcl.h = Detail.HueUtility.FromCMY(c, m, y, min, hcl.c);
			hcl.c *= kInv;
			hcl.l = kInv - (max + min) * kInv * 0.5f;
			hcl.a = a;

			return hcl;
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the HCL color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCL.</param>
		public ColorHCL(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HCL color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HCL.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HCL.</param>
		/// <param name="v">The value component of the HSV color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HCL.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HCL.</param>
		/// <param name="v">The value component of the HSV color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHSV(float h, float s, float v, float a)
		{
			return new ColorHCL(h, Detail.ValueUtility.GetChroma(s, v), v * (1f - s * 0.5f), a);
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the HCL color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HCL.</param>
		public ColorHCL(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HCL color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HCL.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HCL.</param>
		/// <param name="v">The value component of the HCV color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HCL.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HCL.</param>
		/// <param name="v">The value component of the HCV color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHCV(float h, float c, float v, float a)
		{
			return new ColorHCL(h, c, v - c * 0.5f, a);
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the HCL color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HCL.</param>
		public ColorHCL(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HCL color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HCL.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HCL.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HCL.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HCL.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHSL(float h, float s, float l, float a)
		{
			float c = Detail.LightnessUtility.GetChroma(s, l);

			return new ColorHCL(h, c, l, a);
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the HCL color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HCL.</param>
		public ColorHCL(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HCL color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HCL.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HCL.</param>
		/// <param name="y">The luma component of the HSY color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HCL.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HCL.</param>
		/// <param name="y">The luma component of the HSY color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHSY(float h, float s, float y, float a)
		{
			float c = Detail.LumaUtility.GetChroma(h, s, y);
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(h, c, out r, out g, out b);

				float min = y - Detail.LumaUtility.FromRGB(r, g, b);

				return new ColorHCL(h, c, c * 0.5f + min, a);
			}
			else
			{
				return new ColorHCL(h, 0f, y, a);
			}
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the HCL color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HCL.</param>
		public ColorHCL(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HCL color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HCL.</param>
		/// <returns>The color converted to the HCL color space.</returns>
		public static explicit operator ColorHCL(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HCL.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HCL.</param>
		/// <param name="y">The luma component of the HCY color to convert to HCL.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the HCL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HCL.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HCL.</param>
		/// <param name="y">The luma component of the HCY color to convert to HCL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCL representation of the given color.</returns>
		public static ColorHCL FromHCY(float h, float c, float y, float a)
		{
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(h, c, out r, out g, out b);

				float min = y - Detail.LumaUtility.FromRGB(r, g, b);

				return new ColorHCL(h, c, c * 0.5f + min, a);
			}
			else
			{
				return new ColorHCL(h, 0f, y, a);
			}
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with hue as x, chroma as y, and lightness as z, while opacity is discarded.
		/// </summary>
		/// <param name="hcl">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided HCL color.</returns>
		public static explicit operator Vector3(ColorHCL hcl)
		{
			return new Vector3(hcl.h, hcl.c, hcl.l);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, chroma as y, lightness as z, and opacity as w.
		/// </summary>
		/// <param name="hcl">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HCL color.</returns>
		public static explicit operator Vector4(ColorHCL hcl)
		{
			return new Vector4(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an HCL color, with x as hue, y as chroma, z as lightness, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an HCL color.</param>
		/// <returns>The HCL color converted from the provided vector.</returns>
		public static explicit operator ColorHCL(Vector3 v)
		{
			return new ColorHCL(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HCL color, with x as hue, y as chroma, z as lightness, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HCL color.</param>
		/// <returns>The HCL color converted from the provided vector.</returns>
		public static explicit operator ColorHCL(Vector4 v)
		{
			return new ColorHCL(v.x, v.y, v.z, v.w);
		}

		#endregion

		#region Channel Indexing

		/// <summary>
		/// The number of color channels, including opacity, for colors in this color space.
		/// </summary>
		/// <remarks>For HSV colors, the value is 4, for hue, saturation, lightness, and opacity.</remarks>
		public const int channelCount = 4;

		/// <summary>
		/// Provides access to the four color channels using a numeric zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index for accessing hue (0), chroma (1), lightness (2), or opacity (3).</param>
		/// <returns>The color channel corresponding to the channel index specified.</returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return h;
					case 1: return c;
					case 2: return l;
					case 3: return a;
					default: throw new ArgumentOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0: h = value; break;
					case 1: c = value; break;
					case 2: l = value; break;
					case 3: a = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		#endregion

		#region Lerp

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
		/// <seealso cref="LerpUnclamped(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpForward(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpBackward(ColorHCL, ColorHCL, float)"/>
		public static ColorHCL Lerp(ColorHCL a, ColorHCL b, float t)
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
		/// <seealso cref="Lerp(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCL, ColorHCL, float)"/>
		public static ColorHCL LerpUnclamped(ColorHCL a, ColorHCL b, float t)
		{
			return new ColorHCL(
				Detail.HueUtility.LerpUnclamped(a.h, b.h, t),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.l, b.l, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always increasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpForwardUnclamped(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="Lerp(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpBackward(ColorHCL, ColorHCL, float)"/>
		public static ColorHCL LerpForward(ColorHCL a, ColorHCL b, float t)
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
		/// <seealso cref="LerpForward(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCL, ColorHCL, float)"/>
		public static ColorHCL LerpForwardUnclamped(ColorHCL a, ColorHCL b, float t)
		{
			return new ColorHCL(
				Detail.HueUtility.LerpForwardUnclamped(a.h, b.h, t),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.l, b.l, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always decreasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="Lerp(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpForward(ColorHCL, ColorHCL, float)"/>
		public static ColorHCL LerpBackward(ColorHCL a, ColorHCL b, float t)
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
		/// <seealso cref="LerpBackward(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHCL, ColorHCL, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHCL, ColorHCL, float)"/>
		public static ColorHCL LerpBackwardUnclamped(ColorHCL a, ColorHCL b, float t)
		{
			return new ColorHCL(
				Detail.HueUtility.LerpBackwardUnclamped(a.h, b.h, t),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.l, b.l, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		#endregion

		#region Arithmetic Operators

		/// <summary>
		/// Adds the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be added.</param>
		/// <param name="b">The second color whose channels are to be added.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color plus the corresponding value of the second channel.</returns>
		public static ColorHCL operator +(ColorHCL a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(a.h + b.h, 1f), a.c + b.c, a.l + b.l, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorHCL operator -(ColorHCL a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(a.h - b.h, 1f), a.c - b.c, a.l - b.l, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The value by which to multiply the color's channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHCL operator *(float a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(b.h * a, 1f), b.c * a, b.l * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHCL operator *(ColorHCL a, float b)
		{
			return new ColorHCL(Mathf.Repeat(a.h * b, 1f), a.c * b, a.l * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorHCL operator *(ColorHCL a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(a.h * b.h, 1f), a.c * b.c, a.l * b.l, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorHCL operator /(ColorHCL a, float b)
		{
			return new ColorHCL(Mathf.Repeat(a.h / b, 1f), a.c / b, a.l / b, a.a / b);
		}

		#endregion

		#region Comparisons

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
			return other is ColorHCL && this == (ColorHCL)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// or if the hue value changes to a value which is equivalent due to the circular nature of hue's range but are nonetheless
		/// distinct values, then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return h.GetHashCode() ^ c.GetHashCode() ^ l.GetHashCode() ^ a.GetHashCode();
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
		public static bool operator ==(ColorHCL lhs, ColorHCL rhs)
		{
			return lhs.h == rhs.h && lhs.c == rhs.c && lhs.l == rhs.l && lhs.a == rhs.a;
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
		public static bool operator !=(ColorHCL lhs, ColorHCL rhs)
		{
			return lhs.h != rhs.h || lhs.c != rhs.c || lhs.l != rhs.l || lhs.a != rhs.a;
		}

		#endregion

		#region Conversion to String

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("HCLA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, c, l, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("HCLA({0}, {1}, {2}, {3})", h.ToString(format), c.ToString(format), l.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for hue, chroma, and lightness together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		/// <remarks>To be valid within the RGB color space, lightness must be within the range [chroma, 1], and chroma must be in the range [0, lightness].</remarks>
		/// <seealso cref="GetMaxChroma(float)"/>
		/// <seealso cref="GetMinMaxLightness(float, out float, out float)"/>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & l >= 0f & l <= 1f & c >= 0f) && (c <= GetMaxChroma(l));
		}

		/// <summary>
		/// Gets the nearest HCL color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid HCL color.</returns>
		public ColorHCL GetNearestValid()
		{
			float c = this.c;
			float l = this.l;

			if (c * 2f + l < 2.5f || c * 2f - l < 1.5f)
			{
				float lDouble = l * 2f;
				if (c > lDouble)
				{
					float n = (2f * c + l);
					c = 0.4f * n;
					l = 0.2f * n;
				}
				else if (c > 2f - lDouble)
				{
					float n = (2f * c - l);
					c = 0.4f * n + 0.4f;
					l = -0.2f * n + 0.8f;
				}
				return new ColorHCL(Mathf.Repeat(h, 1f), Mathf.Max(0f, c), Mathf.Clamp01(l), Mathf.Clamp01(a));
			}
			else
			{
				return new ColorHCL(Mathf.Repeat(h, 1f), 1f, 0.5f, Mathf.Clamp01(a));
			}
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>For an HCL color to be canonical, the hue must be in the range [0, 1).  Also, if the lightness is 0 or 1,
		/// then the chroma must be 0, and if the lightness is 0 or the chroma is 0 or 1, then the hue must be 0.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return (h >= 0f & h < 1f & (h == 0f | (c != 0f & l != 0f & l != 1f)) & (c == 0f | (l != 0f & l != 1f)));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the HCL color space, if lightness is 0 or 1, then hue and chroma are set to 0.  If chroma is 0,
		/// then hue is set to 0.  Otherwise, if hue is outside the range [0, 1), it is wrapped such that it is
		/// restricted to that range.  In all other cases, the color is already canonical.</para>
		/// </remarks>
		public ColorHCL GetCanonical()
		{
			if (c == 0f | l == 0f | l == 1f) return new ColorHCL(0f, 0f, l, a);
			return new ColorHCL(Mathf.Repeat(h, 1f), c, l, a);
		}

		/// <summary>
		/// Indicates the value that the lightness channel must have when the chroma channel is at its maximum value, if the color is to remain valid within the RGB color space.
		/// </summary>
		/// <returns>The lightness channel at maximum chroma.</returns>
		/// <remarks>For the HCL color space, the lightness channel is always 1 when chroma is at its maximum value of 1/2.</remarks>
		public static float GetLightnessAtMaxChroma()
		{
			return Detail.LightnessUtility.GetLightnessAtMaxChroma();
		}

		/// <summary>
		/// Indicates the range of values that the lightness channel can have for a given chroma value, if the color is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="c">The chroma value for which the lightness channel range is to be returned.</param>
		/// <param name="lMin">The minimum value of the lightness channel for the given chroma.</param>
		/// <param name="lMax">The maximum value of the lightness channel for the given chroma.</param>
		/// <remarks>For the HCL color space, the lightness channel must be in the range [chroma / 2, 1 - chroma / 2].</remarks>
		public static void GetMinMaxLightness(float c, out float lMin, out float lMax)
		{
			Detail.LightnessUtility.GetMinMaxLightness(c, out lMin, out lMax);
		}

		/// <summary>
		/// Indicates the maximum value that the chroma channel can have for a given value of the lightness channel, if it is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="l">The value of the lightness channel for which the maximum chroma is to be returned.</param>
		/// <returns>The maximum chroma for the given lightness.</returns>
		/// <remarks>For the HCL color space, the chroma channel must be in the range [0, 1 - Abs(2 * lightness - 1)].</remarks>
		public static float GetMaxChroma(float l)
		{
			return Detail.LightnessUtility.GetMaxChroma(l);
		}

		#endregion
	}
}
