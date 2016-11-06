/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Experilous.Examples.MakeItColorful
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
