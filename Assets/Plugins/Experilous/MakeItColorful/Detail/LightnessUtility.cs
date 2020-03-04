/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;

namespace Experilous.MakeItColorful.Detail
{
	public static class LightnessUtility
	{
		public static float GetLightness(float min, float max)
		{
			return (max + min) * 0.5f;
		}

		public static float GetChroma(float s, float l)
		{
			return (1f - Mathf.Abs(2f * l - 1f)) * s;
		}

		public static float GetSaturation(float c, float l)
		{
			return (l != 0f & l != 1f) ? c / (1f - Mathf.Abs(l * 2 - 1)) : 0f;
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
