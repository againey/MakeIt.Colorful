/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	[Serializable]
	public struct ColorCMYK
	{
		public float c;
		public float m;
		public float y;
		public float k;
		public float a;

		public ColorCMYK(float c, float m, float y)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			k = 0f;
			a = 1f;
		}

		public ColorCMYK(float c, float m, float y, float k)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			this.k = k;
			a = 1f;
		}

		public ColorCMYK(float c, float m, float y, float k, float a)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			this.k = k;
			this.a = a;
		}

		public ColorCMYK(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorCMYK FromRGB(Color rgb)
		{
			float k = 1f - Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);
			if (k < 1f)
			{
				float kInv = 1f - k;
				return new ColorCMYK((1f - rgb.r - k) / kInv, (1f - rgb.g - k) / kInv, (1f - rgb.b - k) / kInv, k, rgb.a);
			}
			else
			{
				return new ColorCMYK(0f, 0f, 0f, 1f, rgb.a);
			}
		}

		public Color ToRGB()
		{
			float kInv = 1f - k;
			return new Color((1f - c) * kInv, (1f - m) * kInv, (1f - y) * kInv, a);
		}

		public static ColorCMYK FromCMY(ColorCMY cmy)
		{
			return cmy.ToCMYK();
		}

		public ColorCMY ToCMY()
		{
			float kInv = 1f - k;
			return new ColorCMY(c * kInv + k, m * kInv + k, y * kInv + k, a);
		}

		public static explicit operator Color(ColorCMYK cmyk)
		{
			return cmyk.ToRGB();
		}

		public static explicit operator ColorCMYK(Color rgb)
		{
			return FromRGB(rgb);
		}

		public static explicit operator ColorCMY(ColorCMYK cmyk)
		{
			return cmyk.ToCMY();
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return c;
					case 1: return m;
					case 2: return y;
					case 3: return k;
					case 4: return a;
					default: throw new ArgumentOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0: c = value; break;
					case 1: m = value; break;
					case 2: y = value; break;
					case 3: k = value; break;
					case 4: a = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		public static ColorCMYK Lerp(ColorCMYK a, ColorCMYK b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorCMYK LerpUnclamped(ColorCMYK a, ColorCMYK b, float t)
		{
			return new ColorCMYK(
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.m, b.m, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.k, b.k, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorCMYK operator +(ColorCMYK a, ColorCMYK b)
		{
			return new ColorCMYK(a.c + b.c, a.m + b.m, a.y + b.y, a.a + b.a, a.k + b.k);
		}

		public static ColorCMYK operator -(ColorCMYK a, ColorCMYK b)
		{
			return new ColorCMYK(a.c - b.c, a.m - b.m, a.y - b.y, a.a - b.a, a.k - b.k);
		}

		public static ColorCMYK operator *(float b, ColorCMYK a)
		{
			return new ColorCMYK(a.c * b, a.m * b, a.y * b, a.k * b, a.a * b);
		}

		public static ColorCMYK operator *(ColorCMYK a, float b)
		{
			return new ColorCMYK(a.c * b, a.m * b, a.y * b, a.k * b, a.a * b);
		}

		public static ColorCMYK operator *(ColorCMYK a, ColorCMYK b)
		{
			return new ColorCMYK(a.c * b.c, a.m * b.m, a.y * b.y, a.k * b.k, a.a * b.a);
		}

		public static ColorCMYK operator /(ColorCMYK a, float b)
		{
			return new ColorCMYK(a.c / b, a.m / b, a.y / b, a.k / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorCMYK && this == (ColorCMYK)other;
		}

		public override int GetHashCode()
		{
			return c.GetHashCode() ^ m.GetHashCode() ^ y.GetHashCode() ^ k.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorCMYK lhs, ColorCMYK rhs)
		{
			return lhs.c == rhs.c && lhs.m == rhs.m && lhs.y == rhs.y && lhs.k == rhs.k && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorCMYK lhs, ColorCMYK rhs)
		{
			return lhs.c != rhs.c || lhs.m != rhs.m || lhs.y != rhs.y || lhs.k != rhs.k || lhs.a != rhs.a;
		}

		public override string ToString()
		{
			return string.Format("CMYKA({0:F3}, {1:F3}, {2:F3}, {3:F3}, {4:F3})", c, m, y, k, a);
		}

		public string ToString(string format)
		{
			return string.Format("CMYKA({0}, {1}, {2}, {3}, {4})", c.ToString(format), m.ToString(format), y.ToString(format), k.ToString(format), a.ToString(format));
		}
	}
}
