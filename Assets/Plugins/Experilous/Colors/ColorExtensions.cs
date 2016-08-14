/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.Colors
{
	public static class ColorExtensions
	{
		public static ColorHSV ToHSV(this Color rgb)
		{
			return ColorHSV.FromRGB(rgb);
		}

		public static ColorHCV ToHCV(this Color rgb)
		{
			return ColorHCV.FromRGB(rgb);
		}

		public static ColorHSL ToHSL(this Color rgb)
		{
			return ColorHSL.FromRGB(rgb);
		}

		public static ColorHCL ToHCL(this Color rgb)
		{
			return ColorHCL.FromRGB(rgb);
		}

		public static ColorHSY ToHSY(this Color rgb)
		{
			return ColorHSY.FromRGB(rgb);
		}

		public static ColorHCY ToHCY(this Color rgb)
		{
			return ColorHCY.FromRGB(rgb);
		}
	}
}
