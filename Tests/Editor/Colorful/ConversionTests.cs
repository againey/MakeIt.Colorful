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

#if UNITY_5_3_OR_NEWER
using NUnit.Framework;
using UnityEngine;
using System;
using System.Reflection;

using URnd = UnityEngine.Random;

namespace MakeIt.Colorful.Tests
{
	class ConversionTests
	{
		#region Private Helper Functions

		private bool FindChainedConversionOperators(Type first, Type second, Type third, ref MethodInfo firstToSecond, ref MethodInfo secondToThird)
		{
			var methods = second.GetMethods(BindingFlags.Public | BindingFlags.Static);
			foreach (var method in methods)
			{
				if (method.IsSpecialName && (method.Name == "op_Implicit" || method.Name == "op_Explicit"))
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

		private bool FindRoundtrippedConversionOperators<TFirst, TSecond, TThird>(out MethodInfo firstToSecond, out MethodInfo secondToThird, out MethodInfo thirdToFirst)
		{
			var first = typeof(TFirst);
			var second = typeof(TSecond);
			var third = typeof(TThird);
			firstToSecond = secondToThird = thirdToFirst = null;
			FindChainedConversionOperators(first, second, third, ref firstToSecond, ref secondToThird);
			if (FindChainedConversionOperators(second, third, first, ref secondToThird, ref thirdToFirst) && firstToSecond != null) return true;
			if (FindChainedConversionOperators(third, first, second, ref thirdToFirst, ref firstToSecond) && secondToThird != null) return true;
			return false;
		}

		private Func<TColor, TColor> FindGetNearestValidMethod<TColor>()
		{
			var type = typeof(TColor);

			if (type == typeof(Color))
			{
				var method = typeof(ColorExtensions).GetMethod("GetNearestValid");

				if (method == null)
				{
					Debug.LogWarning("No GetNearestValid() method found for the type " + type.Name + ".");
					return null;
				}
				else
				{
					var parameter = new object[1];
					return (TColor color) => { parameter[0] = color; return (TColor)method.Invoke(null, parameter); };
				}
			}

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			foreach (var method in methods)
			{
				if (method.Name == "GetNearestValid" && method.ReturnType == type && method.GetParameters().Length == 0)
				{
					return (TColor color) => { return (TColor)method.Invoke(color, null); };
				}
			}

			Debug.LogWarning("No GetNearestValid() method found for the type " + type.Name + ".");
			return null;
		}

		private string GetColorTypeLabel(Type type)
		{
			if (type == typeof(Color)) return "RGBA";
			string name = type.Name;
			if (name.StartsWith("Color") && name.Length > 5) return name.Substring(5);
			return name;
		}

		private void ValidateConversionRoundtrips<TFirst, TSecond>(Func<TFirst, int, float> indexer, int channelCount, float margin, params TFirst[] colors)
		{
			MethodInfo firstToSecond, secondToFirst;
			if (!FindRoundtrippedConversionOperators<TFirst, TSecond>(out firstToSecond, out secondToFirst)) Assert.Inconclusive();

			var secondGetNearestValid = FindGetNearestValidMethod<TSecond>();

			object[] parameter = new object[1];
			Func<TFirst, TFirst> roundtrip = (TFirst color) =>
			{
				parameter[0] = color;
				parameter[0] = firstToSecond.Invoke(null, parameter);
				return (TFirst)secondToFirst.Invoke(null, parameter);
			};

			Func<TFirst, TFirst> roundtripClamped = (TFirst color) =>
			{
				parameter[0] = color;
				parameter[0] = secondGetNearestValid((TSecond)firstToSecond.Invoke(null, parameter));
				return (TFirst)secondToFirst.Invoke(null, parameter);
			};

			Action<TFirst, TFirst, string> assertNearlyEqual = (TFirst initial, TFirst final, string message) =>
			{
				for (int i = 0; i < channelCount; ++i)
				{
					Assert.LessOrEqual(Mathf.Abs(indexer(initial, i) - indexer(final, i)), margin, string.Format(message, initial, final));
				}
			};

			string roundtripMessage = string.Format("Roundtrip from {{0}} through {0} to {{1}}", GetColorTypeLabel(typeof(TSecond)));
			foreach (var color in colors)
			{
				assertNearlyEqual(color, roundtrip(color), roundtripMessage);
			}

			string roundtripClampMessage = string.Format("Roundtrip from {{0}} through {0} with clamp to {{1}}", GetColorTypeLabel(typeof(TSecond)));
			if (secondGetNearestValid != null)
			{
				foreach (var color in colors)
				{
					assertNearlyEqual(color, roundtripClamped(color), roundtripClampMessage);
				}
			}
		}

		private void ValidateConversionRoundtrips<TFirst, TSecond, TThird>(Func<TFirst, int, float> indexer, int channelCount, float margin, params TFirst[] colors)
		{
			MethodInfo firstToSecond, secondToThird, thirdToFirst;
			if (!FindRoundtrippedConversionOperators<TFirst, TSecond, TThird>(out firstToSecond, out secondToThird, out thirdToFirst)) Assert.Inconclusive();

			var secondGetNearestValid = FindGetNearestValidMethod<TSecond>();
			var thirdGetNearestValid = FindGetNearestValidMethod<TThird>();

			object[] parameter = new object[1];
			Func<TFirst, TFirst> roundtrip = (TFirst color) =>
			{
				parameter[0] = color;
				parameter[0] = firstToSecond.Invoke(null, parameter);
				parameter[0] = secondToThird.Invoke(null, parameter);
				return (TFirst)thirdToFirst.Invoke(null, parameter);
			};

			Func<TFirst, TFirst> roundtripClamped = (TFirst color) =>
			{
				parameter[0] = color;
				parameter[0] = secondGetNearestValid((TSecond)firstToSecond.Invoke(null, parameter));
				parameter[0] = thirdGetNearestValid((TThird)secondToThird.Invoke(null, parameter));
				return (TFirst)thirdToFirst.Invoke(null, parameter);
			};

			Action<TFirst, TFirst, string> assertNearlyEqual = (TFirst initial, TFirst final, string message) =>
			{
				for (int i = 0; i < channelCount; ++i)
				{
					Assert.LessOrEqual(Mathf.Abs(indexer(initial, i) - indexer(final, i)), margin, string.Format(message, initial, final));
				}
			};

			string roundtripMessage = string.Format("Roundtrip from {{0}} through {0} to {{1}}", GetColorTypeLabel(typeof(TSecond)));
			foreach (var color in colors)
			{
				assertNearlyEqual(color, roundtrip(color), roundtripMessage);
			}

			string roundtripClampMessage = string.Format("Roundtrip from {{0}} through {0} with clamp to {{1}}", GetColorTypeLabel(typeof(TSecond)));
			if (secondGetNearestValid != null && thirdGetNearestValid != null)
			{
				foreach (var color in colors)
				{
					assertNearlyEqual(color, roundtripClamped(color), roundtripClampMessage);
				}
			}
		}

		private delegate TColor ColorFactory4<TColor>(float channel0, float channel1, float channel2, float channel3);
		private delegate TColor ColorFactory5<TColor>(float channel0, float channel1, float channel2, float channel3, float channel4);

		private TColor[] CreateColorTestArray<TColor>(ColorFactory4<TColor> constructor, int randomColorCount)
		{
#if UNITY_5_4_OR_NEWER
			URnd.InitState(23498234);
#else
			URnd.seed = 23498234;
#endif
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
#if UNITY_5_4_OR_NEWER
			URnd.InitState(23498234);
#else
			URnd.seed = 23498234;
#endif
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

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_CMY_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorCMY>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_CMYK_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorCMYK>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_HSV_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHSV>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_HCV_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHCV>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_HSL_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHSL>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_HCL_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHCL>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_HSY_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHSY>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_HCY_RGB()
		{
			ValidateConversionRoundtrips<Color, ColorHCY>(
				(Color color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_RGB_TwoStep_RGB()
		{
			var colors = CreateColorTestArray((float r, float g, float b, float a) => new Color(r, g, b, a), 100);

			ValidateConversionRoundtrips<Color, ColorCMY, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorCMY>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSV, ColorHCV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCV, ColorHSV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSL, ColorHCL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCL, ColorHSL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSY, ColorHCY>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCY, ColorHSY>((Color color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<Color, ColorHSV, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorHSV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCV, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorHCV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSL, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorHSL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCL, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorHCL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSY, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorHSY>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCY, ColorCMYK>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorCMYK, ColorHCY>((Color color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<Color, ColorHSV, ColorHCL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCV, ColorHSL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSL, ColorHCV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCL, ColorHSV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSL, ColorHCY>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCL, ColorHSY>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSY, ColorHCL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCY, ColorHSL>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSY, ColorHCV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCY, ColorHSV>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHSV, ColorHCY>((Color color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<Color, ColorHCV, ColorHSY>((Color color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region CMY Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_RGB_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, Color>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_CMYK_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_HSV_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHSV>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_HCV_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHCV>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_HSL_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHSL>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_HCL_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHCL>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_HSY_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHSY>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_HCY_CMY()
		{
			ValidateConversionRoundtrips<ColorCMY, ColorHCY>(
				(ColorCMY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMY_TwoStep_CMY()
		{
			var colors = CreateColorTestArray((float c, float m, float y, float a) => new ColorCMY(c, m, y, a), 100);

			ValidateConversionRoundtrips<ColorCMY, Color, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, Color>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSV, ColorHCV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCV, ColorHSV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSL, ColorHCL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCL, ColorHSL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSY, ColorHCY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCY, ColorHSY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorCMY, ColorHSV, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, ColorHSV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCV, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, ColorHCV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSL, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, ColorHSL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCL, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, ColorHCL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSY, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, ColorHSY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCY, ColorCMYK>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorCMYK, ColorHCY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorCMY, ColorHSV, ColorHCL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCV, ColorHSL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSL, ColorHCV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCL, ColorHSV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSL, ColorHCY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCL, ColorHSY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSY, ColorHCL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCY, ColorHSL>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSY, ColorHCV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCY, ColorHSV>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHSV, ColorHCY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMY, ColorHCV, ColorHSY>((ColorCMY color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region CMYK Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_RGB_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, Color>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_CMY_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_HSV_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHSV>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_HCV_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHCV>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_HSL_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHSL>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_HCL_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHCL>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_HSY_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHSY>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_HCY_CMYK()
		{
			ValidateConversionRoundtrips<ColorCMYK, ColorHCY>(
				(ColorCMYK color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_CMYK_TwoStep_CMYK()
		{
			var colors = CreateColorTestArray((float c, float m, float y, float k, float a) => new ColorCMYK(c, m, y, k, a).GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorCMYK, Color, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, Color>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSV, ColorHCV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCV, ColorHSV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSL, ColorHCL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCL, ColorHSL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSY, ColorHCY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCY, ColorHSY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorCMYK, ColorHSV, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, ColorHSV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCV, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, ColorHCV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSL, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, ColorHSL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCL, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, ColorHCL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSY, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, ColorHSY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCY, ColorCMY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorCMY, ColorHCY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorCMYK, ColorHSV, ColorHCL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCV, ColorHSL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSL, ColorHCV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCL, ColorHSV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSL, ColorHCY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCL, ColorHSY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSY, ColorHCL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCY, ColorHSL>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSY, ColorHCV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCY, ColorHSV>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHSV, ColorHCY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorCMYK, ColorHCV, ColorHSY>((ColorCMYK color, int index) => color[index], 5, 0.0001f, colors);
		}

		#endregion

		#region HSV Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_RGB_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, Color>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV((s > 0f && v > 0f) ? h : 0f, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_CMY_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorCMY>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV((s > 0f && v > 0f) ? h : 0f, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_CMYK_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorCMYK>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV((s > 0f && v > 0f) ? h : 0f, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_HCV_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHCV>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV(h, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_HSL_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHSL>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV(h, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_HCL_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHCL>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV(h, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_HSY_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHSY>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV(h, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_HCY_HSV()
		{
			ValidateConversionRoundtrips<ColorHSV, ColorHCY>(
				(ColorHSV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV(h, v > 0f ? s : 0f, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSV_TwoStep_HSV()
		{
			var colors = CreateColorTestArray((float h, float s, float v, float a) => new ColorHSV(h, s, v, a).GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorHSV, Color, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, Color>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, Color, ColorHCV>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCV, Color>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSL, ColorHCL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCL, ColorHSL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSY, ColorHCY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCY, ColorHSY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHSV, Color, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, Color>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCV, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, ColorHCV>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSL, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, ColorHSL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCL, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, ColorHCL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSY, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, ColorHSY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCY, ColorCMY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorCMY, ColorHCY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHSV, Color, ColorHCL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCV, ColorHSL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSL, ColorHCV>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCL, Color>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSL, ColorHCY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCL, ColorHSY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSY, ColorHCL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCY, ColorHSL>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHSY, ColorHCV>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCY, Color>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, Color, ColorHCY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSV, ColorHCV, ColorHSY>((ColorHSV color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region HCV Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_RGB_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, Color>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(c > 0f ? h : 0f, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_CMY_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorCMY>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(c > 0f ? h : 0f, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_CMYK_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorCMYK>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(c > 0f ? h : 0f, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_HSV_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHSV>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(h, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_HSL_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHSL>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(h, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_HCL_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHCL>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(h, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_HSY_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHSY>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(h, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_HCY_HCV()
		{
			ValidateConversionRoundtrips<ColorHCV, ColorHCY>(
				(ColorHCV color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(h, c, v, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCV_TwoStep_HCV()
		{
			var colors = CreateColorTestArray((float h, float c, float v, float a) => new ColorHCV(h, c, v, a).GetNearestValid().GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorHCV, ColorHSV, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, ColorHSV>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSV, Color>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, Color, ColorHSV>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSL, ColorHCL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCL, ColorHSL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSY, ColorHCY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCY, ColorHSY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHCV, ColorHSV, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, ColorHSV>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, Color, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, Color>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSL, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, ColorHSL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCL, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, ColorHCL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSY, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, ColorHSY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCY, ColorCMY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorCMY, ColorHCY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHCV, ColorHSV, ColorHCL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, Color, ColorHSL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSL, Color>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCL, ColorHSV>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSL, ColorHCY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCL, ColorHSY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSY, ColorHCL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCY, ColorHSL>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSY, Color>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHCY, ColorHSV>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, ColorHSV, ColorHCY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCV, Color, ColorHSY>((ColorHCV color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region HSL Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_RGB_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, Color>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL((s > 0f && l > 0f && l < 1f) ? h : 0f, (l > 0f && l < 1f) ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_CMY_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorCMY>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL((s > 0f && l > 0f && l < 1f) ? h : 0f, (l > 0f && l < 1f) ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_CMYK_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorCMYK>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL((s > 0f && l > 0f && l < 1f) ? h : 0f, (l > 0f && l < 1f) ? s : 0f, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_HSV_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHSV>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid().GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_HCV_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHCV>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid().GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_HCL_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHCL>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid().GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_HSY_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHSY>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid().GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_HCY_HSL()
		{
			ValidateConversionRoundtrips<ColorHSL, ColorHCY>(
				(ColorHSL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetNearestValid().GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSL_TwoStep_HSL()
		{
			var colors = CreateColorTestArray((float h, float s, float l, float a) => new ColorHSL(h, s, l, a).GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorHSL, ColorHSV, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, ColorHSV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHSV, ColorHCV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCV, ColorHSV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, Color, ColorHCL>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCL, Color>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHSY, ColorHCY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCY, ColorHSY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHSL, ColorHSV, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, ColorHSV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCV, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, ColorHCV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, Color, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, Color>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCL, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, ColorHCL>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHSY, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, ColorHSY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCY, ColorCMY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorCMY, ColorHCY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHSL, ColorHSV, ColorHCL>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCV, Color>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, Color, ColorHCV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCL, ColorHSV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, Color, ColorHCY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCL, ColorHSY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHSY, ColorHCL>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCY, Color>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHSY, ColorHCV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCY, ColorHSV>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHSV, ColorHCY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSL, ColorHCV, ColorHSY>((ColorHSL color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region HCL Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_RGB_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, Color>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_CMY_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorCMY>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_CMYK_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorCMYK>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(c > 0f ? h : 0f, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_HSV_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHSV>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_HCV_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHCV>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_HSL_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHSL>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_HSY_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHSY>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_HCY_HCL()
		{
			ValidateConversionRoundtrips<ColorHCL, ColorHCY>(
				(ColorHCL color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCL_TwoStep_HCL()
		{
			var colors = CreateColorTestArray((float h, float c, float l, float a) => new ColorHCL(h, c, l, a).GetNearestValid().GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorHCL, ColorHSV, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, ColorHSV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSV, ColorHCV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCV, ColorHSV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSL, Color>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, Color, ColorHSL>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSY, ColorHCY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCY, ColorHSY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHCL, ColorHSV, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, ColorHSV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCV, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, ColorHCV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSL, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, ColorHSL>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, Color, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, Color>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSY, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, ColorHSY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCY, ColorCMY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorCMY, ColorHCY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHCL, ColorHSV, Color>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCV, ColorHSL>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSL, ColorHCV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, Color, ColorHSV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSL, ColorHCY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, Color, ColorHSY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSY, Color>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCY, ColorHSL>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSY, ColorHCV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCY, ColorHSV>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHSV, ColorHCY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCL, ColorHCV, ColorHSY>((ColorHCL color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region HSY Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_RGB_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, Color>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY((s > 0f && y > 0f && y < 1f) ? h : 0f, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_CMY_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorCMY>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY((s > 0f && y > 0f && y < 1f) ? h : 0f, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_CMYK_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorCMYK>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY((s > 0f && y > 0f && y < 1f) ? h : 0f, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_HSV_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHSV>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_HCV_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHCV>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_HSL_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHSL>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, s, y, a).GetNearestValid().GetCanonical(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_HCL_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHCL>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_HCY_HSY()
		{
			ValidateConversionRoundtrips<ColorHSY, ColorHCY>(
				(ColorHSY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, (y > 0f && y < 1f) ? s : 0f, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HSY_TwoStep_HSY()
		{
			var colors = CreateColorTestArray((float h, float s, float y, float a) => new ColorHSY(h, s, y, a).GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorHSY, ColorHSV, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, ColorHSV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHSV, ColorHCV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCV, ColorHSV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHSL, ColorHCL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCL, ColorHSL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, Color, ColorHCY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCY, Color>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHSY, ColorHSV, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, ColorHSV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCV, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, ColorHCV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHSL, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, ColorHSL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCL, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, ColorHCL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, Color, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, Color>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCY, ColorCMY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorCMY, ColorHCY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHSY, ColorHSV, ColorHCL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCV, ColorHSL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHSL, ColorHCV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCL, ColorHSV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHSL, ColorHCY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCL, Color>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, Color, ColorHCL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCY, ColorHSL>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, Color, ColorHCV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCY, ColorHSV>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHSV, ColorHCY>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHSY, ColorHCV, Color>((ColorHSY color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion

		#region HCY Roundtrip

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_RGB_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, Color>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(c > 0f ? h : 0f, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_CMY_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorCMY>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(c > 0f ? h : 0f, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_CMYK_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorCMYK>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(c > 0f ? h : 0f, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_HSV_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHSV>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_HCV_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHCV>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_HSL_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHSL>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_HCL_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHCL>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_HSY_HCY()
		{
			ValidateConversionRoundtrips<ColorHCY, ColorHSY>(
				(ColorHCY color, int index) => color[index], 4, 0.0001f,
				CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid(), 100));
		}

		[TestCase(Category = "Normal")]
		public void Roundtrip_HCY_TwoStep_HCY()
		{
			var colors = CreateColorTestArray((float h, float c, float y, float a) => new ColorHCY(h, c, y, a).GetNearestValid().GetCanonical(), 100);

			ValidateConversionRoundtrips<ColorHCY, ColorHSV, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, ColorHSV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSV, ColorHCV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCV, ColorHSV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSL, ColorHCL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCL, ColorHSL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSY, Color>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, Color, ColorHSY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHCY, ColorHSV, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, ColorHSV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCV, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, ColorHCV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSL, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, ColorHSL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCL, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, ColorHCL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSY, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, ColorHSY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, Color, ColorCMY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorCMY, Color>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);

			ValidateConversionRoundtrips<ColorHCY, ColorHSV, ColorHCL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCV, ColorHSL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSL, ColorHCV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCL, ColorHSV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSL, Color>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCL, ColorHSY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSY, ColorHCL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, Color, ColorHSL>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSY, ColorHCV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, Color, ColorHSV>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHSV, Color>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
			ValidateConversionRoundtrips<ColorHCY, ColorHCV, ColorHSY>((ColorHCY color, int index) => color[index], 4, 0.0001f, colors);
		}

		#endregion
	}
}
#endif
