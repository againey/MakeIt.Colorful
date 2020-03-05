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
using UnityEngine.UI;

namespace MakeIt.Colorful.Samples
{
	[RequireComponent(typeof(Slider))]
	public class ColorGradientSlider : MonoBehaviour
	{
		public Color source;
		public Color target;

		public RawImage background;
		public Image handle;
		public Toggle slice;

		protected void Awake()
		{
			background.texture = ColorSpacesController.BuildLerpTexture(1024, source, target);
		}

		protected void Start()
		{
			OnSliderChanged(GetComponent<Slider>().value);
		}

		public void OnSliderChanged(float t)
		{
			handle.color = Color.Lerp(source, target, t);
		}
	}
}
