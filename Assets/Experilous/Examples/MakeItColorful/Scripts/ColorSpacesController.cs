/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using Experilous.Colors;

namespace Experilous.Examples.MakeItColorful
{
	public class ColorSpacesController : MonoBehaviour
	{
		public RawImage colorSpaceSliceImage;

		public int imageWidth = 1024;
		public int imageHeight = 1024;

		private Color[] _colorSpaceSliceColors;
		private Texture2D _colorSpaceSliceTexture;

		protected void Awake()
		{
			_colorSpaceSliceColors = new Color[imageWidth * imageHeight];
			_colorSpaceSliceTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, true);
			colorSpaceSliceImage.texture = _colorSpaceSliceTexture;
		}

		protected void Start()
		{
			CalculateHueSliceHCY(0f, imageWidth, imageHeight, _colorSpaceSliceColors);

			_colorSpaceSliceTexture.SetPixels(_colorSpaceSliceColors);
			_colorSpaceSliceTexture.Apply();

		}

		private static void CalculateHueSliceHCY(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + Mathf.PI, Mathf.PI * 2f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = (height - y) / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hcy = new ColorHCY(hue, fx, fy);
						hcy.a = hcy.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcy;
					}
					else
					{
						var hcy = new ColorHCY(complementaryHue, -fx, fy);
						hcy.a = hcy.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcy;
					}
				}
			}
		}
	}
}
