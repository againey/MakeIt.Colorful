/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
using Math = UnityEngine.Mathf;
#else
using Math = Experilous.MakeItColorful.Detail.LerpUtility;
#endif

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// A color struct for storing and maniputing colors in the CMYK (cyan, magenta, yellow, and key) color space.
	/// </summary>
	[Serializable] public struct ColorCMYK
	{
		#region Fields and Direct Constructors

		/// <summary>
		/// The color's cyan channel, in the range [0, 1].
		/// </summary>
		public float c;

		/// <summary>
		/// The color's magenta channel, in the range [0, 1].
		/// </summary>
		public float m;

		/// <summary>
		/// The color's yellow channel, in the range [0, 1].
		/// </summary>
		public float y;

		/// <summary>
		/// The color's key channel, in the range [0, 1].
		/// </summary>
		public float k;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given cyan, magenta, and yellow, assuming an opacity of 1.
		/// </summary>
		/// <param name="c">The color's cyan channel value.</param>
		/// <param name="m">The color's magenta channel value.</param>
		/// <param name="y">The color's yellow channel value.</param>
		/// <param name="k">The color's key channel value.</param>
		public ColorCMYK(float c, float m, float y, float k)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			this.k = k;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given cyan, magenta, yellow, and opacity.
		/// </summary>
		/// <param name="c">The color's cyan channel value.</param>
		/// <param name="m">The color's magenta channel value.</param>
		/// <param name="y">The color's yellow channel value.</param>
		/// <param name="k">The color's key channel value.</param>
		/// <param name="a">The color's opacity channel value.</param>
		public ColorCMYK(float c, float m, float y, float k, float a)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			this.k = k;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the CMYK color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to CMYK.</param>
		public ColorCMYK(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the CMYK color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static implicit operator ColorCMYK(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the CMYK color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to CMYK.</param>
		/// <param name="g">The green component of the RGB color to convert to CMYK.</param>
		/// <param name="b">The blue component of the RGB color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the CMYK color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to CMYK.</param>
		/// <param name="g">The green component of the RGB color to convert to CMYK.</param>
		/// <param name="b">The blue component of the RGB color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromRGB(float r, float g, float b, float a)
		{
			float k = 1f - Mathf.Max(Mathf.Max(r, g), b);
			if (k < 1f)
			{
				float kInv = 1f - k;
				return new ColorCMYK((1f - r - k) / kInv, (1f - g - k) / kInv, (1f - b - k) / kInv, k, a);
			}
			else
			{
				return new ColorCMYK(0f, 0f, 0f, 1f, a);
			}
		}

		/// <summary>
		/// Converts the given CMYK color to the RGB color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static implicit operator Color(ColorCMYK cmyk)
		{
			float kInv = 1f - cmyk.k;
			return new Color((1f - cmyk.c) * kInv, (1f - cmyk.m) * kInv, (1f - cmyk.y) * kInv, cmyk.a);
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the CMYK color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to CMY.</param>
		public ColorCMYK(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the CMYK color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the CMYK color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to CMYK.</param>
		/// <param name="m">The magenta component of the CMY color to convert to CMYK.</param>
		/// <param name="y">The yellow component of the CMY color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the CMYK color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to CMYK.</param>
		/// <param name="m">The magenta component of the CMY color to convert to CMYK.</param>
		/// <param name="y">The yellow component of the CMY color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromCMY(float c, float m, float y, float a)
		{
			float k = Mathf.Min(Mathf.Min(c, m), y);
			if (k < 1f)
			{
				float kInv = 1f - k;
				return new ColorCMYK((c - k) / kInv, (m - k) / kInv, (y - k) / kInv, k, a);
			}
			else
			{
				return new ColorCMYK(0f, 0f, 0f, 1f, a);
			}
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the CMYK color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to CMYK.</param>
		public ColorCMYK(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the CMYK color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to CMYK.</param>
		/// <param name="s">The saturation component of the HSV color to convert to CMYK.</param>
		/// <param name="v">The value component of the HSV color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to CMYK.</param>
		/// <param name="s">The saturation component of the HSV color to convert to CMYK.</param>
		/// <param name="v">The value component of the HSV color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHSV(float h, float s, float v, float a)
		{
			float c = Detail.ValueUtility.GetChroma(s, v);
			float min = v - c;
			return (c > 0f) ? (ColorCMYK)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMYK(0f, 0f, 0f, 1f - min, a);
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the CMYK color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to CMYK.</param>
		public ColorCMYK(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the CMYK color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to CMYK.</param>
		/// <param name="c">The chroma component of the HCV color to convert to CMYK.</param>
		/// <param name="v">The value component of the HCV color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to CMYK.</param>
		/// <param name="c">The chroma component of the HCV color to convert to CMYK.</param>
		/// <param name="v">The value component of the HCV color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHCV(float h, float c, float v, float a)
		{
			float min = v - c;
			return (c > 0f) ? (ColorCMYK)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMYK(0f, 0f, 0f, 1f - min, a);
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the CMYK color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to CMYK.</param>
		public ColorCMYK(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the CMYK color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the CMYK color to convert to CMYK.</param>
		/// <param name="s">The saturation component of the CMYK color to convert to CMYK.</param>
		/// <param name="l">The lightness component of the CMYK color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the CMYK color to convert to CMYK.</param>
		/// <param name="s">The saturation component of the CMYK color to convert to CMYK.</param>
		/// <param name="l">The lightness component of the CMYK color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHSL(float h, float s, float l, float a)
		{
			float c = Detail.LightnessUtility.GetChroma(s, l);
			float min = l - c * 0.5f;
			return (c > 0f) ? (ColorCMYK)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMYK(0f, 0f, 0f, 1f - min, a);
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the CMYK color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to CMYK.</param>
		public ColorCMYK(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the CMYK color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to CMYK.</param>
		/// <param name="c">The chroma component of the HCL color to convert to CMYK.</param>
		/// <param name="l">The lightness component of the HCL color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to CMYK.</param>
		/// <param name="c">The chroma component of the HCL color to convert to CMYK.</param>
		/// <param name="l">The lightness component of the HCL color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHCL(float h, float c, float l, float a)
		{
			float min = l - c * 0.5f;
			return (c > 0f) ? (ColorCMYK)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMYK(0f, 0f, 0f, 1f - min, a);
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the CMYK color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to CMYK.</param>
		public ColorCMYK(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the CMYK color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to CMYK.</param>
		/// <param name="s">The saturation component of the HSY color to convert to CMYK.</param>
		/// <param name="y">The luma component of the HSY color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to CMYK.</param>
		/// <param name="s">The saturation component of the HSY color to convert to CMYK.</param>
		/// <param name="y">The luma component of the HSY color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHSY(float h, float s, float y, float a)
		{
			float c = Detail.LumaUtility.GetChroma(h, s, y);
			if (c > 0f)
			{
				Color rgb = Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, a);
				float min = y - Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
				rgb.r += min;
				rgb.g += min;
				rgb.b += min;
				return rgb;
 			}
			else
			{
				return new ColorCMYK(0f, 0f, 0f, 1f - y, a);
			}
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the CMYK color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to CMYK.</param>
		public ColorCMYK(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the CMYK color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to CMYK.</param>
		/// <returns>The color converted to the CMYK color space.</returns>
		public static explicit operator ColorCMYK(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to CMYK.</param>
		/// <param name="c">The chroma component of the HCY color to convert to CMYK.</param>
		/// <param name="y">The luma component of the HCY color to convert to CMYK.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the CMYK color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to CMYK.</param>
		/// <param name="c">The chroma component of the HCY color to convert to CMYK.</param>
		/// <param name="y">The luma component of the HCY color to convert to CMYK.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMYK representation of the given color.</returns>
		public static ColorCMYK FromHCY(float h, float c, float y, float a)
		{
			if (c > 0f)
			{
				Color rgb = Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, a);
				float min = y - Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
				rgb.r += min;
				rgb.g += min;
				rgb.b += min;
				return rgb;
 			}
			else
			{
				return new ColorCMYK(0f, 0f, 0f, 1f - y, a);
			}
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with cyan as x, magenta as y, and yellow as z, while key and opacity are discarded.
		/// </summary>
		/// <param name="cmyk">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided CMYK color.</returns>
		public static explicit operator Vector3(ColorCMYK cmyk)
		{
			return new Vector3(cmyk.c, cmyk.m, cmyk.y);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with cyan as x, magenta as y, yellow as z, and key as w, while opacity is discarded.
		/// </summary>
		/// <param name="cmyk">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided CMYK color.</returns>
		public static explicit operator Vector4(ColorCMYK cmyk)
		{
			return new Vector4(cmyk.c, cmyk.m, cmyk.y, cmyk.k);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an CMYK color, with x as cyan, y as magenta, z as yellow, assuming a key of 0 and an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an CMYK color.</param>
		/// <returns>The CMYK color converted from the provided vector.</returns>
		public static explicit operator ColorCMYK(Vector3 v)
		{
			return new ColorCMYK(v.x, v.y, v.z, 0f, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an CMYK color, with x as cyan, y as magenta, z as yellow, and w as key, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an CMYK color.</param>
		/// <returns>The CMYK color converted from the provided vector.</returns>
		public static explicit operator ColorCMYK(Vector4 v)
		{
			return new ColorCMYK(v.x, v.y, v.z, v.w, 1f);
		}

		#endregion

		#region Channel Indexing

		/// <summary>
		/// The number of color channels, including opacity, for colors in this color space.
		/// </summary>
		/// <remarks>For CMYK colors, the value is 5, for cyan, magenta, yellow, key, and opacity.</remarks>
		public const int channelCount = 5;

		/// <summary>
		/// Provides access to the five color channels using a numeric zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index for accessing cyan (0), magenta (1), yellow (2), key (3), or opacity (4).</param>
		/// <returns>The color channel corresponding to the channel index specified.</returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return c;
					case 1: return m;
					case 2: return y;
					case 3: return k;
					case 4: return a;
					default: throw new ArgumentOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0: c = value; break;
					case 1: m = value; break;
					case 2: y = value; break;
					case 3: k = value; break;
					case 4: a = value; break;
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
		/// <seealso cref="LerpUnclamped(ColorCMYK, ColorCMYK, float)"/>
		public static ColorCMYK Lerp(ColorCMYK a, ColorCMYK b, float t)
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
		/// <para>When specifying a <paramref name="t"/> value outside the range [0, 1], the resulting color may no longer be
		/// within the valid range of the color space, even if the two original colors are within the valid range.</para>
		/// </remarks>
		/// <seealso cref="Lerp(ColorCMYK, ColorCMYK, float)"/>
		public static ColorCMYK LerpUnclamped(ColorCMYK a, ColorCMYK b, float t)
		{
			return new ColorCMYK(
				Math.LerpUnclamped(a.c, b.c, t),
				Math.LerpUnclamped(a.m, b.m, t),
				Math.LerpUnclamped(a.y, b.y, t),
				Math.LerpUnclamped(a.k, b.k, t),
				Math.LerpUnclamped(a.a, b.a, t));
		}

		#endregion

		#region Arithmetic Operators

		/// <summary>
		/// Adds the color channels of the two specified colors together.
		/// </summary>
		/// <param name="a">The first color whose channels are to be added.</param>
		/// <param name="b">The second color whose channels are to be added.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color plus the corresponding value of the second channel.</returns>
		public static ColorCMYK operator +(ColorCMYK a, ColorCMYK b)
		{
			return new ColorCMYK(a.c + b.c, a.m + b.m, a.y + b.y, a.k + b.k, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorCMYK operator -(ColorCMYK a, ColorCMYK b)
		{
			return new ColorCMYK(a.c - b.c, a.m - b.m, a.y - b.y, a.k - b.k, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value.
		/// </summary>
		/// <param name="a">The value by which to multiply the color'm channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorCMYK operator *(float a, ColorCMYK b)
		{
			return new ColorCMYK(b.c * a, b.m * a, b.y * a, b.k * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color'm channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorCMYK operator *(ColorCMYK a, float b)
		{
			return new ColorCMYK(a.c * b, a.m * b, a.y * b, a.k * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorCMYK operator *(ColorCMYK a, ColorCMYK b)
		{
			return new ColorCMYK(a.c * b.c, a.m * b.m, a.y * b.y, a.k * b.k, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color'm channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorCMYK operator /(ColorCMYK a, float b)
		{
			return new ColorCMYK(a.c / b, a.m / b, a.y / b, a.k / b, a.a / b);
		}

		#endregion

		#region Comparisons

		/// <summary>
		/// Checks if the color is equal to a specified color.
		/// </summary>
		/// <param name="other">The other color to which the color is to be compared.</param>
		/// <returns>Returns true if both colors are equal, false otherwise.</returns>
		/// <remarks>This function checks for perfect bitwise equality.  If any of the channels differ by even the smallest amount,
		/// then this function will return false.</remarks>
		public override bool Equals(object other)
		{
			return other is ColorCMYK && this == (ColorCMYK)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return c.GetHashCode() ^ m.GetHashCode() ^ y.GetHashCode() ^ k.GetHashCode() ^ a.GetHashCode();
		}

		/// <summary>
		/// Checks if the two colors are equal to each other.
		/// </summary>
		/// <param name="lhs">The first color compare for equality.</param>
		/// <param name="rhs">The second color compare for equality.</param>
		/// <returns>Returns true if both colors are equal, false otherwise.</returns>
		/// <remarks>This function checks for perfect bitwise equality.  If any of the channels differ by even the smallest amount,
		/// then this function will return false.</remarks>
		public static bool operator ==(ColorCMYK lhs, ColorCMYK rhs)
		{
			return lhs.c == rhs.c && lhs.m == rhs.m && lhs.y == rhs.y && lhs.k == rhs.k && lhs.a == rhs.a;
		}

		/// <summary>
		/// Checks if the two colors are not equal to each other.
		/// </summary>
		/// <param name="lhs">The first color compare for inequality.</param>
		/// <param name="rhs">The second color compare for inequality.</param>
		/// <returns>Returns true if the two colors are not equal, false if they are equal.</returns>
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels differ by even the smallest amount,
		/// then this function will return true.</remarks>
		public static bool operator !=(ColorCMYK lhs, ColorCMYK rhs)
		{
			return lhs.c != rhs.c || lhs.m != rhs.m || lhs.y != rhs.y || lhs.k != rhs.k || lhs.a != rhs.a;
		}

		#endregion

		#region Conversion to String

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("CMYKA({0:F3}, {1:F3}, {2:F3}, {3:F3}, {4:F3})", c, m, y, k, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("CMYKA({0}, {1}, {2}, {3}, {4})", c.ToString(format), m.ToString(format), y.ToString(format), k.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for cyan, magenta, yellow, and key together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & c >= 0f & c <= 1f & m >= 0f & m <= 1f & y >= 0f & y <= 1f & k >= 0f & k <= 1f);
		}

		/// <summary>
		/// Gets the nearest CMYK color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid CMYK color.</returns>
		public ColorCMYK GetNearestValid()
		{
			return new ColorCMYK(Mathf.Clamp01(c), Mathf.Clamp01(m), Mathf.Clamp01(y), Mathf.Clamp01(k), Mathf.Clamp01(a));
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>For a CMYK color to be canonical, at least one of cyan, magenta, or yellow must be 0.  Also, if
		/// the key is 1, then cyan, magenta, and yellow must all be 0.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return ((c == 0f | m == 0f | y == 0f) & (k != 1f | (c == 0f & m == 0f & y == 0f)));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the CMYK color space, if either the key is 1 or if cyan, magenta, and yellow are all 1, then the
		/// color is essentially black, and the canonical form is to set cyan, magenta, and yellow all to 0 and key to
		/// 1.  Otherwise, if at least one of cyan, magenta, or yellow is 0, then the color is canonical.  If not, then,
		/// these three channels are reduced and the key is increased, so that at least one channel is 0.</para>
		/// </remarks>
		public ColorCMYK GetCanonical()
		{
			if (k == 1f | (c == 1f & m == 1f & y == 1f)) return new ColorCMYK(0f, 0f, 0f, 1f, a);
			float min = Mathf.Min(Mathf.Min(c, m), y);
			if (min > 0f) return (ColorCMYK)((ColorCMY)this);
			return this;
		}

		#endregion

		#region Color Constants

		/// <summary>
		/// Completely transparent black.  CMYKA is (0, 0, 0, 1, 0).
		/// </summary>
		public static ColorCMYK clear { get { return new ColorCMYK(0f, 0f, 0f, 1f, 0f); } }

		/// <summary>
		/// Solid black.  CMYKA is (0, 0, 0, 1, 1).
		/// </summary>
		public static ColorCMYK black { get { return new ColorCMYK(0f, 0f, 0f, 1f, 1f); } }

		/// <summary>
		/// Solid gray.  CMYKA is (0, 0, 0, 1/2, 1).
		/// </summary>
		public static ColorCMYK gray { get { return new ColorCMYK(0f, 0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid gray, with English spelling.  CMYKA is (0, 0, 0, 1/2, 1).
		/// </summary>
		public static ColorCMYK grey { get { return new ColorCMYK(0f, 0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid white.  CMYKA is (0, 0, 0, 0, 1).
		/// </summary>
		public static ColorCMYK white { get { return new ColorCMYK(0f, 0f, 0f, 0f, 1f); } }

		/// <summary>
		/// Solid red.  CMYKA is (0, 1, 1, 0, 1).
		/// </summary>
		public static ColorCMYK red { get { return new ColorCMYK(0f, 1f, 1f, 0f, 1f); } }

		/// <summary>
		/// Solid yellow.  CMYKA is (0, 0, 1, 0, 1).
		/// </summary>
		public static ColorCMYK yellow { get { return new ColorCMYK(0f, 0f, 1f, 0f, 1f); } }

		/// <summary>
		/// Solid green.  CMYKA is (1, 0, 1, 0, 1).
		/// </summary>
		public static ColorCMYK green { get { return new ColorCMYK(1f, 0f, 1f, 0f, 1f); } }

		/// <summary>
		/// Solic cyan.  CMYKA is (1, 0, 0, 0, 1).
		/// </summary>
		public static ColorCMYK cyan { get { return new ColorCMYK(1f, 0f, 0f, 0f, 1f); } }

		/// <summary>
		/// Solid blue.  CMYKA is (1, 1, 0, 0, 1).
		/// </summary>
		public static ColorCMYK blue { get { return new ColorCMYK(1f, 1f, 0f, 0f, 1f); } }

		/// <summary>
		/// Solid magenta.  CMYKA is (0, 1, 0, 0, 1).
		/// </summary>
		public static ColorCMYK magenta { get { return new ColorCMYK(0f, 1f, 0f, 0f, 1f); } }

		#endregion
	}
}
