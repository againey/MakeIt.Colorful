/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	[Serializable]
	public struct ColorCMY
	{
		public float c;
		public float m;
		public float y;
		public float a;

		public ColorCMY(float c, float m, float y)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			a = 1f;
		}

		public ColorCMY(float c, float m, float y, float a)
		{
			this.c = c;
			this.m = m;
			this.y = y;
			this.a = a;
		}

		public ColorCMY(Color rgb)
		{
			this = FromRGB(rgb);
		}

		public static ColorCMY FromRGB(Color rgb)
		{
			return new ColorCMY(1f - rgb.r, 1f - rgb.g, 1f - rgb.b, rgb.a);
		}

		public Color ToRGB()
		{
			return new Color(1f - c, 1f - m, 1f - y, a);
		}

		public static ColorCMY FromCMYK(ColorCMYK cmyk)
		{
			return cmyk.ToCMY();
		}

		public ColorCMYK ToCMYK()
		{
			float k = Mathf.Min(Mathf.Min(c, m), y);
			if (k < 1f)
			{
				float kInv = 1f - k;
				return new ColorCMYK((c - k) / kInv, (m - k) / kInv, (y - k) / kInv, k, a);
			}
			else
			{
				return new ColorCMYK(0f, 0f, 0f, 1f, a);
			}
		}

		public static explicit operator Color(ColorCMY cmy)
		{
			return cmy.ToRGB();
		}

		public static explicit operator ColorCMY(Color rgb)
		{
			return FromRGB(rgb);
		}

		public static explicit operator ColorCMYK(ColorCMY cmy)
		{
			return cmy.ToCMYK();
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
					case 3: return a;
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
					case 3: a = value; break;
					default: throw new ArgumentOutOfRangeException();
				}
			}
		}

		public static ColorCMY Lerp(ColorCMY a, ColorCMY b, float t)
		{
			return LerpUnclamped(a, b, Mathf.Clamp01(t));
		}

		public static ColorCMY LerpUnclamped(ColorCMY a, ColorCMY b, float t)
		{
			return new ColorCMY(
				Numerics.Math.LerpUnclamped(a.c, b.c, t),
				Numerics.Math.LerpUnclamped(a.m, b.m, t),
				Numerics.Math.LerpUnclamped(a.y, b.y, t),
				Numerics.Math.LerpUnclamped(a.a, b.a, t));
		}

		public static ColorCMY operator +(ColorCMY a, ColorCMY b)
		{
			return new ColorCMY(a.c + b.c, a.m + b.m, a.y + b.y, a.a + b.a);
		}

		public static ColorCMY operator -(ColorCMY a, ColorCMY b)
		{
			return new ColorCMY(a.c - b.c, a.m - b.m, a.y - b.y, a.a - b.a);
		}

		public static ColorCMY operator *(float b, ColorCMY a)
		{
			return new ColorCMY(a.c * b, a.m * b, a.y * b, a.a * b);
		}

		public static ColorCMY operator *(ColorCMY a, float b)
		{
			return new ColorCMY(a.c * b, a.m * b, a.y * b, a.a * b);
		}

		public static ColorCMY operator *(ColorCMY a, ColorCMY b)
		{
			return new ColorCMY(a.c * b.c, a.m * b.m, a.y * b.y, a.a * b.a);
		}

		public static ColorCMY operator /(ColorCMY a, float b)
		{
			return new ColorCMY(a.c / b, a.m / b, a.y / b, a.a / b);
		}

		public override bool Equals(object other)
		{
			return other is ColorCMY && this == (ColorCMY)other;
		}

		public override int GetHashCode()
		{
			return c.GetHashCode() ^ m.GetHashCode() ^ y.GetHashCode() ^ a.GetHashCode();
		}

		public static bool operator ==(ColorCMY lhs, ColorCMY rhs)
		{
			return lhs.c == rhs.c && lhs.m == rhs.m && lhs.y == rhs.y && lhs.a == rhs.a;
		}

		public static bool operator !=(ColorCMY lhs, ColorCMY rhs)
		{
			return lhs.c != rhs.c || lhs.m != rhs.m || lhs.y != rhs.y || lhs.a != rhs.a;
		}

		public static implicit operator Vector4(ColorCMY hsv)
		{
			return new Vector4(hsv.c, hsv.m, hsv.y, hsv.a);
		}

		public static implicit operator ColorCMY(Vector4 v)
		{
			return new ColorCMY(v.x, v.y, v.z, v.w);
		}

		public override string ToString()
		{
			return string.Format("CMYA({0:F3}, {1:F3}, {2:F3}, {3:F3})", c, m, y, a);
		}

		public string ToString(string format)
		{
			return string.Format("CMYA({0}, {1}, {2}, {3})", c.ToString(format), m.ToString(format), y.ToString(format), a.ToString(format));
		}
	}
}
