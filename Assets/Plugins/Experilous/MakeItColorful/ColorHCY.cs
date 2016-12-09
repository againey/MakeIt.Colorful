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
	/// A color struct for storing and maniputing colors in the HCY (hue, chroma, and luma) color space.
	/// </summary>
	[Serializable] public struct ColorHCY
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
		/// The color's luma, in the range [0, 1].
		/// </summary>
		public float y;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given hue, chroma, and luma, assuming an opacity of 1.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="c">The color's chroma.</param>
		/// <param name="y">The color's value.</param>
		public ColorHCY(float h, float c, float y)
		{
			this.h = h;
			this.c = c;
			this.y = y;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given hue, chroma, luma, and opacity.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="c">The color's chroma.</param>
		/// <param name="y">The color's luma.</param>
		/// <param name="a">The color's opacity.</param>
		public ColorHCY(float h, float c, float y, float a)
		{
			this.h = h;
			this.c = c;
			this.y = y;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HCY color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HCY.</param>
		public ColorHCY(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HCY color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static implicit operator ColorHCY(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HCY color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HCY.</param>
		/// <param name="g">The green component of the RGB color to convert to HCY.</param>
		/// <param name="b">The blue component of the RGB color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the HCY color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HCY.</param>
		/// <param name="g">The green component of the RGB color to convert to HCY.</param>
		/// <param name="b">The blue component of the RGB color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromRGB(float r, float g, float b, float a)
		{
			ColorHCY hcy;
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);

			hcy.c = max - min;
			hcy.h = Detail.HueUtility.FromRGB(r, g, b, max, hcy.c);
			hcy.y = Detail.LumaUtility.FromRGB(r, g, b);
			hcy.a = a;

			return hcy;
		}

		/// <summary>
		/// Converts the given HCY color to the RGB color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static implicit operator Color(ColorHCY hcy)
		{
			if (hcy.c > 0f)
			{
				Color rgb = Detail.HueUtility.ToRGB(Mathf.Repeat(hcy.h, 1f), hcy.c, hcy.a);
				float min = hcy.y - Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
				rgb.r += min;
				rgb.g += min;
				rgb.b += min;
				return rgb;
 			}
			else
			{
				return new Color(hcy.y, hcy.y, hcy.y, hcy.a);
			}
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the HCY color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HCY.</param>
		public ColorHCY(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HCY color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HCY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HCY.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HCY.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the HCY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HCY.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HCY.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromCMY(float c, float m, float y, float a)
		{
			ColorHCY hcy;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			hcy.c = max - min;
			hcy.h = Detail.HueUtility.FromCMY(c, m, y, min, hcy.c);
			hcy.y = Detail.LumaUtility.FromCMY(c, m, y);
			hcy.a = a;

			return hcy;
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the HCY color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HCY.</param>
		public ColorHCY(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCY color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HCY.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HCY.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HCY.</param>
		/// <param name="k">The key component of the CMYK color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HCY.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HCY.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HCY.</param>
		/// <param name="k">The key component of the CMYK color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromCMYK(float c, float m, float y, float k, float a)
		{
			if (k >= 1f) return new ColorHCY(0f, 0f, 0f, a);

			float kInv = 1f - k;

			ColorHCY hcy;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			hcy.c = max - min;
			hcy.h = Detail.HueUtility.FromCMY(c, m, y, min, hcy.c);
			hcy.c *= kInv;
			hcy.y = Detail.LumaUtility.FromCMY(c, m, y) * kInv;
			hcy.a = a;

			return hcy;
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the HCY color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCY.</param>
		public ColorHCY(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HCY color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HCY.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HCY.</param>
		/// <param name="v">The value component of the HSV color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HCY.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HCY.</param>
		/// <param name="v">The value component of the HSV color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHSV(float h, float s, float v, float a)
		{
			float c = Detail.ValueUtility.GetChroma(s, v);
			float min = v - c;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHCY hcy;
				hcy.h = h;
				hcy.c = c;
				hcy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hcy.a = a;

				return hcy;
			}
			else
			{
				return new ColorHCY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the HCY color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HCY.</param>
		public ColorHCY(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HCY color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HCY.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HCY.</param>
		/// <param name="v">The value component of the HCV color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HCY.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HCY.</param>
		/// <param name="v">The value component of the HCV color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHCV(float h, float c, float v, float a)
		{
			float min = v - c;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHCY hcy;
				hcy.h = h;
				hcy.c = c;
				hcy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hcy.a = a;

				return hcy;
			}
			else
			{
				return new ColorHCY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the HCY color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HCY.</param>
		public ColorHCY(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HCY color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HCY.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HCY.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HCY.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HCY.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHSL(float h, float s, float l, float a)
		{
			float c = Detail.LightnessUtility.GetChroma(s, l);
			float min = l - c * 0.5f;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHCY hcy;
				hcy.h = h;
				hcy.c = c;
				hcy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hcy.a = a;

				return hcy;
			}
			else
			{
				return new ColorHCY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the HCY color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HCY.</param>
		public ColorHCY(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HCY color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HCY.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HCY.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HCY.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HCY.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHCL(float h, float c, float l, float a)
		{
			float min = l - c * 0.5f;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHCY hcy;
				hcy.h = h;
				hcy.c = c;
				hcy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hcy.a = a;

				return hcy;
			}
			else
			{
				return new ColorHCY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the HCY color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HCY.</param>
		public ColorHCY(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HCY color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HCY.</param>
		/// <returns>The color converted to the HCY color space.</returns>
		public static explicit operator ColorHCY(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HCY.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HCY.</param>
		/// <param name="y">The luma component of the HSY color to convert to HCY.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the HCY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HCY.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HCY.</param>
		/// <param name="y">The luma component of the HSY color to convert to HCY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCY representation of the given color.</returns>
		public static ColorHCY FromHSY(float h, float s, float y, float a)
		{
			return new ColorHCY(h, Detail.LumaUtility.GetChroma(h, s, y), y, a);
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with hue as x, chroma as y, and lightness as z, while opacity is discarded.
		/// </summary>
		/// <param name="hcy">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided HCY color.</returns>
		public static explicit operator Vector3(ColorHCY hcy)
		{
			return new Vector3(hcy.h, hcy.c, hcy.y);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, chroma as y, lightness as z, and opacity as w.
		/// </summary>
		/// <param name="hcy">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HCY color.</returns>
		public static explicit operator Vector4(ColorHCY hcy)
		{
			return new Vector4(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an HCY color, with x as hue, y as chroma, z as lightness, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an HCY color.</param>
		/// <returns>The HCY color converted from the provided vector.</returns>
		public static explicit operator ColorHCY(Vector3 v)
		{
			return new ColorHCY(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HCY color, with x as hue, y as chroma, z as lightness, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HCY color.</param>
		/// <returns>The HCY color converted from the provided vector.</returns>
		public static explicit operator ColorHCY(Vector4 v)
		{
			return new ColorHCY(v.x, v.y, v.z, v.w);
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
					case 2: return y;
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
		/// <remarks>
		/// <para>Because hue is a circular range, interpolation between two hue values is a little bit different than
		/// ordinary non-circular values.  Instead of doing a straight mathematical linear interpolation, the distance
		/// from the first hue to the second is checked in both the forward and backward directions, and the interpolation
		/// is performed in whichever direction is shortest.  If the two hues are exact polar opposites and thus equally
		/// far in both directions, the interpolation is always in the positive direction.</para>
		/// </remarks>
		/// <seealso cref="LerpUnclamped(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpForward(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpBackward(ColorHCY, ColorHCY, float)"/>
		public static ColorHCY Lerp(ColorHCY a, ColorHCY b, float t)
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
		/// <seealso cref="Lerp(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCY, ColorHCY, float)"/>
		public static ColorHCY LerpUnclamped(ColorHCY a, ColorHCY b, float t)
		{
			return new ColorHCY(
				Detail.HueUtility.LerpUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.c, b.c, t),
				Math.LerpUnclamped(a.y, b.y, t),
				Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always increasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpForwardUnclamped(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="Lerp(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpBackward(ColorHCY, ColorHCY, float)"/>
		public static ColorHCY LerpForward(ColorHCY a, ColorHCY b, float t)
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
		/// <seealso cref="LerpForward(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCY, ColorHCY, float)"/>
		public static ColorHCY LerpForwardUnclamped(ColorHCY a, ColorHCY b, float t)
		{
			return new ColorHCY(
				Detail.HueUtility.LerpForwardUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.c, b.c, t),
				Math.LerpUnclamped(a.y, b.y, t),
				Math.LerpUnclamped(a.a, b.a, t));
		}

		/// <summary>
		/// Performs a linear interpolation between the two colors specified for each color channel independently, with hue always decreasing and wrapping if necessary.
		/// </summary>
		/// <param name="a">The first color to be interpolated between, corresponding to a <paramref name="t"/> value of 0.</param>
		/// <param name="b">The second color to be interpolated between, corresponding to a <paramref name="t"/> value of 1.</param>
		/// <param name="t">The parameter specifying how much each color is weighted by the interpolation.  Will be clamped to the range [0, 1].</param>
		/// <returns>A new color that is the result of the linear interpolation between the two original colors.</returns>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="Lerp(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpForward(ColorHCY, ColorHCY, float)"/>
		public static ColorHCY LerpBackward(ColorHCY a, ColorHCY b, float t)
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
		/// <seealso cref="LerpBackward(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHCY, ColorHCY, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHCY, ColorHCY, float)"/>
		public static ColorHCY LerpBackwardUnclamped(ColorHCY a, ColorHCY b, float t)
		{
			return new ColorHCY(
				Detail.HueUtility.LerpBackwardUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.c, b.c, t),
				Math.LerpUnclamped(a.y, b.y, t),
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
		public static ColorHCY operator +(ColorHCY a, ColorHCY b)
		{
			return new ColorHCY(Mathf.Repeat(a.h + b.h, 1f), a.c + b.c, a.y + b.y, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorHCY operator -(ColorHCY a, ColorHCY b)
		{
			return new ColorHCY(Mathf.Repeat(a.h - b.h, 1f), a.c - b.c, a.y - b.y, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The value by which to multiply the color's channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHCY operator *(float a, ColorHCY b)
		{
			return new ColorHCY(Mathf.Repeat(b.h * a, 1f), b.c * a, b.y * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHCY operator *(ColorHCY a, float b)
		{
			return new ColorHCY(Mathf.Repeat(a.h * b, 1f), a.c * b, a.y * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorHCY operator *(ColorHCY a, ColorHCY b)
		{
			return new ColorHCY(Mathf.Repeat(a.h * b.h, 1f), a.c * b.c, a.y * b.y, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorHCY operator /(ColorHCY a, float b)
		{
			return new ColorHCY(Mathf.Repeat(a.h / b, 1f), a.c / b, a.y / b, a.a / b);
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
			return other is ColorHCY && this == (ColorHCY)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// or if the hue value changes to a value which is equivalent due to the circular nature of hue's range but are nonetheless
		/// distinct values, then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return h.GetHashCode() ^ c.GetHashCode() ^ y.GetHashCode() ^ a.GetHashCode();
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
		public static bool operator ==(ColorHCY lhs, ColorHCY rhs)
		{
			return lhs.h == rhs.h && lhs.c == rhs.c && lhs.y == rhs.y && lhs.a == rhs.a;
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
		public static bool operator !=(ColorHCY lhs, ColorHCY rhs)
		{
			return lhs.h != rhs.h || lhs.c != rhs.c || lhs.y != rhs.y || lhs.a != rhs.a;
		}

		#endregion

		#region Conversion to String

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("HCYA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, c, y, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("HCYA({0}, {1}, {2}, {3})", h.ToString(format), c.ToString(format), y.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for hue, chroma, and lightness together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		/// <remarks>To be valid within the RGB color space, lightness must be within the range [chroma, 1], and chroma must be in the range [0, lightness].</remarks>
		/// <seealso cref="GetMaxChroma(float, float)"/>
		/// <seealso cref="GetMinMaxLuma(float, float, out float, out float)"/>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & y >= 0f & y <= 1f & c >= 0f) && (c <= GetMaxChroma(Mathf.Repeat(h, 1f), y));
		}

		/// <summary>
		/// Gets the nearest HCY color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid HCY color.</returns>
		public ColorHCY GetNearestValid()
		{
			float h = Mathf.Repeat(this.h, 1f);
			float yMid = GetLumaAtMaxChroma(h);
			float maxChroma = (this.y <= yMid) ? this.y / yMid : (1f - this.y) / (1f - yMid);
			if (this.c <= maxChroma)
			{
				return new ColorHCY(h, Mathf.Max(0f, this.c), Mathf.Clamp01(this.y), Mathf.Clamp01(a));
			}
			else
			{
				float dx = this.c - 1f;
				float dy = this.y - yMid;

				if (dx < -dy * yMid  || dx < dy - dy * yMid)
				{
					float c = this.c;
					float y = this.y;

					if (dy < dx * yMid)
					{
						c = (c + y * yMid) / (1f + yMid * yMid);
						y = c * yMid;
					}
					else if (dy > dx * yMid - dx)
					{
						float n = (dx + dy * yMid - dy) / (yMid * yMid - 2f * yMid + 2f);
						c = n + 1f;
						y = n * yMid - n + yMid;
					}
					return new ColorHCY(h, Mathf.Max(0f, c), Mathf.Clamp01(y), Mathf.Clamp01(a));
				}
				else
				{
					return new ColorHCY(h, 1f, yMid, Mathf.Clamp01(a));
				}
			}
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>For an HCY color to be canonical, the hue must be in the range [0, 1).  Also, if the luma is 0 or 1, then
		/// the chroma must be 0, and if the luma is 0 or the chroma is 0 or 1, then the hue must be 0.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return (h >= 0f & h < 1f & (h == 0f | (c != 0f & y != 0f & y != 1f)) & (c == 0f | (y != 0f & y != 1f)));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the HCY color space, if luma is 0 or 1, then hue and chroma are set to 0.  If chroma is 0, then
		/// hue is set to 0.  Otherwise, if hue is outside the range [0, 1), it is wrapped such that it is restricted
		/// to that range.  In all other cases, the color is already canonical.</para>
		/// </remarks>
		public ColorHCY GetCanonical()
		{
			if (c == 0f | y == 0f | y == 1f) return new ColorHCY(0f, 0f, y, a);
			return new ColorHCY(Mathf.Repeat(h, 1f), c, y, a);
		}

		/// <summary>
		/// Indicates the value that the luma channel must have when the chroma channel is at its maximum value, if the color is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="h">The hue value for which the luma channel range is to be returned.</param>
		/// <returns>The luma channel at maximum chroma.</returns>
		public static float GetLumaAtMaxChroma(float h)
		{
			return Detail.LumaUtility.GetLumaAtMaxChroma(h);
		}

		/// <summary>
		/// Indicates the range of values that the luma channel can have for a given hue and chroma values, if the color is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="h">The hue value for which the luma channel range is to be returned.</param>
		/// <param name="c">The chroma value for which the luma channel range is to be returned.</param>
		/// <param name="yMin">The minimum value of the luma channel for the given hue and chroma.</param>
		/// <param name="yMax">The maximum value of the luma channel for the given hue and chroma.</param>
		public static void GetMinMaxLuma(float h, float c, out float yMin, out float yMax)
		{
			Detail.LumaUtility.GetMinMaxLuma(h, c, out yMin, out yMax);
		}

		/// <summary>
		/// Indicates the maximum value that the chroma channel can have for given values of the hue and luma channels, if it is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="h">The value of the hue channel for which the maximum chroma is to be returned.</param>
		/// <param name="y">The value of the luma channel for which the maximum chroma is to be returned.</param>
		/// <returns>The maximum chroma for the given hue and luma.</returns>
		public static float GetMaxChroma(float h, float y)
		{
			return Detail.LumaUtility.GetMaxChroma(h, y);
		}

		#endregion

		#region Color Constants

		/// <summary>
		/// Completely transparent black.  HCYA is (0, 0, 0, 0).
		/// </summary>
		public static ColorHCY clear { get { return new ColorHCY(0f, 0f, 0f, 0f); } }

		/// <summary>
		/// Solid black.  HCYA is (0, 0, 0, 1).
		/// </summary>
		public static ColorHCY black { get { return new ColorHCY(0f, 0f, 0f, 1f); } }

		/// <summary>
		/// Solid gray.  HCYA is (0, 0, 1/2, 1).
		/// </summary>
		public static ColorHCY gray { get { return new ColorHCY(0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid gray, with English spelling.  HCYA is (0, 0, 1/2, 1).
		/// </summary>
		public static ColorHCY grey { get { return new ColorHCY(0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid white.  HCYA is (0, 0, 1, 1).
		/// </summary>
		public static ColorHCY white { get { return new ColorHCY(0f, 0f, 1f, 1f); } }

		/// <summary>
		/// Solid red.  HCYA is (0, 1, 0.30, 1).
		/// </summary>
		public static ColorHCY red { get { return new ColorHCY(0f, 1f, Detail.LumaUtility.rWeight, 1f); } }

		/// <summary>
		/// Solid yellow.  HCYA is (1/6, 1, 0.89, 1).
		/// </summary>
		public static ColorHCY yellow { get { return new ColorHCY(120f / 360f, 1f, Detail.LumaUtility.rWeight + Detail.LumaUtility.gWeight, 1f); } }

		/// <summary>
		/// Solid green.  HCYA is (1/3, 1, 0.59, 1).
		/// </summary>
		public static ColorHCY green { get { return new ColorHCY(120f / 360f, 1f, Detail.LumaUtility.gWeight, 1f); } }

		/// <summary>
		/// Solic cyan.  HCYA is (1/2, 1, 0.70, 1).
		/// </summary>
		public static ColorHCY cyan { get { return new ColorHCY(240f / 360f, 1f, Detail.LumaUtility.gWeight + Detail.LumaUtility.bWeight, 1f); } }

		/// <summary>
		/// Solid blue.  HCYA is (2/3, 1, 0.11, 1).
		/// </summary>
		public static ColorHCY blue { get { return new ColorHCY(240f / 360f, 1f, Detail.LumaUtility.bWeight, 1f); } }

		/// <summary>
		/// Solid magenta.  HCYA is (5/6, 1, 0.41, 1).
		/// </summary>
		public static ColorHCY magenta { get { return new ColorHCY(300f / 360f, 1f, Detail.LumaUtility.bWeight + Detail.LumaUtility.rWeight, 1f); } }

		#endregion
	}
}
