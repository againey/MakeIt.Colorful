/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;
using Experilous.Numerics;

namespace Experilous.Colors
{
	[Serializable]
	public struct ColorHSV
	{
		public float h;
		public float s;
		public float v;
		public float a;

		public ColorHSV(float h, float s, float v)
		{
			this.h = h;
			this.s = s;
			this.v = v;
			a = 1f;
		}

		public ColorHSV(float h, float s, float v, float a)
		{
			this.h = h;
			this.s = s;
			this.v = v;
			this.a = a;
		}

		public ColorHSV(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorHSV FromRGB(Color rgb)
		{
			ColorHSV hsv;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			float chroma = max - min;

			if (chroma > 0f)
			{
				if (rgb.r == max)
				{
					hsv.h = Mathf.Repeat((rgb.g - rgb.b) / chroma, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hsv.h = ((rgb.b - rgb.r) / chroma + 2f) / 6f;
				}
				else
				{
					hsv.h = ((rgb.r - rgb.g) / chroma + 4f) / 6f;
				}

				hsv.s = chroma / max;
			}
			else
			{
				hsv.h = 0f;
				hsv.s = 0f;
			}

			hsv.v = max;
			hsv.a = rgb.a;

			return hsv;
		}

		public Color ToRGB()
		{
			float chroma = v * s;
			float min = v - chroma;
			Color rgb = new Color(min, min, min, a);
			if (chroma > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					rgb.r += chroma;
					rgb.g += chroma * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					rgb.g += chroma;
					rgb.r += chroma * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					rgb.g += chroma;
					rgb.b += chroma * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					rgb.b += chroma;
					rgb.g += chroma * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					rgb.b += chroma;
					rgb.r += chroma * (scaledHue - 4f);
				}
				else
				{
					rgb.r += chroma;
					rgb.b += chroma * (6f - scaledHue);
				}
			}
			return rgb;
		}

		public static explicit operator Color(ColorHSV hsv)
		{
			return hsv.ToRGB();
		}

		public static explicit operator ColorHSV(Color rgb)
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

		public static ColorHSV Lerp(ColorHSV a, ColorHSV b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorHSV LerpUnclamped(ColorHSV a, ColorHSV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHSV(
				hueDelta <= 0.5f
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t)
							: Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSV LerpForward(ColorHSV a, ColorHSV b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

		public static ColorHSV LerpForwardUnclamped(ColorHSV a, ColorHSV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSV(
				hueA <= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSV LerpBackward(ColorHSV a, ColorHSV b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

		public static ColorHSV LerpBackwardUnclamped(ColorHSV a, ColorHSV b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSV(
				hueA >= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.v, b.v, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSV operator +(ColorHSV a, ColorHSV b)
		{
			return new ColorHSV(a.h + b.h, a.s + b.s, a.v + b.v, a.a + b.a);
		}

		public static ColorHSV operator -(ColorHSV a, ColorHSV b)
		{
			return new ColorHSV(a.h - b.h, a.s - b.s, a.v - b.v, a.a - b.a);
		}

		public static ColorHSV operator *(float b, ColorHSV a)
		{
			return new ColorHSV(a.h * b, a.s * b, a.v * b, a.a * b);
		}

		public static ColorHSV operator *(ColorHSV a, float b)
		{
			return new ColorHSV(a.h * b, a.s * b, a.v * b, a.a * b);
		}

		public static ColorHSV operator *(ColorHSV a, ColorHSV b)
		{
			return new ColorHSV(a.h * b.h, a.s * b.s, a.v * b.v, a.a * b.a);
		}

		public static ColorHSV operator /(ColorHSV a, float b)
		{
			return new ColorHSV(a.h / b, a.s / b, a.v / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorHSV && this == (ColorHSV)other;
		}

		public override int GetHashCode()
		{
			return h.GetHashCode() ^ s.GetHashCode() ^ v.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorHSV lhs, ColorHSV rhs)
		{
			return lhs.h == rhs.h && lhs.s == rhs.s && lhs.v == rhs.v && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorHSV lhs, ColorHSV rhs)
		{
			return lhs.h != rhs.h || lhs.s != rhs.s || lhs.v != rhs.v || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorHSV hsv)
		{
			return new Vector4(hsv.h, hsv.s, hsv.v, hsv.a);
		}

		public static implicit operator ColorHSV(Vector4 v)
		{
			return new ColorHSV(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("HSVA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, v, a);
		}

		public string ToString(string format)
		{
			return string.Format("HSVA({0}, {1}, {2}, {3})", h.ToString(format), s.ToString(format), v.ToString(format), a.ToString(format));
		}
	}
}
