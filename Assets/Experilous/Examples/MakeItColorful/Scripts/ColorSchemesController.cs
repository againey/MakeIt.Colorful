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
	public class ColorSchemesController : MonoBehaviour
	{
		[Header("UI Elements")]
		public Toggle hsvToggle;
		public Toggle hcvToggle;
		public Toggle hslToggle;
		public Toggle hclToggle;
		public Toggle hsyToggle;
		public Toggle hcyToggle;

		public Slider hueSlider;
		public RawImage hueSliderBackground;
		public Image hueSliderHandle;

		public ToggleGroup templateToggleGroup;

		public Slider angleSlider;
		public Text angleValue;

		public Text vibranceLabel;
		public Slider vibranceSlider;
		public Text vibranceValue;

		public Text luminanceLabel;
		public Slider luminanceSlider;
		public Text luminanceValue;

		public RectTransform templateColorsPanel;
		public RectTransform schemeColorsPanel;

		public RectTransform colorSelectionOutline;

		private Toggle[] _templateToggles;

		private readonly List<Toggle> _templateColorToggles = new List<Toggle>();
		private readonly List<Image> _templateColorImages = new List<Image>();
		private readonly List<Toggle> _schemeColorToggles = new List<Toggle>();
		private readonly List<Image> _schemeColorImages = new List<Image>();

		private System.Action _regenerate;
		private System.Action _selectTemplate;

		private Color _baseColor;
		private int _templateIndex;

		private const float _hueToRadians = Mathf.PI * 2f;

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

		protected void Awake()
		{
			hueSliderBackground.texture = BuildHueTexture(2048);

			_templateToggles = templateToggleGroup.GetComponentsInChildren<Toggle>();

			foreach (var toggle in templateColorsPanel.GetComponentsInChildren<Toggle>())
			{
				_templateColorToggles.Add(toggle);
				_templateColorImages.Add(toggle.GetComponentInChildren<Image>());
				toggle.gameObject.SetActive(false);
			}
			foreach (var toggle in schemeColorsPanel.GetComponentsInChildren<Toggle>())
			{
				_schemeColorToggles.Add(toggle);
				_schemeColorImages.Add(toggle.GetComponentInChildren<Image>());
				toggle.gameObject.SetActive(false);
			}
		}

		protected void Start()
		{
			if (hsvToggle.isOn) ToggleHSV(true);
			//if (hcvToggle.isOn) ToggleHCV(true);
			//if (hslToggle.isOn) ToggleHSL(true);
			//if (hclToggle.isOn) ToggleHCL(true);
			//if (hsyToggle.isOn) ToggleHSY(true);
			//if (hcyToggle.isOn) ToggleHCY(true);

			hueSliderHandle.color = new ColorHSV(hueSlider.value, 1f, 1f);

			ToggleHSV(true);
			OnAngleSliderChanged(angleSlider.value);
			OnVibranceSliderChanged(vibranceSlider.value);
			OnLuminanceSliderChanged(luminanceSlider.value);
		}

		protected void Update()
		{
		}

		public void OnHueSliderChanged(float hue)
		{
			hueSliderHandle.color = new ColorHSV(hue, 1f, 1f);

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnTemplateToggle(bool isOn)
		{
			_templateIndex = _templateToggles.Length;
			if (isOn)
			{
				for (int i = 0; i < _templateToggles.Length; ++i)
				{
					if (_templateToggles[i].isOn)
					{
						_templateIndex = i;
						break;
					}
				}
			}

			if (_selectTemplate != null)
			{
				_selectTemplate();
			}

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnAngleSliderChanged(float angle)
		{
			angleValue.text = string.Format("{0:N0}°", angle);

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnVibranceSliderChanged(float vibrance)
		{
			vibranceValue.text = vibrance.ToString("N2");

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnLuminanceSliderChanged(float luminance)
		{
			luminanceValue.text = luminance.ToString("N2");

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnColorToggle(bool isOn)
		{
			if (isOn)
			{
				foreach (var toggle in _templateColorToggles)
				{
					if (toggle.isOn)
					{
						SelectColorToggle(toggle);
						return;
					}
				}

				foreach (var toggle in _schemeColorToggles)
				{
					if (toggle.isOn)
					{
						SelectColorToggle(toggle);
						return;
					}
				}

				SelectColorToggle(null);
			}
		}

		private void SelectColorToggle(Toggle toggle)
		{
			if (toggle != null)
			{
				colorSelectionOutline.gameObject.SetActive(true);
				var targetTransform = toggle.transform as RectTransform;
				colorSelectionOutline.position = targetTransform.position;
				colorSelectionOutline.sizeDelta = targetTransform.sizeDelta + new Vector2(40f, 40f);
			}
			else
			{
				colorSelectionOutline.gameObject.SetActive(false);
			}
		}

		private void SetColorCount(int count)
		{
			for (int i = _templateColorToggles.Count; i < count; ++i)
			{
				var newToggle = Instantiate(_templateColorToggles[0]);
				newToggle.isOn = false;
				newToggle.transform.SetParent(templateColorsPanel, false);
				_templateColorToggles.Add(newToggle);
				_templateColorImages.Add(newToggle.GetComponentsInChildren<Image>(true)[0]);
			}
			for (int i = _schemeColorToggles.Count; i < count; ++i)
			{
				var newToggle = Instantiate(_schemeColorToggles[0]);
				newToggle.isOn = false;
				newToggle.transform.SetParent(schemeColorsPanel, false);
				_schemeColorToggles.Add(newToggle);
				_schemeColorImages.Add(newToggle.GetComponentsInChildren<Image>(true)[0]);
			}
			for (int i = 0; i < _templateColorToggles.Count; ++i)
			{
				_templateColorToggles[i].gameObject.SetActive(i < count);
			}
			for (int i = 0; i < _schemeColorToggles.Count; ++i)
			{
				_schemeColorToggles[i].gameObject.SetActive(i < count);
			}
		}

		public void ToggleHSV(bool isOn)
		{
			if (isOn)
			{
				_selectTemplate = HSV_SelectTemplate;
				_regenerate = HSV_Regenerate;
				_selectTemplate();
				_regenerate();
			}
		}

		private ColorHSV[] _template_HSV;
		private ColorHSV[] _scheme_HSV;

		private void HSV_SelectTemplate()
		{
			switch (_templateIndex)
			{
				case 0: HSV_InitializeTemplate(180f); break;
				case 3: HSV_InitializeTemplate(30f, true); break;
				case 6: HSV_InitializeTemplate(120f, false); break;
				case 9: HSV_InitializeTemplate(30f, true); break;
				case 12: HSV_InitializeTemplate(30f, true); break;
				default: HSV_InitializeTemplate(180f); break;
			}
		}

		private void HSV_InitializeTemplate(float defaultAngle = 0f, bool variableAngle = false)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.value = defaultAngle;
			vibranceLabel.text = "Saturation:";
			vibranceSlider.value = 1f;
			luminanceLabel.text = "Value:";
			luminanceSlider.value = 1f;
		}

		private void HSV_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HSV = ColorSchemeUtility.HSV.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 3: _template_HSV = ColorSchemeUtility.HSV.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 6: _template_HSV = ColorSchemeUtility.HSV.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 9: _template_HSV = ColorSchemeUtility.HSV.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 12: _template_HSV = ColorSchemeUtility.HSV.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				default: _template_HSV = ColorSchemeUtility.HSV.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HSV == null || _scheme_HSV.Length != _template_HSV.Length)
			{
				_scheme_HSV = new ColorHSV[_template_HSV.Length];
			}

			ColorSchemeUtility.Create(hueSlider.value, _template_HSV, _scheme_HSV);

			SetColorCount(_scheme_HSV.Length);
			for (int i = 0; i < _scheme_HSV.Length; ++i)
			{
				_templateColorImages[i].color = _template_HSV[i];
				_schemeColorImages[i].color = _scheme_HSV[i];
			}
		}

		public void ToggleHSY(bool isOn)
		{
			if (isOn)
			{
				_selectTemplate = HSY_SelectTemplate;
				_regenerate = HSY_Regenerate;
				_selectTemplate();
				_regenerate();
			}
		}

		private ColorHSY[] _template_HSY;
		private ColorHSY[] _scheme_HSY;

		private void HSY_SelectTemplate()
		{
			switch (_templateIndex)
			{
				case 0: HSY_InitializeTemplate(180f); break;
				case 3: HSY_InitializeTemplate(30f, true); break;
				case 6: HSY_InitializeTemplate(120f, false); break;
				case 9: HSY_InitializeTemplate(30f, true); break;
				case 12: HSY_InitializeTemplate(30f, true); break;
				default: HSY_InitializeTemplate(180f); break;
			}
		}

		private void HSY_InitializeTemplate(float defaultAngle = 0f, bool variableAngle = false)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.value = defaultAngle;
			vibranceLabel.text = "Saturation:";
			vibranceSlider.value = 1f;
			luminanceLabel.text = "Luma:";
			luminanceSlider.value = 0.5f;
		}

		private void HSY_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HSY = ColorSchemeUtility.HSY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 3: _template_HSY = ColorSchemeUtility.HSY.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 6: _template_HSY = ColorSchemeUtility.HSY.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 9: _template_HSY = ColorSchemeUtility.HSY.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 12: _template_HSY = ColorSchemeUtility.HSY.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				default: _template_HSY = ColorSchemeUtility.HSY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HSY == null || _scheme_HSY.Length != _template_HSY.Length)
			{
				_scheme_HSY = new ColorHSY[_template_HSY.Length];
			}

			ColorSchemeUtility.Create(hueSlider.value, _template_HSY, _scheme_HSY);

			SetColorCount(_scheme_HSY.Length);
			for (int i = 0; i < _scheme_HSY.Length; ++i)
			{
				_templateColorImages[i].color = _template_HSY[i];
				_schemeColorImages[i].color = _scheme_HSY[i];
			}
		}

		public void ToggleHCY(bool isOn)
		{
			if (isOn)
			{
				_selectTemplate = HCY_SelectTemplate;
				_regenerate = HCY_Regenerate;
				_selectTemplate();
				_regenerate();
			}
		}

		private ColorHCY[] _template_HCY;
		private ColorHCY[] _scheme_HCY;

		private void HCY_SelectTemplate()
		{
			switch (_templateIndex)
			{
				case 0: HCY_InitializeTemplate(180f); break;
				case 3: HCY_InitializeTemplate(30f, true); break;
				case 6: HCY_InitializeTemplate(120f, false); break;
				case 9: HCY_InitializeTemplate(30f, true); break;
				case 12: HCY_InitializeTemplate(30f, true); break;
				default: HCY_InitializeTemplate(180f); break;
			}
		}

		private void HCY_InitializeTemplate(float defaultAngle = 0f, bool variableAngle = false)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.value = defaultAngle;
			vibranceLabel.text = "Chroma:";
			vibranceSlider.value = 1f;
			luminanceLabel.text = "Luma:";
			luminanceSlider.value = 0.5f;
		}

		private void HCY_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HCY = ColorSchemeUtility.HCY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 3: _template_HCY = ColorSchemeUtility.HCY.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 6: _template_HCY = ColorSchemeUtility.HCY.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 9: _template_HCY = ColorSchemeUtility.HCY.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 12: _template_HCY = ColorSchemeUtility.HCY.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				default: _template_HCY = ColorSchemeUtility.HCY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HCY == null || _scheme_HCY.Length != _template_HCY.Length)
			{
				_scheme_HCY = new ColorHCY[_template_HCY.Length];
			}

			ColorSchemeUtility.Create(hueSlider.value, _template_HCY, _scheme_HCY);

			SetColorCount(_scheme_HCY.Length);
			for (int i = 0; i < _scheme_HCY.Length; ++i)
			{
				_templateColorImages[i].color = _template_HCY[i];
				_schemeColorImages[i].color = _scheme_HCY[i];
			}
		}
	}
}
