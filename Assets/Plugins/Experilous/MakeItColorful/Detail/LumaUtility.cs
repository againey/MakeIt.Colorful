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
	}
}
