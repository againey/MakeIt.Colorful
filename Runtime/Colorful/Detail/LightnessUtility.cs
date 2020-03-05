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

using UnityEngine;

namespace MakeIt.Colorful.Detail
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
