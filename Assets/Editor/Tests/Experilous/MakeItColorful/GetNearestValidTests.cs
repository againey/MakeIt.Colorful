/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

#if UNITY_5_3_OR_NEWER
using NUnit.Framework;
using UnityEngine;

namespace Experilous.MakeItColorful.Tests
{
	class GetNearestValidTests
	{
		private static void AssertNearest(Color expected, Color original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.r - nearest.r), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.g - nearest.g), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.b - nearest.b), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_RGB()
		{
			AssertNearest(new Color(0f, 0f, 0f, 0.25f), new Color(-1f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 0f, 0f, 0.25f), new Color(2f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 1f, 0f, 0.25f), new Color(-1f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 0f, 1f, 0.25f), new Color(-1f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 1f, 0f, 0.25f), new Color(2f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 1f, 1f, 0.25f), new Color(-1f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 0f, 1f, 0.25f), new Color(2f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 1f, 1f, 0.25f), new Color(2f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new Color(0.875f, 0f, 0f, 0.25f), new Color(0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0.875f, 1f, 0f, 0.25f), new Color(0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0.875f, 0f, 1f, 0.25f), new Color(0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(0.875f, 1f, 1f, 0.25f), new Color(0.875f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 0.875f, 0f, 0.25f), new Color(-1f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 0.875f, 0f, 0.25f), new Color(2f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 0.875f, 1f, 0.25f), new Color(-1f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 0.875f, 1f, 0.25f), new Color(2f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 0f, 0.875f, 0.25f), new Color(-1f, -1f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 0f, 0.875f, 0.25f), new Color(2f, -1f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 1f, 0.875f, 0.25f), new Color(-1f, 2f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 1f, 0.875f, 0.25f), new Color(2f, 2f, 0.875f, 0.25f), 0.0001f);

			AssertNearest(new Color(0.875f, 0.625f, 0f, 0.25f), new Color(0.875f, 0.625f, -1f, 0.25f), 0.0001f);
			AssertNearest(new Color(0.875f, 0.625f, 1f, 0.25f), new Color(0.875f, 0.625f, 2f, 0.25f), 0.0001f);
			AssertNearest(new Color(0f, 0.875f, 0.625f, 0.25f), new Color(-1f, 0.875f, 0.625f, 0.25f), 0.0001f);
			AssertNearest(new Color(1f, 0.875f, 0.625f, 0.25f), new Color(2f, 0.875f, 0.625f, 0.25f), 0.0001f);
			AssertNearest(new Color(0.625f, 0f, 0.875f, 0.25f), new Color(0.625f, -1f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new Color(0.625f, 1f, 0.875f, 0.25f), new Color(0.625f, 2f, 0.875f, 0.25f), 0.0001f);

			AssertNearest(new Color(0.875f, 0.625f, 0.375f, 0.25f), new Color(0.875f, 0.625f, 0.375f, 0.25f), 0.0001f, true);
		}

		private static void AssertNearest(ColorCMY expected, ColorCMY original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.c - nearest.c), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.m - nearest.m), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.y - nearest.y), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_CMY()
		{
			AssertNearest(new ColorCMY(0f, 0f, 0f, 0.25f), new ColorCMY(-1f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 0f, 0f, 0.25f), new ColorCMY(2f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 1f, 0f, 0.25f), new ColorCMY(-1f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 0f, 1f, 0.25f), new ColorCMY(-1f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 1f, 0f, 0.25f), new ColorCMY(2f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 1f, 1f, 0.25f), new ColorCMY(-1f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 0f, 1f, 0.25f), new ColorCMY(2f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 1f, 1f, 0.25f), new ColorCMY(2f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMY(0.875f, 0f, 0f, 0.25f), new ColorCMY(0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0.875f, 1f, 0f, 0.25f), new ColorCMY(0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0.875f, 0f, 1f, 0.25f), new ColorCMY(0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0.875f, 1f, 1f, 0.25f), new ColorCMY(0.875f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 0.875f, 0f, 0.25f), new ColorCMY(-1f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 0.875f, 0f, 0.25f), new ColorCMY(2f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 0.875f, 1f, 0.25f), new ColorCMY(-1f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 0.875f, 1f, 0.25f), new ColorCMY(2f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 0f, 0.875f, 0.25f), new ColorCMY(-1f, -1f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 0f, 0.875f, 0.25f), new ColorCMY(2f, -1f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 1f, 0.875f, 0.25f), new ColorCMY(-1f, 2f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 1f, 0.875f, 0.25f), new ColorCMY(2f, 2f, 0.875f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMY(0.875f, 0.625f, 0f, 0.25f), new ColorCMY(0.875f, 0.625f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0.875f, 0.625f, 1f, 0.25f), new ColorCMY(0.875f, 0.625f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0f, 0.875f, 0.625f, 0.25f), new ColorCMY(-1f, 0.875f, 0.625f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(1f, 0.875f, 0.625f, 0.25f), new ColorCMY(2f, 0.875f, 0.625f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0.625f, 0f, 0.875f, 0.25f), new ColorCMY(0.625f, -1f, 0.875f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMY(0.625f, 1f, 0.875f, 0.25f), new ColorCMY(0.625f, 2f, 0.875f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMY(0.875f, 0.625f, 0.375f, 0.25f), new ColorCMY(0.875f, 0.625f, 0.375f, 0.25f), 0.0001f, true);
		}

		private static void AssertNearest(ColorCMYK expected, ColorCMYK original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.c - nearest.c), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.m - nearest.m), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.y - nearest.y), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.k - nearest.k), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_CMYK()
		{
			AssertNearest(new ColorCMYK(0f, 0f, 0f, 0f, 0.25f), new ColorCMYK(-1f, -1f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 0f, 0f, 0.25f), new ColorCMYK(2f, -1f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 0f, 0f, 0.25f), new ColorCMYK(-1f, 2f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0f, 1f, 0f, 0.25f), new ColorCMYK(-1f, -1f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 0f, 0f, 0.25f), new ColorCMYK(2f, 2f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 1f, 0f, 0.25f), new ColorCMYK(-1f, 2f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 1f, 0f, 0.25f), new ColorCMYK(2f, -1f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 1f, 0f, 0.25f), new ColorCMYK(2f, 2f, 2f, -1f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0f, 0f, 0f, 1f, 0.25f), new ColorCMYK(-1f, -1f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 0f, 1f, 0.25f), new ColorCMYK(2f, -1f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 0f, 1f, 0.25f), new ColorCMYK(-1f, 2f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0f, 1f, 1f, 0.25f), new ColorCMYK(-1f, -1f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 0f, 1f, 0.25f), new ColorCMYK(2f, 2f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 1f, 1f, 0.25f), new ColorCMYK(-1f, 2f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 1f, 1f, 0.25f), new ColorCMYK(2f, -1f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 1f, 1f, 0.25f), new ColorCMYK(2f, 2f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0f, 0f, 0f, 0.25f), new ColorCMYK(0.875f, -1f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 1f, 0f, 0f, 0.25f), new ColorCMYK(0.875f, 2f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 0f, 1f, 0f, 0.25f), new ColorCMYK(0.875f, -1f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 1f, 1f, 0f, 0.25f), new ColorCMYK(0.875f, 2f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 0f, 0f, 0.25f), new ColorCMYK(-1f, 0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 0f, 0f, 0.25f), new ColorCMYK(2f, 0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 1f, 0f, 0.25f), new ColorCMYK(-1f, 0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 1f, 0f, 0.25f), new ColorCMYK(2f, 0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0f, 0.875f, 0f, 0.25f), new ColorCMYK(-1f, -1f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 0.875f, 0f, 0.25f), new ColorCMYK(2f, -1f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 0.875f, 0f, 0.25f), new ColorCMYK(-1f, 2f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 0.875f, 0f, 0.25f), new ColorCMYK(2f, 2f, 0.875f, -1f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0f, 0f, 1f, 0.25f), new ColorCMYK(0.875f, -1f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 1f, 0f, 1f, 0.25f), new ColorCMYK(0.875f, 2f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 0f, 1f, 1f, 0.25f), new ColorCMYK(0.875f, -1f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 1f, 1f, 1f, 0.25f), new ColorCMYK(0.875f, 2f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 0f, 1f, 0.25f), new ColorCMYK(-1f, 0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 0f, 1f, 0.25f), new ColorCMYK(2f, 0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 1f, 1f, 0.25f), new ColorCMYK(-1f, 0.875f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 1f, 1f, 0.25f), new ColorCMYK(2f, 0.875f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0f, 0.875f, 1f, 0.25f), new ColorCMYK(-1f, -1f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 0.875f, 1f, 0.25f), new ColorCMYK(2f, -1f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 0.875f, 1f, 0.25f), new ColorCMYK(-1f, 2f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 0.875f, 1f, 0.25f), new ColorCMYK(2f, 2f, 0.875f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0f, 0f, 0f, 0.125f, 0.25f), new ColorCMYK(-1f, -1f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 0f, 0.125f, 0.25f), new ColorCMYK(2f, -1f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 0f, 0.125f, 0.25f), new ColorCMYK(-1f, 2f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0f, 1f, 0.125f, 0.25f), new ColorCMYK(-1f, -1f, 2f, 0.125f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0.625f, 0f, 0f, 0.25f), new ColorCMYK(0.875f, 0.625f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 0.625f, 1f, 0f, 0.25f), new ColorCMYK(0.875f, 0.625f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 0.625f, 0f, 0.25f), new ColorCMYK(-1f, 0.875f, 0.625f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 0.625f, 0f, 0.25f), new ColorCMYK(2f, 0.875f, 0.625f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.625f, 0f, 0.875f, 0f, 0.25f), new ColorCMYK(0.625f, -1f, 0.875f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.625f, 1f, 0.875f, 0f, 0.25f), new ColorCMYK(0.625f, 2f, 0.875f, -1f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0.625f, 0f, 1f, 0.25f), new ColorCMYK(0.875f, 0.625f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 0.625f, 1f, 1f, 0.25f), new ColorCMYK(0.875f, 0.625f, 2f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 0.625f, 1f, 0.25f), new ColorCMYK(-1f, 0.875f, 0.625f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 0.625f, 1f, 0.25f), new ColorCMYK(2f, 0.875f, 0.625f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.625f, 0f, 0.875f, 1f, 0.25f), new ColorCMYK(0.625f, -1f, 0.875f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.625f, 1f, 0.875f, 1f, 0.25f), new ColorCMYK(0.625f, 2f, 0.875f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0f, 0f, 0.125f, 0.25f), new ColorCMYK(0.875f, -1f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 1f, 0f, 0.125f, 0.25f), new ColorCMYK(0.875f, 2f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 0f, 1f, 0.125f, 0.25f), new ColorCMYK(0.875f, -1f, 2f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 1f, 1f, 0.125f, 0.25f), new ColorCMYK(0.875f, 2f, 2f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 0f, 0.125f, 0.25f), new ColorCMYK(-1f, 0.875f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 0f, 0.125f, 0.25f), new ColorCMYK(2f, 0.875f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 1f, 0.125f, 0.25f), new ColorCMYK(-1f, 0.875f, 2f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 1f, 0.125f, 0.25f), new ColorCMYK(2f, 0.875f, 2f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0f, 0.875f, 0.125f, 0.25f), new ColorCMYK(-1f, -1f, 0.875f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0f, 0.875f, 0.125f, 0.25f), new ColorCMYK(2f, -1f, 0.875f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 1f, 0.875f, 0.125f, 0.25f), new ColorCMYK(-1f, 2f, 0.875f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 1f, 0.875f, 0.125f, 0.25f), new ColorCMYK(2f, 2f, 0.875f, 0.125f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0.625f, 0.375f, 0f, 0.25f), new ColorCMYK(0.875f, 0.625f, 0.375f, -1f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0.625f, 0.375f, 1f, 0.25f), new ColorCMYK(0.875f, 0.625f, 0.375f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.875f, 0.625f, 0f, 0.125f, 0.25f), new ColorCMYK(0.875f, 0.625f, -1f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.875f, 0.625f, 1f, 0.125f, 0.25f), new ColorCMYK(0.875f, 0.625f, 2f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0f, 0.875f, 0.625f, 0.125f, 0.25f), new ColorCMYK(-1f, 0.875f, 0.625f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(1f, 0.875f, 0.625f, 0.125f, 0.25f), new ColorCMYK(2f, 0.875f, 0.625f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.625f, 0f, 0.875f, 0.125f, 0.25f), new ColorCMYK(0.625f, -1f, 0.875f, 0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorCMYK(0.625f, 1f, 0.875f, 0.125f, 0.25f), new ColorCMYK(0.625f, 2f, 0.875f, 0.125f, 0.25f), 0.0001f);

			AssertNearest(new ColorCMYK(0.625f, 0.375f, 0.875f, 0.125f, 0.25f), new ColorCMYK(0.625f, 0.375f, 0.875f, 0.125f, 0.25f), 0.0001f, true);
		}

		private static void AssertNearest(ColorHSV expected, ColorHSV original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.h - nearest.h), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.s - nearest.s), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.v - nearest.v), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_HSV()
		{
			AssertNearest(new ColorHSV(0.875f, 0f, 0f, 0.25f), new ColorHSV(-0.125f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 0f, 0.25f), new ColorHSV(-0.125f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 1f, 0f, 0.25f), new ColorHSV(-0.125f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0f, 0.5f, 0.25f), new ColorHSV(-0.125f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSV(-0.125f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSV(0.875f, 1f, 0.5f, 0.25f), new ColorHSV(-0.125f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0f, 1f, 0.25f), new ColorHSV(-0.125f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 1f, 0.25f), new ColorHSV(-0.125f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 1f, 1f, 0.25f), new ColorHSV(-0.125f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorHSV(0.875f, 0f, 0f, 0.25f), new ColorHSV(0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 0f, 0.25f), new ColorHSV(0.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 1f, 0f, 0.25f), new ColorHSV(0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0f, 0.5f, 0.25f), new ColorHSV(0.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSV(0.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSV(0.875f, 1f, 0.5f, 0.25f), new ColorHSV(0.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0f, 1f, 0.25f), new ColorHSV(0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 1f, 0.25f), new ColorHSV(0.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 1f, 1f, 0.25f), new ColorHSV(0.875f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorHSV(0.875f, 0f, 0f, 0.25f), new ColorHSV(1.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 0f, 0.25f), new ColorHSV(1.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 1f, 0f, 0.25f), new ColorHSV(1.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0f, 0.5f, 0.25f), new ColorHSV(1.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSV(1.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSV(0.875f, 1f, 0.5f, 0.25f), new ColorHSV(1.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0f, 1f, 0.25f), new ColorHSV(1.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 0.5f, 1f, 0.25f), new ColorHSV(1.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSV(0.875f, 1f, 1f, 0.25f), new ColorHSV(1.875f, 2f, 2f, 0.25f), 0.0001f);
		}

		private static void AssertNearest(ColorHCV expected, ColorHCV original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.h - nearest.h), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.c - nearest.c), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.v - nearest.v), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_HCV()
		{
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(-0.125f, -1f, -0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(-0.125f, -0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(-0.125f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCV(-0.125f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(-0.125f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(-0.125f, 2f, 1.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(-0.125f, 1.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 1f, 0.25f), new ColorHCV(-0.125f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 1f, 0.25f), new ColorHCV(-0.125f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0.5f, 0.25f), new ColorHCV(-0.125f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCV(-0.125f, 0.5f, 0.5f, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(0.875f, -1f, -0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(0.875f, -0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(0.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCV(0.875f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(0.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(0.875f, 2f, 1.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(0.875f, 1.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 1f, 0.25f), new ColorHCV(0.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 1f, 0.25f), new ColorHCV(0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0.5f, 0.25f), new ColorHCV(0.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(1.875f, -1f, -0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(1.875f, -0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0f, 0.25f), new ColorHCV(1.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCV(1.875f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(1.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(1.875f, 2f, 1.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 1f, 1f, 0.25f), new ColorHCV(1.875f, 1.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 1f, 0.25f), new ColorHCV(1.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 1f, 0.25f), new ColorHCV(1.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0f, 0.5f, 0.25f), new ColorHCV(1.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCV(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCV(1.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
		}

		private static void AssertNearest(ColorHSL expected, ColorHSL original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.h - nearest.h), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.s - nearest.s), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.l - nearest.l), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_HSL()
		{
			AssertNearest(new ColorHSL(0.875f, 0f, 0f, 0.25f), new ColorHSL(-0.125f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 0f, 0.25f), new ColorHSL(-0.125f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 1f, 0f, 0.25f), new ColorHSL(-0.125f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0f, 0.5f, 0.25f), new ColorHSL(-0.125f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSL(-0.125f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSL(0.875f, 1f, 0.5f, 0.25f), new ColorHSL(-0.125f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0f, 1f, 0.25f), new ColorHSL(-0.125f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 1f, 0.25f), new ColorHSL(-0.125f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 1f, 1f, 0.25f), new ColorHSL(-0.125f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorHSL(0.875f, 0f, 0f, 0.25f), new ColorHSL(0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 0f, 0.25f), new ColorHSL(0.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 1f, 0f, 0.25f), new ColorHSL(0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0f, 0.5f, 0.25f), new ColorHSL(0.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSL(0.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSL(0.875f, 1f, 0.5f, 0.25f), new ColorHSL(0.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0f, 1f, 0.25f), new ColorHSL(0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 1f, 0.25f), new ColorHSL(0.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 1f, 1f, 0.25f), new ColorHSL(0.875f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorHSL(0.875f, 0f, 0f, 0.25f), new ColorHSL(1.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 0f, 0.25f), new ColorHSL(1.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 1f, 0f, 0.25f), new ColorHSL(1.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0f, 0.5f, 0.25f), new ColorHSL(1.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSL(1.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSL(0.875f, 1f, 0.5f, 0.25f), new ColorHSL(1.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0f, 1f, 0.25f), new ColorHSL(1.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 0.5f, 1f, 0.25f), new ColorHSL(1.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSL(0.875f, 1f, 1f, 0.25f), new ColorHSL(1.875f, 2f, 2f, 0.25f), 0.0001f);
		}

		private static void AssertNearest(ColorHCL expected, ColorHCL original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.h - nearest.h), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.c - nearest.c), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.l - nearest.l), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_HCL()
		{
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(-0.125f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(-0.125f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(-0.125f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.8f, 0.4f, 0.25f), new ColorHCL(-0.125f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(-0.125f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(-0.125f, 1.5f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(-0.125f, 1.5f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.8f, 0.6f, 0.25f), new ColorHCL(-0.125f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(-0.125f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(-0.125f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(-0.125f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0.5f, 0.25f), new ColorHCL(-0.125f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCL(-0.125f, 0.5f, 0.5f, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(0.875f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(0.875f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(0.875f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.8f, 0.4f, 0.25f), new ColorHCL(0.875f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(0.875f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(0.875f, 1.5f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(0.875f, 1.5f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.8f, 0.6f, 0.25f), new ColorHCL(0.875f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(0.875f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(0.875f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(0.875f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0.5f, 0.25f), new ColorHCL(0.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCL(0.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(1.875f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(1.875f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0f, 0.25f), new ColorHCL(1.875f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.8f, 0.4f, 0.25f), new ColorHCL(1.875f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(1.875f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(1.875f, 1.5f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 1f, 0.5f, 0.25f), new ColorHCL(1.875f, 1.5f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.8f, 0.6f, 0.25f), new ColorHCL(1.875f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(1.875f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(1.875f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 1f, 0.25f), new ColorHCL(1.875f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0f, 0.5f, 0.25f), new ColorHCL(1.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCL(0.875f, 0.5f, 0.5f, 0.25f), new ColorHCL(1.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
		}

		private static void AssertNearest(ColorHSY expected, ColorHSY original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.h - nearest.h), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.s - nearest.s), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.y - nearest.y), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_HSY()
		{
			AssertNearest(new ColorHSY(0.875f, 0f, 0f, 0.25f), new ColorHSY(-0.125f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 0f, 0.25f), new ColorHSY(-0.125f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 1f, 0f, 0.25f), new ColorHSY(-0.125f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0f, 0.5f, 0.25f), new ColorHSY(-0.125f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSY(-0.125f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSY(0.875f, 1f, 0.5f, 0.25f), new ColorHSY(-0.125f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0f, 1f, 0.25f), new ColorHSY(-0.125f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 1f, 0.25f), new ColorHSY(-0.125f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 1f, 1f, 0.25f), new ColorHSY(-0.125f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorHSY(0.875f, 0f, 0f, 0.25f), new ColorHSY(0.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 0f, 0.25f), new ColorHSY(0.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 1f, 0f, 0.25f), new ColorHSY(0.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0f, 0.5f, 0.25f), new ColorHSY(0.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSY(0.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSY(0.875f, 1f, 0.5f, 0.25f), new ColorHSY(0.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0f, 1f, 0.25f), new ColorHSY(0.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 1f, 0.25f), new ColorHSY(0.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 1f, 1f, 0.25f), new ColorHSY(0.875f, 2f, 2f, 0.25f), 0.0001f);

			AssertNearest(new ColorHSY(0.875f, 0f, 0f, 0.25f), new ColorHSY(1.875f, -1f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 0f, 0.25f), new ColorHSY(1.875f, 0.5f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 1f, 0f, 0.25f), new ColorHSY(1.875f, 2f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0f, 0.5f, 0.25f), new ColorHSY(1.875f, -1f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 0.5f, 0.25f), new ColorHSY(1.875f, 0.5f, 0.5f, 0.25f), 0.0001f, true);
			AssertNearest(new ColorHSY(0.875f, 1f, 0.5f, 0.25f), new ColorHSY(1.875f, 2f, 0.5f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0f, 1f, 0.25f), new ColorHSY(1.875f, -1f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 0.5f, 1f, 0.25f), new ColorHSY(1.875f, 0.5f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHSY(0.875f, 1f, 1f, 0.25f), new ColorHSY(1.875f, 2f, 2f, 0.25f), 0.0001f);
		}

		private static void AssertNearest(ColorHCY expected, ColorHCY original, float margin, bool isValid = false)
		{
			Assert.AreEqual(isValid, original.IsValid(), original.ToString());
			var nearest = original.GetNearestValid();
			var message = string.Format("Expected {0} but got {1}", expected, original);
			Assert.LessOrEqual(Mathf.Abs(expected.h - nearest.h), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.c - nearest.c), margin, message);
			Assert.LessOrEqual(Mathf.Abs(expected.y - nearest.y), margin, message);
			Assert.AreEqual(expected.a, nearest.a, message);
		}

		[Test]
		public void GetNearestValid_HCY()
		{
			float hueAtHalfLuma = (0.5f - Detail.LumaUtility.rWeight) / (Detail.LumaUtility.gWeight * 6f);
			float halfLuma = 0.5f;

			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.8f, 0.4f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 1.5f, halfLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 1.5f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.8f, 0.6f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, -1f, halfLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.5f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma - 1f, 0.5f, halfLuma, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.8f, 0.4f, 0.25f), new ColorHCY(hueAtHalfLuma, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma, 1.5f, halfLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma, 1.5f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.8f, 0.6f, 0.25f), new ColorHCY(hueAtHalfLuma, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma, -1f, halfLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.5f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma, 0.5f, halfLuma, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.8f, 0.4f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 1.5f, halfLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 1f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 1.5f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.8f, 0.6f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, -1f, halfLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtHalfLuma, 0.5f, halfLuma, 0.25f), new ColorHCY(hueAtHalfLuma + 1f, 0.5f, halfLuma, 0.25f), 0.0001f, true);

			float hueAtMinLuma = 4f / 6f;
			float minLuma = Detail.LumaUtility.bWeight;
			float minLower = 1f - minLuma * minLuma / (1f + minLuma * minLuma);
			float minUpper = 1f - (1f - minLuma) * (1f - minLuma) / (1f + (1f - minLuma) * (1f - minLuma));

			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma - 1f, -1f, -0.03125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma - 1f, -0.03125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 0.03125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, minLower, minLuma * minLower, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 1.05f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 1.5f, minLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 2f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, minUpper, 1f - (1f - minLuma) * minUpper, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma - 1f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma - 1f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma - 1f, -1f, minLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0.5f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma - 1f, 0.5f, minLuma, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma, -1f, -0.03125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma, -0.03125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma, 0.03125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, minLower, minLuma * minLower, 0.25f), new ColorHCY(hueAtMinLuma, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma, 1.05f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma, 1.5f, minLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma, 2f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, minUpper, 1f - (1f - minLuma) * minUpper, 0.25f), new ColorHCY(hueAtMinLuma, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma, -1f, minLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0.5f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma, 0.5f, minLuma, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma + 1f, -1f, -0.03125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma + 1f, -0.03125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 0.03125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, minLower, minLuma * minLower, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 1.05f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 1.5f, minLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 1f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 2f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, minUpper, 1f - (1f - minLuma) * minUpper, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma + 1f, -0.125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMinLuma + 1f, -1f, 1.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma + 1f, -1f, minLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMinLuma, 0.5f, minLuma, 0.25f), new ColorHCY(hueAtMinLuma + 1f, 0.5f, minLuma, 0.25f), 0.0001f, true);

			float hueAtMaxLuma = 2f / 6f;
			float maxLuma = Detail.LumaUtility.gWeight;
			float maxLower = 1f - maxLuma * maxLuma / (1f + maxLuma * maxLuma);
			float maxUpper = 1f - (1f - maxLuma) * (1f - maxLuma) / (1f + (1f - maxLuma) * (1f - maxLuma));

			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, maxLower, maxLuma * maxLower, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 1.5f, maxLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 1.25f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, maxUpper, 1f - (1f - maxLuma) * maxUpper, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 0.03125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, -0.03125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, -1f, 1.03125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, -1f, maxLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0.5f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma - 1f, 0.5f, maxLuma, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, maxLower, maxLuma * maxLower, 0.25f), new ColorHCY(hueAtMaxLuma, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma, 1.5f, maxLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma, 1.25f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, maxUpper, 1f - (1f - maxLuma) * maxUpper, 0.25f), new ColorHCY(hueAtMaxLuma, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma, 0.03125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma, -0.03125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma, -1f, 1.03125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma, -1f, maxLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0.5f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma, 0.5f, maxLuma, 0.25f), 0.0001f, true);

			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, -1f, -0.125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, -0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 0f, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 0.125f, -1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, maxLower, maxLuma * maxLower, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 1f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 1.5f, 0f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 1.5f, maxLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 1f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 1.25f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, maxUpper, 1f - (1f - maxLuma) * maxUpper, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 1f, 1f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 0.03125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, -0.03125f, 2f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, 1f, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, -1f, 1.03125f, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, -1f, maxLuma, 0.25f), 0.0001f);
			AssertNearest(new ColorHCY(hueAtMaxLuma, 0.5f, maxLuma, 0.25f), new ColorHCY(hueAtMaxLuma + 1f, 0.5f, maxLuma, 0.25f), 0.0001f, true);
		}
	}
}
#endif
