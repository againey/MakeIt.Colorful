/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;

namespace Experilous.MakeItColorful.Detail
{
	public static class ValueUtility
	{
		public static float GetChroma(float s, float v)
		{
			return v * s;
		}

		public static float GetSaturation(float c, float v)
		{
			return (v != 0f) ? c / v : 0f;
		}

		public static float GetValueAtMaxChroma()
		{
			return 1f;
		}

		public static void GetMinMaxValue(float c, out float vMin, out float vMax)
		{
			vMin = c;
			vMax = 1f;
		}

		public static float GetMaxChroma(float v)
		{
			return v;
		}
	}
}
