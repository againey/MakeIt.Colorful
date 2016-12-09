/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;

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
		/// <param name="rgb">The RGB color instance to which this extension function will apply.</param>
		/// <returns>Returns true if the color is valid, false if not.</returns>
		public static bool IsValid(this Color rgb)
		{
			return (rgb.a >= 0f & rgb.a <= 1f & rgb.r >= 0f & rgb.r <= 1f & rgb.g >= 0f & rgb.g <= 1f & rgb.b >= 0f & rgb.b <= 1f);
		}

		/// <summary>
		/// Gets the nearest color that is within the RGB color space.
		/// </summary>
		/// <param name="rgb">The RGB color instance to which this extension function will apply.</param>
		/// <returns>The nearest valid RGB color.</returns>
		public static Color GetNearestValid(this Color rgb)
		{
			return new Color(Mathf.Clamp01(rgb.r), Mathf.Clamp01(rgb.g), Mathf.Clamp01(rgb.b), Mathf.Clamp01(rgb.a));
		}

		/// <summary>
		/// Indicates if the color is canonical, or if there is a different representation of this color that is canonical.
		/// </summary>
		/// <param name="rgb">The RGB color instance to which this extension function will apply.</param>
		/// <returns>Returns true if the color is canonical, false if there is a different representation that is canonical.</returns>
		/// <remarks>
		/// <para>An RGB color is always canonical, because there is never more than one representation of any color.</para>
		/// </remarks>
		public static bool IsCanonical(this Color rgb)
		{
			return true;
		}

		/// <summary>
		/// Gets the canonical representation of the color.
		/// </summary>
		/// <param name="rgb">The RGB color instance to which this extension function will apply.</param>
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

		#region Opacity Operations

		/// <summary>
		/// Gets the fully opaque variant of the current color.
		/// </summary>
		/// <returns>Returns a copy of the current color, but with opacity set to 1.</returns>
		public static Color Opaque(this Color rgb) { return new Color(rgb.r, rgb.g, rgb.b, 1f); }

		/// <summary>
		/// Gets a partially translucent variant of the current color.
		/// </summary>
		/// <param name="a">The desired opacity for the returned color.</param>
		/// <returns>Returns a copy of the current color, but with opacity set to the provided value.</returns>
		public static Color Translucent(this Color rgb, float a) { return new Color(rgb.r, rgb.g, rgb.b, a); }

		/// <summary>
		/// Gets the fully transparent variant of the current color.
		/// </summary>
		/// <returns>Returns a copy of the current color, but with opacity set to 0.</returns>
		public static Color Transparent(this Color rgb) { return new Color(rgb.r, rgb.g, rgb.b, 0f); }

		#endregion
	}
}
