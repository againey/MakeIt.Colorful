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
		#region Fields and Direct Constructors

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

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSV.</param>
		public ColorHSV(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HSV.</param>
		/// <param name="g">The green component of the RGB color to convert to HSV.</param>
		/// <param name="b">The blue component of the RGB color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the HSV color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HSV.</param>
		/// <param name="g">The green component of the RGB color to convert to HSV.</param>
		/// <param name="b">The blue component of the RGB color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromRGB(float r, float g, float b, float a)
		{
			ColorHSV hsv;
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);

			float c = max - min;

			hsv.h = Detail.HueUtility.FromRGB(r, g, b, max, c);
			hsv.s = Detail.ValueUtility.GetSaturation(c, max);
			hsv.v = max;
			hsv.a = a;

			return hsv;
		}

		/// <summary>
		/// Converts the given HSV color to the RGB color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static explicit operator Color(ColorHSV hsv)
		{
			float c = Detail.ValueUtility.GetChroma(hsv.s, hsv.v);
			float min = hsv.v - c;
			return (c > 0f) ? Detail.HueUtility.ToRGB(hsv.h, c, min, hsv.a) : new Color(min, min, min, hsv.a);
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the HSV color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HSV.</param>
		public ColorHSV(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HSV color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HSV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HSV.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HSV.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the HSV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HSV.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HSV.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromCMY(float c, float m, float y, float a)
		{
			ColorHSV hsv;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			hsv.h = Detail.HueUtility.FromCMY(c, m, y, min, chroma);
			hsv.v = 1f - min;
			hsv.s = Detail.ValueUtility.GetSaturation(chroma, hsv.v);
			hsv.a = a;

			return hsv;
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the HSV color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HSV.</param>
		public ColorHSV(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSV color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HSV.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HSV.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HSV.</param>
		/// <param name="k">The key component of the CMYK color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HSV.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HSV.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HSV.</param>
		/// <param name="k">The key component of the CMYK color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromCMYK(float c, float m, float y, float k, float a)
		{
			if (k >= 1f) return new ColorHSV(0f, 0f, 0f, a);

			float kInv = 1f - k;

			ColorHSV hsv;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			hsv.h = Detail.HueUtility.FromCMY(c, m, y, min, chroma);
			hsv.v = (1f - min) * kInv;
			hsv.s = Detail.ValueUtility.GetSaturation(chroma * kInv, hsv.v);
			hsv.a = a;

			return hsv;
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the HSV color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSV.</param>
		public ColorHSV(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HSV color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HSV.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HSV.</param>
		/// <param name="v">The value component of the HCV color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HSV.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HSV.</param>
		/// <param name="v">The value component of the HCV color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCV(float h, float c, float v, float a)
		{
			return new ColorHSV(h, Detail.ValueUtility.GetSaturation(c, v), v, a);
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the HSV color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HSV.</param>
		public ColorHSV(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HSV color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HSV.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HSV.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HSV.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HSV.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHSL(float h, float s, float l, float a)
		{
			float c = Detail.LightnessUtility.GetChroma(s, l);
			float v = l + c * 0.5f;
			return new ColorHSV(h, Detail.ValueUtility.GetSaturation(c, v), v, a);
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the HSV color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HSV.</param>
		public ColorHSV(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HSV color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HSV.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HSV.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HSV.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HSV.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCL(float h, float c, float l, float a)
		{
			float v = l + c * 0.5f;

			return new ColorHSV(h, Detail.ValueUtility.GetSaturation(c, v), v, a);
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the HSV color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HSV.</param>
		public ColorHSV(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HSV color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HSV.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HSV.</param>
		/// <param name="y">The luma component of the HSY color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HSV.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HSV.</param>
		/// <param name="y">The luma component of the HSY color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHSY(float h, float s, float y, float a)
		{
			float c = Detail.LumaUtility.GetChroma(h, s, y);
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(h, c, out r, out g, out b);

				float min = y - Detail.LumaUtility.FromRGB(r, g, b);
				float max = c + min;

				return new ColorHSV(h, Detail.ValueUtility.GetSaturation(c, max), max, a);
			}
			else
			{
				return new ColorHSV(h, 0f, y, a);
			}
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the HSV color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HSV.</param>
		public ColorHSV(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HSV color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HSV.</param>
		/// <returns>The color converted to the HSV color space.</returns>
		public static explicit operator ColorHSV(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HSV.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HSV.</param>
		/// <param name="y">The luma component of the HCY color to convert to HSV.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the HSV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HSV.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HSV.</param>
		/// <param name="y">The luma component of the HCY color to convert to HSV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSV representation of the given color.</returns>
		public static ColorHSV FromHCY(float h, float c, float y, float a)
		{
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(h, c, out r, out g, out b);

				float min = y - Detail.LumaUtility.FromRGB(r, g, b);
				float max = c + min;

				return new ColorHSV(h, Detail.ValueUtility.GetSaturation(c, max), max, a);
			}
			else
			{
				return new ColorHSV(h, 0f, y, a);
			}
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with hue as x, saturation as y, and value as z, while opacity is discarded.
		/// </summary>
		/// <param name="hsv">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided HSV color.</returns>
		public static explicit operator Vector3(ColorHSV hsv)
		{
			return new Vector3(hsv.h, hsv.s, hsv.v);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, saturation as y, value as z, and opacity as w.
		/// </summary>
		/// <param name="hsv">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HSV color.</returns>
		public static explicit operator Vector4(ColorHSV hsv)
		{
			return new Vector4(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an HSV color, with x as hue, y as saturation, z as value, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an HSV color.</param>
		/// <returns>The HSV color converted from the provided vector.</returns>
		public static explicit operator ColorHSV(Vector3 v)
		{
			return new ColorHSV(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HSV color, with x as hue, y as saturation, z as value, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HSV color.</param>
		/// <returns>The HSV color converted from the provided vector.</returns>
		public static explicit operator ColorHSV(Vector4 v)
		{
			return new ColorHSV(v.x, v.y, v.z, v.w);
		}

		#endregion

		#region Channel Indexing

		/// <summary>
		/// The number of color channels, including opacity, for colors in this color space.
		/// </summary>
		/// <remarks>For HSV colors, the value is 4, for hue, saturation, value, and opacity.</remarks>
		public const int channelCount = 4;

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
			return new ColorHSV(
				Detail.HueUtility.LerpUnclamped(a.h, b.h, t),
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
			return new ColorHSV(
				Detail.HueUtility.LerpForwardUnclamped(a.h, b.h, t),
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
			return new ColorHSV(
				Detail.HueUtility.LerpBackwardUnclamped(a.h, b.h, t),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
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
		public static ColorHSV operator *(float a, ColorHSV b)
		{
			return new ColorHSV(Mathf.Repeat(b.h * a, 1f), b.s * a, b.v * a, b.a * a);
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

		#endregion

		#region Conversion to String

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

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for hue, saturation, and value together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & s >= 0f & s <= 1f & v >= 0f & v <= 1f);
		}

		/// <summary>
		/// Gets the nearest HSV color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid HSV color.</returns>
		public ColorHSV GetNearestValid()
		{
			return new ColorHSV(Mathf.Repeat(h, 1f), Mathf.Clamp01(s), Mathf.Clamp01(v), Mathf.Clamp01(a));
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>For an HSV color to be canonical, the hue must be in the range [0, 1).  Also, if the value is 0, then
		/// the saturation must also be 0, and if either the value or saturation are 0, then the hue must be 0.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return (h >= 0f & h < 1f & (h == 0f | (s != 0f & v != 0f)) & (s == 0f | v != 0f));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the HSV color space, if value is 0, then hue and saturation are set to 0.  If saturationis 0, then
		/// hue is set to 0.  Otherwise, if hue is outside the range [0, 1), it is wrapped such that it is restricted to
		/// that range.  In all other cases, the color is already canonical.</para>
		/// </remarks>
		public ColorHSV GetCanonical()
		{
			if (s == 0f | v == 0f) return new ColorHSV(0f, 0f, v, a);
			return new ColorHSV(Mathf.Repeat(h, 1f), s, v, a);
		}

		#endregion
	}
}
