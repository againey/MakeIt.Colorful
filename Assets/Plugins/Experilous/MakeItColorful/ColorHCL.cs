/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	[Serializable]
	public struct ColorHCL
	{
		public float h;
		public float c;
		public float l;
		public float a;

		public ColorHCL(float h, float c, float l)
		{
			this.h = h;
			this.c = c;
			this.l = l;
			a = 1f;
		}

		public ColorHCL(float h, float c, float l, float a)
		{
			this.h = h;
			this.c = c;
			this.l = l;
			this.a = a;
		}

		public ColorHCL(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorHCL FromRGB(Color rgb)
		{
			ColorHCL hcl;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			hcl.c = max - min;

			if (hcl.c > 0f)
			{
				if (rgb.r == max)
				{
					hcl.h = Mathf.Repeat((rgb.g - rgb.b) / hcl.c, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hcl.h = ((rgb.b - rgb.r) / hcl.c + 2f) / 6f;
				}
				else
				{
					hcl.h = ((rgb.r - rgb.g) / hcl.c + 4f) / 6f;
				}
			}
			else
			{
				hcl.h = 0f;
			}

			hcl.l = (max + min) * 0.5f;
			hcl.a = rgb.a;

			return hcl;
		}

		public Color ToRGB()
		{
			Color rgb = new Color(0f, 0f, 0f, a);
			if (c > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					rgb.r = c;
					rgb.g = c * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					rgb.g = c;
					rgb.r = c * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					rgb.g = c;
					rgb.b = c * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					rgb.b = c;
					rgb.g = c * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					rgb.b = c;
					rgb.r = c * (scaledHue - 4f);
				}
				else
				{
					rgb.r = c;
					rgb.b = c * (6f - scaledHue);
				}
			}

			float min = l - c * 0.5f;
			rgb.r = Mathf.Clamp01(rgb.r + min);
			rgb.g = Mathf.Clamp01(rgb.g + min);
			rgb.b = Mathf.Clamp01(rgb.b + min);
			return rgb;
		}

		public static ColorHCL FromHSL(ColorHSL hsl)
		{
			return hsl.ToHCL();
		}

		public ColorHSL ToHSL()
		{
			float max = 1f - Mathf.Abs(2f * l - 1f);
			if (max != 0f)
			{
				return new ColorHSL(h, c / max, l, a);
			}
			else
			{
				return new ColorHSL(h, 0f, l, a);
			}
		}

		public static explicit operator Color(ColorHCL hcl)
		{
			return hcl.ToRGB();
		}

		public static explicit operator ColorHSL(ColorHCL hcl)
		{
			return hcl.ToHSL();
		}

		public static explicit operator ColorHCL(Color rgb)
		{
			return FromRGB(rgb);
		}

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

		public static ColorHCL Lerp(ColorHCL a, ColorHCL b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorHCL LerpUnclamped(ColorHCL a, ColorHCL b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHCL(
				hueDelta <= 0.5f
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t)
							: Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.l, b.l, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHCL LerpForward(ColorHCL a, ColorHCL b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

		public static ColorHCL LerpForwardUnclamped(ColorHCL a, ColorHCL b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHCL(
				hueA <= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.l, b.l, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHCL LerpBackward(ColorHCL a, ColorHCL b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

		public static ColorHCL LerpBackwardUnclamped(ColorHCL a, ColorHCL b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHCL(
				hueA >= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.l, b.l, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHCL operator +(ColorHCL a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(a.h + b.h, 1f), a.c + b.c, a.l + b.l, a.a + b.a);
		}

		public static ColorHCL operator -(ColorHCL a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(a.h - b.h, 1f), a.c - b.c, a.l - b.l, a.a - b.a);
		}

		public static ColorHCL operator *(float b, ColorHCL a)
		{
			return new ColorHCL(Mathf.Repeat(a.h * b, 1f), a.c * b, a.l * b, a.a * b);
		}

		public static ColorHCL operator *(ColorHCL a, float b)
		{
			return new ColorHCL(Mathf.Repeat(a.h * b, 1f), a.c * b, a.l * b, a.a * b);
		}

		public static ColorHCL operator *(ColorHCL a, ColorHCL b)
		{
			return new ColorHCL(Mathf.Repeat(a.h * b.h, 1f), a.c * b.c, a.l * b.l, a.a * b.a);
		}

		public static ColorHCL operator /(ColorHCL a, float b)
		{
			return new ColorHCL(Mathf.Repeat(a.h / b, 1f), a.c / b, a.l / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorHCL && this == (ColorHCL)other;
		}

		public override int GetHashCode()
		{
			return h.GetHashCode() ^ c.GetHashCode() ^ l.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorHCL lhs, ColorHCL rhs)
		{
			return lhs.h == rhs.h && lhs.c == rhs.c && lhs.l == rhs.l && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorHCL lhs, ColorHCL rhs)
		{
			return lhs.h != rhs.h || lhs.c != rhs.c || lhs.l != rhs.l || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorHCL hcl)
		{
			return new Vector4(hcl.h, hcl.c, hcl.l, hcl.a);
		}

		public static implicit operator ColorHCL(Vector4 v)
		{
			return new ColorHCL(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("HCLA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, c, l, a);
		}

		public string ToString(string format)
		{
			return string.Format("HCLA({0}, {1}, {2}, {3})", h.ToString(format), c.ToString(format), l.ToString(format), a.ToString(format));
		}

		public bool canConvertToRGB
		{
			get
			{
				return c <= GetMaxChroma(l);
			}
		}

		public static float GetLightnessAtMaxChroma()
		{
			return 0.5f;
		}

		public static void GetMinMaxLightness(float c, out float lMin, out float lMax)
		{
			lMin = c * 0.5f;
			lMax = 1f - lMin;
		}

		public static float GetMaxChroma(float l)
		{
			return 1f - Mathf.Abs(2f * l - 1f);
		}
	}
}
