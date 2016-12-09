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
	/// A color struct for storing and maniputing colors in the HSL (hue, saturation, and lightness) color space.
	/// </summary>
	[Serializable] public struct ColorHSL : IEquatable<ColorHSL>, IComparable<ColorHSL>
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
		/// The color's lightness, in the range [0, 1].
		/// </summary>
		public float l;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given hue, saturation, and lightness, assuming an opacity of 1.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="s">The color's saturation.</param>
		/// <param name="l">The color's lightness.</param>
		public ColorHSL(float h, float s, float l)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given hue, saturation, lightness, and opacity.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="s">The color's saturation.</param>
		/// <param name="l">The color's lightness.</param>
		/// <param name="a">The color's opacity.</param>
		public ColorHSL(float h, float s, float l, float a)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HSL color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSL.</param>
		public ColorHSL(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HSL color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static implicit operator ColorHSL(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HSL color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HSL.</param>
		/// <param name="g">The green component of the RGB color to convert to HSL.</param>
		/// <param name="b">The blue component of the RGB color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the HSL color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HSL.</param>
		/// <param name="g">The green component of the RGB color to convert to HSL.</param>
		/// <param name="b">The blue component of the RGB color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromRGB(float r, float g, float b, float a)
		{
			ColorHSL hsl;
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);

			float c = max - min;

			hsl.h = Detail.HueUtility.FromRGB(r, g, b, max, c);
			hsl.l = (max + min) * 0.5f;
			hsl.s = Detail.LightnessUtility.GetSaturation(c, hsl.l);
			hsl.a = a;

			return hsl;
		}

		/// <summary>
		/// Converts the given HSL color to the RGB color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static implicit operator Color(ColorHSL hsl)
		{
			float c = Detail.LightnessUtility.GetChroma(hsl.s, hsl.l);
			float min = hsl.l - c * 0.5f;
			return (c > 0f) ? Detail.HueUtility.ToRGB(Mathf.Repeat(hsl.h, 1f), c, min, hsl.a) : new Color(min, min, min, hsl.a);
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the HSL color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HSL.</param>
		public ColorHSL(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HSL color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HSL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HSL.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HSL.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the HSL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HSL.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HSL.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromCMY(float c, float m, float y, float a)
		{
			ColorHSL hsl;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			hsl.h = Detail.HueUtility.FromCMY(c, m, y, min, chroma);
			hsl.l = 1f - (max + min) * 0.5f;
			hsl.s = Detail.LightnessUtility.GetSaturation(chroma, hsl.l);
			hsl.a = a;

			return hsl;
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the HSL color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HSL.</param>
		public ColorHSL(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSL color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HSL.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HSL.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HSL.</param>
		/// <param name="k">The key component of the CMYK color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSL color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HSL.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HSL.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HSL.</param>
		/// <param name="k">The key component of the CMYK color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromCMYK(float c, float m, float y, float k, float a)
		{
			if (k >= 1f) return new ColorHSL(0f, 0f, 0f, a);

			float kInv = 1f - k;

			ColorHSL hsl;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			hsl.h = Detail.HueUtility.FromCMY(c, m, y, min, chroma);
			hsl.l = kInv - (max + min) * kInv * 0.5f;
			hsl.s = Detail.LightnessUtility.GetSaturation(chroma * kInv, hsl.l);
			hsl.a = a;

			return hsl;
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the HSL color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HSL.</param>
		public ColorHSL(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HSL color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HSL.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HSL.</param>
		/// <param name="v">The value component of the HSV color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HSL.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HSL.</param>
		/// <param name="v">The value component of the HSV color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHSV(float h, float s, float v, float a)
		{
			float c = Detail.ValueUtility.GetChroma(s, v);
			float l = v - c * 0.5f;
			return new ColorHSL(h, Detail.LightnessUtility.GetSaturation(c, l), l, a);
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the HSL color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSL.</param>
		public ColorHSL(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HSL color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HSL.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HSL.</param>
		/// <param name="v">The value component of the HCV color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HSL.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HSL.</param>
		/// <param name="v">The value component of the HCV color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHCV(float h, float c, float v, float a)
		{
			float l = v - c * 0.5f;
			float s = Detail.LightnessUtility.GetSaturation(c, l);

			return new ColorHSL(h, s, l, a);
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the HSL color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HSL.</param>
		public ColorHSL(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HSL color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HSL.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HSL.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HSL.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HSL.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHCL(float h, float c, float l, float a)
		{
			return new ColorHSL(h, Detail.LightnessUtility.GetSaturation(c, l), l, a);
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the HSL color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HSL.</param>
		public ColorHSL(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HSL color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HSL.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HSL.</param>
		/// <param name="y">The luma component of the HSY color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HSL.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HSL.</param>
		/// <param name="y">The luma component of the HSY color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHSY(float h, float s, float y, float a)
		{
			float c = Detail.LumaUtility.GetChroma(h, s, y);
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, out r, out g, out b);

				float min = y - Detail.LumaUtility.FromRGB(r, g, b);
				float max = c + min;
				float l = (max + min) * 0.5f;

				return new ColorHSL(h, Detail.LightnessUtility.GetSaturation(c, l), l, a);
			}
			else
			{
				return new ColorHSL(h, 0f, y, a);
			}
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the HSL color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HSL.</param>
		public ColorHSL(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HSL color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HSL.</param>
		/// <returns>The color converted to the HSL color space.</returns>
		public static explicit operator ColorHSL(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HSL.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HSL.</param>
		/// <param name="y">The luma component of the HCY color to convert to HSL.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the HSL color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HSL.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HSL.</param>
		/// <param name="y">The luma component of the HCY color to convert to HSL.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSL representation of the given color.</returns>
		public static ColorHSL FromHCY(float h, float c, float y, float a)
		{
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, out r, out g, out b);

				float min = y - Detail.LumaUtility.FromRGB(r, g, b);
				float max = c + min;
				float l = (max + min) * 0.5f;

				return new ColorHSL(h, Detail.LightnessUtility.GetSaturation(c, l), l, a);
			}
			else
			{
				return new ColorHSL(h, 0f, y, a);
			}
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with hue as x, saturation as y, and lightness as z, while opacity is discarded.
		/// </summary>
		/// <param name="hsl">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided HSL color.</returns>
		public static explicit operator Vector3(ColorHSL hsl)
		{
			return new Vector3(hsl.h, hsl.s, hsl.l);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, saturation as y, lightness as z, and opacity as w.
		/// </summary>
		/// <param name="hsl">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HSL color.</returns>
		public static explicit operator Vector4(ColorHSL hsl)
		{
			return new Vector4(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an HSL color, with x as hue, y as saturation, z as lightness, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an HSL color.</param>
		/// <returns>The HSL color converted from the provided vector.</returns>
		public static explicit operator ColorHSL(Vector3 v)
		{
			return new ColorHSL(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HSL color, with x as hue, y as saturation, z as lightness, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HSL color.</param>
		/// <returns>The HSL color converted from the provided vector.</returns>
		public static explicit operator ColorHSL(Vector4 v)
		{
			return new ColorHSL(v.x, v.y, v.z, v.w);
		}

		#endregion

		#region Channel Indexing

		/// <summary>
		/// The number of color channels, including opacity, for colors in this color space.
		/// </summary>
		/// <remarks>For HSL colors, the value is 4, for hue, saturation, lightness, and opacity.</remarks>
		public const int channelCount = 4;

		/// <summary>
		/// Provides access to the four color channels using a numeric zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index for accessing hue (0), saturation (1), lightness (2), or opacity (3).</param>
		/// <returns>The color channel corresponding to the channel index specified.</returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return h;
					case 1: return s;
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
					case 1: s = value; break;
					case 2: l = value; break;
					case 3: a = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		#endregion

		#region Opacity Operations

		/// <summary>
		/// Gets the fully opaque variant of the current color.
		/// </summary>
		/// <returns>Returns a copy of the current color, but with opacity set to 1.</returns>
		public ColorHSL Opaque() { return new ColorHSL(h, s, l, 1f); }

		/// <summary>
		/// Gets a partially translucent variant of the current color.
		/// </summary>
		/// <param name="a">The desired opacity for the returned color.</param>
		/// <returns>Returns a copy of the current color, but with opacity set to the provided value.</returns>
		public ColorHSL Translucent(float a) { return new ColorHSL(h, s, l, a); }

		/// <summary>
		/// Gets the fully transparent variant of the current color.
		/// </summary>
		/// <returns>Returns a copy of the current color, but with opacity set to 0.</returns>
		public ColorHSL Transparent() { return new ColorHSL(h, s, l, 0f); }

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
		/// far in both directions, the interpolation is always in the positive direction.</para>
		/// </remarks>
		/// <seealso cref="LerpUnclamped(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpForward(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpBackward(ColorHSL, ColorHSL, float)"/>
		public static ColorHSL Lerp(ColorHSL a, ColorHSL b, float t)
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
		/// far in both directions, the interpolation is always in the positive direction.</para>
		/// <para>When specifying a <paramref name="t"/> value outside the range [0, 1], the resulting color may no longer be
		/// within the valid range of the color space, even if the two original colors are within the valid range.</para>
		/// </remarks>
		/// <seealso cref="Lerp(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSL, ColorHSL, float)"/>
		public static ColorHSL LerpUnclamped(ColorHSL a, ColorHSL b, float t)
		{
			return new ColorHSL(
				Detail.HueUtility.LerpUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.s, b.s, t),
				Math.LerpUnclamped(a.l, b.l, t),
				Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always increasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpForwardUnclamped(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="Lerp(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpBackward(ColorHSL, ColorHSL, float)"/>
		public static ColorHSL LerpForward(ColorHSL a, ColorHSL b, float t)
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
		/// <seealso cref="LerpForward(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSL, ColorHSL, float)"/>
		public static ColorHSL LerpForwardUnclamped(ColorHSL a, ColorHSL b, float t)
		{
			return new ColorHSL(
				Detail.HueUtility.LerpForwardUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.s, b.s, t),
				Math.LerpUnclamped(a.l, b.l, t),
				Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always decreasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="Lerp(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpForward(ColorHSL, ColorHSL, float)"/>
		public static ColorHSL LerpBackward(ColorHSL a, ColorHSL b, float t)
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
		/// <seealso cref="LerpBackward(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHSL, ColorHSL, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHSL, ColorHSL, float)"/>
		public static ColorHSL LerpBackwardUnclamped(ColorHSL a, ColorHSL b, float t)
		{
			return new ColorHSL(
				Detail.HueUtility.LerpBackwardUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.s, b.s, t),
				Math.LerpUnclamped(a.l, b.l, t),
				Math.LerpUnclamped(a.a, b.a, t));
		}

		#endregion

		#region Arithmetic Operators

		/// <summary>
		/// Adds the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be added.</param>
		/// <param name="b">The second color whose channels are to be added.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color plus the corresponding value of the second channel.</returns>
		public static ColorHSL operator +(ColorHSL a, ColorHSL b)
		{
			return new ColorHSL(Mathf.Repeat(a.h + b.h, 1f), a.s + b.s, a.l + b.l, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorHSL operator -(ColorHSL a, ColorHSL b)
		{
			return new ColorHSL(Mathf.Repeat(a.h - b.h, 1f), a.s - b.s, a.l - b.l, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The value by which to multiply the color's channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHSL operator *(float a, ColorHSL b)
		{
			return new ColorHSL(Mathf.Repeat(b.h * a, 1f), b.s * a, b.l * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHSL operator *(ColorHSL a, float b)
		{
			return new ColorHSL(Mathf.Repeat(a.h * b, 1f), a.s * b, a.l * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorHSL operator *(ColorHSL a, ColorHSL b)
		{
			return new ColorHSL(Mathf.Repeat(a.h * b.h, 1f), a.s * b.s, a.l * b.l, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorHSL operator /(ColorHSL a, float b)
		{
			return new ColorHSL(Mathf.Repeat(a.h / b, 1f), a.s / b, a.l / b, a.a / b);
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
		public bool Equals(ColorHSL other)
		{
			return this == other;
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
			return other is ColorHSL && this == (ColorHSL)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// or if the hue value changes to a value which is equivalent due to the circular nature of hue's range but are nonetheless
		/// distinct values, then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return h.GetHashCode() ^ s.GetHashCode() ^ l.GetHashCode() ^ a.GetHashCode();
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
		public static bool operator ==(ColorHSL lhs, ColorHSL rhs)
		{
			return lhs.h == rhs.h && lhs.s == rhs.s && lhs.l == rhs.l && lhs.a == rhs.a;
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
		public static bool operator !=(ColorHSL lhs, ColorHSL rhs)
		{
			return lhs.h != rhs.h || lhs.s != rhs.s || lhs.l != rhs.l || lhs.a != rhs.a;
		}

		/// <summary>
		/// Determines the ordering of this color with the specified color.
		/// </summary>
		/// <param name="other">The other color to compare against this one.</param>
		/// <returns>Returns -1 if this color is ordered before the other color, +1 if it is ordered after the other color, and 0 if neither is ordered before the other.</returns>
		public int CompareTo(ColorHSL other)
		{
			return Detail.OrderUtility.Compare(h, s, l, a, other.h, other.s, other.l, other.a);
		}

		/// <summary>
		/// Determines the ordering of the first color in relation to the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns -1 if the first color is ordered before the second color, +1 if it is ordered after the second color, and 0 if neither is ordered before the other.</returns>
		public int Compare(ColorHSL lhs, ColorHSL rhs)
		{
			return Detail.OrderUtility.Compare(lhs.h, lhs.s, lhs.l, lhs.a, rhs.h, rhs.s, rhs.l, rhs.a);
		}

		/// <summary>
		/// Checks if the first color is lexicographically ordered before the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is lexicographically ordered before the second color, false otherwise.</returns>
		/// <remarks>No checks are performed to make sure that both colors are canonical.  If this is important, ensure that you are
		/// passing it canonical colors, or use the comparison operators which will do so for you.</remarks>
		public static bool AreOrdered(ColorHSL lhs, ColorHSL rhs)
		{
			return Detail.OrderUtility.AreOrdered(lhs.h, lhs.s, lhs.l, lhs.a, rhs.h, rhs.s, rhs.l, rhs.a);
		}

		/// <summary>
		/// Checks if the first color is lexicographically ordered before the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is lexicographically ordered before the second color, false otherwise.</returns>
		/// <remarks>This operator gets the canonical representation of both colors before performing the lexicographical comparison.
		/// If you already know that the colors are canonical, specifically want to compare non-canonical colors, or wish to avoid
		/// excessive computations, use <see cref="AreOrdered(ColorHSL, ColorHSL)"/> instead.</remarks>
		public static bool operator < (ColorHSL lhs, ColorHSL rhs)
		{
			return AreOrdered(lhs.GetCanonical(), rhs.GetCanonical());
		}

		/// <summary>
		/// Checks if the first color is not lexicographically ordered after the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is not lexicographically ordered after the second color, false otherwise.</returns>
		/// <remarks>This operator gets the canonical representation of both colors before performing the lexicographical comparison.
		/// If you already know that the colors are canonical, specifically want to compare non-canonical colors, or wish to avoid
		/// excessive computations, use <see cref="AreOrdered(ColorHSL, ColorHSL)"/> instead.</remarks>
		public static bool operator <= (ColorHSL lhs, ColorHSL rhs)
		{
			return !AreOrdered(rhs.GetCanonical(), lhs.GetCanonical());
		}

		/// <summary>
		/// Checks if the first color is lexicographically ordered after the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is lexicographically ordered after the second color, false otherwise.</returns>
		/// <remarks>This operator gets the canonical representation of both colors before performing the lexicographical comparison.
		/// If you already know that the colors are canonical, specifically want to compare non-canonical colors, or wish to avoid
		/// excessive computations, use <see cref="AreOrdered(ColorHSL, ColorHSL)"/> instead.</remarks>
		public static bool operator > (ColorHSL lhs, ColorHSL rhs)
		{
			return AreOrdered(rhs.GetCanonical(), lhs.GetCanonical());
		}

		/// <summary>
		/// Checks if the first color is not lexicographically ordered before the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is not lexicographically ordered before the second color, false otherwise.</returns>
		/// <remarks>This operator gets the canonical representation of both colors before performing the lexicographical comparison.
		/// If you already know that the colors are canonical, specifically want to compare non-canonical colors, or wish to avoid
		/// excessive computations, use <see cref="AreOrdered(ColorHSL, ColorHSL)"/> instead.</remarks>
		public static bool operator >= (ColorHSL lhs, ColorHSL rhs)
		{
			return !AreOrdered(lhs.GetCanonical(), rhs.GetCanonical());
		}

		#endregion

		#region Conversion to String

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("HSLA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, l, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("HSLA({0}, {1}, {2}, {3})", h.ToString(format), s.ToString(format), l.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for hue, saturation, and lightness together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & s >= 0f & s <= 1f & l >= 0f & l <= 1f);
		}

		/// <summary>
		/// Gets the nearest HSL color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid HSL color.</returns>
		public ColorHSL GetNearestValid()
		{
			return new ColorHSL(Mathf.Repeat(h, 1f), Mathf.Clamp01(s), Mathf.Clamp01(l), Mathf.Clamp01(a));
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>For an HSL color to be canonical, the hue must be in the range [0, 1).  Also, if the lightness is 0 or 1,
		/// then the saturation must be 0, and if the lightness is 0 or the saturation is 0 or 1, then the hue must be 0.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return (h >= 0f & h < 1f & (h == 0f | (s != 0f & l != 0f & l != 1f)) & (s == 0f | (l != 0f & l != 1f)));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the HSL color space, if lightness is 0 or 1, then hue and saturation are set to 0.  If saturation
		/// is 0, then hue is set to 0.  Otherwise, if hue is outside the range [0, 1), it is wrapped such that it is
		/// restricted to that range.  In all other cases, the color is already canonical.</para>
		/// </remarks>
		public ColorHSL GetCanonical()
		{
			if (s == 0f | l == 0f | l == 1f) return new ColorHSL(0f, 0f, l, a);
			return new ColorHSL(Mathf.Repeat(h, 1f), s, l, a);
		}

		#endregion

		#region Color Constants

		/// <summary>
		/// Completely transparent black.  HSLA is (0, 0, 0, 0).
		/// </summary>
		public static ColorHSL clear { get { return new ColorHSL(0f, 0f, 0f, 0f); } }

		/// <summary>
		/// Solid black.  HSLA is (0, 0, 0, 1).
		/// </summary>
		public static ColorHSL black { get { return new ColorHSL(0f, 0f, 0f, 1f); } }

		/// <summary>
		/// Solid gray.  HSLA is (0, 0, 1/2, 1).
		/// </summary>
		public static ColorHSL gray { get { return new ColorHSL(0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid gray, with English spelling.  HSLA is (0, 0, 1/2, 1).
		/// </summary>
		public static ColorHSL grey { get { return new ColorHSL(0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid white.  HSLA is (0, 0, 1, 1).
		/// </summary>
		public static ColorHSL white { get { return new ColorHSL(0f, 0f, 1f, 1f); } }

		/// <summary>
		/// Solid red.  HSLA is (0, 1, 1/2, 1).
		/// </summary>
		public static ColorHSL red { get { return new ColorHSL(0f, 1f, 0.5f, 1f); } }

		/// <summary>
		/// Solid yellow.  HSLA is (1/6, 1, 1/2, 1).
		/// </summary>
		public static ColorHSL yellow { get { return new ColorHSL(120f / 360f, 1f, 0.5f, 1f); } }

		/// <summary>
		/// Solid green.  HSLA is (1/3, 1, 1/2, 1).
		/// </summary>
		public static ColorHSL green { get { return new ColorHSL(120f / 360f, 1f, 0.5f, 1f); } }

		/// <summary>
		/// Solic cyan.  HSLA is (1/2, 1, 1/2, 1).
		/// </summary>
		public static ColorHSL cyan { get { return new ColorHSL(240f / 360f, 1f, 0.5f, 1f); } }

		/// <summary>
		/// Solid blue.  HSLA is (2/3, 1, 1/2, 1).
		/// </summary>
		public static ColorHSL blue { get { return new ColorHSL(240f / 360f, 1f, 0.5f, 1f); } }

		/// <summary>
		/// Solid magenta.  HSLA is (5/6, 1, 1/2, 1).
		/// </summary>
		public static ColorHSL magenta { get { return new ColorHSL(300f / 360f, 1f, 0.5f, 1f); } }

		#endregion
	}
}
