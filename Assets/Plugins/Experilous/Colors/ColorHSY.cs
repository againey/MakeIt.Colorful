/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.Colors
{
	[Serializable]
	public struct ColorHSY
	{
		public float h;
		public float s;
		public float y;
		public float a;

		private const float _redLumaFactor = 0.30f;
		private const float _greenLumaFactor = 0.59f;
		private const float _blueLumaFactor = 0.11f;

		public ColorHSY(float h, float s, float y)
		{
			this.h = h;
			this.s = s;
			this.y = y;
			a = 1f;
		}

		public ColorHSY(float h, float s, float y, float a)
		{
			this.h = h;
			this.s = s;
			this.y = y;
			this.a = a;
		}

		public ColorHSY(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorHSY FromRGB(Color rgb)
		{
			ColorHSY hsy;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			float chroma = max - min;

			if (chroma > 0f)
			{
				if (rgb.r == max)
				{
					hsy.h = Mathf.Repeat((rgb.g - rgb.b) / chroma, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hsy.h = ((rgb.b - rgb.r) / chroma + 2f) / 6f;
				}
				else
				{
					hsy.h = ((rgb.r - rgb.g) / chroma + 4f) / 6f;
				}

				hsy.y = rgb.r * _redLumaFactor + rgb.g * _greenLumaFactor + rgb.b * _blueLumaFactor;

				if (hsy.y != 0f)
				{
					hsy.s = chroma / ColorHCY.GetMaxChroma(hsy.h, hsy.y);
				}
				else
				{
					hsy.s = 0f;
				}
			}
			else
			{
				hsy.h = 0f;
				hsy.s = 0f;
				hsy.y = rgb.r * _redLumaFactor + rgb.g * _greenLumaFactor + rgb.b * _blueLumaFactor;
			}

			hsy.a = rgb.a;

			return hsy;
		}

		public Color ToRGB()
		{
			Color rgb = new Color(0f, 0f, 0f, a);
			float chroma = s * ColorHCY.GetMaxChroma(h, y);
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

			float min = y - (rgb.r * _redLumaFactor + rgb.g * _greenLumaFactor + rgb.b * _blueLumaFactor);
			rgb.r = Mathf.Clamp01(rgb.r + min);
			rgb.g = Mathf.Clamp01(rgb.g + min);
			rgb.b = Mathf.Clamp01(rgb.b + min);
			return rgb;
		}

		public static ColorHSY FromHCY(ColorHCY hcy)
		{
			return hcy.ToHSY();
		}

		public ColorHCY ToHCY()
		{
			return new ColorHCY(h, s * ColorHCY.GetMaxChroma(h, y), y, a);
		}

		public static explicit operator Color(ColorHSY hsy)
		{
			return hsy.ToRGB();
		}

		public static explicit operator ColorHCY(ColorHSY hsy)
		{
			return hsy.ToHCY();
		}

		public static explicit operator ColorHSY(Color rgb)
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
					case 1: s = value; break;
					case 2: y = value; break;
					case 3: a = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		public static ColorHSY Lerp(ColorHSY a, ColorHSY b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorHSY LerpUnclamped(ColorHSY a, ColorHSY b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHSY(
				hueDelta <= 0.5f
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t)
							: Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSY LerpForward(ColorHSY a, ColorHSY b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

		public static ColorHSY LerpForwardUnclamped(ColorHSY a, ColorHSY b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSY(
				hueA <= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSY LerpBackward(ColorHSY a, ColorHSY b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

		public static ColorHSY LerpBackwardUnclamped(ColorHSY a, ColorHSY b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHSY(
				hueA >= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				Numerics.Math.LerpUnclamped(a.s, b.s, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHSY operator +(ColorHSY a, ColorHSY b)
		{
			return new ColorHSY(a.h + b.h, a.s + b.s, a.y + b.y, a.a + b.a);
		}

		public static ColorHSY operator -(ColorHSY a, ColorHSY b)
		{
			return new ColorHSY(a.h - b.h, a.s - b.s, a.y - b.y, a.a - b.a);
		}

		public static ColorHSY operator *(float b, ColorHSY a)
		{
			return new ColorHSY(a.h * b, a.s * b, a.y * b, a.a * b);
		}

		public static ColorHSY operator *(ColorHSY a, float b)
		{
			return new ColorHSY(a.h * b, a.s * b, a.y * b, a.a * b);
		}

		public static ColorHSY operator *(ColorHSY a, ColorHSY b)
		{
			return new ColorHSY(a.h * b.h, a.s * b.s, a.y * b.y, a.a * b.a);
		}

		public static ColorHSY operator /(ColorHSY a, float b)
		{
			return new ColorHSY(a.h / b, a.s / b, a.y / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorHSY && this == (ColorHSY)other;
		}

		public override int GetHashCode()
		{
			return h.GetHashCode() ^ s.GetHashCode() ^ y.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorHSY lhs, ColorHSY rhs)
		{
			return lhs.h == rhs.h && lhs.s == rhs.s && lhs.y == rhs.y && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorHSY lhs, ColorHSY rhs)
		{
			return lhs.h != rhs.h || lhs.s != rhs.s || lhs.y != rhs.y || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorHSY hsy)
		{
			return new Vector4(hsy.h, hsy.s, hsy.y, hsy.a);
		}

		public static implicit operator ColorHSY(Vector4 v)
		{
			return new ColorHSY(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("HSYA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, s, y, a);
		}

		public string ToString(string format)
		{
			return string.Format("HSYA({0}, {1}, {2}, {3})", h.ToString(format), s.ToString(format), y.ToString(format), a.ToString(format));
		}
	}
}
