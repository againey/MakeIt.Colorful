/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

#if UNITY_5_3_OR_NEWER
using NUnit.Framework;
using UnityEngine;
using System;
using System.Reflection;

using URnd = UnityEngine.Random;

namespace Experilous.MakeItColorful.Tests
{
	class ConversionTests
	{
		#region Private Helper Functions

		private bool FindChainedConversionOperators(Type first, Type second, Type third, ref MethodInfo firstToSecond, ref MethodInfo secondToThird)
		{
			var methods = second.GetMethods(BindingFlags.Public | BindingFlags.Static);
			foreach (var method in methods)
			{
				if (method.IsSpecialName && method.Name == "op_Explicit")
				{
					if (method.ReturnType == second || method.ReturnType == third)
					{
						var parameters = method.GetParameters();
						if (parameters.Length == 1)
						{
							var parameter = parameters[0];
							if (parameter.ParameterType == first && method.ReturnType == second)
							{
								firstToSecond = method;
								if (secondToThird != null) return true;
							}
							else if (parameter.ParameterType == second && method.ReturnType == third)
							{
								secondToThird = method;
								if (firstToSecond != null) return true;
							}
						}
					}
				}
			}
			return false;
		}

		private bool FindRoundtrippedConversionOperators<TFirst, TSecond>(out MethodInfo firstToSecond, out MethodInfo secondToFirst)
		{
			var first = typeof(TFirst);
			var second = typeof(TSecond);
			firstToSecond = secondToFirst = null;
			if (FindChainedConversionOperators(first, second, first, ref firstToSecond, ref secondToFirst)) return true;
			if (FindChainedConversionOperators(second, first, second, ref secondToFirst, ref firstToSecond)) return true;
			return false;
		}

		//TODO Delete if unused
		private bool FindChannelIndexer<TColor>(out PropertyInfo indexer, out int channelCount)
		{
			indexer = null;
			channelCount = 0;

			var type = typeof(TColor);
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var parameters = property.GetIndexParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
				{
					indexer = property;
					break;
				}
			}

			if (indexer == null) return false;

			var field = type.GetField("channelCount", BindingFlags.Public | BindingFlags.Static);
			if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(int))
			{
				channelCount = (int)field.GetRawConstantValue();
				return true;
			}

			return false;
		}

		private void ValidateConversionRoundtrips<TFirst, TSecond>(Func<TFirst, int, float> indexer, int channelCount, float margin, params TFirst[] colors)
		{
			MethodInfo firstToSecond, secondToFirst;
			if (!FindRoundtrippedConversionOperators<TFirst, TSecond>(out firstToSecond, out secondToFirst)) Assert.Inconclusive();

			object[] parameter = new object[1];
			Func<TFirst, TFirst> roundtrip = (TFirst color) =>
			{
				parameter[0] = color;
				parameter[0] = firstToSecond.Invoke(null, parameter);
				return (TFirst)secondToFirst.Invoke(null, parameter);
			};

			Action<TFirst, TFirst> assertNearlyEqual = (TFirst initial, TFirst final) =>
			{
				for (int i = 0; i < channelCount; ++i)
				{
					Assert.LessOrEqual(Mathf.Abs(indexer(initial, i) - indexer(final, i)), margin, string.Format("From {0} to {1}", initial, final));
				}
			};

			foreach (var color in colors)
			{
				assertNearlyEqual(color, roundtrip(color));
			}
		}

		private delegate TColor ColorFactory4<TColor>(float channel0, float channel1, float channel2, float channel3);
		private delegate TColor ColorFactory5<TColor>(float channel0, float channel1, float channel2, float channel3, float channel4);

		private TColor[] CreateColorTestArray<TColor>(ColorFactory4<TColor> constructor, int randomColorCount)
		{
			URnd.InitState(23498234);
			Func<float> rndChannel = () => URnd.Range(0, 11) * 0.1f;
			Func<TColor> rndColor = () => constructor(rndChannel(), rndChannel(), rndChannel(), rndChannel());
			int colorCount = 21 + randomColorCount;
			TColor[] colors = new TColor[colorCount];
			colors[0] = constructor(0f, 0f, 0f, rndChannel());
			colors[1] = constructor(1f, 0f, 0f, rndChannel());
			colors[2] = constructor(0f, 1f, 0f, rndChannel());
			colors[3] = constructor(0f, 0f, 1f, rndChannel());
			colors[4] = constructor(1f, 1f, 0f, rndChannel());
			colors[5] = constructor(1f, 0f, 1f, rndChannel());
			colors[6] = constructor(0f, 1f, 1f, rndChannel());
			colors[7] = constructor(1f, 1f, 1f, rndChannel());
			colors[8] = constructor(0.5f, 0f, 0f, rndChannel());
			colors[9] = constructor(0f, 0.5f, 0f, rndChannel());
			colors[10] = constructor(0f, 0f, 0.5f, rndChannel());
			colors[11] = constructor(0.5f, 0.5f, 0f, rndChannel());
			colors[12] = constructor(0.5f, 0f, 0.5f, rndChannel());
			colors[13] = constructor(0f, 0.5f, 0.5f, rndChannel());
			colors[14] = constructor(0.5f, 0.5f, 0.5f, rndChannel());
			colors[15] = constructor(1f, 0.4f, 0.1f, rndChannel());
			colors[16] = constructor(0.1f, 1f, 0.4f, rndChannel());
			colors[17] = constructor(0.4f, 0.1f, 1f, rndChannel());
			colors[18] = constructor(0.4f, 1f, 0.1f, rndChannel());
			colors[19] = constructor(0.1f, 0.4f, 1f, rndChannel());
			colors[20] = constructor(1f, 0.1f, 0.4f, rndChannel());
			for (int i = 21; i < colorCount; ++i)
			{
				colors[i] = rndColor();
			}
			return colors;
		}

		private TColor[] CreateColorTestArray<TColor>(ColorFactory5<TColor> constructor, int randomColorCount)
		{
			URnd.InitState(23498234);
			Func<float> rndChannel = () => URnd.Range(0, 11) * 0.1f;
			Func<TColor> rndColor = () => constructor(rndChannel(), rndChannel(), rndChannel(), rndChannel(), rndChannel());
			int colorCount = 31 + randomColorCount;
			TColor[] colors = new TColor[colorCount];
			colors[0] = constructor(0f, 0f, 0f, 0f, rndChannel());
			colors[1] = constructor(1f, 0f, 0f, 0f, rndChannel());
			colors[2] = constructor(0f, 1f, 0f, 0f, rndChannel());
			colors[3] = constructor(0f, 0f, 1f, 0f, rndChannel());
			colors[4] = constructor(0f, 0f, 0f, 1f, rndChannel());
			colors[5] = constructor(1f, 1f, 0f, 0f, rndChannel());
			colors[6] = constructor(1f, 0f, 1f, 0f, rndChannel());
			colors[7] = constructor(1f, 0f, 0f, 1f, rndChannel());
			colors[8] = constructor(0f, 1f, 1f, 0f, rndChannel());
			colors[9] = constructor(0f, 1f, 0f, 1f, rndChannel());
			colors[10] = constructor(0f, 0f, 1f, 1f, rndChannel());
			colors[11] = constructor(1f, 1f, 1f, 0f, rndChannel());
			colors[12] = constructor(1f, 1f, 0f, 1f, rndChannel());
			colors[13] = constructor(1f, 0f, 1f, 1f, rndChannel());
			colors[14] = constructor(0f, 1f, 1f, 1f, rndChannel());
			colors[15] = constructor(1f, 1f, 1f, 1f, rndChannel());
			colors[16] = constructor(0.5f, 0f, 0f, 0f, rndChannel());
			colors[17] = constructor(0f, 0.5f, 0f, 0f, rndChannel());
			colors[18] = constructor(0f, 0f, 0.5f, 0f, rndChannel());
			colors[19] = constructor(0f, 0f, 0f, 0.5f, rndChannel());
			colors[20] = constructor(0.5f, 0.5f, 0f, 0f, rndChannel());
			colors[21] = constructor(0.5f, 0f, 0.5f, 0f, rndChannel());
			colors[22] = constructor(0.5f, 0f, 0f, 0.5f, rndChannel());
			colors[23] = constructor(0f, 0.5f, 0.5f, 0f, rndChannel());
			colors[24] = constructor(0f, 0.5f, 0f, 0.5f, rndChannel());
			colors[25] = constructor(0f, 0f, 0.5f, 0.5f, rndChannel());
			colors[26] = constructor(0.5f, 0.5f, 0.5f, 0f, rndChannel());
			colors[27] = constructor(0.5f, 0.5f, 0f, 0.5f, rndChannel());
			colors[28] = constructor(0.5f, 0f, 0.5f, 0.5f, rndChannel());
			colors[29] = constructor(0f, 0.5f, 0.5f, 0.5f, rndChannel());
			colors[30] = constructor(0.5f, 0.5f, 0.5f, 0.5f, rndChannel());
			for (int i = 31; i < colorCount; ++i)
			{
				colors[i] = rndColor();
			}
			return colors;
		}

		#endregion

		#region RGB Roundtrip

		[Test]
		public void Roundtrip_RGB_CMY_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorCMY>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_CMYK_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorCMYK>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_HSV_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHSV>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_HCV_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHCV>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_HSL_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHSL>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_HCL_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHCL>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_HSY_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHSY>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[Test]
		public void Roundtrip_RGB_HCY_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHCY>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		#endregion

		#region CMY Roundtrip

		[Test]
		public void Roundtrip_CMY_RGB_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, Color>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_CMYK_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_HSV_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHSV>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_HCV_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHCV>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_HSL_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHSL>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_HCL_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHCL>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_HSY_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHSY>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[Test]
		public void Roundtrip_CMY_HCY_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHCY>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		#endregion

		#region CMYK Roundtrip

		[Test]
		public void Roundtrip_CMYK_RGB_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, Color>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_CMY_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_HSV_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHSV>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_HCV_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHCV>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_HSL_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHSL>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_HCL_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHCL>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_HSY_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHSY>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[Test]
		public void Roundtrip_CMYK_HCY_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHCY>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		#endregion

		#region HSV Roundtrip

		[Test]
		public void Roundtrip_HSV_RGB_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, Color>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV((s > 0f && l > 0f) ? h : 0f, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_CMY_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorCMY>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV((s > 0f && l > 0f) ? h : 0f, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_CMYK_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorCMYK>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV((s > 0f && l > 0f) ? h : 0f, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_HCV_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHCV>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV(h, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_HSL_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHSL>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV(h, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_HCL_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHCL>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV(h, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_HSY_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHSY>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV(h, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSV_HCY_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHCY>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSV(h, l > 0f ? s : 0f, l, a).GetNearestValid(), 100));
		}

		#endregion

		#region HCV Roundtrip

		[Test]
		public void Roundtrip_HCV_RGB_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, Color>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_CMY_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorCMY>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_CMYK_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorCMYK>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_HSV_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHSV>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_HSL_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHSL>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_HCL_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHCL>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_HSY_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHSY>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCV_HCY_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHCY>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCV(h, c, l, a).GetNearestValid(), 100));
		}

		#endregion

		#region HSL Roundtrip

		[Test]
		public void Roundtrip_HSL_RGB_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, Color>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL((s > 0f && l > 0f && l < 1f) ? h : 0f, (l > 0f && l < 1f) ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_CMY_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorCMY>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL((s > 0f && l > 0f && l < 1f) ? h : 0f, (l > 0f && l < 1f) ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_CMYK_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorCMYK>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL((s > 0f && l > 0f && l < 1f) ? h : 0f, (l > 0f && l < 1f) ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_HSV_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHSV>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_HCV_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHCV>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_HCL_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHCL>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_HSY_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHSY>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSL_HCY_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHCY>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid(), 100));
		}

		#endregion

		#region HCL Roundtrip

		[Test]
		public void Roundtrip_HCL_RGB_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, Color>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_CMY_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorCMY>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_CMYK_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorCMYK>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_HSV_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHSV>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_HCV_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHCV>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_HSL_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHSL>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_HSY_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHSY>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCL_HCY_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHCY>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		#endregion

		#region HSY Roundtrip

		[Test]
		public void Roundtrip_HSY_RGB_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, Color>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY((s > 0f && y > 0f && y < 1f) ? h : 0f, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_CMY_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorCMY>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY((s > 0f && y > 0f && y < 1f) ? h : 0f, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_CMYK_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorCMYK>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY((s > 0f && y > 0f && y < 1f) ? h : 0f, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_HSV_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHSV>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_HCV_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHCV>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_HSL_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHSL>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, s, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_HCL_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHCL>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HSY_HCY_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHCY>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		#endregion

		#region HCY Roundtrip

		[Test]
		public void Roundtrip_HCY_RGB_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, Color>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(c > 0f ? h : 0f, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_CMY_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorCMY>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(c > 0f ? h : 0f, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_CMYK_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorCMYK>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(c > 0f ? h : 0f, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_HSV_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHSV>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_HCV_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHCV>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_HSL_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHSL>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_HCL_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHCL>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[Test]
		public void Roundtrip_HCY_HSY_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHSY>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		#endregion
	}
}
#endif
