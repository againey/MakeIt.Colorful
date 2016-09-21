/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using System;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Extensions to the standard <see cref="Color"/> struct for colors in the RGB color space.
	/// </summary>
	public static class ColorExtensions
	{
		#region Color Space Boundaries

		/// <summary>
		/// Indicates if the values for red, green, and blue together represent a valid color within the RGB color space.
		/// </summary>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public static bool IsValid(this Color rgb)
		{
			return (rgb.a >= 0f & rgb.a <= 1f & rgb.r >= 0f & rgb.r <= 1f & rgb.g >= 0f & rgb.g <= 1f & rgb.b >= 0f & rgb.b <= 1f);
		}

		/// <summary>
		/// Gets the nearest color that is within the RGB color space.
		/// </summary>
		/// <returns>The nearest valid RGB color.</returns>
		public static Color GetNearestValid(this Color rgb)
		{
			return new Color(Mathf.Clamp01(rgb.r), Mathf.Clamp01(rgb.g), Mathf.Clamp01(rgb.b), Mathf.Clamp01(rgb.a));
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <returns>The canonical representation of the color.</returns>
		/// <remarks>
		/// <para>The canonical color representation, when converted to RGB and back, should not be any different from
		/// its original value, aside from any minor loss of accuracy that could occur during the conversions.</para>
		/// <para>For the RGB color space, all possible representations are unique and therefore already canonical.</para>
		/// </remarks>
		public static Color GetCanonical(this Color rgb)
		{
			return rgb;
		}

		#endregion
	}
}
