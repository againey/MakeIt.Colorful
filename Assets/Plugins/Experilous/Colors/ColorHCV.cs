/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.Colors
{
	[Serializable]
	public struct ColorHCV
	{
		public float h;
		public float c;
		public float v;
		public float a;

		public ColorHCV(float h, float c, float v)
		{
			this.h = h;
			this.c = c;
			this.v = v;
			a = 1f;
		}

		public ColorHCV(float h, float c, float v, float a)
		{
			this.h = h;
			this.c = c;
			this.v = v;
			this.a = a;
		}

		public ColorHCV(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorHCV FromRGB(Color rgb)
		{
			ColorHCV hcv;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			hcv.c = max - min;

			if (hcv.c > 0f)
			{
				if (rgb.r == max)
				{
					hcv.h = Mathf.Repeat((rgb.g - rgb.b) / hcv.c, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hcv.h = ((rgb.b - rgb.r) / hcv.c + 2f) / 6f;
				}
				else
				{
					hcv.h = ((rgb.r - rgb.g) / hcv.c + 4f) / 6f;
				}
			}
			else
			{
				hcv.h = 0f;
			}

			hcv.v = max;
			hcv.a = rgb.a;

			return hcv;
		}

		public Color ToRGB()
		{
			float min = v - c;
			Color rgb = new Color(min, min, min, a);
			if (c > 0f)
			{
				float scaledHue = h * 6f;
				if (scaledHue < 1f)
				{
					rgb.r += c;
					rgb.g += c * scaledHue;
				}
				else if (scaledHue < 2f)
				{
					rgb.g += c;
					rgb.r += c * (2f - scaledHue);
				}
				else if (scaledHue < 3f)
				{
					rgb.g += c;
					rgb.b += c * (scaledHue - 2f);
				}
				else if (scaledHue < 4f)
				{
					rgb.b += c;
					rgb.g += c * (4f - scaledHue);
				}
				else if (scaledHue < 5f)
				{
					rgb.b += c;
					rgb.r += c * (scaledHue - 4f);
				}
				else
				{
					rgb.r += c;
					rgb.b += c * (6f - scaledHue);
				}
			}
			rgb.r = Mathf.Clamp01(rgb.r);
			rgb.g = Mathf.Clamp01(rgb.g);
			rgb.b = Mathf.Clamp01(rgb.b);
			return rgb;
		}

		public static ColorHCV FromHSV(ColorHSV hsv)
		{
			return hsv.ToHCV();
		}

		public ColorHSV ToHSV()
		{
			if (v != 0f)
			{
				return new ColorHSV(h, c / v, v, a);
			}
			else
			{
				return new ColorHSV(h, 0f, 0f, a);
			}
		}

		public static explicit operator Color(ColorHCV hcv)
		{
			return hcv.ToRGB();
		}

		public static explicit operator ColorHCV(Color rgb)
		{
			return FromRGB(rgb);
		}

		public static explicit operator ColorHSV(ColorHCV hcv)
		{
			return hcv.ToHSV();
		}

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

		public static ColorHCV Lerp(ColorHCV a, ColorHCV b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

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

		public static ColorHCV LerpForward(ColorHCV a, ColorHCV b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

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

		public static ColorHCV LerpBackward(ColorHCV a, ColorHCV b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

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

		public static ColorHCV operator +(ColorHCV a, ColorHCV b)
		{
			return new ColorHCV(a.h + b.h, a.c + b.c, a.v + b.v, a.a + b.a);
		}

		public static ColorHCV operator -(ColorHCV a, ColorHCV b)
		{
			return new ColorHCV(a.h - b.h, a.c - b.c, a.v - b.v, a.a - b.a);
		}

		public static ColorHCV operator *(float b, ColorHCV a)
		{
			return new ColorHCV(a.h * b, a.c * b, a.v * b, a.a * b);
		}

		public static ColorHCV operator *(ColorHCV a, float b)
		{
			return new ColorHCV(a.h * b, a.c * b, a.v * b, a.a * b);
		}

		public static ColorHCV operator *(ColorHCV a, ColorHCV b)
		{
			return new ColorHCV(a.h * b.h, a.c * b.c, a.v * b.v, a.a * b.a);
		}

		public static ColorHCV operator /(ColorHCV a, float b)
		{
			return new ColorHCV(a.h / b, a.c / b, a.v / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorHCV && this == (ColorHCV)other;
		}

		public override int GetHashCode()
		{
			return h.GetHashCode() ^ c.GetHashCode() ^ v.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorHCV lhs, ColorHCV rhs)
		{
			return lhs.h == rhs.h && lhs.c == rhs.c && lhs.v == rhs.v && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorHCV lhs, ColorHCV rhs)
		{
			return lhs.h != rhs.h || lhs.c != rhs.c || lhs.v != rhs.v || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorHCV hcv)
		{
			return new Vector4(hcv.h, hcv.c, hcv.v, hcv.a);
		}

		public static implicit operator ColorHCV(Vector4 v)
		{
			return new ColorHCV(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("HCVA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, c, v, a);
		}

		public string ToString(string format)
		{
			return string.Format("HCVA({0}, {1}, {2}, {3})", h.ToString(format), c.ToString(format), v.ToString(format), a.ToString(format));
		}
	}
}
