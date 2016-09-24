/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Experilous.MakeItColorful;

namespace Experilous.Examples.MakeItColorful
{
	public class ColorSpacesController : MonoBehaviour
	{
		public int visualizationWidth = 1024;
		public int visualizationHeight = 1024;
		public float updateSliceTextureDelay = 0.1f;
		public float updateLerpGradientTextureDelay = 0.01f;

		[Header("UI Elements")]
		public Toggle rgbToggle;
		public Toggle cmyToggle;
		public Toggle cmykToggle;
		public Toggle hsvToggle;
		public Toggle hcvToggle;
		public Toggle hslToggle;
		public Toggle hclToggle;
		public Toggle hsyToggle;
		public Toggle hcyToggle;

		public Slider hueSlider;
		public RawImage hueSliderBackground;
		public Image hueSliderHandle;

		public RectTransform rgbPanel;
		public Slider rgbRedSlider;
		public Slider rgbGreenSlider;
		public Slider rgbBlueSlider;

		public RectTransform cmyPanel;
		public Slider cmyCyanSlider;
		public Slider cmyMagentaSlider;
		public Slider cmyYellowSlider;

		public RectTransform cmykPanel;
		public Slider cmykCyanSlider;
		public Slider cmykMagentaSlider;
		public Slider cmykYellowSlider;
		public Slider cmykKeySlider;

		public RawImage fullVisualizationImage;

		public RawImage sliceVisualizationImage;
		public Text sliceHorizontalAxisLabel;
		public Text sliceVerticalAxisLabel;

		public Toggle lerpSourceToggle;
		public Image lerpSourceToggleImage;

		public Toggle lerpTargetToggle;
		public Image lerpTargetToggleImage;

		public RawImage lerpGradientImage;

		public Toggle lerpNearestToggle;
		public Toggle lerpForwardToggle;
		public Toggle lerpBackwardToggle;

		private System.Action<float> _onHueChanged;
		private System.Action<Color> _onActiveColorChanged;
		private System.Func<float, float, Color> _getSliceColor;
		private System.Action _updateLerpGradientTexture;

		private int _inScript = 0;
		private float _updateSliceTextureQueue = float.NaN;
		private float _updateLerpGradientTextureQueue = float.NaN;

		private Color[] _sliceColors;
		private Texture2D _sliceTexture;
		private Texture2D _lerpGradientTexture;

		private Toggle _rgbRedSliceToggle;
		private Toggle _rgbGreenSliceToggle;
		private Toggle _rgbBlueSliceToggle;

		private Toggle _cmyCyanSliceToggle;
		private Toggle _cmyMagentaSliceToggle;
		private Toggle _cmyYellowSliceToggle;

		private Toggle _cmykCyanSliceToggle;
		private Toggle _cmykMagentaSliceToggle;
		private Toggle _cmykYellowSliceToggle;

		private Color _baseColor;

		private Color activeColor
		{
			get
			{
				if (lerpSourceToggle.isOn)
				{
					return lerpSourceToggleImage.color;
				}
				else if (lerpTargetToggle.isOn)
				{
					return lerpTargetToggleImage.color;
				}
				else
				{
					return Color.clear;
				}
			}
		}

		private static void SetSpectrumTexel(Texture2D texture, float spectrumIndex, Color color)
		{
			int texelIndex = Mathf.RoundToInt(spectrumIndex * (texture.width - 1));
			texture.SetPixel(texelIndex, 0, color);
			texture.SetPixel(texelIndex, 1, color);
		}

		private static void BuildHueTextureSegment(Texture2D texture, int iStart, int iEnd, System.Func<float, Color> getColor)
		{
			float fullRange = texture.width - 1;
			float segmentRange = iEnd - iStart;
			for (int i = iStart; i < iEnd; ++i)
			{
				SetSpectrumTexel(texture, i / fullRange, getColor((i - iStart) / segmentRange));
			}
		}

		private static Texture2D BuildHueTexture(int width)
		{
			Texture2D texture = new Texture2D(width, 2, TextureFormat.RGBAFloat, false);
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;

			int i = 0;
			BuildHueTextureSegment(texture, i, i = width * 1 / 6, (float t) => new Color(1f, t, 0f));
			BuildHueTextureSegment(texture, i, i = width * 2 / 6, (float t) => new Color(1f - t, 1f, 0f));
			BuildHueTextureSegment(texture, i, i = width * 3 / 6, (float t) => new Color(0f, 1f, t));
			BuildHueTextureSegment(texture, i, i = width * 4 / 6, (float t) => new Color(0f, 1f - t, 1f));
			BuildHueTextureSegment(texture, i, i = width * 5 / 6, (float t) => new Color(t, 0f, 1f));
			BuildHueTextureSegment(texture, i, i = width * 6 / 6, (float t) => new Color(1f, 0f, 1f - t));
			texture.Apply(true, true);

			return texture;
		}

		public static Texture2D BuildLerpTexture(int width, bool mipmap)
		{
			Texture2D texture = new Texture2D(width, 2, TextureFormat.RGBAFloat, mipmap);
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
			return texture;
		}

		public static Texture2D BuildLerpTexture(int width, Color first, Color second)
		{
			var texture = BuildLerpTexture(width, true);

			float fullRange = texture.width - 1;
			for (int i = 0; i < width; ++i)
			{
				float t = i / fullRange;
				SetSpectrumTexel(texture, t, Color.Lerp(first, second, t));
			}

			texture.Apply(true, true);

			return texture;
		}

		public static void UpdateLerpTexture<TColor>(Texture2D texture, TColor first, TColor second, System.Func<TColor, TColor, float, Color> lerp)
		{
			float fullRange = texture.width - 1;
			for (int i = 0; i < texture.width; ++i)
			{
				float t = i / fullRange;
				SetSpectrumTexel(texture, t, lerp(first, second, t));
			}

			texture.Apply(false, false);
		}

		private static void BuildSliceTextureColors(int width, int height, Color[] colors, System.Func<float, float, Color> getColor)
		{
			float rx = width - 1f;
			float ry = height - 1f;

			for (int y = 0, i = 0; y < height; ++y)
			{
				float fy = y / ry;
				for (int x = 0; x < width; ++x, ++i)
				{
					float fx = x / rx;
					colors[i] = getColor(fx, fy);
				}
			}
		}

		private void UpdateSliceTexture(System.Func<float, float, Color> getColor)
		{
			BuildSliceTextureColors(visualizationWidth, visualizationHeight, _sliceColors, _getSliceColor);

			_sliceTexture.SetPixels(_sliceColors);
			_sliceTexture.Apply();
		}

		private void QueueUpdateSliceTexture(System.Func<float, float, Color> getColor)
		{
			if (getColor == null) return;
			_getSliceColor = getColor;
			if (float.IsNaN(_updateSliceTextureQueue))
			{
				_updateSliceTextureQueue = updateSliceTextureDelay;
			}
		}

		private void QueueUpdateLerpGradientTexture(System.Action updateLerpGradientTexture)
		{
			if (updateLerpGradientTexture == null) return;
			_updateLerpGradientTexture = updateLerpGradientTexture;
			if (float.IsNaN(_updateLerpGradientTextureQueue))
			{
				_updateLerpGradientTextureQueue = updateLerpGradientTextureDelay;
			}
		}

		private void SetSliceAxisLabels(string horizontalAxis, string verticalAxis)
		{
			sliceHorizontalAxisLabel.text = horizontalAxis;
			sliceVerticalAxisLabel.text = verticalAxis;
		}

		private void ExecuteFromScript(System.Action action)
		{
			if (_inScript > 0) return;
			++_inScript;
			try
			{
				action();
			}
			finally
			{
				--_inScript;
			}
		}

		protected void Awake()
		{
			hueSliderBackground.texture = BuildHueTexture(2048);

			_sliceColors = new Color[visualizationWidth * visualizationHeight];
			_sliceTexture = new Texture2D(visualizationWidth, visualizationHeight, TextureFormat.ARGB32, true);
			_sliceTexture.filterMode = FilterMode.Bilinear;
			_sliceTexture.wrapMode = TextureWrapMode.Clamp;
			sliceVisualizationImage.texture = _sliceTexture;

			_lerpGradientTexture = BuildLerpTexture(2048, false);
			lerpGradientImage.texture = _lerpGradientTexture;

			_rgbRedSliceToggle = rgbRedSlider.GetComponent<ColorGradientSlider>().slice;
			_rgbGreenSliceToggle = rgbGreenSlider.GetComponent<ColorGradientSlider>().slice;
			_rgbBlueSliceToggle = rgbBlueSlider.GetComponent<ColorGradientSlider>().slice;

			_cmyCyanSliceToggle = cmyCyanSlider.GetComponent<ColorGradientSlider>().slice;
			_cmyMagentaSliceToggle = cmyMagentaSlider.GetComponent<ColorGradientSlider>().slice;
			_cmyYellowSliceToggle = cmyYellowSlider.GetComponent<ColorGradientSlider>().slice;

			_cmykCyanSliceToggle = cmykCyanSlider.GetComponent<ColorGradientSlider>().slice;
			_cmykMagentaSliceToggle = cmykMagentaSlider.GetComponent<ColorGradientSlider>().slice;
			_cmykYellowSliceToggle = cmykYellowSlider.GetComponent<ColorGradientSlider>().slice;
		}

		protected void Start()
		{
			if (rgbToggle.isOn) ToggleRGB(true);
			if (cmyToggle.isOn) ToggleCMY(true);
			if (cmykToggle.isOn) ToggleCMYK(true);
			if (hsvToggle.isOn) ToggleHSV(true);
			if (hcvToggle.isOn) ToggleHCV(true);
			if (hslToggle.isOn) ToggleHSL(true);
			if (hclToggle.isOn) ToggleHCL(true);
			if (hsyToggle.isOn) ToggleHSY(true);
			if (hcyToggle.isOn) ToggleHCY(true);

			SetActiveColor((Color)new ColorHSV(hueSlider.value, 1f, 1f));
			hueSliderHandle.color = activeColor;

			_updateSliceTextureQueue = 0f;
			_updateLerpGradientTextureQueue = 0f;
		}

		protected void Update()
		{
			if (!float.IsNaN(_updateSliceTextureQueue))
			{
				_updateSliceTextureQueue -= Time.deltaTime;
				if (_updateSliceTextureQueue <= 0f)
				{
					UpdateSliceTexture(_getSliceColor);
					_updateSliceTextureQueue = float.NaN;
				}
			}

			if (!float.IsNaN(_updateLerpGradientTextureQueue))
			{
				_updateLerpGradientTextureQueue -= Time.deltaTime;
				if (_updateLerpGradientTextureQueue <= 0f)
				{
					_updateLerpGradientTexture();
					_updateLerpGradientTextureQueue = float.NaN;
				}
			}
		}

		public void OnHueSliderChanged(float hue)
		{
			hueSliderHandle.color = (Color)(new ColorHSV(hue, 1f, 1f));

			if (_onHueChanged != null)
			{
				_onHueChanged(hue);
			}
		}

		private void UpdateHueSlider(Color color)
		{
			ExecuteFromScript(() =>
			{
				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;
			});
		}

		private void UpdateHueSlice(Color color)
		{
			QueueUpdateSliceTexture(_getSliceColor);
			UpdateHueSlider(color);
		}

		private void EnableHue(bool enabled)
		{
			lerpNearestToggle.gameObject.SetActive(enabled);
			lerpForwardToggle.gameObject.SetActive(enabled);
			lerpBackwardToggle.gameObject.SetActive(enabled);
		}

		public void EnableRGBPanel()
		{
			rgbPanel.gameObject.SetActive(true);
			cmyPanel.gameObject.SetActive(false);
			cmykPanel.gameObject.SetActive(false);
			fullVisualizationImage.gameObject.SetActive(false);
		}

		public void EnableCMYPanel()
		{
			rgbPanel.gameObject.SetActive(false);
			cmyPanel.gameObject.SetActive(true);
			cmykPanel.gameObject.SetActive(false);
			fullVisualizationImage.gameObject.SetActive(false);
		}

		public void EnableCMYKPanel()
		{
			rgbPanel.gameObject.SetActive(false);
			cmyPanel.gameObject.SetActive(false);
			cmykPanel.gameObject.SetActive(true);
			fullVisualizationImage.gameObject.SetActive(false);
		}

		public void EnableFullVisualizationImage()
		{
			rgbPanel.gameObject.SetActive(false);
			cmyPanel.gameObject.SetActive(false);
			cmykPanel.gameObject.SetActive(false);
			fullVisualizationImage.gameObject.SetActive(true);
		}

		public void ToggleRGB(bool isOn)
		{
			if (isOn)
			{
				EnableHue(false);
				EnableRGBPanel();

				_onHueChanged = RGB_OnHueChanged;
				_onActiveColorChanged = UpdateRgbSliders;
				SetRgbSlice();
				_updateLerpGradientTexture = RGB_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void SetRgbSlice()
		{
			if (_rgbRedSliceToggle.isOn)
			{
				float r = rgbRedSlider.value;
				_getSliceColor = (float g, float b) => new Color(r, g, b).GetNearestValid();
				SetSliceAxisLabels("Green", "Blue");
			}
			else if (_rgbGreenSliceToggle.isOn)
			{
				float g = rgbGreenSlider.value;
				_getSliceColor = (float r, float b) => new Color(r, g, b).GetNearestValid();
				SetSliceAxisLabels("Red", "Blue");
			}
			else if (_rgbBlueSliceToggle.isOn)
			{
				float b = rgbBlueSlider.value;
				_getSliceColor = (float r, float g) => new Color(r, g, b).GetNearestValid();
				SetSliceAxisLabels("Red", "Green");
			}
		}

		private void UpdateRgbSliders(Color color)
		{
			ExecuteFromScript(() =>
			{
				rgbRedSlider.value = color.r;
				rgbGreenSlider.value = color.g;
				rgbBlueSlider.value = color.b;

				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetRgbSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		public void OnRgbComponentSliceChanged(bool isOn)
		{
			if (isOn)
			{
				SetRgbSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			}
		}

		private void RGB_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var color = (Color)new ColorHSV(hue, 1f, 1f);
				SetActiveColor(color);

				rgbRedSlider.value = color.r;
				rgbGreenSlider.value = color.g;
				rgbBlueSlider.value = color.b;

				SetRgbSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		public void OnRgbComponentSliderChanged()
		{
			ExecuteFromScript(() =>
			{
				var color = new Color(rgbRedSlider.value, rgbGreenSlider.value, rgbBlueSlider.value);
				SetActiveColor(color);
				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetRgbSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void RGB_BuildLerpGradient()
		{
			UpdateLerpTexture(_lerpGradientTexture, lerpSourceToggleImage.color, lerpTargetToggleImage.color, Color.Lerp);
		}

		public void ToggleCMY(bool isOn)
		{
			if (isOn)
			{
				EnableHue(false);
				EnableCMYPanel();

				_onHueChanged = CMY_OnHueChanged;
				_onActiveColorChanged = UpdateCmySliders;
				SetCmySlice();
				_updateLerpGradientTexture = CMY_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void SetCmySlice()
		{
			if (_cmyCyanSliceToggle.isOn)
			{
				float c = cmyCyanSlider.value;
				_getSliceColor = (float m, float y) => (Color)new ColorCMY(c, m, y).GetNearestValid();
				SetSliceAxisLabels("Magenta", "Yellow");
			}
			else if (_cmyMagentaSliceToggle.isOn)
			{
				float m = cmyMagentaSlider.value;
				_getSliceColor = (float c, float y) => (Color)new ColorCMY(c, m, y).GetNearestValid();
				SetSliceAxisLabels("Cyan", "Yellow");
			}
			else if (_cmyYellowSliceToggle.isOn)
			{
				float y = cmyYellowSlider.value;
				_getSliceColor = (float c, float m) => (Color)new ColorCMY(c, m, y).GetNearestValid();
				SetSliceAxisLabels("Cyan", "Magenta");
			}
		}

		private void UpdateCmySliders(Color color)
		{
			ExecuteFromScript(() =>
			{
				var cmy = (ColorCMY)color;
				cmyCyanSlider.value = cmy.c;
				cmyMagentaSlider.value = cmy.m;
				cmyYellowSlider.value = cmy.y;

				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetCmySlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		public void OnCmyComponentSliceChanged(bool isOn)
		{
			if (isOn)
			{
				SetCmySlice();
				QueueUpdateSliceTexture(_getSliceColor);
			}
		}

		private void CMY_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var color = (ColorCMY)new ColorHSV(hue, 1f, 1f);
				SetActiveColor((Color)color);

				cmyCyanSlider.value = color.c;
				cmyMagentaSlider.value = color.m;
				cmyYellowSlider.value = color.y;

				SetCmySlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		public void OnCmyComponentSliderChanged()
		{
			ExecuteFromScript(() =>
			{
				var color = new ColorCMY(cmyCyanSlider.value, cmyMagentaSlider.value, cmyYellowSlider.value);
				SetActiveColor((Color)color);
				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetCmySlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void CMY_BuildLerpGradient()
		{
			System.Func<ColorCMY, ColorCMY, float, Color> lerp = (ColorCMY source, ColorCMY target, float t) => (Color)ColorCMY.Lerp(source, target, t);
			UpdateLerpTexture(_lerpGradientTexture, (ColorCMY)lerpSourceToggleImage.color, (ColorCMY)lerpTargetToggleImage.color, lerp);
		}

		public void ToggleCMYK(bool isOn)
		{
			if (isOn)
			{
				EnableHue(false);
				EnableCMYKPanel();

				_onHueChanged = CMYK_OnHueChanged;
				_onActiveColorChanged = UpdateCmykSliders;
				SetCmykSlice();
				_updateLerpGradientTexture = CMYK_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void SetCmykSlice()
		{
			var color = (ColorCMY)new ColorCMYK(cmykCyanSlider.value, cmykMagentaSlider.value, cmykYellowSlider.value, cmykKeySlider.value);
			if (_cmykCyanSliceToggle.isOn)
			{
				float c = color.c;
				_getSliceColor = (float m, float y) => (Color)new ColorCMY(c, m, y).GetNearestValid();
				SetSliceAxisLabels("Magenta", "Yellow");
			}
			else if (_cmykMagentaSliceToggle.isOn)
			{
				float m = color.m;
				_getSliceColor = (float c, float y) => (Color)new ColorCMY(c, m, y).GetNearestValid();
				SetSliceAxisLabels("Cyan", "Yellow");
			}
			else if (_cmykYellowSliceToggle.isOn)
			{
				float y = color.y;
				_getSliceColor = (float c, float m) => (Color)new ColorCMY(c, m, y).GetNearestValid();
				SetSliceAxisLabels("Cyan", "Magenta");
			}
		}

		private void UpdateCmykSliders(Color color)
		{
			ExecuteFromScript(() =>
			{
				var cmyk = (ColorCMYK)color;
				cmykCyanSlider.value = cmyk.c;
				cmykMagentaSlider.value = cmyk.m;
				cmykYellowSlider.value = cmyk.y;
				cmykKeySlider.value = cmyk.k;

				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetCmykSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		public void OnCmykComponentSliceChanged(bool isOn)
		{
			if (isOn)
			{
				SetCmykSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			}
		}

		private void CMYK_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var color = (ColorCMYK)new ColorHSV(hue, 1f, 1f);
				SetActiveColor((Color)color);

				cmykCyanSlider.value = color.c;
				cmykMagentaSlider.value = color.m;
				cmykYellowSlider.value = color.y;
				cmykKeySlider.value = color.k;

				SetCmykSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		public void OnCmykComponentSliderChanged()
		{
			ExecuteFromScript(() =>
			{
				var color = new ColorCMYK(cmykCyanSlider.value, cmykMagentaSlider.value, cmykYellowSlider.value, cmykKeySlider.value);
				SetActiveColor((Color)color);
				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetCmykSlice();
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void CMYK_BuildLerpGradient()
		{
			System.Func<ColorCMYK, ColorCMYK, float, Color> lerp = (ColorCMYK source, ColorCMYK target, float t) => (Color)ColorCMYK.Lerp(source, target, t);
			UpdateLerpTexture(_lerpGradientTexture, (ColorCMYK)lerpSourceToggleImage.color, (ColorCMYK)lerpTargetToggleImage.color, lerp);
		}

		public void ToggleHSV(bool isOn)
		{
			if (isOn)
			{
				EnableHue(true);
				EnableFullVisualizationImage();

				_onHueChanged = HSV_OnHueChanged;
				_onActiveColorChanged = UpdateHueSlice;
				_getSliceColor = (float s, float v) => (Color)new ColorHSV(hueSlider.value, s, v).GetNearestValid();
				SetSliceAxisLabels("Saturation", "Value");
				_updateLerpGradientTexture = HSV_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void HSV_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hsv = (ColorHSV)activeColor;
				hsv.h = hue;
				SetActiveColor((Color)hsv);
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void HSV_BuildLerpGradient()
		{
			System.Func<ColorHSV, ColorHSV, float, Color> lerp = null;
			if (lerpNearestToggle.isOn)
			{
				lerp = (ColorHSV source, ColorHSV target, float t) => (Color)ColorHSV.Lerp(source, target, t);
			}
			else if (lerpForwardToggle.isOn)
			{
				lerp = (ColorHSV source, ColorHSV target, float t) => (Color)ColorHSV.LerpForward(source, target, t);
			}
			else if (lerpBackwardToggle.isOn)
			{
				lerp = (ColorHSV source, ColorHSV target, float t) => (Color)ColorHSV.LerpBackward(source, target, t);
			}
			if (lerp != null)
			{
				UpdateLerpTexture(_lerpGradientTexture, (ColorHSV)lerpSourceToggleImage.color, (ColorHSV)lerpTargetToggleImage.color, lerp);
			}
		}

		public void ToggleHCV(bool isOn)
		{
			if (isOn)
			{
				EnableHue(true);
				EnableFullVisualizationImage();

				_onHueChanged = HCV_OnHueChanged;
				_onActiveColorChanged = UpdateHueSlice;
				_getSliceColor = (float c, float v) =>
				{
					var color = new ColorHCV(hueSlider.value, c, v);
					if (color.IsValid()) return (Color)color;
					color.a = 0f;
					return (Color)color.GetNearestValid();
				};
				SetSliceAxisLabels("Chroma", "Value");
				_updateLerpGradientTexture = HCV_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void HCV_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hcv = (ColorHCV)activeColor;
				hcv.h = hue;
				SetActiveColor((Color)hcv);
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void HCV_BuildLerpGradient()
		{
			System.Func<ColorHCV, ColorHCV, float, Color> lerp = null;
			if (lerpNearestToggle.isOn)
			{
				lerp = (ColorHCV source, ColorHCV target, float t) => (Color)ColorHCV.Lerp(source, target, t);
			}
			else if (lerpForwardToggle.isOn)
			{
				lerp = (ColorHCV source, ColorHCV target, float t) => (Color)ColorHCV.LerpForward(source, target, t);
			}
			else if (lerpBackwardToggle.isOn)
			{
				lerp = (ColorHCV source, ColorHCV target, float t) => (Color)ColorHCV.LerpBackward(source, target, t);
			}
			if (lerp != null)
			{
				UpdateLerpTexture(_lerpGradientTexture, (ColorHCV)lerpSourceToggleImage.color, (ColorHCV)lerpTargetToggleImage.color, lerp);
			}
		}

		public void ToggleHSL(bool isOn)
		{
			if (isOn)
			{
				EnableHue(true);
				EnableFullVisualizationImage();

				_onHueChanged = HSL_OnHueChanged;
				_onActiveColorChanged = UpdateHueSlice;
				_getSliceColor = (float s, float l) => (Color)new ColorHSL(hueSlider.value, s, l).GetNearestValid();
				SetSliceAxisLabels("Saturation", "Lightness");
				_updateLerpGradientTexture = HSL_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void HSL_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hsl = (ColorHSL)activeColor;
				hsl.h = hue;
				SetActiveColor((Color)hsl);
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void HSL_BuildLerpGradient()
		{
			System.Func<ColorHSL, ColorHSL, float, Color> lerp = null;
			if (lerpNearestToggle.isOn)
			{
				lerp = (ColorHSL source, ColorHSL target, float t) => (Color)ColorHSL.Lerp(source, target, t);
			}
			else if (lerpForwardToggle.isOn)
			{
				lerp = (ColorHSL source, ColorHSL target, float t) => (Color)ColorHSL.LerpForward(source, target, t);
			}
			else if (lerpBackwardToggle.isOn)
			{
				lerp = (ColorHSL source, ColorHSL target, float t) => (Color)ColorHSL.LerpBackward(source, target, t);
			}
			if (lerp != null)
			{
				UpdateLerpTexture(_lerpGradientTexture, (ColorHSL)lerpSourceToggleImage.color, (ColorHSL)lerpTargetToggleImage.color, lerp);
			}
		}

		public void ToggleHCL(bool isOn)
		{
			if (isOn)
			{
				EnableHue(true);
				EnableFullVisualizationImage();

				_onHueChanged = HCL_OnHueChanged;
				_onActiveColorChanged = UpdateHueSlice;
				_getSliceColor = (float c, float l) =>
				{
					var color = new ColorHCL(hueSlider.value, c, l);
					if (color.IsValid()) return (Color)color;
					color.a = 0f;
					return (Color)color.GetNearestValid();
				};
				SetSliceAxisLabels("Chroma", "Lightness");
				_updateLerpGradientTexture = HCL_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void HCL_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hcl = (ColorHCL)activeColor;
				hcl.h = hue;
				SetActiveColor((Color)hcl);
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void HCL_BuildLerpGradient()
		{
			System.Func<ColorHCL, ColorHCL, float, Color> lerp = null;
			if (lerpNearestToggle.isOn)
			{
				lerp = (ColorHCL source, ColorHCL target, float t) => (Color)ColorHCL.Lerp(source, target, t);
			}
			else if (lerpForwardToggle.isOn)
			{
				lerp = (ColorHCL source, ColorHCL target, float t) => (Color)ColorHCL.LerpForward(source, target, t);
			}
			else if (lerpBackwardToggle.isOn)
			{
				lerp = (ColorHCL source, ColorHCL target, float t) => (Color)ColorHCL.LerpBackward(source, target, t);
			}
			if (lerp != null)
			{
				UpdateLerpTexture(_lerpGradientTexture, (ColorHCL)lerpSourceToggleImage.color, (ColorHCL)lerpTargetToggleImage.color, lerp);
			}
		}

		public void ToggleHSY(bool isOn)
		{
			if (isOn)
			{
				EnableHue(true);
				EnableFullVisualizationImage();

				_onHueChanged = HSY_OnHueChanged;
				_onActiveColorChanged = UpdateHueSlice;
				_getSliceColor = (float s, float y) => (Color)new ColorHSY(hueSlider.value, s, y).GetNearestValid();
				SetSliceAxisLabels("Saturation", "Luma");
				_updateLerpGradientTexture = HSY_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void HSY_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hsy = (ColorHSY)activeColor;
				hsy.h = hue;
				SetActiveColor((Color)hsy);
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void HSY_BuildLerpGradient()
		{
			System.Func<ColorHSY, ColorHSY, float, Color> lerp = null;
			if (lerpNearestToggle.isOn)
			{
				lerp = (ColorHSY source, ColorHSY target, float t) => (Color)ColorHSY.Lerp(source, target, t);
			}
			else if (lerpForwardToggle.isOn)
			{
				lerp = (ColorHSY source, ColorHSY target, float t) => (Color)ColorHSY.LerpForward(source, target, t);
			}
			else if (lerpBackwardToggle.isOn)
			{
				lerp = (ColorHSY source, ColorHSY target, float t) => (Color)ColorHSY.LerpBackward(source, target, t);
			}
			if (lerp != null)
			{
				UpdateLerpTexture(_lerpGradientTexture, (ColorHSY)lerpSourceToggleImage.color, (ColorHSY)lerpTargetToggleImage.color, lerp);
			}
		}

		public void ToggleHCY(bool isOn)
		{
			if (isOn)
			{
				EnableHue(true);
				EnableFullVisualizationImage();

				_onHueChanged = HCY_OnHueChanged;
				_onActiveColorChanged = UpdateHueSlice;
				_getSliceColor = (float c, float y) =>
				{
					var color = new ColorHCY(hueSlider.value, c, y);
					if (color.IsValid()) return (Color)color;
					color.a = 0f;
					return (Color)color.GetNearestValid();
				};
				SetSliceAxisLabels("Chroma", "Luma");
				_updateLerpGradientTexture = HCY_BuildLerpGradient;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
				_onActiveColorChanged(activeColor);
			}
		}

		private void HCY_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hcy = (ColorHCY)_baseColor;
				hcy.h = hue;
				SetActiveColor((Color)hcy, false);
				QueueUpdateSliceTexture(_getSliceColor);
			});
		}

		private void HCY_BuildLerpGradient()
		{
			System.Func<ColorHCY, ColorHCY, float, Color> lerp = null;
			if (lerpNearestToggle.isOn)
			{
				lerp = (ColorHCY source, ColorHCY target, float t) => (Color)ColorHCY.Lerp(source, target, t);
			}
			else if (lerpForwardToggle.isOn)
			{
				lerp = (ColorHCY source, ColorHCY target, float t) => (Color)ColorHCY.LerpForward(source, target, t);
			}
			else if (lerpBackwardToggle.isOn)
			{
				lerp = (ColorHCY source, ColorHCY target, float t) => (Color)ColorHCY.LerpBackward(source, target, t);
			}
			if (lerp != null)
			{
				UpdateLerpTexture(_lerpGradientTexture, (ColorHCY)lerpSourceToggleImage.color, (ColorHCY)lerpTargetToggleImage.color, lerp);
			}
		}

		public void OnSliceClicked(BaseEventData ev)
		{
			var ptr = (PointerEventData)ev;
			var rectTransform = sliceVisualizationImage.rectTransform;
			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, ptr.position, ptr.pressEventCamera, out position);
			float x = position.x / (rectTransform.rect.width - 1);
			float y = position.y / (rectTransform.rect.height - 1);
			var color = _getSliceColor(x, y);
			color.a = 1f;
			SetActiveColor(color);
		}

		private void SetActiveColor(Color color, bool updateBaseColor = true)
		{
			if (lerpSourceToggle.isOn)
			{
				lerpSourceToggleImage.color = color;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
			}
			else if (lerpTargetToggle.isOn)
			{
				lerpTargetToggleImage.color = color;
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
			}

			if (updateBaseColor) _baseColor = color;

			if (_onActiveColorChanged != null) _onActiveColorChanged(activeColor);
		}

		public void OnLerpHueToggleChanged(bool isOn)
		{
			if (isOn)
			{
				QueueUpdateLerpGradientTexture(_updateLerpGradientTexture);
			}
		}

		public void OnLerpEndpointToggleChanged(Toggle toggle)
		{
			if (toggle.isOn)
			{
				if (toggle == lerpSourceToggle)
				{
					SetActiveColor(lerpSourceToggleImage.color);
				}
				else if (toggle == lerpTargetToggle)
				{
					SetActiveColor(lerpTargetToggleImage.color);
				}
			}
		}
	}
}
