/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using Experilous.MakeItColorful;

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
			CalculateHueSliceHSY(0f, imageWidth, imageHeight, _colorSpaceSliceColors);

			_colorSpaceSliceTexture.SetPixels(_colorSpaceSliceColors);
			_colorSpaceSliceTexture.Apply();

		}

		private static void CalculateHueSliceHSV(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + 0.5f, 1f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hsv = new ColorHSV(hue, -fx, fy);
						hsv.a = 1f;
						colors[i] = (Color)hsv;
					}
					else
					{
						var hsv = new ColorHSV(complementaryHue, fx, fy);
						hsv.a = 1f;
						colors[i] = (Color)hsv;
					}
				}
			}
		}

		private static void CalculateHueSliceHCV(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + 0.5f, 1f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hcv = new ColorHCV(hue, -fx, fy);
						hcv.a = hcv.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcv;
					}
					else
					{
						var hcv = new ColorHCV(complementaryHue, fx, fy);
						hcv.a = hcv.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcv;
					}
				}
			}
		}

		private static void CalculateHueSliceHSL(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + 0.5f, 1f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hsl = new ColorHSL(hue, -fx, fy);
						hsl.a = 1f;
						colors[i] = (Color)hsl;
					}
					else
					{
						var hsl = new ColorHSL(complementaryHue, fx, fy);
						hsl.a = 1f;
						colors[i] = (Color)hsl;
					}
				}
			}
		}

		private static void CalculateHueSliceHCL(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + 0.5f, 1f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hcl = new ColorHCL(hue, -fx, fy);
						hcl.a = hcl.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcl;
					}
					else
					{
						var hcl = new ColorHCL(complementaryHue, fx, fy);
						hcl.a = hcl.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcl;
					}
				}
			}
		}

		private static void CalculateHueSliceHSY(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + 0.5f, 1f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hsy = new ColorHSY(hue, -fx, fy);
						hsy.a = 1f;
						colors[i] = (Color)hsy;
					}
					else
					{
						var hsy = new ColorHSY(complementaryHue, fx, fy);
						hsy.a = 1f;
						colors[i] = (Color)hsy;
					}
				}
			}
		}

		private static void CalculateHueSliceHCY(float hue, int width, int height, Color[] colors)
		{
			float rx = width / 2 - 1f;
			float ry = height - 1f;

			float complementaryHue = Mathf.Repeat(hue + 0.5f, 1f);

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = (x - rx) / rx;
					if (fx < 0f)
					{
						var hcy = new ColorHCY(hue, -fx, fy);
						hcy.a = hcy.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcy;
					}
					else
					{
						var hcy = new ColorHCY(complementaryHue, fx, fy);
						hcy.a = hcy.canConvertToRGB ? 1f : 0f;
						colors[i] = (Color)hcy;
					}
				}
			}
		}
	}
}
