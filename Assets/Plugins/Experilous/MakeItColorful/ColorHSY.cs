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
	/// A color struct for storing and maniputing colors in the HSY (hue, saturation, and luma) color space.
	/// </summary>
	[Serializable] public struct ColorHSY : IEquatable<ColorHSY>, IComparable<ColorHSY>
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
		/// The color's luma, in the range [0, 1].
		/// </summary>
		public float y;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given hue, saturation, and luma, assuming an opacity of 1.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="s">The color's saturation.</param>
		/// <param name="y">The color's luma.</param>
		public ColorHSY(float h, float s, float y)
		{
			this.h = h;
			this.s = s;
			this.y = y;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given hue, saturation, luma, and opacity.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="s">The color's saturation.</param>
		/// <param name="y">The color's luma.</param>
		/// <param name="a">The color's opacity.</param>
		public ColorHSY(float h, float s, float y, float a)
		{
			this.h = h;
			this.s = s;
			this.y = y;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HSY color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSY.</param>
		public ColorHSY(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HSY color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static implicit operator ColorHSY(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HSY color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HSY.</param>
		/// <param name="g">The green component of the RGB color to convert to HSY.</param>
		/// <param name="b">The blue component of the RGB color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the HSY color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HSY.</param>
		/// <param name="g">The green component of the RGB color to convert to HSY.</param>
		/// <param name="b">The blue component of the RGB color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromRGB(float r, float g, float b, float a)
		{
			ColorHSY hsy;
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);

			float c = max - min;

			hsy.h = Detail.HueUtility.FromRGB(r, g, b, max, c);
			hsy.y = Detail.LumaUtility.FromRGB(r, g, b);
			hsy.s = Detail.LumaUtility.GetSaturation(hsy.h, c, hsy.y);
			hsy.a = a;

			return hsy;
		}

		/// <summary>
		/// Converts the given HSY color to the RGB color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static implicit operator Color(ColorHSY hsy)
		{
			float c = Detail.LumaUtility.GetChroma(hsy.h, hsy.s, hsy.y);
			if (c > 0f)
			{
				Color rgb = Detail.HueUtility.ToRGB(Mathf.Repeat(hsy.h, 1f), c, hsy.a);
				float min = hsy.y - Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
				rgb.r += min;
				rgb.g += min;
				rgb.b += min;
				return rgb;
 			}
			else
			{
				return new Color(hsy.y, hsy.y, hsy.y, hsy.a);
			}
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the HSY color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HSY.</param>
		public ColorHSY(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HSY color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HSY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HSY.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HSY.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the HSY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HSY.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HSY.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromCMY(float c, float m, float y, float a)
		{
			ColorHSY hsy;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			hsy.h = Detail.HueUtility.FromCMY(c, m, y, min, chroma);
			hsy.y = Detail.LumaUtility.FromCMY(c, m, y);
			hsy.s = Detail.LumaUtility.GetSaturation(hsy.h, chroma, hsy.y);
			hsy.a = a;

			return hsy;
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the HSY color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HSY.</param>
		public ColorHSY(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSY color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HSY.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HSY.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HSY.</param>
		/// <param name="k">The key component of the CMYK color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the HSY color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HSY.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HSY.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HSY.</param>
		/// <param name="k">The key component of the CMYK color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromCMYK(float c, float m, float y, float k, float a)
		{
			if (k >= 1f) return new ColorHSY(0f, 0f, 0f, a);

			float kInv = 1f - k;

			ColorHSY hsy;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			hsy.h = Detail.HueUtility.FromCMY(c, m, y, min, chroma);
			hsy.y = Detail.LumaUtility.FromCMY(c, m, y) * kInv;
			hsy.s = Detail.LumaUtility.GetSaturation(hsy.h, chroma * kInv, hsy.y);
			hsy.a = a;

			return hsy;
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the HSY color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HSY.</param>
		public ColorHSY(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HSY color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HSY.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HSY.</param>
		/// <param name="v">The value component of the HSV color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HSY.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HSY.</param>
		/// <param name="v">The value component of the HSV color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHSV(float h, float s, float v, float a)
		{
			float c = Detail.ValueUtility.GetChroma(s, v);
			float min = v - c;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHSY hsy;
				hsy.h = h;
				hsy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hsy.s = Detail.LumaUtility.GetSaturation(h, c, hsy.y);
				hsy.a = a;

				return hsy;
			}
			else
			{
				return new ColorHSY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HCV

		/// <summary>
		/// Initializes a color by converting the given HCV color to the HSY color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSY.</param>
		public ColorHSY(ColorHCV hcv)
		{
			this = FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HSY color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorHCV hcv)
		{
			return FromHCV(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the given HCV color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HSY.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HSY.</param>
		/// <param name="v">The value component of the HCV color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHCV(float h, float c, float v)
		{
			return FromHCV(h, c, v, 1f);
		}

		/// <summary>
		/// Converts the given HCV color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCV color to convert to HSY.</param>
		/// <param name="c">The chroma component of the HCV color to convert to HSY.</param>
		/// <param name="v">The value component of the HCV color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHCV(float h, float c, float v, float a)
		{
			float min = v - c;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHSY hsy;
				hsy.h = h;
				hsy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hsy.s = Detail.LumaUtility.GetSaturation(h, c, hsy.y);
				hsy.a = a;

				return hsy;
			}
			else
			{
				return new ColorHSY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the HSY color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HSY.</param>
		public ColorHSY(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HSY color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HSY.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HSY.</param>
		/// <param name="l">The lightness component of the HSY color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HSY.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HSY.</param>
		/// <param name="l">The lightness component of the HSY color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHSL(float h, float s, float l, float a)
		{
			float c = Detail.LightnessUtility.GetChroma(s, l);
			float min = l - c * 0.5f;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHSY hsy;
				hsy.h = h;
				hsy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hsy.s = Detail.LumaUtility.GetSaturation(h, c, hsy.y);
				hsy.a = a;

				return hsy;
			}
			else
			{
				return new ColorHSY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the HSY color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HSY.</param>
		public ColorHSY(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HSY color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HSY.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HSY.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HSY.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HSY.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHCL(float h, float c, float l, float a)
		{
			float min = l - c * 0.5f;
			if (c > 0f)
			{
				float r, g, b;
				Detail.HueUtility.ToRGB(Mathf.Repeat(h, 1), c, min, out r, out g, out b);

				ColorHSY hsy;
				hsy.h = h;
				hsy.y = Detail.LumaUtility.FromRGB(r, g, b);
				hsy.s = Detail.LumaUtility.GetSaturation(h, c, hsy.y);
				hsy.a = a;

				return hsy;
			}
			else
			{
				return new ColorHSY(h, 0f, min, a);
			}
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the HSY color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HSY.</param>
		public ColorHSY(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HSY color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HSY.</param>
		/// <returns>The color converted to the HSY color space.</returns>
		public static explicit operator ColorHSY(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HSY.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HSY.</param>
		/// <param name="y">The luma component of the HCY color to convert to HSY.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the HSY color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HSY.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HSY.</param>
		/// <param name="y">The luma component of the HCY color to convert to HSY.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HSY representation of the given color.</returns>
		public static ColorHSY FromHCY(float h, float c, float y, float a)
		{
			return new ColorHSY(h, Detail.LumaUtility.GetSaturation(h, c, y), y, a);
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with hue as x, saturation as y, and luma as z, while opacity is discarded.
		/// </summary>
		/// <param name="hsy">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided HSY color.</returns>
		public static explicit operator Vector3(ColorHSY hsy)
		{
			return new Vector3(hsy.h, hsy.s, hsy.y);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, saturation as y, luma as z, and opacity as w.
		/// </summary>
		/// <param name="hsy">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HSY color.</returns>
		public static explicit operator Vector4(ColorHSY hsy)
		{
			return new Vector4(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an HSY color, with x as hue, y as saturation, z as luma, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an HSY color.</param>
		/// <returns>The HSY color converted from the provided vector.</returns>
		public static explicit operator ColorHSY(Vector3 v)
		{
			return new ColorHSY(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HSY color, with x as hue, y as saturation, z as luma, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HSY color.</param>
		/// <returns>The HSY color converted from the provided vector.</returns>
		public static explicit operator ColorHSY(Vector4 v)
		{
			return new ColorHSY(v.x, v.y, v.z, v.w);
		}

		#endregion

		#region Channel Indexing

		/// <summary>
		/// The number of color channels, including opacity, for colors in this color space.
		/// </summary>
		/// <remarks>For HSY colors, the value is 4, for hue, saturation, luma, and opacity.</remarks>
		public const int channelCount = 4;

		/// <summary>
		/// Provides access to the four color channels using a numeric zero-based index.
		/// </summary>
		/// <param name="index">The zero-based index for accessing hue (0), saturation (1), luma (2), or opacity (3).</param>
		/// <returns>The color channel corresponding to the channel index specified.</returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return h;
					case 1: return s;
					case 2: return y;
					case 3: return a;
					default: throw new ArgumentOutOfRangeException("index", index, "The index must be in the range [0, 3].");
				}
			}
			set
			{
				switch (index)
				{
					case 0: h = value; break;
					case 1: s = value; break;
					case 2: y = value; break;
					case 3: a = value; break;
					default: throw new ArgumentOutOfRangeException("index", index, "The index must be in the range [0, 3].");
				}
			}
		}

		#endregion

		#region Opacity Operations

		/// <summary>
		/// Gets the fully opaque variant of the current color.
		/// </summary>
		/// <returns>Returns a copy of the current color, but with opacity set to 1.</returns>
		public ColorHSY Opaque() { return new ColorHSY(h, s, y, 1f); }

		/// <summary>
		/// Gets a partially translucent variant of the current color.
		/// </summary>
		/// <param name="a">The desired opacity for the returned color.</param>
		/// <returns>Returns a copy of the current color, but with opacity set to the provided value.</returns>
		public ColorHSY Translucent(float a) { return new ColorHSY(h, s, y, a); }

		/// <summary>
		/// Gets the fully transparent variant of the current color.
		/// </summary>
		/// <returns>Returns a copy of the current color, but with opacity set to 0.</returns>
		public ColorHSY Transparent() { return new ColorHSY(h, s, y, 0f); }

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
		/// <seealso cref="LerpUnclamped(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpForward(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpBackward(ColorHSY, ColorHSY, float)"/>
		public static ColorHSY Lerp(ColorHSY a, ColorHSY b, float t)
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
		/// <seealso cref="Lerp(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSY, ColorHSY, float)"/>
		public static ColorHSY LerpUnclamped(ColorHSY a, ColorHSY b, float t)
		{
			return new ColorHSY(
				Detail.HueUtility.LerpUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.s, b.s, t),
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
		/// <seealso cref="LerpForwardUnclamped(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="Lerp(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpBackward(ColorHSY, ColorHSY, float)"/>
		public static ColorHSY LerpForward(ColorHSY a, ColorHSY b, float t)
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
		/// <seealso cref="LerpForward(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHSY, ColorHSY, float)"/>
		public static ColorHSY LerpForwardUnclamped(ColorHSY a, ColorHSY b, float t)
		{
			return new ColorHSY(
				Detail.HueUtility.LerpForwardUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.s, b.s, t),
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
		/// <seealso cref="LerpBackwardUnclamped(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="Lerp(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpForward(ColorHSY, ColorHSY, float)"/>
		public static ColorHSY LerpBackward(ColorHSY a, ColorHSY b, float t)
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
		/// <seealso cref="LerpBackward(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHSY, ColorHSY, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHSY, ColorHSY, float)"/>
		public static ColorHSY LerpBackwardUnclamped(ColorHSY a, ColorHSY b, float t)
		{
			return new ColorHSY(
				Detail.HueUtility.LerpBackwardUnclamped(a.h, b.h, t),
				Math.LerpUnclamped(a.s, b.s, t),
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
		public static ColorHSY operator +(ColorHSY a, ColorHSY b)
		{
			return new ColorHSY(Mathf.Repeat(a.h + b.h, 1f), a.s + b.s, a.y + b.y, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorHSY operator -(ColorHSY a, ColorHSY b)
		{
			return new ColorHSY(Mathf.Repeat(a.h - b.h, 1f), a.s - b.s, a.y - b.y, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The value by which to multiply the color's channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHSY operator *(float a, ColorHSY b)
		{
			return new ColorHSY(Mathf.Repeat(b.h * a, 1f), b.s * a, b.y * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHSY operator *(ColorHSY a, float b)
		{
			return new ColorHSY(Mathf.Repeat(a.h * b, 1f), a.s * b, a.y * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorHSY operator *(ColorHSY a, ColorHSY b)
		{
			return new ColorHSY(Mathf.Repeat(a.h * b.h, 1f), a.s * b.s, a.y * b.y, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorHSY operator /(ColorHSY a, float b)
		{
			return new ColorHSY(Mathf.Repeat(a.h / b, 1f), a.s / b, a.y / b, a.a / b);
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
		public bool Equals(ColorHSY other)
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
			return other is ColorHSY && this == (ColorHSY)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// or if the hue value changes to a value which is equivalent due to the circular nature of hue's range but are nonetheless
		/// distinct values, then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return h.GetHashCode() ^ s.GetHashCode() ^ y.GetHashCode() ^ a.GetHashCode();
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
		public static bool operator ==(ColorHSY lhs, ColorHSY rhs)
		{
			return lhs.h == rhs.h && lhs.s == rhs.s && lhs.y == rhs.y && lhs.a == rhs.a;
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
		public static bool operator !=(ColorHSY lhs, ColorHSY rhs)
		{
			return lhs.h != rhs.h || lhs.s != rhs.s || lhs.y != rhs.y || lhs.a != rhs.a;
		}

		/// <summary>
		/// Determines the ordering of this color with the specified color.
		/// </summary>
		/// <param name="other">The other color to compare against this one.</param>
		/// <returns>Returns -1 if this color is ordered before the other color, +1 if it is ordered after the other color, and 0 if neither is ordered before the other.</returns>
		public int CompareTo(ColorHSY other)
		{
			return Detail.OrderUtility.Compare(h, s, y, a, other.h, other.s, other.y, other.a);
		}

		/// <summary>
		/// Determines the ordering of the first color in relation to the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns -1 if the first color is ordered before the second color, +1 if it is ordered after the second color, and 0 if neither is ordered before the other.</returns>
		public int Compare(ColorHSY lhs, ColorHSY rhs)
		{
			return Detail.OrderUtility.Compare(lhs.h, lhs.s, lhs.y, lhs.a, rhs.h, rhs.s, rhs.y, rhs.a);
		}

		/// <summary>
		/// Checks if the first color is lexicographically ordered before the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is lexicographically ordered before the second color, false otherwise.</returns>
		/// <remarks>No checks are performed to make sure that both colors are canonical.  If this is important, ensure that you are
		/// passing it canonical colors, or use the comparison operators which will do so for you.</remarks>
		public static bool AreOrdered(ColorHSY lhs, ColorHSY rhs)
		{
			return Detail.OrderUtility.AreOrdered(lhs.h, lhs.s, lhs.y, lhs.a, rhs.h, rhs.s, rhs.y, rhs.a);
		}

		/// <summary>
		/// Checks if the first color is lexicographically ordered before the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is lexicographically ordered before the second color, false otherwise.</returns>
		/// <remarks>This operator gets the canonical representation of both colors before performing the lexicographical comparison.
		/// If you already know that the colors are canonical, specifically want to compare non-canonical colors, or wish to avoid
		/// excessive computations, use <see cref="AreOrdered(ColorHSY, ColorHSY)"/> instead.</remarks>
		public static bool operator < (ColorHSY lhs, ColorHSY rhs)
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
		/// excessive computations, use <see cref="AreOrdered(ColorHSY, ColorHSY)"/> instead.</remarks>
		public static bool operator <= (ColorHSY lhs, ColorHSY rhs)
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
		/// excessive computations, use <see cref="AreOrdered(ColorHSY, ColorHSY)"/> instead.</remarks>
		public static bool operator > (ColorHSY lhs, ColorHSY rhs)
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
		/// excessive computations, use <see cref="AreOrdered(ColorHSY, ColorHSY)"/> instead.</remarks>
		public static bool operator >= (ColorHSY lhs, ColorHSY rhs)
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
			return string.Format("HSYA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, y, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("HSYA({0}, {1}, {2}, {3})", h.ToString(format), s.ToString(format), y.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for hue, saturation, and luma together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & s >= 0f & s <= 1f & y >= 0f & y <= 1f);
		}

		/// <summary>
		/// Gets the nearest HSY color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid HSY color.</returns>
		public ColorHSY GetNearestValid()
		{
			return new ColorHSY(Mathf.Repeat(h, 1f), Mathf.Clamp01(s), Mathf.Clamp01(y), Mathf.Clamp01(a));
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>For an HSY color to be canonical, the hue must be in the range [0, 1).  Also, if the luma is 0 or 1, then
		/// the saturation must be 0, and if the luma is 0 or the saturation is 0 or 1, then the hue must be 0.</para>
		/// </remarks>
		public bool IsCanonical()
		{
			return (h >= 0f & h < 1f & (h == 0f | (s != 0f & y != 0f & y != 1f)) & (s == 0f | (y != 0f & y != 1f)));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the HSY color space, if luma is 0 or 1, then hue and saturation are set to 0.  If saturation is
		/// 0, then hue is set to 0.  Otherwise, if hue is outside the range [0, 1), it is wrapped such that it is
		/// restricted to that range.  In all other cases, the color is already canonical.</para>
		/// </remarks>
		public ColorHSY GetCanonical()
		{
			if (s == 0f | y == 0f | y == 1f) return new ColorHSY(0f, 0f, y, a);
			return new ColorHSY(Mathf.Repeat(h, 1f), s, y, a);
		}

		#endregion

		#region Color Constants

		/// <summary>
		/// Completely transparent black.  HSYA is (0, 0, 0, 0).
		/// </summary>
		public static ColorHSY clear { get { return new ColorHSY(0f, 0f, 0f, 0f); } }

		/// <summary>
		/// Solid black.  HSYA is (0, 0, 0, 1).
		/// </summary>
		public static ColorHSY black { get { return new ColorHSY(0f, 0f, 0f, 1f); } }

		/// <summary>
		/// Solid gray.  HSYA is (0, 0, 1/2, 1).
		/// </summary>
		public static ColorHSY gray { get { return new ColorHSY(0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid gray, with English spelling.  HSYA is (0, 0, 1/2, 1).
		/// </summary>
		public static ColorHSY grey { get { return new ColorHSY(0f, 0f, 0.5f, 1f); } }

		/// <summary>
		/// Solid white.  HSYA is (0, 0, 1, 1).
		/// </summary>
		public static ColorHSY white { get { return new ColorHSY(0f, 0f, 1f, 1f); } }

		/// <summary>
		/// Solid red.  HSYA is (0, 1, 0.30, 1).
		/// </summary>
		public static ColorHSY red { get { return new ColorHSY(0f, 1f, Detail.LumaUtility.rWeight, 1f); } }

		/// <summary>
		/// Solid yellow.  HSYA is (1/6, 1, 0.89, 1).
		/// </summary>
		public static ColorHSY yellow { get { return new ColorHSY(120f / 360f, 1f, Detail.LumaUtility.rWeight + Detail.LumaUtility.gWeight, 1f); } }

		/// <summary>
		/// Solid green.  HSYA is (1/3, 1, 0.59, 1).
		/// </summary>
		public static ColorHSY green { get { return new ColorHSY(120f / 360f, 1f, Detail.LumaUtility.gWeight, 1f); } }

		/// <summary>
		/// Solic cyan.  HSYA is (1/2, 1, 0.70, 1).
		/// </summary>
		public static ColorHSY cyan { get { return new ColorHSY(240f / 360f, 1f, Detail.LumaUtility.gWeight + Detail.LumaUtility.bWeight, 1f); } }

		/// <summary>
		/// Solid blue.  HSYA is (2/3, 1, 0.11, 1).
		/// </summary>
		public static ColorHSY blue { get { return new ColorHSY(240f / 360f, 1f, Detail.LumaUtility.bWeight, 1f); } }

		/// <summary>
		/// Solid magenta.  HSYA is (5/6, 1, 0.41, 1).
		/// </summary>
		public static ColorHSY magenta { get { return new ColorHSY(300f / 360f, 1f, Detail.LumaUtility.bWeight + Detail.LumaUtility.rWeight, 1f); } }

		#endregion
	}
}
