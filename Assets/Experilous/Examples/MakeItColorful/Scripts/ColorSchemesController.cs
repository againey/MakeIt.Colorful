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

		public Toggle biasToggle;
		public Text biasLabel;
		public Slider biasSlider;
		public Text biasValue;

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
			if (hcvToggle.isOn) ToggleHCV(true);
			if (hslToggle.isOn) ToggleHSL(true);
			if (hclToggle.isOn) ToggleHCL(true);
			if (hsyToggle.isOn) ToggleHSY(true);
			if (hcyToggle.isOn) ToggleHCY(true);

			hueSliderHandle.color = new ColorHSV(hueSlider.value, 1f, 1f);

			ToggleHSV(true);
			OnAngleSliderChanged(angleSlider.value);
			OnVibranceSliderChanged(vibranceSlider.value);
			OnLuminanceSliderChanged(luminanceSlider.value);
			OnBiasToggleChanged(biasToggle.isOn);
			OnBiasSliderChanged(biasSlider.value);
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
			angleValue.text = string.Format("{0:F0}°", angle);

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnVibranceSliderChanged(float vibrance)
		{
			vibranceValue.text = vibrance.ToString("F2");

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnLuminanceSliderChanged(float luminance)
		{
			luminanceValue.text = luminance.ToString("F2");

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnBiasToggleChanged(bool isOn)
		{
			if (isOn)
			{
				biasSlider.interactable = true;
				OnBiasSliderChanged(biasSlider.value);
			}
			else
			{
				biasSlider.interactable = false;
				biasValue.text = "N/A";
			}

			if (_regenerate != null)
			{
				_regenerate();
			}
		}

		public void OnBiasSliderChanged(float bias)
		{
			biasValue.text = bias.ToString("F2");

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

		#region HSV

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
				case 0: HSV_InitializeTemplate(180f, 180f); break;
				case 1: HSV_InitializeTemplate(180f, 180f, false, false, false); break;
				case 2: HSV_InitializeTemplate(180f, 180f, false, false, false); break;
				case 3: HSV_InitializeTemplate(30f, 90f, true); break;
				case 4: HSV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 5: HSV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 6: HSV_InitializeTemplate(120f, 120f, false); break;
				case 7: HSV_InitializeTemplate(120f, 120f, false, false, false); break;
				case 8: HSV_InitializeTemplate(120f, 120f, false, false, false); break;
				case 9: HSV_InitializeTemplate(30f, 90f, true); break;
				case 10: HSV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 11: HSV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 12: HSV_InitializeTemplate(30f, 90f, true); break;
				case 13: HSV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 14: HSV_InitializeTemplate(30f, 90f, false, false, false); break;
				default: HSV_InitializeTemplate(180f, 180f); break;
			}
		}

		private void HSV_InitializeTemplate(float defaultAngle = 0f, float maxAngle = 90f, bool variableAngle = false, bool variableVibrance = true, bool variableLuminance = true)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.maxValue = maxAngle;
			angleSlider.value = defaultAngle;
			vibranceSlider.interactable = variableVibrance;
			vibranceLabel.text = "Saturation:";
			vibranceSlider.value = 1f;
			luminanceSlider.interactable = variableLuminance;
			luminanceLabel.text = "Value:";
			luminanceSlider.value = 1f;
			biasLabel.text = "Bias:";
			biasToggle.isOn = false;
			biasToggle.interactable = false;
			biasSlider.interactable = false;
		}

		private void HSV_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HSV = ColorSchemeUtility.Templates.HSV.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 1: _template_HSV = ColorSchemeUtility.Templates.HSV.complementaryMix1; break;
				case 2: _template_HSV = ColorSchemeUtility.Templates.HSV.complementaryMix2; break;
				case 3: _template_HSV = ColorSchemeUtility.Templates.HSV.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 4: _template_HSV = ColorSchemeUtility.Templates.HSV.analogousMix1; break;
				case 5: _template_HSV = ColorSchemeUtility.Templates.HSV.analogousMix2; break;
				case 6: _template_HSV = ColorSchemeUtility.Templates.HSV.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 7: _template_HSV = ColorSchemeUtility.Templates.HSV.triadicMix1; break;
				case 8: _template_HSV = ColorSchemeUtility.Templates.HSV.triadicMix2; break;
				case 9: _template_HSV = ColorSchemeUtility.Templates.HSV.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 10: _template_HSV = ColorSchemeUtility.Templates.HSV.splitComplementaryMix1; break;
				case 11: _template_HSV = ColorSchemeUtility.Templates.HSV.splitComplementaryMix2; break;
				case 12: _template_HSV = ColorSchemeUtility.Templates.HSV.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 13: _template_HSV = ColorSchemeUtility.Templates.HSV.tetraticMix1; break;
				case 14: _template_HSV = ColorSchemeUtility.Templates.HSV.tetraticMix2; break;
				default: _template_HSV = ColorSchemeUtility.Templates.HSV.Complementary(vibranceSlider.value, luminanceSlider.value); break;
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

		#endregion

		#region HCV

		public void ToggleHCV(bool isOn)
		{
			if (isOn)
			{
				_selectTemplate = HCV_SelectTemplate;
				_regenerate = HCV_Regenerate;
				_selectTemplate();
				_regenerate();
			}
		}

		private ColorHCV[] _template_HCV;
		private ColorHCV[] _scheme_HCV;

		private void HCV_SelectTemplate()
		{
			switch (_templateIndex)
			{
				case 0: HCV_InitializeTemplate(180f, 180f); break;
				case 1: HCV_InitializeTemplate(180f, 180f, false, false, false); break;
				case 2: HCV_InitializeTemplate(180f, 180f, false, false, false); break;
				case 3: HCV_InitializeTemplate(30f, 90f, true); break;
				case 4: HCV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 5: HCV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 6: HCV_InitializeTemplate(120f, 120f, false); break;
				case 7: HCV_InitializeTemplate(120f, 120f, false, false, false); break;
				case 8: HCV_InitializeTemplate(120f, 120f, false, false, false); break;
				case 9: HCV_InitializeTemplate(30f, 90f, true); break;
				case 10: HCV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 11: HCV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 12: HCV_InitializeTemplate(30f, 90f, true); break;
				case 13: HCV_InitializeTemplate(30f, 90f, false, false, false); break;
				case 14: HCV_InitializeTemplate(30f, 90f, false, false, false); break;
				default: HCV_InitializeTemplate(180f, 180f); break;
			}
		}

		private void HCV_InitializeTemplate(float defaultAngle = 0f, float maxAngle = 90f, bool variableAngle = false, bool variableVibrance = true, bool variableLuminance = true)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.maxValue = maxAngle;
			angleSlider.value = defaultAngle;
			vibranceSlider.interactable = variableVibrance;
			vibranceLabel.text = "Chroma:";
			vibranceSlider.value = 1f;
			luminanceSlider.interactable = variableLuminance;
			luminanceLabel.text = "Value:";
			luminanceSlider.value = 1f;
			biasLabel.text = "Y/C Bias:";
			biasToggle.isOn = false;
			biasToggle.interactable = true;
			biasSlider.interactable = true;
		}

		private void HCV_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HCV = ColorSchemeUtility.Templates.HCV.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 1: _template_HCV = ColorSchemeUtility.Templates.HCV.complementaryMix1; break;
				case 2: _template_HCV = ColorSchemeUtility.Templates.HCV.complementaryMix2; break;
				case 3: _template_HCV = ColorSchemeUtility.Templates.HCV.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 4: _template_HCV = ColorSchemeUtility.Templates.HCV.analogousMix1; break;
				case 5: _template_HCV = ColorSchemeUtility.Templates.HCV.analogousMix2; break;
				case 6: _template_HCV = ColorSchemeUtility.Templates.HCV.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 7: _template_HCV = ColorSchemeUtility.Templates.HCV.triadicMix1; break;
				case 8: _template_HCV = ColorSchemeUtility.Templates.HCV.triadicMix2; break;
				case 9: _template_HCV = ColorSchemeUtility.Templates.HCV.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 10: _template_HCV = ColorSchemeUtility.Templates.HCV.splitComplementaryMix1; break;
				case 11: _template_HCV = ColorSchemeUtility.Templates.HCV.splitComplementaryMix2; break;
				case 12: _template_HCV = ColorSchemeUtility.Templates.HCV.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 13: _template_HCV = ColorSchemeUtility.Templates.HCV.tetraticMix1; break;
				case 14: _template_HCV = ColorSchemeUtility.Templates.HCV.tetraticMix2; break;
				default: _template_HCV = ColorSchemeUtility.Templates.HCV.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HCV == null || _scheme_HCV.Length != _template_HCV.Length)
			{
				_scheme_HCV = new ColorHCV[_template_HCV.Length];
			}

			if (biasToggle.isOn)
			{
				ColorSchemeUtility.Create(hueSlider.value, _template_HCV, _scheme_HCV, biasSlider.value);
			}
			else
			{
				ColorSchemeUtility.Create(hueSlider.value, _template_HCV, _scheme_HCV);
			}

			SetColorCount(_scheme_HCV.Length);
			for (int i = 0; i < _scheme_HCV.Length; ++i)
			{
				_templateColorImages[i].color = _template_HCV[i];
				_schemeColorImages[i].color = _scheme_HCV[i];
			}
		}

		#endregion

		#region HSL

		public void ToggleHSL(bool isOn)
		{
			if (isOn)
			{
				_selectTemplate = HSL_SelectTemplate;
				_regenerate = HSL_Regenerate;
				_selectTemplate();
				_regenerate();
			}
		}

		private ColorHSL[] _template_HSL;
		private ColorHSL[] _scheme_HSL;

		private void HSL_SelectTemplate()
		{
			switch (_templateIndex)
			{
				case 0: HSL_InitializeTemplate(180f, 180f); break;
				case 1: HSL_InitializeTemplate(180f, 180f, false, false, false); break;
				case 2: HSL_InitializeTemplate(180f, 180f, false, false, false); break;
				case 3: HSL_InitializeTemplate(30f, 90f, true); break;
				case 4: HSL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 5: HSL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 6: HSL_InitializeTemplate(120f, 120f, false); break;
				case 7: HSL_InitializeTemplate(120f, 120f, false, false, false); break;
				case 8: HSL_InitializeTemplate(120f, 120f, false, false, false); break;
				case 9: HSL_InitializeTemplate(30f, 90f, true); break;
				case 10: HSL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 11: HSL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 12: HSL_InitializeTemplate(30f, 90f, true); break;
				case 13: HSL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 14: HSL_InitializeTemplate(30f, 90f, false, false, false); break;
				default: HSL_InitializeTemplate(180f, 180f); break;
			}
		}

		private void HSL_InitializeTemplate(float defaultAngle = 0f, float maxAngle = 90f, bool variableAngle = false, bool variableVibrance = true, bool variableLuminance = true)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.maxValue = maxAngle;
			angleSlider.value = defaultAngle;
			vibranceSlider.interactable = variableVibrance;
			vibranceLabel.text = "Saturation:";
			vibranceSlider.value = 1f;
			luminanceSlider.interactable = variableLuminance;
			luminanceLabel.text = "Lightness:";
			luminanceSlider.value = 0.5f;
			biasLabel.text = "Bias:";
			biasToggle.isOn = false;
			biasToggle.interactable = false;
			biasSlider.interactable = false;
		}

		private void HSL_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HSL = ColorSchemeUtility.Templates.HSL.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 1: _template_HSL = ColorSchemeUtility.Templates.HSL.complementaryMix1; break;
				case 2: _template_HSL = ColorSchemeUtility.Templates.HSL.complementaryMix2; break;
				case 3: _template_HSL = ColorSchemeUtility.Templates.HSL.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 4: _template_HSL = ColorSchemeUtility.Templates.HSL.analogousMix1; break;
				case 5: _template_HSL = ColorSchemeUtility.Templates.HSL.analogousMix2; break;
				case 6: _template_HSL = ColorSchemeUtility.Templates.HSL.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 7: _template_HSL = ColorSchemeUtility.Templates.HSL.triadicMix1; break;
				case 8: _template_HSL = ColorSchemeUtility.Templates.HSL.triadicMix2; break;
				case 9: _template_HSL = ColorSchemeUtility.Templates.HSL.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 10: _template_HSL = ColorSchemeUtility.Templates.HSL.splitComplementaryMix1; break;
				case 11: _template_HSL = ColorSchemeUtility.Templates.HSL.splitComplementaryMix2; break;
				case 12: _template_HSL = ColorSchemeUtility.Templates.HSL.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 13: _template_HSL = ColorSchemeUtility.Templates.HSL.tetraticMix1; break;
				case 14: _template_HSL = ColorSchemeUtility.Templates.HSL.tetraticMix2; break;
				default: _template_HSL = ColorSchemeUtility.Templates.HSL.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HSL == null || _scheme_HSL.Length != _template_HSL.Length)
			{
				_scheme_HSL = new ColorHSL[_template_HSL.Length];
			}

			ColorSchemeUtility.Create(hueSlider.value, _template_HSL, _scheme_HSL);

			SetColorCount(_scheme_HSL.Length);
			for (int i = 0; i < _scheme_HSL.Length; ++i)
			{
				_templateColorImages[i].color = _template_HSL[i];
				_schemeColorImages[i].color = _scheme_HSL[i];
			}
		}

		#endregion

		#region HCL

		public void ToggleHCL(bool isOn)
		{
			if (isOn)
			{
				_selectTemplate = HCL_SelectTemplate;
				_regenerate = HCL_Regenerate;
				_selectTemplate();
				_regenerate();
			}
		}

		private ColorHCL[] _template_HCL;
		private ColorHCL[] _scheme_HCL;

		private void HCL_SelectTemplate()
		{
			switch (_templateIndex)
			{
				case 0: HCL_InitializeTemplate(180f, 180f); break;
				case 1: HCL_InitializeTemplate(180f, 180f, false, false, false); break;
				case 2: HCL_InitializeTemplate(180f, 180f, false, false, false); break;
				case 3: HCL_InitializeTemplate(30f, 90f, true); break;
				case 4: HCL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 5: HCL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 6: HCL_InitializeTemplate(120f, 120f, false); break;
				case 7: HCL_InitializeTemplate(120f, 120f, false, false, false); break;
				case 8: HCL_InitializeTemplate(120f, 120f, false, false, false); break;
				case 9: HCL_InitializeTemplate(30f, 90f, true); break;
				case 10: HCL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 11: HCL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 12: HCL_InitializeTemplate(30f, 90f, true); break;
				case 13: HCL_InitializeTemplate(30f, 90f, false, false, false); break;
				case 14: HCL_InitializeTemplate(30f, 90f, false, false, false); break;
				default: HCL_InitializeTemplate(180f, 180f); break;
			}
		}

		private void HCL_InitializeTemplate(float defaultAngle = 0f, float maxAngle = 90f, bool variableAngle = false, bool variableVibrance = true, bool variableLuminance = true)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.maxValue = maxAngle;
			angleSlider.value = defaultAngle;
			vibranceSlider.interactable = variableVibrance;
			vibranceLabel.text = "Chroma:";
			vibranceSlider.value = 1f;
			luminanceSlider.interactable = variableLuminance;
			luminanceLabel.text = "Lightness:";
			luminanceSlider.value = 0.5f;
			biasLabel.text = "Y/C Bias:";
			biasToggle.isOn = false;
			biasToggle.interactable = true;
			biasSlider.interactable = true;
		}

		private void HCL_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HCL = ColorSchemeUtility.Templates.HCL.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 1: _template_HCL = ColorSchemeUtility.Templates.HCL.complementaryMix1; break;
				case 2: _template_HCL = ColorSchemeUtility.Templates.HCL.complementaryMix2; break;
				case 3: _template_HCL = ColorSchemeUtility.Templates.HCL.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 4: _template_HCL = ColorSchemeUtility.Templates.HCL.analogousMix1; break;
				case 5: _template_HCL = ColorSchemeUtility.Templates.HCL.analogousMix2; break;
				case 6: _template_HCL = ColorSchemeUtility.Templates.HCL.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 7: _template_HCL = ColorSchemeUtility.Templates.HCL.triadicMix1; break;
				case 8: _template_HCL = ColorSchemeUtility.Templates.HCL.triadicMix2; break;
				case 9: _template_HCL = ColorSchemeUtility.Templates.HCL.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 10: _template_HCL = ColorSchemeUtility.Templates.HCL.splitComplementaryMix1; break;
				case 11: _template_HCL = ColorSchemeUtility.Templates.HCL.splitComplementaryMix2; break;
				case 12: _template_HCL = ColorSchemeUtility.Templates.HCL.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 13: _template_HCL = ColorSchemeUtility.Templates.HCL.tetraticMix1; break;
				case 14: _template_HCL = ColorSchemeUtility.Templates.HCL.tetraticMix2; break;
				default: _template_HCL = ColorSchemeUtility.Templates.HCL.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HCL == null || _scheme_HCL.Length != _template_HCL.Length)
			{
				_scheme_HCL = new ColorHCL[_template_HCL.Length];
			}

			if (biasToggle.isOn)
			{
				ColorSchemeUtility.Create(hueSlider.value, _template_HCL, _scheme_HCL, biasSlider.value);
			}
			else
			{
				ColorSchemeUtility.Create(hueSlider.value, _template_HCL, _scheme_HCL);
			}

			SetColorCount(_scheme_HCL.Length);
			for (int i = 0; i < _scheme_HCL.Length; ++i)
			{
				_templateColorImages[i].color = _template_HCL[i];
				_schemeColorImages[i].color = _scheme_HCL[i];
			}
		}

		#endregion

		#region HSY

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
				case 0: HSY_InitializeTemplate(180f, 180f); break;
				case 1: HSY_InitializeTemplate(180f, 180f, false, false, false); break;
				case 2: HSY_InitializeTemplate(180f, 180f, false, false, false); break;
				case 3: HSY_InitializeTemplate(30f, 90f, true); break;
				case 4: HSY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 5: HSY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 6: HSY_InitializeTemplate(120f, 120f, false); break;
				case 7: HSY_InitializeTemplate(120f, 120f, false, false, false); break;
				case 8: HSY_InitializeTemplate(120f, 120f, false, false, false); break;
				case 9: HSY_InitializeTemplate(30f, 90f, true); break;
				case 10: HSY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 11: HSY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 12: HSY_InitializeTemplate(30f, 90f, true); break;
				case 13: HSY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 14: HSY_InitializeTemplate(30f, 90f, false, false, false); break;
				default: HSY_InitializeTemplate(180f, 180f); break;
			}
		}

		private void HSY_InitializeTemplate(float defaultAngle = 0f, float maxAngle = 90f, bool variableAngle = false, bool variableVibrance = true, bool variableLuminance = true)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.maxValue = maxAngle;
			angleSlider.value = defaultAngle;
			vibranceSlider.interactable = variableVibrance;
			vibranceLabel.text = "Saturation:";
			vibranceSlider.value = 1f;
			luminanceSlider.interactable = variableLuminance;
			luminanceLabel.text = "Luma:";
			luminanceSlider.value = 0.5f;
			biasLabel.text = "Bias:";
			biasToggle.isOn = false;
			biasToggle.interactable = false;
			biasSlider.interactable = false;
		}

		private void HSY_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HSY = ColorSchemeUtility.Templates.HSY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 1: _template_HSY = ColorSchemeUtility.Templates.HSY.complementaryMix1; break;
				case 2: _template_HSY = ColorSchemeUtility.Templates.HSY.complementaryMix2; break;
				case 3: _template_HSY = ColorSchemeUtility.Templates.HSY.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 4: _template_HSY = ColorSchemeUtility.Templates.HSY.analogousMix1; break;
				case 5: _template_HSY = ColorSchemeUtility.Templates.HSY.analogousMix2; break;
				case 6: _template_HSY = ColorSchemeUtility.Templates.HSY.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 7: _template_HSY = ColorSchemeUtility.Templates.HSY.triadicMix1; break;
				case 8: _template_HSY = ColorSchemeUtility.Templates.HSY.triadicMix2; break;
				case 9: _template_HSY = ColorSchemeUtility.Templates.HSY.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 10: _template_HSY = ColorSchemeUtility.Templates.HSY.splitComplementaryMix1; break;
				case 11: _template_HSY = ColorSchemeUtility.Templates.HSY.splitComplementaryMix2; break;
				case 12: _template_HSY = ColorSchemeUtility.Templates.HSY.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 13: _template_HSY = ColorSchemeUtility.Templates.HSY.tetraticMix1; break;
				case 14: _template_HSY = ColorSchemeUtility.Templates.HSY.tetraticMix2; break;
				default: _template_HSY = ColorSchemeUtility.Templates.HSY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
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

		#endregion

		#region HCY

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
				case 0: HCY_InitializeTemplate(180f, 180f); break;
				case 1: HCY_InitializeTemplate(180f, 180f, false, false, false); break;
				case 2: HCY_InitializeTemplate(180f, 180f, false, false, false); break;
				case 3: HCY_InitializeTemplate(30f, 90f, true); break;
				case 4: HCY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 5: HCY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 6: HCY_InitializeTemplate(120f, 120f, false); break;
				case 7: HCY_InitializeTemplate(120f, 120f, false, false, false); break;
				case 8: HCY_InitializeTemplate(120f, 120f, false, false, false); break;
				case 9: HCY_InitializeTemplate(30f, 90f, true); break;
				case 10: HCY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 11: HCY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 12: HCY_InitializeTemplate(30f, 90f, true); break;
				case 13: HCY_InitializeTemplate(30f, 90f, false, false, false); break;
				case 14: HCY_InitializeTemplate(30f, 90f, false, false, false); break;
				default: HCY_InitializeTemplate(180f, 180f); break;
			}
		}

		private void HCY_InitializeTemplate(float defaultAngle = 0f, float maxAngle = 90f, bool variableAngle = false, bool variableVibrance = true, bool variableLuminance = true)
		{
			angleSlider.interactable = variableAngle;
			angleSlider.maxValue = maxAngle;
			angleSlider.value = defaultAngle;
			vibranceSlider.interactable = variableVibrance;
			vibranceLabel.text = "Chroma:";
			vibranceSlider.value = 1f;
			luminanceSlider.interactable = variableLuminance;
			luminanceLabel.text = "Luma:";
			luminanceSlider.value = 0.5f;
			biasLabel.text = "Y/C Bias:";
			biasToggle.isOn = false;
			biasToggle.interactable = true;
			biasSlider.interactable = true;
		}

		private void HCY_Regenerate()
		{
			switch (_templateIndex)
			{
				case 0: _template_HCY = ColorSchemeUtility.Templates.HCY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
				case 1: _template_HCY = ColorSchemeUtility.Templates.HCY.complementaryMix1; break;
				case 2: _template_HCY = ColorSchemeUtility.Templates.HCY.complementaryMix2; break;
				case 3: _template_HCY = ColorSchemeUtility.Templates.HCY.Analogous(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 4: _template_HCY = ColorSchemeUtility.Templates.HCY.analogousMix1; break;
				case 5: _template_HCY = ColorSchemeUtility.Templates.HCY.analogousMix2; break;
				case 6: _template_HCY = ColorSchemeUtility.Templates.HCY.Triadic(vibranceSlider.value, luminanceSlider.value); break;
				case 7: _template_HCY = ColorSchemeUtility.Templates.HCY.triadicMix1; break;
				case 8: _template_HCY = ColorSchemeUtility.Templates.HCY.triadicMix2; break;
				case 9: _template_HCY = ColorSchemeUtility.Templates.HCY.SplitComplementary(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 10: _template_HCY = ColorSchemeUtility.Templates.HCY.splitComplementaryMix1; break;
				case 11: _template_HCY = ColorSchemeUtility.Templates.HCY.splitComplementaryMix2; break;
				case 12: _template_HCY = ColorSchemeUtility.Templates.HCY.Tetratic(angleSlider.value, vibranceSlider.value, luminanceSlider.value); break;
				case 13: _template_HCY = ColorSchemeUtility.Templates.HCY.tetraticMix1; break;
				case 14: _template_HCY = ColorSchemeUtility.Templates.HCY.tetraticMix2; break;
				default: _template_HCY = ColorSchemeUtility.Templates.HCY.Complementary(vibranceSlider.value, luminanceSlider.value); break;
			}

			if (_scheme_HCY == null || _scheme_HCY.Length != _template_HCY.Length)
			{
				_scheme_HCY = new ColorHCY[_template_HCY.Length];
			}

			if (biasToggle.isOn)
			{
				ColorSchemeUtility.Create(hueSlider.value, _template_HCY, _scheme_HCY, biasSlider.value);
			}
			else
			{
				ColorSchemeUtility.Create(hueSlider.value, _template_HCY, _scheme_HCY);
			}

			SetColorCount(_scheme_HCY.Length);
			for (int i = 0; i < _scheme_HCY.Length; ++i)
			{
				_templateColorImages[i].color = _template_HCY[i];
				_schemeColorImages[i].color = _scheme_HCY[i];
			}
		}

		#endregion
	}
}
