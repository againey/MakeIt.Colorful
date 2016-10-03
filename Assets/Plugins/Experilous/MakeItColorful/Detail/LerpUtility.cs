/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;

#if !UNITY_5_2 && !UNITY_5_3_OR_NEWER

namespace Experilous.MakeItColorful.Detail
{
	public static class LerpUtility
	{
		public static float LerpUnclamped(float a, float b, float t)
		{
			return (b - a) * t + a;
		}
	}
}

#endif
