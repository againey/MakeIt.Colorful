/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeIt.Utilities
{
	[Serializable]
	public struct ColorHSL
	{
		public float h;
		public float s;
		public float l;
		public float a;

		public ColorHSL(float h, float s, float l)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			a = 1f;
		}

		public ColorHSL(float h, float s, float l, float a)
		{
			this.h = h;
			this.s = s;
			this.l = l;
			this.a = a;
		}

		public ColorHSL(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorHSL FromRGB(Color rgb)
		{
			ColorHSL hsl;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			float chroma = max - min;

			if (chroma > 0f)
			{
				if (rgb.r == max)
				{
					hsl.h = Mathf.Repeat((rgb.g - rgb.b) / chroma, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hsl.h = ((rgb.b - rgb.r) / chroma + 2f) / 6f;
				}
				else
				{
					hsl.h = ((rgb.r - rgb.g) / chroma + 4f) / 6f;
				}

				hsl.s = chroma / max;
			}
			else
			{
				hsl.h = 0f;
				hsl.s = 0f;
			}

			hsl.l = (max + min) * 0.5f;
			hsl.a = rgb.a;

			return hsl;
		}

		public Color ToRGB()
		{
			float chroma = (1f - Mathf.Abs(2f * l - 1f)) * s;
			Color rgb = new Color(0f, 0f, 0f, a);
			if (chroma > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					rgb.r = chroma;
					rgb.g = chroma * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					rgb.g = chroma;
					rgb.r = chroma * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					rgb.g = chroma;
					rgb.b = chroma * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					rgb.b = chroma;
					rgb.g = chroma * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					rgb.b = chroma;
					rgb.r = chroma * (scaledHue - 4f);
				}
				else
				{
					rgb.r = chroma;
					rgb.b = chroma * (6f - scaledHue);
				}
			}

			float min = l - chroma * 0.5f;
			rgb.r += min;
			rgb.g += min;
			rgb.b += min;
			return rgb;
		}

		public static explicit operator Color(ColorHSL hsl)
		{
			return hsl.ToRGB();
		}

		public static explicit operator ColorHSL(Color rgb)
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

		public static ColorHSL Lerp(ColorHSL a, ColorHSL b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorHSL LerpUnclamped(ColorHSL a, ColorHSL b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHSL(
				hueDelta <= 0.5f
					? MathTools.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? MathTools.LerpUnclamped(hueA + 1f, hueB, t)
							: MathTools.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				MathTools.LerpUnclamped(a.s, b.s, t),
				MathTools.LerpUnclamped(a.l, b.l, t),
				MathTools.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSL LerpForward(ColorHSL a, ColorHSL b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

		public static ColorHSL LerpForwardUnclamped(ColorHSL a, ColorHSL b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSL(
				hueA <= hueB
					? MathTools.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(MathTools.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				MathTools.LerpUnclamped(a.s, b.s, t),
				MathTools.LerpUnclamped(a.l, b.l, t),
				MathTools.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSL LerpBackward(ColorHSL a, ColorHSL b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

		public static ColorHSL LerpBackwardUnclamped(ColorHSL a, ColorHSL b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSL(
				hueA >= hueB
					? MathTools.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(MathTools.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				MathTools.LerpUnclamped(a.s, b.s, t),
				MathTools.LerpUnclamped(a.l, b.l, t),
				MathTools.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSL operator +(ColorHSL a, ColorHSL b)
		{
			return new ColorHSL(a.h + b.h, a.s + b.s, a.l + b.l, a.a + b.a);
		}

		public static ColorHSL operator -(ColorHSL a, ColorHSL b)
		{
			return new ColorHSL(a.h - b.h, a.s - b.s, a.l - b.l, a.a - b.a);
		}

		public static ColorHSL operator *(float b, ColorHSL a)
		{
			return new ColorHSL(a.h * b, a.s * b, a.l * b, a.a * b);
		}

		public static ColorHSL operator *(ColorHSL a, float b)
		{
			return new ColorHSL(a.h * b, a.s * b, a.l * b, a.a * b);
		}

		public static ColorHSL operator *(ColorHSL a, ColorHSL b)
		{
			return new ColorHSL(a.h * b.h, a.s * b.s, a.l * b.l, a.a * b.a);
		}

		public static ColorHSL operator /(ColorHSL a, float b)
		{
			return new ColorHSL(a.h / b, a.s / b, a.l / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorHSL && this == (ColorHSL)other;
		}

		public override int GetHashCode()
		{
			return h.GetHashCode() ^ s.GetHashCode() ^ l.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorHSL lhs, ColorHSL rhs)
		{
			return lhs.h == rhs.h && lhs.s == rhs.s && lhs.l == rhs.l && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorHSL lhs, ColorHSL rhs)
		{
			return lhs.h != rhs.h || lhs.s != rhs.s || lhs.l != rhs.l || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorHSL hsl)
		{
			return new Vector4(hsl.h, hsl.s, hsl.l, hsl.a);
		}

		public static implicit operator ColorHSL(Vector4 v)
		{
			return new ColorHSL(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("HSLA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, l, a);
		}

		public string ToString(string format)
		{
			return string.Format("HSLA({0}, {1}, {2}, {3})", h.ToString(format), s.ToString(format), l.ToString(format), a.ToString(format));
		}
	}
}
