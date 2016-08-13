/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.Colors
{
	[Serializable]
	public struct ColorHCY
	{
		public float h;
		public float c;
		public float y;
		public float a;

		private const float _redLumaFactor = 0.30f;
		private const float _greenLumaFactor = 0.59f;
		private const float _blueLumaFactor = 0.11f;

		public ColorHCY(float h, float c, float y)
		{
			this.h = h;
			this.c = c;
			this.y = y;
			a = 1f;
		}

		public ColorHCY(float h, float c, float y, float a)
		{
			this.h = h;
			this.c = c;
			this.y = y;
			this.a = a;
		}

		public ColorHCY(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorHCY FromRGB(Color rgb)
		{
			ColorHCY hcy;
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);

			hcy.c = max - min;

			if (hcy.c > 0f)
			{
				if (rgb.r == max)
				{
					hcy.h = Mathf.Repeat((rgb.g - rgb.b) / hcy.c, 6f) / 6f;
				}
				else if (rgb.g == max)
				{
					hcy.h = ((rgb.b - rgb.r) / hcy.c + 2f) / 6f;
				}
				else
				{
					hcy.h = ((rgb.r - rgb.g) / hcy.c + 4f) / 6f;
				}

				hcy.y = rgb.r * _redLumaFactor + rgb.g * _greenLumaFactor + rgb.b * _blueLumaFactor;
			}
			else
			{
				hcy.h = 0f;
				hcy.y = rgb.r * _redLumaFactor + rgb.g * _greenLumaFactor + rgb.b * _blueLumaFactor;
			}

			hcy.a = rgb.a;

			return hcy;
		}

		public static ColorHCY FromHSY(float h, float s, float y)
		{
			return FromHSY(h, s, y, 1f);
		}

		public static ColorHCY FromHSY(float h, float s, float y, float a)
		{
			return new ColorHCY(h, s * GetMaxChroma(h, y), y, a);
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

			float min = y - (rgb.r * _redLumaFactor + rgb.g * _greenLumaFactor + rgb.b * _blueLumaFactor);
			rgb.r = Mathf.Clamp01(rgb.r + min);
			rgb.g = Mathf.Clamp01(rgb.g + min);
			rgb.b = Mathf.Clamp01(rgb.b + min);
			return rgb;
		}

		public static explicit operator Color(ColorHCY hcy)
		{
			return hcy.ToRGB();
		}

		public static explicit operator ColorHCY(Color rgb)
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

		public static ColorHCY Lerp(ColorHCY a, ColorHCY b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorHCY LerpUnclamped(ColorHCY a, ColorHCY b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			float hueDelta = Mathf.Abs(hueB - hueA);
			return new ColorHCY(
				hueDelta <= 0.5f
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(
						hueA < hueB
							? Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t)
							: Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t),
						1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHCY LerpForward(ColorHCY a, ColorHCY b, float t)
		{
			return LerpForwardUnclamped(a, b, t);
		}

		public static ColorHCY LerpForwardUnclamped(ColorHCY a, ColorHCY b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHCY(
				hueA <= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA, hueB + 1f, t), 1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHCY LerpBackward(ColorHCY a, ColorHCY b, float t)
		{
			return LerpBackwardUnclamped(a, b, t);
		}

		public static ColorHCY LerpBackwardUnclamped(ColorHCY a, ColorHCY b, float t)
		{
			float hueA = Mathf.Repeat(a.h, 1f);
			float hueB = Mathf.Repeat(b.h, 1f);
			return new ColorHCY(
				hueA >= hueB
					? Numerics.Math.LerpUnclamped(hueA, hueB, t)
					: Mathf.Repeat(Numerics.Math.LerpUnclamped(hueA + 1f, hueB, t), 1f),
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorHCY operator +(ColorHCY a, ColorHCY b)
		{
			return new ColorHCY(a.h + b.h, a.c + b.c, a.y + b.y, a.a + b.a);
		}

		public static ColorHCY operator -(ColorHCY a, ColorHCY b)
		{
			return new ColorHCY(a.h - b.h, a.c - b.c, a.y - b.y, a.a - b.a);
		}

		public static ColorHCY operator *(float b, ColorHCY a)
		{
			return new ColorHCY(a.h * b, a.c * b, a.y * b, a.a * b);
		}

		public static ColorHCY operator *(ColorHCY a, float b)
		{
			return new ColorHCY(a.h * b, a.c * b, a.y * b, a.a * b);
		}

		public static ColorHCY operator *(ColorHCY a, ColorHCY b)
		{
			return new ColorHCY(a.h * b.h, a.c * b.c, a.y * b.y, a.a * b.a);
		}

		public static ColorHCY operator /(ColorHCY a, float b)
		{
			return new ColorHCY(a.h / b, a.c / b, a.y / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorHCY && this == (ColorHCY)other;
		}

		public override int GetHashCode()
		{
			return h.GetHashCode() ^ c.GetHashCode() ^ y.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorHCY lhs, ColorHCY rhs)
		{
			return lhs.h == rhs.h && lhs.c == rhs.c && lhs.y == rhs.y && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorHCY lhs, ColorHCY rhs)
		{
			return lhs.h != rhs.h || lhs.c != rhs.c || lhs.y != rhs.y || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorHCY hcy)
		{
			return new Vector4(hcy.h, hcy.c, hcy.y, hcy.a);
		}

		public static implicit operator ColorHCY(Vector4 v)
		{
			return new ColorHCY(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("HCYA({0:F3}, {1:F3}, {2:F3}, {3:F3})", h, c, y, a);
		}

		public string ToString(string format)
		{
			return string.Format("HCYA({0}, {1}, {2}, {3})", h.ToString(format), c.ToString(format), y.ToString(format), a.ToString(format));
		}

		public bool canConvertToRGB
		{
			get
			{
				return c <= GetMaxChroma(h, y);
			}
		}

		public static float GetLumaAtMaxChroma(float h)
		{
			float r = 0f;
			float g = 0f;
			float b = 0f;

			float scaledHue = h * 6f;
			if (scaledHue < 1f)
			{
				r = 1f;
				g = scaledHue;
			}
			else if (scaledHue < 2f)
			{
				g = 1f;
				r = 2f - scaledHue;
			}
			else if (scaledHue < 3f)
			{
				g = 1f;
				b = scaledHue - 2f;
			}
			else if (scaledHue < 4f)
			{
				b = 1f;
				g = 4f - scaledHue;
			}
			else if (scaledHue < 5f)
			{
				b = 1f;
				r = scaledHue - 4f;
			}
			else
			{
				r = 1f;
				b = 6f - scaledHue;
			}

			return r * _redLumaFactor + g * _greenLumaFactor + b * _blueLumaFactor;
		}

		public static void GetMinMaxLuma(float h, float c, out float yMin, out float yMax)
		{
			float yMid = GetLumaAtMaxChroma(h);
			yMin = c * yMid;
			yMax = (1f - c) * (1f - yMid) + yMid;
		}

		public static float GetMaxChroma(float h, float y)
		{
			float yMid = GetLumaAtMaxChroma(h);
			return (y <= yMid) ? y / yMid : (1f - y) / (1f - yMid);
		}
	}
}
