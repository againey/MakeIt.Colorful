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
		[Header("Rendering")]
		public int visualizationWidth = 1024;
		public int visualizationHeight = 1024;
		public int modelHueSliceCount = 60;
		public int modelChromaValueSliceCount = 9;
		public float modelWedgeAngle = 90;
		public Material modelMaterial;
		public Camera modelCamera;
		public Camera sliceCamera;

		[Header("Timing")]
		public float updateLerpGradientTextureDelay = 0.01f;
		public float updateColorSpaceMeshDelay = 0.01f;

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
		private System.Action _updateColorSpaceMesh;

		private int _inScript = 0;
		private float _updateLerpGradientTextureQueue = float.NaN;
		private float _updateColorSpaceMeshQueue = float.NaN;

		private Texture2D _lerpGradientTexture;
		private RenderTexture _modelTexture;
		private RenderTexture _sliceTexture;

		private Mesh _colorSpaceCapsMesh;
		private Mesh _colorSpaceSidesMesh;
		private Mesh _colorSpaceRectangularSliceMesh;
		private Mesh _colorSpaceTriangularSliceMesh;

		private Vector3[] _colorSpaceCapsVertices;
		private Color[] _colorSpaceCapsColors;
		private Vector2[] _colorSpaceCapsHueChroma;

		private Vector3[] _colorSpaceSidesVertices;
		private Color[] _colorSpaceSidesColors;
		private Vector2[] _colorSpaceSidesHueValue;

		private Vector3[] _colorSpaceRectangularSliceVertices;
		private Color[] _colorSpaceRectangularSliceColors;
		private Vector2[] _colorSpaceRectangularSliceChromaValue;

		private Vector3[] _colorSpaceTriangularSliceVertices;
		private Color[] _colorSpaceTriangularSliceColors;
		private Vector2[] _colorSpaceTriangularSliceChromaValue;

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

		private const float _hueToRadians = Mathf.PI * 2f;

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

		private void QueueUpdateLerpGradientTexture(System.Action updateLerpGradientTexture)
		{
			if (updateLerpGradientTexture == null) return;
			_updateLerpGradientTexture = updateLerpGradientTexture;
			if (float.IsNaN(_updateLerpGradientTextureQueue))
			{
				_updateLerpGradientTextureQueue = updateLerpGradientTextureDelay;
			}
		}

		private void QueueUpdateColorSpaceMesh(System.Action updateColorSpaceMesh)
		{
			if (updateColorSpaceMesh == null) return;
			_updateColorSpaceMesh = updateColorSpaceMesh;
			if (float.IsNaN(_updateColorSpaceMeshQueue))
			{
				_updateColorSpaceMeshQueue = updateColorSpaceMeshDelay;
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

		private delegate void GetColorSpaceModelCapsInfo(float h, float c, out float y0, out float y1, out Color color0, out Color color1);
		private delegate void GetColorSpaceModelSidesInfo(float h, float v, out Color color);
		private delegate void GetColorSpaceModelSliceInfo(float h0, float h1, float c, float v, out float y0, out float y1, out Color color0, out Color color1);

		private void RenderColorSpaceMesh(GetColorSpaceModelCapsInfo getCapsInfo, GetColorSpaceModelSidesInfo getSidesInfo, GetColorSpaceModelSliceInfo getSliceInfo)
		{
			int halfVertexCount = _colorSpaceCapsVertices.Length / 2;

			if (getCapsInfo != null)
			{
				for (int i = 0; i < halfVertexCount; ++i)
				{
					Vector2 hc = _colorSpaceCapsHueChroma[i];
					float h = Mathf.Repeat(hc.x + hueSlider.value, 1f);
					float c = hc.y;
					float r = hc.x * _hueToRadians;
					float x = Mathf.Cos(r) * c;
					float z = Mathf.Sin(r) * c;

					float y0, y1;
					Color color0, color1;
					getCapsInfo(h, c, out y0, out y1, out color0, out color1);

					_colorSpaceCapsVertices[i] = new Vector3(x, y0, z);
					_colorSpaceCapsColors[i] = color0;
					_colorSpaceCapsVertices[i + halfVertexCount] = new Vector3(x, y1, z);
					_colorSpaceCapsColors[i + halfVertexCount] = color1;
				}

				_colorSpaceCapsMesh.vertices = _colorSpaceCapsVertices;
				_colorSpaceCapsMesh.colors = _colorSpaceCapsColors;
				_colorSpaceCapsMesh.RecalculateNormals();
				_colorSpaceCapsMesh.UploadMeshData(false);
			}

			if (getSidesInfo != null)
			{
				for (int i = 0; i < _colorSpaceSidesVertices.Length; ++i)
				{
					Vector2 hv = _colorSpaceSidesHueValue[i];
					float h = Mathf.Repeat(hv.x + hueSlider.value, 1f);
					float v = hv.y;
					float r = hv.x * _hueToRadians;
					float x = Mathf.Cos(r);
					float z = Mathf.Sin(r);

					Color color;
					getSidesInfo(h, v, out color);

					_colorSpaceSidesVertices[i] = new Vector3(x, v, z);
					_colorSpaceSidesColors[i] = color;
				}

				_colorSpaceSidesMesh.vertices = _colorSpaceSidesVertices;
				_colorSpaceSidesMesh.colors = _colorSpaceSidesColors;
				_colorSpaceSidesMesh.RecalculateNormals();
				_colorSpaceSidesMesh.UploadMeshData(false);
			}

			if (getSidesInfo != null || getCapsInfo == null)
			{
				float h0 = hueSlider.value;
				float h1 = Mathf.Repeat(hueSlider.value - modelWedgeAngle / 360f, 1f);
				float r1 = -modelWedgeAngle / 360f * _hueToRadians;
				float cos1 = Mathf.Cos(r1);
				float sin1 = Mathf.Sin(r1);
				halfVertexCount = _colorSpaceRectangularSliceVertices.Length / 2;
				for (int i = 0; i < halfVertexCount; ++i)
				{
					Vector2 cv = _colorSpaceRectangularSliceChromaValue[i];
					float c = cv.x;
					float v = cv.y;

					float y0, y1;
					Color color0, color1;
					getSliceInfo(h0, h1, c, v, out y0, out y1, out color0, out color1);

					_colorSpaceRectangularSliceVertices[i] = new Vector3(c, y0, 0f);
					_colorSpaceRectangularSliceColors[i] = color0;
					_colorSpaceRectangularSliceVertices[i + halfVertexCount] = new Vector3(cos1 * c, y1, sin1 * c);
					_colorSpaceRectangularSliceColors[i + halfVertexCount] = color1;
				}

				_colorSpaceRectangularSliceMesh.vertices = _colorSpaceRectangularSliceVertices;
				_colorSpaceRectangularSliceMesh.colors = _colorSpaceRectangularSliceColors;
				_colorSpaceRectangularSliceMesh.RecalculateNormals();
				_colorSpaceRectangularSliceMesh.UploadMeshData(false);
			}
			else
			{
				float h0 = hueSlider.value;
				float h1 = Mathf.Repeat(hueSlider.value - modelWedgeAngle / 360f, 1f);
				float r1 = -modelWedgeAngle / 360f * _hueToRadians;
				float cos1 = Mathf.Cos(r1);
				float sin1 = Mathf.Sin(r1);
				halfVertexCount = _colorSpaceTriangularSliceVertices.Length / 2;
				for (int i = 0; i < halfVertexCount; ++i)
				{
					Vector2 cv = _colorSpaceTriangularSliceChromaValue[i];
					float c = cv.x;
					float v = cv.y;

					float y0, y1;
					Color color0, color1;
					getSliceInfo(h0, h1, c, v, out y0, out y1, out color0, out color1);

					_colorSpaceTriangularSliceVertices[i] = new Vector3(c, y0, 0f);
					_colorSpaceTriangularSliceColors[i] = color0;
					_colorSpaceTriangularSliceVertices[i + halfVertexCount] = new Vector3(cos1 * c, y1, sin1 * c);
					_colorSpaceTriangularSliceColors[i + halfVertexCount] = color1;
				}

				_colorSpaceTriangularSliceMesh.vertices = _colorSpaceTriangularSliceVertices;
				_colorSpaceTriangularSliceMesh.colors = _colorSpaceTriangularSliceColors;
				_colorSpaceTriangularSliceMesh.RecalculateNormals();
				_colorSpaceTriangularSliceMesh.UploadMeshData(false);
			}

			modelCamera.enabled = true;
			sliceCamera.enabled = true;
			Graphics.DrawMesh(_colorSpaceCapsMesh, Matrix4x4.identity, modelMaterial, 0, modelCamera, 0, null, false, false);
			if (getSidesInfo != null)
			{
				Graphics.DrawMesh(_colorSpaceSidesMesh, Matrix4x4.identity, modelMaterial, 0, modelCamera, 0, null, false, false);
			}

			if (getSidesInfo != null || getCapsInfo == null)
			{
				Graphics.DrawMesh(_colorSpaceRectangularSliceMesh, Matrix4x4.identity, modelMaterial, 0, modelCamera, 0, null, false, false);
				Graphics.DrawMesh(_colorSpaceRectangularSliceMesh, Matrix4x4.identity, modelMaterial, 0, sliceCamera, 0, null, false, false);
			}
			else
			{
				Graphics.DrawMesh(_colorSpaceTriangularSliceMesh, Matrix4x4.identity, modelMaterial, 0, modelCamera, 0, null, false, false);
				Graphics.DrawMesh(_colorSpaceTriangularSliceMesh, Matrix4x4.identity, modelMaterial, 0, sliceCamera, 0, null, false, false);
			}

			StartCoroutine(DisableRenderTextureCameras());
		}

		private System.Collections.IEnumerator DisableRenderTextureCameras()
		{
			yield return new WaitForEndOfFrame();
			modelCamera.enabled = false;
			sliceCamera.enabled = false;
		}

		protected void Awake()
		{
			hueSliderBackground.texture = BuildHueTexture(2048);

			_lerpGradientTexture = BuildLerpTexture(2048, false);
			lerpGradientImage.texture = _lerpGradientTexture;

			_colorSpaceCapsVertices = new Vector3[modelHueSliceCount * modelChromaValueSliceCount * 2];
			_colorSpaceCapsColors = new Color[modelHueSliceCount * modelChromaValueSliceCount * 2];
			_colorSpaceCapsHueChroma = new Vector2[modelHueSliceCount * modelChromaValueSliceCount];
			_colorSpaceSidesVertices = new Vector3[modelHueSliceCount * modelChromaValueSliceCount];
			_colorSpaceSidesColors = new Color[modelHueSliceCount * modelChromaValueSliceCount];
			_colorSpaceSidesHueValue = new Vector2[modelHueSliceCount * modelChromaValueSliceCount];
			float hueScale = (360 - modelWedgeAngle) / (360f * (modelHueSliceCount - 1));
			for (int hIndex = 0; hIndex < modelHueSliceCount; ++hIndex)
			{
				int cIndexBase = hIndex * modelChromaValueSliceCount;
				int vIndexBase = hIndex * modelChromaValueSliceCount;
				float h = hIndex * hueScale;

				for (int cIndex = 0; cIndex < modelChromaValueSliceCount; ++cIndex)
				{
					float c = (float)cIndex / (modelChromaValueSliceCount - 1);
					_colorSpaceCapsHueChroma[cIndexBase + cIndex] = new Vector2(h, c);
				}

				for (int vIndex = 0; vIndex < modelChromaValueSliceCount; ++vIndex)
				{
					float v = (float)vIndex / (modelChromaValueSliceCount - 1);
					_colorSpaceSidesHueValue[vIndexBase + vIndex] = new Vector2(h, v);
				}
			}

			_colorSpaceRectangularSliceVertices = new Vector3[modelChromaValueSliceCount * modelChromaValueSliceCount * 2];
			_colorSpaceRectangularSliceColors = new Color[modelChromaValueSliceCount * modelChromaValueSliceCount * 2];
			_colorSpaceRectangularSliceChromaValue = new Vector2[modelChromaValueSliceCount * modelChromaValueSliceCount];
			for (int cIndex = 0; cIndex < modelChromaValueSliceCount; ++cIndex)
			{
				float c = (float)cIndex / (modelChromaValueSliceCount - 1);
				int vIndexBase = cIndex * modelChromaValueSliceCount;

				for (int vIndex = 0; vIndex < modelChromaValueSliceCount; ++vIndex)
				{
					float v = (float)vIndex / (modelChromaValueSliceCount - 1);
					_colorSpaceRectangularSliceChromaValue[vIndexBase + vIndex] = new Vector2(c, v);
				}
			}

			_colorSpaceTriangularSliceVertices = new Vector3[modelChromaValueSliceCount * (modelChromaValueSliceCount + 1)];
			_colorSpaceTriangularSliceColors = new Color[_colorSpaceTriangularSliceVertices.Length];
			_colorSpaceTriangularSliceChromaValue = new Vector2[_colorSpaceTriangularSliceVertices.Length / 2];
			for (int cIndex = 0; cIndex < modelChromaValueSliceCount; ++cIndex)
			{
				float c = (float)cIndex / (modelChromaValueSliceCount - 1);
				int cIndexInverse = modelChromaValueSliceCount - cIndex;
				int vIndexBase = _colorSpaceTriangularSliceChromaValue.Length - cIndexInverse * (cIndexInverse + 1) / 2;

				for (int vIndex = 0; vIndex < modelChromaValueSliceCount - cIndex; ++vIndex)
				{
					int d = modelChromaValueSliceCount - cIndex - 1;
					float v = d > 0 ? (float)vIndex / d : 0f;
					_colorSpaceTriangularSliceChromaValue[vIndexBase + vIndex] = new Vector2(c, v);
				}
			}

			_colorSpaceCapsMesh = new Mesh();
			_colorSpaceCapsMesh.vertices = _colorSpaceCapsVertices;
			_colorSpaceCapsMesh.colors = _colorSpaceCapsColors;
			var colorSpaceCapsTriangles = new int[(modelHueSliceCount - 1) * (modelChromaValueSliceCount - 1) * 12];
			for (int hIndex = 1; hIndex < modelHueSliceCount; ++hIndex)
			{
				int triangleIndexBase0 = (hIndex - 1) * (modelChromaValueSliceCount - 1) * 6;
				int triangleIndexBase1 = triangleIndexBase0 + (modelHueSliceCount - 1) * (modelChromaValueSliceCount - 1) * 6;
				int vertexIndexBase0 = (hIndex - 1) * modelChromaValueSliceCount;
				int vertexIndexBase1 = vertexIndexBase0 + modelHueSliceCount * modelChromaValueSliceCount;

				for (int cIndex = 1; cIndex < modelChromaValueSliceCount; ++cIndex)
				{
					int vertexIndex0 = vertexIndexBase0 + cIndex - 1;
					int vertexIndex1 = vertexIndexBase1 + cIndex - 1;
					int triangleIndex0 = triangleIndexBase0 + (cIndex - 1) * 6;
					int triangleIndex1 = triangleIndexBase1 + (cIndex - 1) * 6;

					colorSpaceCapsTriangles[triangleIndex0 + 0] = vertexIndex0;
					colorSpaceCapsTriangles[triangleIndex0 + 1] = vertexIndex0 + 1;
					colorSpaceCapsTriangles[triangleIndex0 + 2] = vertexIndex0 + modelChromaValueSliceCount;
					colorSpaceCapsTriangles[triangleIndex0 + 3] = vertexIndex0 + modelChromaValueSliceCount;
					colorSpaceCapsTriangles[triangleIndex0 + 4] = vertexIndex0 + 1;
					colorSpaceCapsTriangles[triangleIndex0 + 5] = vertexIndex0 + modelChromaValueSliceCount + 1;

					colorSpaceCapsTriangles[triangleIndex1 + 0] = vertexIndex1;
					colorSpaceCapsTriangles[triangleIndex1 + 1] = vertexIndex1 + modelChromaValueSliceCount;
					colorSpaceCapsTriangles[triangleIndex1 + 2] = vertexIndex1 + 1;
					colorSpaceCapsTriangles[triangleIndex1 + 3] = vertexIndex1 + 1;
					colorSpaceCapsTriangles[triangleIndex1 + 4] = vertexIndex1 + modelChromaValueSliceCount;
					colorSpaceCapsTriangles[triangleIndex1 + 5] = vertexIndex1 + modelChromaValueSliceCount + 1;
				}
			}
			_colorSpaceCapsMesh.triangles = colorSpaceCapsTriangles;
			_colorSpaceCapsMesh.bounds = new Bounds(new Vector3(0f, 0.5f, 0f), new Vector3(2f, 1f, 2f));

			_colorSpaceSidesMesh = new Mesh();
			_colorSpaceSidesMesh.vertices = _colorSpaceSidesVertices;
			_colorSpaceSidesMesh.colors = _colorSpaceSidesColors;
			var colorSpaceSidesTriangles = new int[(modelHueSliceCount - 1) * (modelChromaValueSliceCount - 1) * 6];
			for (int hIndex = 1; hIndex < modelHueSliceCount; ++hIndex)
			{
				int triangleIndexBase = (hIndex - 1) * (modelChromaValueSliceCount - 1) * 6;
				int vertexIndexBase = (hIndex - 1) * modelChromaValueSliceCount;

				for (int cIndex = 1; cIndex < modelChromaValueSliceCount; ++cIndex)
				{
					int vertexIndex = vertexIndexBase + cIndex - 1;
					int triangleIndex = triangleIndexBase + (cIndex - 1) * 6;

					colorSpaceSidesTriangles[triangleIndex + 0] = vertexIndex;
					colorSpaceSidesTriangles[triangleIndex + 1] = vertexIndex + 1;
					colorSpaceSidesTriangles[triangleIndex + 2] = vertexIndex + modelChromaValueSliceCount;
					colorSpaceSidesTriangles[triangleIndex + 3] = vertexIndex + modelChromaValueSliceCount;
					colorSpaceSidesTriangles[triangleIndex + 4] = vertexIndex + 1;
					colorSpaceSidesTriangles[triangleIndex + 5] = vertexIndex + modelChromaValueSliceCount + 1;
				}
			}
			_colorSpaceSidesMesh.triangles = colorSpaceSidesTriangles;
			_colorSpaceSidesMesh.bounds = new Bounds(new Vector3(0f, 0.5f, 0f), new Vector3(2f, 1f, 2f));

			_colorSpaceRectangularSliceMesh = new Mesh();
			_colorSpaceRectangularSliceMesh.vertices = _colorSpaceRectangularSliceVertices;
			_colorSpaceRectangularSliceMesh.colors = _colorSpaceRectangularSliceColors;
			var colorSpaceRectangularSliceTriangles = new int[(modelChromaValueSliceCount - 1) * (modelChromaValueSliceCount - 1) * 12];
			for (int cIndex = 1; cIndex < modelChromaValueSliceCount; ++cIndex)
			{
				int triangleIndexBase0 = (cIndex - 1) * (modelChromaValueSliceCount - 1) * 6;
				int triangleIndexBase1 = triangleIndexBase0 + (modelChromaValueSliceCount - 1) * (modelChromaValueSliceCount - 1) * 6;
				int vertexIndexBase0 = (cIndex - 1) * modelChromaValueSliceCount;
				int vertexIndexBase1 = vertexIndexBase0 + modelChromaValueSliceCount * modelChromaValueSliceCount;

				for (int vIndex = 1; vIndex < modelChromaValueSliceCount; ++vIndex)
				{
					int vertexIndex0 = vertexIndexBase0 + vIndex - 1;
					int vertexIndex1 = vertexIndexBase1 + vIndex - 1;
					int triangleIndex0 = triangleIndexBase0 + (vIndex - 1) * 6;
					int triangleIndex1 = triangleIndexBase1 + (vIndex - 1) * 6;

					colorSpaceRectangularSliceTriangles[triangleIndex0 + 0] = vertexIndex0;
					colorSpaceRectangularSliceTriangles[triangleIndex0 + 1] = vertexIndex0 + 1;
					colorSpaceRectangularSliceTriangles[triangleIndex0 + 2] = vertexIndex0 + modelChromaValueSliceCount;
					colorSpaceRectangularSliceTriangles[triangleIndex0 + 3] = vertexIndex0 + modelChromaValueSliceCount;
					colorSpaceRectangularSliceTriangles[triangleIndex0 + 4] = vertexIndex0 + 1;
					colorSpaceRectangularSliceTriangles[triangleIndex0 + 5] = vertexIndex0 + modelChromaValueSliceCount + 1;

					colorSpaceRectangularSliceTriangles[triangleIndex1 + 0] = vertexIndex1;
					colorSpaceRectangularSliceTriangles[triangleIndex1 + 1] = vertexIndex1 + modelChromaValueSliceCount;
					colorSpaceRectangularSliceTriangles[triangleIndex1 + 2] = vertexIndex1 + 1;
					colorSpaceRectangularSliceTriangles[triangleIndex1 + 3] = vertexIndex1 + 1;
					colorSpaceRectangularSliceTriangles[triangleIndex1 + 4] = vertexIndex1 + modelChromaValueSliceCount;
					colorSpaceRectangularSliceTriangles[triangleIndex1 + 5] = vertexIndex1 + modelChromaValueSliceCount + 1;
				}
			}
			_colorSpaceRectangularSliceMesh.triangles = colorSpaceRectangularSliceTriangles;
			_colorSpaceRectangularSliceMesh.bounds = new Bounds(new Vector3(0f, 0.5f, 0f), new Vector3(2f, 1f, 2f));

			_colorSpaceTriangularSliceMesh = new Mesh();
			_colorSpaceTriangularSliceMesh.vertices = _colorSpaceTriangularSliceVertices;
			_colorSpaceTriangularSliceMesh.colors = _colorSpaceTriangularSliceColors;
			var colorSpaceTriangularSliceTriangleCount = (modelChromaValueSliceCount - 1) * (modelChromaValueSliceCount - 1);
			var colorSpaceTriangularSliceTriangles = new int[colorSpaceTriangularSliceTriangleCount * 6];
			for (int cIndex = 1; cIndex < modelChromaValueSliceCount; ++cIndex)
			{
				int cIndexInverse = modelChromaValueSliceCount - cIndex;
				int triangleIndexBase0 = (colorSpaceTriangularSliceTriangleCount - cIndexInverse * cIndexInverse) * 3;
				int triangleIndexBase1 = triangleIndexBase0 + colorSpaceTriangularSliceTriangleCount * 3;
				int vertexIndexBase0 = _colorSpaceTriangularSliceChromaValue.Length - (cIndexInverse + 1) * (cIndexInverse + 2) / 2;
				int vertexIndexBase1 = vertexIndexBase0 + _colorSpaceTriangularSliceChromaValue.Length;
				int vertexSpan = modelChromaValueSliceCount - cIndex;

				for (int vIndex = 0; vIndex < modelChromaValueSliceCount - cIndex; ++vIndex)
				{
					int vertexIndex0 = vertexIndexBase0 + vIndex;
					int vertexIndex1 = vertexIndexBase1 + vIndex;
					int triangleIndex0 = triangleIndexBase0 + vIndex * 6 - 3;
					int triangleIndex1 = triangleIndexBase1 + vIndex * 6 - 3;

					if (vIndex > 0)
					{
						colorSpaceTriangularSliceTriangles[triangleIndex0 + 0] = vertexIndex0;
						colorSpaceTriangularSliceTriangles[triangleIndex0 + 1] = vertexIndex0 + vertexSpan + 1;
						colorSpaceTriangularSliceTriangles[triangleIndex0 + 2] = vertexIndex0 + vertexSpan;

						colorSpaceTriangularSliceTriangles[triangleIndex1 + 0] = vertexIndex1;
						colorSpaceTriangularSliceTriangles[triangleIndex1 + 1] = vertexIndex1 + vertexSpan;
						colorSpaceTriangularSliceTriangles[triangleIndex1 + 2] = vertexIndex1 + vertexSpan + 1;
					}

					colorSpaceTriangularSliceTriangles[triangleIndex0 + 3] = vertexIndex0;
					colorSpaceTriangularSliceTriangles[triangleIndex0 + 4] = vertexIndex0 + 1;
					colorSpaceTriangularSliceTriangles[triangleIndex0 + 5] = vertexIndex0 + vertexSpan + 1;

					colorSpaceTriangularSliceTriangles[triangleIndex1 + 3] = vertexIndex1;
					colorSpaceTriangularSliceTriangles[triangleIndex1 + 4] = vertexIndex1 + vertexSpan + 1;
					colorSpaceTriangularSliceTriangles[triangleIndex1 + 5] = vertexIndex1 + 1;
				}
			}
			_colorSpaceTriangularSliceMesh.triangles = colorSpaceTriangularSliceTriangles;
			_colorSpaceTriangularSliceMesh.bounds = new Bounds(new Vector3(0f, 0.5f, 0f), new Vector3(2f, 1f, 2f));

			_modelTexture = new RenderTexture(visualizationWidth, visualizationHeight, 16, RenderTextureFormat.ARGB32);
			_modelTexture.filterMode = FilterMode.Bilinear;
			_modelTexture.wrapMode = TextureWrapMode.Clamp;
			fullVisualizationImage.texture = _modelTexture;
			modelCamera.targetTexture = _modelTexture;
			modelCamera.transform.LookAt(_colorSpaceCapsMesh.bounds.center);

			_sliceTexture = new RenderTexture(visualizationWidth, visualizationHeight, 16, RenderTextureFormat.ARGB32);
			_sliceTexture.filterMode = FilterMode.Bilinear;
			_sliceTexture.wrapMode = TextureWrapMode.Clamp;
			sliceVisualizationImage.texture = _sliceTexture;
			sliceCamera.targetTexture = _sliceTexture;
			sliceCamera.transform.LookAt(new Vector3(0.5f, 0.5f, 0f));

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

			SetActiveColor(new ColorHSV(hueSlider.value, 1f, 1f));
			hueSliderHandle.color = activeColor;

			_updateLerpGradientTextureQueue = 0f;
		}

		protected void Update()
		{
			if (!float.IsNaN(_updateLerpGradientTextureQueue))
			{
				_updateLerpGradientTextureQueue -= Time.deltaTime;
				if (_updateLerpGradientTextureQueue <= 0f)
				{
					_updateLerpGradientTexture();
					_updateLerpGradientTextureQueue = float.NaN;
				}
			}

			if (!float.IsNaN(_updateColorSpaceMeshQueue))
			{
				_updateColorSpaceMeshQueue -= Time.deltaTime;
				if (_updateColorSpaceMeshQueue <= 0f)
				{
					_updateColorSpaceMesh();
					_updateColorSpaceMeshQueue = float.NaN;
				}
			}
		}

		public void OnHueSliderChanged(float hue)
		{
			hueSliderHandle.color = new ColorHSV(hue, 1f, 1f);

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
				RGB_UpdateColorSpaceMesh();
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
				QueueUpdateColorSpaceMesh(RGB_UpdateColorSpaceMesh);
			});
		}

		public void OnRgbComponentSliceChanged(bool isOn)
		{
			if (isOn)
			{
				SetRgbSlice();
				QueueUpdateColorSpaceMesh(RGB_UpdateColorSpaceMesh);
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
				QueueUpdateColorSpaceMesh(RGB_UpdateColorSpaceMesh);
			});
		}

		private void RGB_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(null, null, RGB_GetColorSpaceModelSliceInfo);
		}

		private void RGB_GetColorSpaceModelSliceInfo(float h0, float h1, float a, float b, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = b;
			y1 = b;
			color0 = color1 = _getSliceColor(a, b);
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
				QueueUpdateColorSpaceMesh(RGB_UpdateColorSpaceMesh);
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
				CMY_UpdateColorSpaceMesh();
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
				QueueUpdateColorSpaceMesh(CMY_UpdateColorSpaceMesh);
			});
		}

		public void OnCmyComponentSliceChanged(bool isOn)
		{
			if (isOn)
			{
				SetCmySlice();
				QueueUpdateColorSpaceMesh(CMY_UpdateColorSpaceMesh);
			}
		}

		private void CMY_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var color = (ColorCMY)new ColorHSV(hue, 1f, 1f);
				SetActiveColor(color);

				cmyCyanSlider.value = color.c;
				cmyMagentaSlider.value = color.m;
				cmyYellowSlider.value = color.y;

				SetCmySlice();
				QueueUpdateColorSpaceMesh(CMY_UpdateColorSpaceMesh);
			});
		}

		private void CMY_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(null, null, CMY_GetColorSpaceModelSliceInfo);
		}

		private void CMY_GetColorSpaceModelSliceInfo(float h0, float h1, float a, float b, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = b;
			y1 = b;
			color0 = color1 = _getSliceColor(a, b);
		}

		public void OnCmyComponentSliderChanged()
		{
			ExecuteFromScript(() =>
			{
				var color = new ColorCMY(cmyCyanSlider.value, cmyMagentaSlider.value, cmyYellowSlider.value);
				SetActiveColor(color);
				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetCmySlice();
				QueueUpdateColorSpaceMesh(CMY_UpdateColorSpaceMesh);
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
				CMYK_UpdateColorSpaceMesh();
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
				QueueUpdateColorSpaceMesh(CMYK_UpdateColorSpaceMesh);
			});
		}

		public void OnCmykComponentSliceChanged(bool isOn)
		{
			if (isOn)
			{
				SetCmykSlice();
				QueueUpdateColorSpaceMesh(CMYK_UpdateColorSpaceMesh);
			}
		}

		private void CMYK_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var color = (ColorCMYK)new ColorHSV(hue, 1f, 1f);
				SetActiveColor(color);

				cmykCyanSlider.value = color.c;
				cmykMagentaSlider.value = color.m;
				cmykYellowSlider.value = color.y;
				cmykKeySlider.value = color.k;

				SetCmykSlice();
				QueueUpdateColorSpaceMesh(CMYK_UpdateColorSpaceMesh);
			});
		}

		private void CMYK_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(null, null, CMYK_GetColorSpaceModelSliceInfo);
		}

		private void CMYK_GetColorSpaceModelSliceInfo(float h0, float h1, float a, float b, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = b;
			y1 = b;
			color0 = color1 = _getSliceColor(a, b);
		}

		public void OnCmykComponentSliderChanged()
		{
			ExecuteFromScript(() =>
			{
				var color = new ColorCMYK(cmykCyanSlider.value, cmykMagentaSlider.value, cmykYellowSlider.value, cmykKeySlider.value);
				SetActiveColor(color);
				var hcv = (ColorHCV)color;
				if (hcv.c > 0f) hueSlider.value = hcv.h;

				SetCmykSlice();
				QueueUpdateColorSpaceMesh(CMYK_UpdateColorSpaceMesh);
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
				HSV_UpdateColorSpaceMesh();
			}
		}

		private void HSV_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hsv = (ColorHSV)activeColor;
				hsv.h = hue;
				SetActiveColor(hsv);
				QueueUpdateColorSpaceMesh(HSV_UpdateColorSpaceMesh);
			});
		}

		private void HSV_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(HSV_GetColorSpaceModelCapsInfo, HSV_GetColorSpaceModelSidesInfo, HSV_GetColorSpaceModelSliceInfo);
		}

		private static void HSV_GetColorSpaceModelCapsInfo(float h, float s, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = 0f;
			y1 = 1f;
			color0 = new ColorHSV(h, s, 0f);
			color1 = new ColorHSV(h, s, 1f);
		}

		private static void HSV_GetColorSpaceModelSidesInfo(float h, float v, out Color color)
		{
			color = new ColorHSV(h, 1f, v);
		}

		private static void HSV_GetColorSpaceModelSliceInfo(float h0, float h1, float s, float v, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = v;
			y1 = v;
			color0 = new ColorHSV(h0, s, v);
			color1 = new ColorHSV(h1, s, v);
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
				HCV_UpdateColorSpaceMesh();
			}
		}

		private void HCV_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hcv = (ColorHCV)activeColor;
				hcv.h = hue;
				SetActiveColor(hcv);
				QueueUpdateColorSpaceMesh(HCV_UpdateColorSpaceMesh);
			});
		}

		private void HCV_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(HCV_GetColorSpaceModelCapsInfo, null, HCV_GetColorSpaceModelSliceInfo);
		}

		private static void HCV_GetColorSpaceModelCapsInfo(float h, float c, out float y0, out float y1, out Color color0, out Color color1)
		{
			ColorHCV.GetMinMaxValue(c, out y0, out y1);
			color0 = new ColorHCV(h, c, y0);
			color1 = new ColorHCV(h, c, y1);
		}

		private static void HCV_GetColorSpaceModelSliceInfo(float h0, float h1, float c, float v, out float y0, out float y1, out Color color0, out Color color1)
		{
			float lMin, lMax;

			ColorHCV.GetMinMaxValue(c, out lMin, out lMax);
			y0 = y1 = Mathf.Lerp(lMin, lMax, v);

			color0 = new ColorHCV(h0, c, y0);
			color1 = new ColorHCV(h1, c, y1);
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
				HSL_UpdateColorSpaceMesh();
			}
		}

		private void HSL_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hsl = (ColorHSL)activeColor;
				hsl.h = hue;
				SetActiveColor(hsl);
				QueueUpdateColorSpaceMesh(HSL_UpdateColorSpaceMesh);
				HSL_UpdateColorSpaceMesh();
			});
		}

		private void HSL_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(HSL_GetColorSpaceModelCapsInfo, HSL_GetColorSpaceModelSidesInfo, HSL_GetColorSpaceModelSliceInfo);
		}

		private static void HSL_GetColorSpaceModelCapsInfo(float h, float s, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = 0f;
			y1 = 1f;
			color0 = new ColorHSL(h, s, 0f);
			color1 = new ColorHSL(h, s, 1f);
		}

		private static void HSL_GetColorSpaceModelSidesInfo(float h, float l, out Color color)
		{
			color = new ColorHSL(h, 1f, l);
		}

		private static void HSL_GetColorSpaceModelSliceInfo(float h0, float h1, float s, float l, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = l;
			y1 = l;
			color0 = new ColorHSL(h0, s, l);
			color1 = new ColorHSL(h1, s, l);
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
				HCL_UpdateColorSpaceMesh();
			}
		}

		private void HCL_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hcl = (ColorHCL)activeColor;
				hcl.h = hue;
				SetActiveColor(hcl);
				QueueUpdateColorSpaceMesh(HCL_UpdateColorSpaceMesh);
			});
		}

		private void HCL_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(HCL_GetColorSpaceModelCapsInfo, null, HCL_GetColorSpaceModelSliceInfo);
		}

		private static void HCL_GetColorSpaceModelCapsInfo(float h, float c, out float y0, out float y1, out Color color0, out Color color1)
		{
			ColorHCL.GetMinMaxLightness(c, out y0, out y1);
			color0 = new ColorHCL(h, c, y0);
			color1 = new ColorHCL(h, c, y1);
		}

		private static void HCL_GetColorSpaceModelSliceInfo(float h0, float h1, float c, float v, out float y0, out float y1, out Color color0, out Color color1)
		{
			float lMin, lMax;

			ColorHCL.GetMinMaxLightness(c, out lMin, out lMax);
			y0 = y1 = Mathf.Lerp(lMin, lMax, v);

			color0 = new ColorHCL(h0, c, y0);
			color1 = new ColorHCL(h1, c, y1);
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
				HSY_UpdateColorSpaceMesh();
			}
		}

		private void HSY_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hsy = (ColorHSY)activeColor;
				hsy.h = hue;
				SetActiveColor(hsy);
				QueueUpdateColorSpaceMesh(HSY_UpdateColorSpaceMesh);
			});
		}

		private void HSY_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(HSY_GetColorSpaceModelCapsInfo, HSY_GetColorSpaceModelSidesInfo, HSY_GetColorSpaceModelSliceInfo);
		}

		private static void HSY_GetColorSpaceModelCapsInfo(float h, float s, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = 0f;
			y1 = 1f;
			color0 = new ColorHSY(h, s, 0f);
			color1 = new ColorHSY(h, s, 1f);
		}

		private static void HSY_GetColorSpaceModelSidesInfo(float h, float y, out Color color)
		{
			color = new ColorHSY(h, 1f, y);
		}

		private static void HSY_GetColorSpaceModelSliceInfo(float h0, float h1, float s, float y, out float y0, out float y1, out Color color0, out Color color1)
		{
			y0 = y;
			y1 = y;
			color0 = new ColorHSY(h0, s, y);
			color1 = new ColorHSY(h1, s, y);
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
				HCY_UpdateColorSpaceMesh();
			}
		}

		private void HCY_OnHueChanged(float hue)
		{
			ExecuteFromScript(() =>
			{
				var hcy = (ColorHCY)_baseColor;
				hcy.h = hue;
				SetActiveColor(hcy, false);
				QueueUpdateColorSpaceMesh(HCY_UpdateColorSpaceMesh);
			});
		}

		private void HCY_UpdateColorSpaceMesh()
		{
			RenderColorSpaceMesh(HCY_GetColorSpaceModelCapsInfo, null, HCY_GetColorSpaceModelSliceInfo);
		}

		private static void HCY_GetColorSpaceModelCapsInfo(float h, float c, out float y0, out float y1, out Color color0, out Color color1)
		{
			ColorHCY.GetMinMaxLuma(h, c, out y0, out y1);
			color0 = new ColorHCY(h, c, y0);
			color1 = new ColorHCY(h, c, y1);
		}

		private static void HCY_GetColorSpaceModelSliceInfo(float h0, float h1, float c, float v, out float y0, out float y1, out Color color0, out Color color1)
		{
			float yMin, yMax;

			ColorHCY.GetMinMaxLuma(h0, c, out yMin, out yMax);
			y0 = Mathf.Lerp(yMin, yMax, v);
			ColorHCY.GetMinMaxLuma(h1, c, out yMin, out yMax);
			y1 = Mathf.Lerp(yMin, yMax, v);

			color0 = new ColorHCY(h0, c, y0);
			color1 = new ColorHCY(h1, c, y1);
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
