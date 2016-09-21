/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// A color struct for storing and maniputing colors in the HCV (hue, chroma, and value) color space.
	/// </summary>
	[Serializable] public struct ColorHCV
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
		/// The color's value, in the range [0, 1].
		/// </summary>
		public float v;

		/// <summary>
		/// The color's alpha, or opacity, in the range [0, 1].
		/// </summary>
		/// <remarks>A value of 0 means the color is entirely transparent and invisible, while a value of 1 is completely opaque.</remarks>
		public float a;

		/// <summary>
		/// Initializes a color with the given hue, chroma, and value, assuming an opacity of 1.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="c">The color's chroma.</param>
		/// <param name="v">The color's value.</param>
		public ColorHCV(float h, float c, float v)
		{
			this.h = h;
			this.c = c;
			this.v = v;
			a = 1f;
		}

		/// <summary>
		/// Initializes a color with the given hue, chroma, value, and opacity.
		/// </summary>
		/// <param name="h">The color's hue.</param>
		/// <param name="c">The color's chroma.</param>
		/// <param name="v">The color's value.</param>
		/// <param name="a">The color's opacity.</param>
		public ColorHCV(float h, float c, float v, float a)
		{
			this.h = h;
			this.c = c;
			this.v = v;
			this.a = a;
		}

		#endregion

		#region Conversion to/from RGB

		/// <summary>
		/// Initializes a color by converting the given RGB color to the HCV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HCV.</param>
		public ColorHCV(Color rgb)
		{
			this = FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HCV color space.
		/// </summary>
		/// <param name="rgb">The RGB color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(Color rgb)
		{
			return FromRGB(rgb.r, rgb.g, rgb.b, rgb.a);
		}

		/// <summary>
		/// Converts the given RGB color to the HCV color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HCV.</param>
		/// <param name="g">The green component of the RGB color to convert to HCV.</param>
		/// <param name="b">The blue component of the RGB color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromRGB(float r, float g, float b)
		{
			return FromRGB(r, g, b, 1f);
		}

		/// <summary>
		/// Converts the given RGB color to the HCV color space.
		/// </summary>
		/// <param name="r">The red component of the RGB color to convert to HCV.</param>
		/// <param name="g">The green component of the RGB color to convert to HCV.</param>
		/// <param name="b">The blue component of the RGB color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromRGB(float r, float g, float b, float a)
		{
			ColorHCV hcv;
			float min = Mathf.Min(Mathf.Min(r, g), b);
			float max = Mathf.Max(Mathf.Max(r, g), b);

			hcv.c = max - min;

			if (hcv.c > 0f)
			{
				if (r == max)
				{
					hcv.h = Mathf.Repeat((g - b) / hcv.c, 6f) / 6f;
				}
				else if (g == max)
				{
					hcv.h = ((b - r) / hcv.c + 2f) / 6f;
				}
				else
				{
					hcv.h = ((r - g) / hcv.c + 4f) / 6f;
				}
			}
			else
			{
				hcv.h = 0f;
			}

			hcv.v = max;
			hcv.a = a;

			return hcv;
		}

		/// <summary>
		/// Converts the given HCV color to the RGB color space.
		/// </summary>
		/// <param name="hcv">The HCV color to convert to RGB.</param>
		/// <returns>The color converted to the RGB color space.</returns>
		public static explicit operator Color(ColorHCV hcv)
		{
			float min = hcv.v - hcv.c;
			Color rgb = new Color(min, min, min, hcv.a);
			if (hcv.c > 0f)
			{
				float scaledHue = hcv.h * 6f;
				if (scaledHue < 1f)
				{
					rgb.r += hcv.c;
					rgb.g += hcv.c * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					rgb.g += hcv.c;
					rgb.r += hcv.c * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					rgb.g += hcv.c;
					rgb.b += hcv.c * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					rgb.b += hcv.c;
					rgb.g += hcv.c * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					rgb.b += hcv.c;
					rgb.r += hcv.c * (scaledHue - 4f);
				}
				else
				{
					rgb.r += hcv.c;
					rgb.b += hcv.c * (6f - scaledHue);
				}
			}
			rgb.r = Mathf.Clamp01(rgb.r);
			rgb.g = Mathf.Clamp01(rgb.g);
			rgb.b = Mathf.Clamp01(rgb.b);
			return rgb;
		}

		#endregion

		#region Conversion from CMY

		/// <summary>
		/// Initializes a color by converting the given CMY color to the HCV color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HCV.</param>
		public ColorHCV(ColorCMY cmy)
		{
			this = FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HCV color space.
		/// </summary>
		/// <param name="cmy">The CMY color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorCMY cmy)
		{
			return FromCMY(cmy.c, cmy.m, cmy.y, cmy.a);
		}

		/// <summary>
		/// Converts the given CMY color to the HCV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HCV.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HCV.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromCMY(float c, float m, float y)
		{
			return FromCMY(c, m, y, 1f);
		}

		/// <summary>
		/// Converts the given CMY color to the HCV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMY color to convert to HCV.</param>
		/// <param name="m">The magenta component of the CMY color to convert to HCV.</param>
		/// <param name="y">The yellow component of the CMY color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromCMY(float c, float m, float y, float a)
		{
			ColorHCV hcv;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			hcv.c = max - min;

			if (hcv.c > 0f)
			{
				if (c == min)
				{
					hcv.h = Mathf.Repeat((y - m) / hcv.c, 6f) / 6f;
				}
				else if (m == min)
				{
					hcv.h = ((c - y) / hcv.c + 2f) / 6f;
				}
				else
				{
					hcv.h = ((m - c) / hcv.c + 4f) / 6f;
				}
			}
			else
			{
				hcv.h = 0f;
			}

			hcv.v = 1f - min;
			hcv.a = a;

			return hcv;
		}

		#endregion

		#region Conversion from CMYK

		/// <summary>
		/// Initializes a color by converting the given CMYK color to the HCV color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HCV.</param>
		public ColorHCV(ColorCMYK cmyk)
		{
			this = FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCV color space.
		/// </summary>
		/// <param name="cmyk">The CMYK color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorCMYK cmyk)
		{
			return FromCMYK(cmyk.c, cmyk.m, cmyk.y, cmyk.k, cmyk.a);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HCV.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HCV.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HCV.</param>
		/// <param name="k">The key component of the CMYK color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromCMYK(float c, float m, float y, float k)
		{
			return FromCMYK(c, m, y, k, 1f);
		}

		/// <summary>
		/// Converts the given CMYK color to the HCV color space.
		/// </summary>
		/// <param name="c">The cyan component of the CMYK color to convert to HCV.</param>
		/// <param name="m">The magenta component of the CMYK color to convert to HCV.</param>
		/// <param name="y">The yellow component of the CMYK color to convert to HCV.</param>
		/// <param name="k">The key component of the CMYK color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromCMYK(float c, float m, float y, float k, float a)
		{
			if (k >= 1f) return new ColorHCV(0f, 0f, 0f, a);

			float kInv = 1f - k;

			ColorHCV hcv;
			float min = Mathf.Min(Mathf.Min(c, m), y);
			float max = Mathf.Max(Mathf.Max(c, m), y);

			float chroma = max - min;

			if (chroma > 0f)
			{
				if (c == min)
				{
					hcv.h = Mathf.Repeat((y - m) / chroma, 6f) / 6f;
				}
				else if (m == min)
				{
					hcv.h = ((c - y) / chroma + 2f) / 6f;
				}
				else
				{
					hcv.h = ((m - c) / chroma + 4f) / 6f;
				}

				hcv.c = chroma * kInv;
			}
			else
			{
				hcv.h = 0f;
				hcv.c = 0f;
			}

			hcv.v = (1f - min) * kInv;
			hcv.a = a;

			return hcv;
		}

		#endregion

		#region Conversion from HSV

		/// <summary>
		/// Initializes a color by converting the given HSV color to the HCV color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCV.</param>
		public ColorHCV(ColorHSV hsv)
		{
			this = FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HCV color space.
		/// </summary>
		/// <param name="hsv">The HSV color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorHSV hsv)
		{
			return FromHSV(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		/// <summary>
		/// Converts the given HSV color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HCV.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HCV.</param>
		/// <param name="v">The value component of the HSV color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHSV(float h, float s, float v)
		{
			return FromHSV(h, s, v, 1f);
		}

		/// <summary>
		/// Converts the given HSV color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSV color to convert to HCV.</param>
		/// <param name="s">The saturation component of the HSV color to convert to HCV.</param>
		/// <param name="v">The value component of the HSV color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHSV(float h, float s, float v, float a)
		{
			return new ColorHCV(h, s * v, v, a);
		}

		#endregion

		#region Conversion from HSL

		/// <summary>
		/// Initializes a color by converting the given HSL color to the HCV color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HCV.</param>
		public ColorHCV(ColorHSL hsl)
		{
			this = FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HCV color space.
		/// </summary>
		/// <param name="hsl">The HSL color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorHSL hsl)
		{
			return FromHSL(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		/// <summary>
		/// Converts the given HSL color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HCV.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HCV.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHSL(float h, float s, float l)
		{
			return FromHSL(h, s, l, 1f);
		}

		/// <summary>
		/// Converts the given HSL color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSL color to convert to HCV.</param>
		/// <param name="s">The saturation component of the HSL color to convert to HCV.</param>
		/// <param name="l">The lightness component of the HSL color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHSL(float h, float s, float l, float a)
		{
			float c = (1f - Mathf.Abs(2f * l - 1f)) * s;
			float min = l - c * 0.5f;
			float max = c + min;

			return new ColorHCV(h, c, max, a);
		}

		#endregion

		#region Conversion from HCL

		/// <summary>
		/// Initializes a color by converting the given HCL color to the HCV color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HCV.</param>
		public ColorHCV(ColorHCL hcl)
		{
			this = FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HCV color space.
		/// </summary>
		/// <param name="hcl">The HCL color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorHCL hcl)
		{
			return FromHCL(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		/// <summary>
		/// Converts the given HCL color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HCV.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HCV.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHCL(float h, float c, float l)
		{
			return FromHCL(h, c, l, 1f);
		}

		/// <summary>
		/// Converts the given HCL color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCL color to convert to HCV.</param>
		/// <param name="c">The chroma component of the HCL color to convert to HCV.</param>
		/// <param name="l">The lightness component of the HCL color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHCL(float h, float c, float l, float a)
		{
			float min = l - c * 0.5f;
			float max = c + min;

			return new ColorHCV(h, c, max, a);
		}

		#endregion

		#region Conversion from HSY

		/// <summary>
		/// Initializes a color by converting the given HSY color to the HCV color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HCV.</param>
		public ColorHCV(ColorHSY hsy)
		{
			this = FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HCV color space.
		/// </summary>
		/// <param name="hsy">The HSY color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorHSY hsy)
		{
			return FromHSY(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		/// <summary>
		/// Converts the given HSY color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HCV.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HCV.</param>
		/// <param name="y">The luma component of the HSY color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		/// <summary>
		/// Converts the given HSY color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HSY color to convert to HCV.</param>
		/// <param name="s">The saturation component of the HSY color to convert to HCV.</param>
		/// <param name="y">The luma component of the HSY color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHSY(float h, float s, float y, float a)
		{
			float r = 0f;
			float g = 0f;
			float b = 0f;
			float c = s * ColorHCY.GetMaxChroma(h, y);
			if (c > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					r = c;
					g = c * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					g = c;
					r = c * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					g = c;
					b = c * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					b = c;
					g = c * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					b = c;
					r = c * (scaledHue - 4f);
				}
				else
				{
					r = c;
					b = c * (6f - scaledHue);
				}
			}

			float min = y - (r * ColorHSY.redLumaFactor + g * ColorHSY.greenLumaFactor + b * ColorHSY.blueLumaFactor);
			float max = c + min;

			return new ColorHCV(h, c, max, a);
		}

		#endregion

		#region Conversion from HCY

		/// <summary>
		/// Initializes a color by converting the given HCY color to the HCV color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HCV.</param>
		public ColorHCV(ColorHCY hcy)
		{
			this = FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HCV color space.
		/// </summary>
		/// <param name="hcy">The HCY color to convert to HCV.</param>
		/// <returns>The color converted to the HCV color space.</returns>
		public static explicit operator ColorHCV(ColorHCY hcy)
		{
			return FromHCY(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		/// <summary>
		/// Converts the given HCY color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HCV.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HCV.</param>
		/// <param name="y">The luma component of the HCY color to convert to HCV.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHCY(float h, float c, float y)
		{
			return FromHCY(h, c, y, 1f);
		}

		/// <summary>
		/// Converts the given HCY color to the HCV color space.
		/// </summary>
		/// <param name="h">The hue component of the HCY color to convert to HCV.</param>
		/// <param name="c">The chroma component of the HCY color to convert to HCV.</param>
		/// <param name="y">The luma component of the HCY color to convert to HCV.</param>
		/// <param name="a">The opacity of the color.</param>
		/// <returns>The HCV representation of the given color.</returns>
		public static ColorHCV FromHCY(float h, float c, float y, float a)
		{
			float r = 0f;
			float g = 0f;
			float b = 0f;
			if (c > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					r = c;
					g = c * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					g = c;
					r = c * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					g = c;
					b = c * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					b = c;
					g = c * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					b = c;
					r = c * (scaledHue - 4f);
				}
				else
				{
					r = c;
					b = c * (6f - scaledHue);
				}
			}

			float min = y - (r * ColorHCY.redLumaFactor + g * ColorHCY.greenLumaFactor + b * ColorHCY.blueLumaFactor);
			float max = c + min;

			return new ColorHCV(h, c, max, a);
		}

		#endregion

		#region Conversion to/from Vector

		/// <summary>
		/// Converts the specified color to a <see cref="Vector3"/>, with hue as x, chroma as y, and value as z, while opacity is discarded.
		/// </summary>
		/// <param name="hcv">The color to convert to a <see cref="Vector3"/>.</param>
		/// <returns>The vector converted from the provided HCV color.</returns>
		public static explicit operator Vector3(ColorHCV hcv)
		{
			return new Vector3(hcv.h, hcv.c, hcv.v);
		}

		/// <summary>
		/// Converts the specified color to a <see cref="Vector4"/>, with hue as x, chroma as y, value as z, and opacity as w.
		/// </summary>
		/// <param name="hcv">The color to convert to a <see cref="Vector4"/>.</param>
		/// <returns>The vector converted from the provided HCV color.</returns>
		public static explicit operator Vector4(ColorHCV hcv)
		{
			return new Vector4(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector3"/> color to an HCV color, with x as hue, y as chroma, z as value, assuming an opacity of 1.
		/// </summary>
		/// <param name="v">The <see cref="Vector3"/> to convert to an HCV color.</param>
		/// <returns>The HCV color converted from the provided vector.</returns>
		public static explicit operator ColorHCV(Vector3 v)
		{
			return new ColorHCV(v.x, v.y, v.z, 1f);
		}

		/// <summary>
		/// Converts the specified <see cref="Vector4"/> color to an HCV color, with x as hue, y as chroma, z as value, and w as opacity.
		/// </summary>
		/// <param name="v">The <see cref="Vector4"/> to convert to an HCV color.</param>
		/// <returns>The HCV color converted from the provided vector.</returns>
		public static explicit operator ColorHCV(Vector4 v)
		{
			return new ColorHCV(v.x, v.y, v.z, v.w);
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
		/// <param name="index">The zero-based index for accessing hue (0), chroma (1), value (2), or opacity (3).</param>
		/// <returns>The color channel corresponding to the channel index specified.</returns>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return h;
					case 1: return c;
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
					case 1: c = value; break;
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
		/// <seealso cref="LerpUnclamped(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpForward(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpBackward(ColorHCV, ColorHCV, float)"/>
		public static ColorHCV Lerp(ColorHCV a, ColorHCV b, float t)
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
		/// <seealso cref="Lerp(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCV, ColorHCV, float)"/>
		public static ColorHCV LerpUnclamped(ColorHCV a, ColorHCV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHCV(
				hueDelta <= 0.5f
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t)
							: Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
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
		/// <seealso cref="LerpForwardUnclamped(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="Lerp(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpBackward(ColorHCV, ColorHCV, float)"/>
		public static ColorHCV LerpForward(ColorHCV a, ColorHCV b, float t)
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
		/// <seealso cref="LerpForward(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpBackwardUnclamped(ColorHCV, ColorHCV, float)"/>
		public static ColorHCV LerpForwardUnclamped(ColorHCV a, ColorHCV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHCV(
				hueA <= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
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
		/// <seealso cref="LerpBackwardUnclamped(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="Lerp(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpForward(ColorHCV, ColorHCV, float)"/>
		public static ColorHCV LerpBackward(ColorHCV a, ColorHCV b, float t)
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
		/// <seealso cref="LerpBackward(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpUnclamped(ColorHCV, ColorHCV, float)"/>
		/// <seealso cref="LerpForwardUnclamped(ColorHCV, ColorHCV, float)"/>
		public static ColorHCV LerpBackwardUnclamped(ColorHCV a, ColorHCV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHCV(
				hueA >= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
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
		public static ColorHCV operator +(ColorHCV a, ColorHCV b)
		{
			return new ColorHCV(Mathf.Repeat(a.h + b.h, 1f), a.c + b.c, a.v + b.v, a.a + b.a);
		}

		/// <summary>
		/// Subtracts the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be subtracted.</param>
		/// <param name="b">The second color whose channels are to be subtracted.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color minus the corresponding value of the second channel.</returns>
		public static ColorHCV operator -(ColorHCV a, ColorHCV b)
		{
			return new ColorHCV(Mathf.Repeat(a.h - b.h, 1f), a.c - b.c, a.v - b.v, a.a - b.a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The value by which to multiply the color's channels.</param>
		/// <param name="b">The color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHCV operator *(float a, ColorHCV b)
		{
			return new ColorHCV(Mathf.Repeat(b.h * a, 1f), b.c * a, b.v * a, b.a * a);
		}

		/// <summary>
		/// Multiplies the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be multiplied.</param>
		/// <param name="b">The value by which to multiply the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color multiplied by the provided value.</returns>
		public static ColorHCV operator *(ColorHCV a, float b)
		{
			return new ColorHCV(Mathf.Repeat(a.h * b, 1f), a.c * b, a.v * b, a.a * b);
		}

		/// <summary>
		/// Multiplies the color channels of the two specified colors together, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The first color whose channels are to be multiplied.</param>
		/// <param name="b">The second color whose channels are to be multiplied.</param>
		/// <returns>A new color with each channel set to the corresponding value of the first color multiplied by the corresponding value of the second channel.</returns>
		public static ColorHCV operator *(ColorHCV a, ColorHCV b)
		{
			return new ColorHCV(Mathf.Repeat(a.h * b.h, 1f), a.c * b.c, a.v * b.v, a.a * b.a);
		}

		/// <summary>
		/// Divides the color channels of the specified color by the specified value, wrapping the hue channel if necessary.
		/// </summary>
		/// <param name="a">The color whose channels are to be divided.</param>
		/// <param name="b">The value by which to divide the color's channels.</param>
		/// <returns>A new color with each channel set to the corresponding value of the provided color divided by the provided value.</returns>
		public static ColorHCV operator /(ColorHCV a, float b)
		{
			return new ColorHCV(Mathf.Repeat(a.h / b, 1f), a.c / b, a.v / b, a.a / b);
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
			return other is ColorHCV && this == (ColorHCV)other;
		}

		/// <inheritdoc />
		/// <remarks>This function is based on exact bitwise representation.  If any of the channels change by even the smallest amount,
		/// or if the hue value changes to a value which is equivalent due to the circular nature of hue's range but are nonetheless
		/// distinct values, then this function will likely return a different hash code than before the change.</remarks>
		public override int GetHashCode()
		{
			return h.GetHashCode() ^ c.GetHashCode() ^ v.GetHashCode() ^ a.GetHashCode();
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
		public static bool operator ==(ColorHCV lhs, ColorHCV rhs)
		{
			return lhs.h == rhs.h && lhs.c == rhs.c && lhs.v == rhs.v && lhs.a == rhs.a;
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
		public static bool operator !=(ColorHCV lhs, ColorHCV rhs)
		{
			return lhs.h != rhs.h || lhs.c != rhs.c || lhs.v != rhs.v || lhs.a != rhs.a;
		}

		#endregion

		#region Conversion to String

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <returns>A string representation of the color using default formatting.</returns>
		public override string ToString()
		{
			return string.Format("HCVA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, c, v, a);
		}

		/// <summary>
		/// Converts the color to string representation, appropriate for diagnositic display.
		/// </summary>
		/// <param name="format">The numeric format string to be used for each channel.  Accepts the same values that can be passed to <see cref="System.Single.ToString(string)"/>.</param>
		/// <returns>A string representation of the color using the specified formatting.</returns>
		public string ToString(string format)
		{
			return string.Format("HCVA({0}, {1}, {2}, {3})", h.ToString(format), c.ToString(format), v.ToString(format), a.ToString(format));
		}

		#endregion

		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for hue, chroma, and value together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		/// <remarks>To be valid within the RGB color space, value must be within the range [chroma, 1], and chroma must be in the range [0, value].</remarks>
		/// <seealso cref="GetMaxChroma(float)"/>
		/// <seealso cref="GetMinMaxValue(float, out float, out float)"/>
		public bool IsValid()
		{
			return (a >= 0f & a <= 1f & v >= 0f & v <= 1f & c >= 0f) && (c <= v);
		}

		/// <summary>
		/// Gets the nearest HCV color that is also valid within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid HCV color.</returns>
		public ColorHCV GetNearestValid()
		{
			float c = this.c;
			float v = this.v;
			if (c > v) c = v = (c + v) * 0.5f;
			return new ColorHCV(Mathf.Repeat(h, 1f), Mathf.Clamp01(c), Mathf.Clamp01(v), Mathf.Clamp01(a));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the HCV color space, if value is 0, then hue and chroma are set to 0.  If chroma is 0, then hue
		/// is set to 0.  Otherwise, if hue is outside the range [0, 1), it is wrapped such that it is restricted to
		/// that range.  In all other cases, the color is already canonical.</para>
		/// </remarks>
		public ColorHCV GetCanonical()
		{
			if (c == 0f | v == 0f) return new ColorHCV(0f, 0f, v, a);
			return new ColorHCV(Mathf.Repeat(h, 1f), c, v, a);
		}

		/// <summary>
		/// Indicates the value that the value channel must have when the chroma channel is at its maximum value, if the color is to remain valid within the RGB color space.
		/// </summary>
		/// <returns>The value channel at maximum chroma.</returns>
		/// <remarks>For the HCV color space, the value channel is always 1 when chroma is at its maximum value of 1.</remarks>
		public static float GetValueAtMaxChroma()
		{
			return 1f;
		}

		/// <summary>
		/// Indicates the range of values that the value channel can have for a given chroma value, if the color is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="c">The chroma value for which the value channel range is to be returned.</param>
		/// <param name="vMin">The minimum value of the value channel for the given chroma.</param>
		/// <param name="vMax">The maximum value of the value channel for the given chroma.</param>
		/// <remarks>For the HCV color space, the value channel must be in the range [chroma, 1].</remarks>
		public static void GetMinMaxValue(float c, out float vMin, out float vMax)
		{
			vMin = c;
			vMax = 1f;
		}

		/// <summary>
		/// Indicates the maximum value that the chroma channel can have for a given value of the value channel, if it is to remain valid within the RGB color space.
		/// </summary>
		/// <param name="v">The value of the value channel for which the maximum chroma is to be returned.</param>
		/// <returns>The maximum chroma for the given value.</returns>
		/// <remarks>For the HCV color space, the chroma channel must be in the range [0, value].</remarks>
		public static float GetMaxChroma(float v)
		{
			return v;
		}

		#endregion
	}
}
