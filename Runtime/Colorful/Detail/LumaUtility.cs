/******************************************************************************\
* Copyright Andy Gainey                                                        *
*                                                                              *
* Licensed under the Apache License, Version 2.0 (the "License");              *
* you may not use this file except in compliance with the License.             *
* You may obtain a copy of the License at                                      *
*                                                                              *
*     http://www.apache.org/licenses/LICENSE-2.0                               *
*                                                                              *
* Unless required by applicable law or agreed to in writing, software          *
* distributed under the License is distributed on an "AS IS" BASIS,            *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.     *
* See the License for the specific language governing permissions and          *
* limitations under the License.                                               *
\******************************************************************************/

namespace MakeIt.Colorful.Detail
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
