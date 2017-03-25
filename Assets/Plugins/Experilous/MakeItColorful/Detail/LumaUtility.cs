/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

namespace Experilous.MakeItColorful.Detail
{
	public static class LumaUtility
	{
		public const float rWeight = 0.30f;
		public const float gWeight = 0.59f;
		public const float bWeight = 0.11f;

		public static float FromRGB(float r, float g, float b)
		{
			return r * rWeight + g * gWeight + b * bWeight;
		}

		public static float FromCMY(float c, float m, float y)
		{
			return 1f - (c * rWeight + m * gWeight + y * bWeight);
		}

		public static float GetChroma(float h, float s, float y)
		{
			float cMax = GetMaxChroma(h, y);
			return s * cMax;
		}

		public static float GetSaturation(float h, float c, float y)
		{
			float cMax = GetMaxChroma(h, y);
			return (cMax != 0f) ? c / cMax : 0f;
		}

		public static float GetLumaAtMaxChroma(float h)
		{
			float r, g, b;
			HueUtility.ToRGB(UnityEngine.Mathf.Repeat(h, 1), out r, out g, out b);
			return FromRGB(r, g, b);
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
