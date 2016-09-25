/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// A color struct for storing and maniputing colors in the CMY (cyan, magenta, and yellow) color space.
	/// </summary>
	[Serializable] public struct ColorCMY
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
		public ColorCMY(float c, float m, float y)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given cyan, magenta, yellow, and opacity.
		/// </summary>
		/// <param name="c">The color's cyan channel value.</param>
		/// <param name="m">The color's magenta channel value.</param>
		/// <param name="y">The color's yellow channel value.</param>
		/// <param name="a">The color's opacity channel value.</param>
		public ColorCMY(float c, float m, float y, float a)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the CMY color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to CMY.</param>
		public ColorCMY(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the CMY color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the CMY color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to CMY.</param>
		/// <param name="g">The green component of the RGB color to convert to CMY.</param>
		/// <param name="b">The blue component of the RGB color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the CMY color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to CMY.</param>
		/// <param name="g">The green component of the RGB color to convert to CMY.</param>
		/// <param name="b">The blue component of the RGB color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromRGB(float r, float g, float b, float a)
		{
			return new ColorCMY(1f - r, 1f - g, 1f - b, a);
		}

		/// <summary>
		/// Converts the given CMY color to the RGB color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static explicit operator Color(ColorCMY cmy)
		{
			return new Color(1f - cmy.c, 1f - cmy.m, 1f - cmy.y, cmy.a);
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the CMY color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to CMY.</param>
		public ColorCMY(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the CMY color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the CMY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to CMY.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to CMY.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to CMY.</param>
		/// <param name="k">The key component of the CMYK color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the CMY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to CMY.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to CMY.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to CMY.</param>
		/// <param name="k">The key component of the CMYK color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromCMYK(float c, float m, float y, float k, float a)
		{
			float kInv = 1f - k;
			return new ColorCMY(c * kInv + k, m * kInv + k, y * kInv + k, a);
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the CMY color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to CMY.</param>
		public ColorCMY(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the CMY color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to CMY.</param>
		/// <param name="s">The saturation component of the HSV color to convert to CMY.</param>
		/// <param name="v">The value component of the HSV color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to CMY.</param>
		/// <param name="s">The saturation component of the HSV color to convert to CMY.</param>
		/// <param name="v">The value component of the HSV color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHSV(float h, float s, float v, float a)
		{
			float c = Detail.ValueUtility.GetChroma(s, v);
			float min = v - c;
			return (c > 0f) ? (ColorCMY)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMY(1f - min, 1f - min, 1f - min, a);
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the CMY color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to CMY.</param>
		public ColorCMY(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the CMY color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to CMY.</param>
		/// <param name="c">The chroma component of the HCV color to convert to CMY.</param>
		/// <param name="v">The value component of the HCV color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to CMY.</param>
		/// <param name="c">The chroma component of the HCV color to convert to CMY.</param>
		/// <param name="v">The value component of the HCV color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHCV(float h, float c, float v, float a)
		{
			float min = v - c;
			return (c > 0f) ? (ColorCMY)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMY(1f - min, 1f - min, 1f - min, a);
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the CMY color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to CMY.</param>
		public ColorCMY(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the CMY color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the CMY color to convert to CMY.</param>
		/// <param name="s">The saturation component of the CMY color to convert to CMY.</param>
		/// <param name="l">The lightness component of the CMY color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the CMY color to convert to CMY.</param>
		/// <param name="s">The saturation component of the CMY color to convert to CMY.</param>
		/// <param name="l">The lightness component of the CMY color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHSL(float h, float s, float l, float a)
		{
			float c = Detail.LightnessUtility.GetChroma(s, l);
			float min = l - c * 0.5f;
			return (c > 0f) ? (ColorCMY)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMY(1f - min, 1f - min, 1f - min, a);
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the CMY color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to CMY.</param>
		public ColorCMY(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the CMY color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to CMY.</param>
		/// <param name="c">The chroma component of the HCL color to convert to CMY.</param>
		/// <param name="l">The lightness component of the HCL color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to CMY.</param>
		/// <param name="c">The chroma component of the HCL color to convert to CMY.</param>
		/// <param name="l">The lightness component of the HCL color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHCL(float h, float c, float l, float a)
		{
			float min = l - c * 0.5f;
			return (c > 0f) ? (ColorCMY)Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, a) : new ColorCMY(1f - min, 1f - min, 1f - min, a);
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the CMY color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to CMY.</param>
		public ColorCMY(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the CMY color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to CMY.</param>
		/// <param name="s">The saturation component of the HSY color to convert to CMY.</param>
		/// <param name="y">The luma component of the HSY color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to CMY.</param>
		/// <param name="s">The saturation component of the HSY color to convert to CMY.</param>
		/// <param name="y">The luma component of the HSY color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHSY(float h, float s, float y, float a)
		{
			float c = Detail.LumaUtility.GetChroma(h, s, y);
			if (c > 0f)
			{
				Color rgb = Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, a);
				float min = y - Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
				rgb.r += min;
				rgb.g += min;
				rgb.b += min;
				return (ColorCMY)rgb;
 			}
			else
			{
				return new ColorCMY(1f - y, 1f - y, 1f - y, a);
			}
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the CMY color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to CMY.</param>
		public ColorCMY(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the CMY color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to CMY.</param>
		/// <returns>The color converted to the CMY color space.</returns>
		public static explicit operator ColorCMY(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to CMY.</param>
		/// <param name="c">The chroma component of the HCY color to convert to CMY.</param>
		/// <param name="y">The luma component of the HCY color to convert to CMY.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the CMY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to CMY.</param>
		/// <param name="c">The chroma component of the HCY color to convert to CMY.</param>
		/// <param name="y">The luma component of the HCY color to convert to CMY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The CMY representation of the given color.</returns>
		public static ColorCMY FromHCY(float h, float c, float y, float a)
		{
			if (c > 0f)
			{
				Color rgb = Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, a);
				float min = y - Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
				rgb.r += min;
				rgb.g += min;
				rgb.b += min;
				return (ColorCMY)rgb;
 			}
			else
			{
				return new ColorCMY(1f - y, 1f - y, 1f - y, a);
			}
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with cyan as x, magenta as y, and yellow as z, while opacity is discarded.
		/// </summary>
		/// <param name="cmy">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided CMY color.</returns>
		public static explicit operator Vector3(ColorCMY cmy)
		{
			return new Vector3(cmy.c, cmy.m, cmy.y);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with cyan as x, magenta as y, yellow as z, and opacity as w.
		/// </summary>
		/// <param name="cmy">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided CMY color.</returns>
		public static explicit operator Vector4(ColorCMY cmy)
		{
			return new Vector4(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an CMY color, with x as cyan, y as magenta, z as yellow, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an CMY color.</param>
		/// <returns>The CMY color converted from the provided vector.</returns>
		public static explicit operator ColorCMY(Vector3 v)
		{
			return new ColorCMY(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an CMY color, with x as cyan, y as magenta, z as yellow, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an CMY color.</param>
		/// <returns>The CMY color converted from the provided vector.</returns>
		public static explicit operator ColorCMY(Vector4 v)
		{
			return new ColorCMY(v.x, v.y, v.z, v.w);
		}

		#endregion

		#region Channel Indexing

		/// <summary>
		/// The number of color channels, including opacity, for colors in this color space.
		/// </summary>
		/// <remarks>For CMY colors, the value is 4, for cyan, magenta, yellow, and opacity.</remarks>
		public const int channelCount = 4;

		/// <summary>
		/// Provides access to the four color channels using a numeric zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index for accessing cyan (0), magenta (1), yellow (2), or opacity (3).</param>
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
					case 3: return a;
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
		/// <seealso cref="LerpUnclamped(ColorCMY, ColorCMY, float)"/>
		public static ColorCMY Lerp(ColorCMY a, ColorCMY b, float t)
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
		/// <seealso cref="Lerp(ColorCMY, ColorCMY, float)"/>
		public static ColorCMY LerpUnclamped(ColorCMY a, ColorCMY b, float t)
		{
			return new ColorCMY(
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.m, b.m, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		#endregion

		#region Arithmetic Operators

		/// <summary>
		/// Adds the color channels of the two specified colors together.
		/// </summary>
		/// <param name="a">The first color whose channels are to be added.</param>
		/// <param name="b">The second color whose channels are to be added.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color plus the corresponding value of the second channel.</returns>
		public static ColorCMY operator +(ColorCMY a, ColorCMY b)
		{
			return new ColorCMY(a.c + b.c, a.m + b.m, a.y + b.y, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorCMY operator -(ColorCMY a, ColorCMY b)
		{
			return new ColorCMY(a.c - b.c, a.m - b.m, a.y - b.y, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value.
		/// </summary>
		/// <param name="a">The value by which to multiply the color'm channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorCMY operator *(float a, ColorCMY b)
		{
			return new ColorCMY(b.c * a, b.m * a, b.y * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color'm channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorCMY operator *(ColorCMY a, float b)
		{
			return new ColorCMY(a.c * b, a.m * b, a.y * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorCMY operator *(ColorCMY a, ColorCMY b)
		{
			return new ColorCMY(a.c * b.c, a.m * b.m, a.y * b.y, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color'm channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorCMY operator /(ColorCMY a, float b)
		{
			return new ColorCMY(a.c / b, a.m / b, a.y / b, a.a / b);
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
			return other is ColorCMY && this == (ColorCMY)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return c.GetHashCode() ^ m.GetHashCode() ^ y.GetHashCode() ^ a.GetHashCode();
		}

		/// <summary>
		/// Checks if the two colors are equal to each other.
		/// </summary>
		/// <param name="lhs">The first color compare for equality.</param>
		/// <param name="rhs">The second color compare for equality.</param>
		/// <returns>Returns true if both colors are equal, false otherwise.</returns>
		/// <remarks>This function checks for perfect bitwise equality.  If any of the channels differ by even the smallest amount,
		/// then this function will return false.</remarks>
		public static bool operator ==(ColorCMY lhs, ColorCMY rhs)
		{
			return lhs.c == rhs.c && lhs.m == rhs.m && lhs.y == rhs.y && lhs.a == rhs.a;
		}

		/// <summary>
		/// Checks if the two colors are not equal to each other.
		/// </summary>
		/// <param name="lhs">The first color compare for inequality.</param>
		/// <param name="rhs">The second color compare for inequality.</param>
		/// <returns>Returns true if the two colors are not equal, false if they are equal.</returns>
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels differ by even the smallest amount,
		/// then this function will return true.</remarks>
		public static bool operator !=(ColorCMY lhs, ColorCMY rhs)
		{
			return lhs.c != rhs.c || lhs.m != rhs.m || lhs.y != rhs.y || lhs.a != rhs.a;
		}

		#endregion

		#region Conversion to String

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("CMYA({0:F3}, {1:F3}, {2:F3}, {3:F3})", c, m, y, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("CMYA({0}, {1}, {2}, {3})", c.ToString(format), m.ToString(format), y.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for cyan, magenta, and yellow together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & c >= 0f & c <= 1f & m >= 0f & m <= 1f & y >= 0f & y <= 1f);
		}

		/// <summary>
		/// Gets the nearest CMY color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid CMY color.</returns>
		public ColorCMY GetNearestValid()
		{
			return new ColorCMY(Mathf.Clamp01(c), Mathf.Clamp01(m), Mathf.Clamp01(y), Mathf.Clamp01(a));
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>A CMY color is always canonical, because there is never more than one representation of any color.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return true;
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the CMY color space, all possible representations are unique and therefore already canonical.</para>
		/// </remarks>
		public ColorCMY GetCanonical()
		{
			return this;
		}

		#endregion
	}
}
