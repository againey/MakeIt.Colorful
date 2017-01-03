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

		#region Attributes

		/// <summary>
		/// Gets the hue of the color.
		/// </summary>
		/// <returns>The color's hue.</returns>
		public static float GetHue(this Color rgb)
		{
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);
			return Detail.HueUtility.FromRGB(rgb.r, rgb.g, rgb.b, max, max - min);
		}

		/// <summary>
		/// Gets the chroma of the color.
		/// </summary>
		/// <returns>The color's chroma.</returns>
		public static float GetChroma(this Color rgb)
		{
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);
			return max - min;
		}

		/// <summary>
		/// Gets the intensity of the color.
		/// </summary>
		/// <returns>The color's intensity.</returns>
		public static float GetIntensity(this Color rgb)
		{
			return (rgb.r + rgb.g + rgb.b) / 3f;
		}

		/// <summary>
		/// Gets the value of the color.
		/// </summary>
		/// <returns>The color's value.</returns>
		public static float GetValue(this Color rgb)
		{
			return Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);
		}

		/// <summary>
		/// Gets the lightness of the color.
		/// </summary>
		/// <returns>The color's lightness.</returns>
		public static float GetLightness(this Color rgb)
		{
			float min = Mathf.Min(Mathf.Min(rgb.r, rgb.g), rgb.b);
			float max = Mathf.Max(Mathf.Max(rgb.r, rgb.g), rgb.b);
			return (min + max) / 2f;
		}

		/// <summary>
		/// Gets the luma (apparent brightness) of the color.
		/// </summary>
		/// <returns>The color's luma.</returns>
		public static float GetLuma(this Color rgb)
		{
			return Detail.LumaUtility.FromRGB(rgb.r, rgb.g, rgb.b);
		}

		#endregion

		#region Comparisons

		/// <summary>
		/// Determines the ordering of this color with the specified color.
		/// </summary>
		/// <param name="other">The other color to compare against this one.</param>
		/// <returns>Returns -1 if this color is ordered before the other color, +1 if it is ordered after the other color, and 0 if neither is ordered before the other.</returns>
		public static int CompareTo(this Color rgb, Color other)
		{
			return Detail.OrderUtility.Compare(rgb.r, rgb.g, rgb.b, rgb.a, other.r, other.g, other.b, other.a);
		}

		/// <summary>
		/// Determines the ordering of the first color in relation to the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns -1 if the first color is ordered before the second color, +1 if it is ordered after the second color, and 0 if neither is ordered before the other.</returns>
		public static int Compare(Color lhs, Color rhs)
		{
			return Detail.OrderUtility.Compare(lhs.r, lhs.g, lhs.b, lhs.a, rhs.r, rhs.g, rhs.b, rhs.a);
		}

		/// <summary>
		/// Checks if the first color is lexicographically ordered before the second color.
		/// </summary>
		/// <param name="lhs">The first color compare.</param>
		/// <param name="rhs">The second color compare.</param>
		/// <returns>Returns true if the first color is lexicographically ordered before the second color, false otherwise.</returns>
		public static bool AreOrdered(Color lhs, Color rhs)
		{
			return Detail.OrderUtility.AreOrdered(lhs.r, lhs.g, lhs.b, lhs.a, rhs.r, rhs.g, rhs.b, rhs.a);
		}

		#endregion
	}
}
