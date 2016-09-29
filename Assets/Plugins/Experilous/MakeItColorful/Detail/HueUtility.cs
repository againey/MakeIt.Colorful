/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;

#if UNITY_5_2 || UNITY_5_3_OR_NEWER
using Math = UnityEngine.Mathf;
#else
using Math = Experilous.Numerics.Math;
#endif

namespace Experilous.MakeItColorful.Detail
{
	public static class HueUtility
	{
		public static Color ToRGB(float hue, float chroma, float a)
		{
			float scaledHue = hue * 6f;
			if (scaledHue < 1f)
			{
				return new Color(chroma, chroma * scaledHue, 0f, a);
			}
			else if (scaledHue < 2f)
			{
				return new Color(chroma * (2f - scaledHue), chroma, 0f, a);
			}
			else if (scaledHue < 3f)
			{
				return new Color(0f, chroma, chroma * (scaledHue - 2f), a);
			}
			else if (scaledHue < 4f)
			{
				return new Color(0f, chroma * (4f - scaledHue), chroma, a);
			}
			else if (scaledHue < 5f)
			{
				return new Color(chroma * (scaledHue - 4f), 0f, chroma, a);
			}
			else
			{
				return new Color(chroma, 0f, chroma * (6f - scaledHue), a);
			}
		}

		public static Color ToRGB(float hue, float chroma, float min, float a)
		{
			float scaledHue = hue * 6f;
			if (scaledHue < 1f)
			{
				return new Color(chroma + min, chroma * scaledHue + min, min, a);
			}
			else if (scaledHue < 2f)
			{
				return new Color(chroma * (2f - scaledHue) + min, chroma + min, min, a);
			}
			else if (scaledHue < 3f)
			{
				return new Color(min, chroma + min, chroma * (scaledHue - 2f) + min, a);
			}
			else if (scaledHue < 4f)
			{
				return new Color(min, chroma * (4f - scaledHue) + min, chroma + min, a);
			}
			else if (scaledHue < 5f)
			{
				return new Color(chroma * (scaledHue - 4f) + min, min, chroma + min, a);
			}
			else
			{
				return new Color(chroma + min, min, chroma * (6f - scaledHue) + min, a);
			}
		}

		public static void ToRGB(float hue, out float r, out float g, out float b)
		{
			float scaledHue = hue * 6f;
			if (scaledHue < 1f)
			{
				r = 1f;
				g = scaledHue;
				b = 0f;
			}
			else if (scaledHue < 2f)
			{
				r = 2f - scaledHue;
				g = 1f;
				b = 0f;
			}
			else if (scaledHue < 3f)
			{
				r = 0f;
				g = 1f;
				b = scaledHue - 2f;
			}
			else if (scaledHue < 4f)
			{
				r = 0f;
				g = 4f - scaledHue;
				b = 1f;
			}
			else if (scaledHue < 5f)
			{
				r = scaledHue - 4f;
				g = 0f;
				b = 1f;
			}
			else
			{
				r = 1f;
				g = 0f;
				b = 6f - scaledHue;
			}
		}

		public static void ToRGB(float hue, float chroma, out float r, out float g, out float b)
		{
			float scaledHue = hue * 6f;
			if (scaledHue < 1f)
			{
				r = chroma;
				g = scaledHue * chroma;
				b = 0f;
			}
			else if (scaledHue < 2f)
			{
				r = (2f - scaledHue) * chroma;
				g = chroma;
				b = 0f;
			}
			else if (scaledHue < 3f)
			{
				r = 0f;
				g = chroma;
				b = (scaledHue - 2f) * chroma;
			}
			else if (scaledHue < 4f)
			{
				r = 0f;
				g = (4f - scaledHue) * chroma;
				b = chroma;
			}
			else if (scaledHue < 5f)
			{
				r = (scaledHue - 4f) * chroma;
				g = 0f;
				b = chroma;
			}
			else
			{
				r = chroma;
				g = 0f;
				b = (6f - scaledHue) * chroma;
			}
		}

		public static void ToRGB(float hue, float chroma, float min, out float r, out float g, out float b)
		{
			float scaledHue = hue * 6f;
			if (scaledHue < 1f)
			{
				r = chroma + min;
				g = scaledHue * chroma + min;
				b = min;
			}
			else if (scaledHue < 2f)
			{
				r = (2f - scaledHue) * chroma + min;
				g = chroma + min;
				b = min;
			}
			else if (scaledHue < 3f)
			{
				r = min;
				g = chroma + min;
				b = (scaledHue - 2f) * chroma + min;
			}
			else if (scaledHue < 4f)
			{
				r = min;
				g = (4f - scaledHue) * chroma + min;
				b = chroma + min;
			}
			else if (scaledHue < 5f)
			{
				r = (scaledHue - 4f) * chroma + min;
				g = min;
				b = chroma + min;
			}
			else
			{
				r = chroma + min;
				g = min;
				b = (6f - scaledHue) * chroma + min;
			}
		}

		public static float FromRGB(float r, float g, float b, float max, float chroma)
		{
			if (chroma > 0f & max > 0f)
			{
				if (r == max)
				{
					return Mathf.Repeat((g - b) / chroma, 6f) / 6f;
				}
				else if (g == max)
				{
					return ((b - r) / chroma + 2f) / 6f;
				}
				else
				{
					return ((r - g) / chroma + 4f) / 6f;
				}
			}
			else
			{
				return 0f;
			}
		}

		public static float FromCMY(float c, float m, float y, float min, float chroma)
		{
			if (chroma > 0f & min < 1f)
			{
				if (c == min)
				{
					return Mathf.Repeat((y - m) / chroma, 6f) / 6f;
				}
				else if (m == min)
				{
					return ((c - y) / chroma + 2f) / 6f;
				}
				else
				{
					return ((m - c) / chroma + 4f) / 6f;
				}
			}
			else
			{
				return 0f;
			}
		}

		public static float LerpUnclamped(float a, float b, float t)
		{
			a = Mathf.Repeat(a, 1f);
			b = Mathf.Repeat(b, 1f);
			float delta = Mathf.Abs(b - a);
			if (delta < 0.5f)
			{
				return Math.LerpUnclamped(a, b, t);
			}
			else if (delta > 0.5f)
			{
				if (a < b)
				{
					return Mathf.Repeat(Math.LerpUnclamped(a + 1f, b, t), 1f);
				}
				else
				{
					return Mathf.Repeat(Math.LerpUnclamped(a, b + 1f, t), 1f);
				}
			}
			else
			{
				if (a < b)
				{
					return Math.LerpUnclamped(a, b, t);
				}
				else
				{
					return Mathf.Repeat(Math.LerpUnclamped(a, b + 1f, t), 1f);
				}
			}
		}

		public static float LerpForwardUnclamped(float a, float b, float t)
		{
			a = Mathf.Repeat(a, 1f);
			b = Mathf.Repeat(b, 1f);
			if (a <= b)
			{
				return Math.LerpUnclamped(a, b, t);
			}
			else
			{
				return Mathf.Repeat(Math.LerpUnclamped(a, b + 1f, t), 1f);
			}
		}

		public static float LerpBackwardUnclamped(float a, float b, float t)
		{
			a = Mathf.Repeat(a, 1f);
			b = Mathf.Repeat(b, 1f);
			if (a >= b)
			{
				return Math.LerpUnclamped(a, b, t);
			}
			else
			{
				return Mathf.Repeat(Math.LerpUnclamped(a + 1f, b, t), 1f);
			}
		}
	}
}
