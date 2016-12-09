/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Extensions to the standard <see cref="Color"/> struct for colors in the RGB color space.
	/// </summary>
	public static class ColorSchemeUtility
	{
		#region Create From HSV

		public static ColorHSV Create(float baseHue, ColorHSV template)
		{
			return new ColorHSV(template.h + baseHue, template.s, template.v, template.a).GetNearestValid().GetCanonical();
		}

		public static ColorHSV[] Create(float baseHue, ColorHSV[] template)
		{
			return Create(baseHue, template, new ColorHSV[template.Length]);
		}

		public static ColorHSV[] Create(float baseHue, ColorHSV[] template, ColorHSV[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		public static Color[] Create(float baseHue, ColorHSV[] template, Color[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		#endregion

		#region Create From HCV

		public static ColorHCV Create(float baseHue, ColorHCV template)
		{
			return new ColorHCV(template.h + baseHue, template.c, template.v, template.a).GetNearestValid().GetCanonical();
		}

		public static ColorHCV[] Create(float baseHue, ColorHCV[] template)
		{
			return Create(baseHue, template, new ColorHCV[template.Length]);
		}

		public static ColorHCV[] Create(float baseHue, ColorHCV[] template, ColorHCV[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		public static Color[] Create(float baseHue, ColorHCV[] template, Color[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		#endregion

		#region Create From HSL

		public static ColorHSL Create(float baseHue, ColorHSL template)
		{
			return new ColorHSL(template.h + baseHue, template.s, template.l, template.a).GetNearestValid().GetCanonical();
		}

		public static ColorHSL[] Create(float baseHue, ColorHSL[] template)
		{
			return Create(baseHue, template, new ColorHSL[template.Length]);
		}

		public static ColorHSL[] Create(float baseHue, ColorHSL[] template, ColorHSL[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		public static Color[] Create(float baseHue, ColorHSL[] template, Color[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		#endregion

		#region Create From HCL

		public static ColorHCL Create(float baseHue, ColorHCL template)
		{
			return new ColorHCL(template.h + baseHue, template.c, template.l, template.a).GetNearestValid().GetCanonical();
		}

		public static ColorHCL[] Create(float baseHue, ColorHCL[] template)
		{
			return Create(baseHue, template, new ColorHCL[template.Length]);
		}

		public static ColorHCL[] Create(float baseHue, ColorHCL[] template, ColorHCL[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		public static Color[] Create(float baseHue, ColorHCL[] template, Color[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		#endregion

		#region Create From HSY

		public static ColorHSY Create(float baseHue, ColorHSY template)
		{
			return new ColorHSY(template.h + baseHue, template.s, template.y, template.a).GetNearestValid().GetCanonical();
		}

		public static ColorHSY[] Create(float baseHue, ColorHSY[] template)
		{
			return Create(baseHue, template, new ColorHSY[template.Length]);
		}

		public static ColorHSY[] Create(float baseHue, ColorHSY[] template, ColorHSY[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		public static Color[] Create(float baseHue, ColorHSY[] template, Color[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		#endregion

		#region Create From HCY

		public static ColorHCY Create(float baseHue, ColorHCY template)
		{
			return new ColorHCY(template.h + baseHue, template.c, template.y, template.a).GetNearestValid().GetCanonical();
		}

		public static ColorHCY[] Create(float baseHue, ColorHCY[] template)
		{
			return Create(baseHue, template, new ColorHCY[template.Length]);
		}

		public static ColorHCY[] Create(float baseHue, ColorHCY[] template, ColorHCY[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		public static Color[] Create(float baseHue, ColorHCY[] template, Color[] scheme)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i]);
			}
			return scheme;
		}

		#endregion

		#region Templates

		public static class HSV
		{
			public static ColorHSV[] Complementary(float saturation, float value)
			{
				return new ColorHSV[]
				{
					new ColorHSV(0f/360f, saturation, value),
					new ColorHSV(180/360f, saturation, value),
				};
			}

			public static ColorHSV[] complementaryPure { get { return Complementary(1f, 1f); } }

			public static ColorHSV[] Analogous(float angle, float saturation, float value)
			{
				return new ColorHSV[]
				{
					new ColorHSV(0f/360f, saturation, value),
					new ColorHSV(angle/360f, saturation, value),
					new ColorHSV(-angle/360f, saturation, value),
				};
			}

			public static ColorHSV[] analogousPure { get { return Analogous(30f, 1f, 1f); } }

			public static ColorHSV[] Triadic(float saturation, float value)
			{
				return new ColorHSV[]
				{
					new ColorHSV(0f/360f, saturation, value),
					new ColorHSV(120f/360f, saturation, value),
					new ColorHSV(240f/360f, saturation, value),
				};
			}

			public static ColorHSV[] triadicPure { get { return Triadic(1f, 1f); } }

			public static ColorHSV[] SplitComplementary(float angle, float saturation, float value)
			{
				return new ColorHSV[]
				{
					new ColorHSV(0f/360f, saturation, value),
					new ColorHSV((180f - angle)/360f, saturation, value),
					new ColorHSV((180f + angle)/360f, saturation, value),
				};
			}

			public static ColorHSV[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 1f); } }

			public static ColorHSV[] Tetratic(float angle, float saturation, float value)
			{
				return new ColorHSV[]
				{
					new ColorHSV(0f/360f, saturation, value),
					new ColorHSV(angle/360f, saturation, value),
					new ColorHSV(180f/360f, saturation, value),
					new ColorHSV((180f + angle)/360f, saturation, value),
				};
			}

			public static ColorHSV[] tetraticPure { get { return Tetratic(30f, 1f, 1f); } }
		}

		public static class HCV
		{
			public static ColorHCV[] complementaryPure { get { return new ColorHCV[]
			{
				new ColorHCV(0f/360f, 1f, 1f),
				new ColorHCV(180f/360f, 1f, 1f),
			}; } }

			public static ColorHCV[] triadicPure { get { return new ColorHCV[]
			{
				new ColorHCV(0f/360f, 1f, 1f),
				new ColorHCV(120f/360f, 1f, 1f),
				new ColorHCV(240f/360f, 1f, 1f),
			}; } }
		}

		public static class HSL
		{
			public static ColorHSL[] complementaryPure { get { return new ColorHSL[]
			{
				new ColorHSL(0f/360f, 1f, 0.5f),
				new ColorHSL(180f/360f, 1f, 0.5f),
			}; } }

			public static ColorHSL[] triadicPure { get { return new ColorHSL[]
			{
				new ColorHSL(0f/360f, 1f, 0.5f),
				new ColorHSL(120f/360f, 1f, 0.5f),
				new ColorHSL(240f/360f, 1f, 0.5f),
			}; } }
		}

		public static class HCL
		{
			public static ColorHCL[] complementaryPure { get { return new ColorHCL[]
			{
				new ColorHCL(0f/360f, 1f, 0.5f),
				new ColorHCL(180f/360f, 1f, 0.5f),
			}; } }

			public static ColorHCL[] triadicPure { get { return new ColorHCL[]
			{
				new ColorHCL(0f/360f, 1f, 0.5f),
				new ColorHCL(120f/360f, 1f, 0.5f),
				new ColorHCL(240f/360f, 1f, 0.5f),
			}; } }
		}

		public static class HSY
		{
			public static ColorHSY[] Complementary(float saturation, float luma)
			{
				return new ColorHSY[]
				{
					new ColorHSY(0f/360f, saturation, luma),
					new ColorHSY(180/360f, saturation, luma),
				};
			}

			public static ColorHSY[] complementaryPure { get { return Complementary(1f, 0.5f); } }

			public static ColorHSY[] Analogous(float angle, float saturation, float luma)
			{
				return new ColorHSY[]
				{
					new ColorHSY(0f/360f, saturation, luma),
					new ColorHSY(angle/360f, saturation, luma),
					new ColorHSY(-angle/360f, saturation, luma),
				};
			}

			public static ColorHSY[] analogousPure { get { return Analogous(30f, 1f, 0.5f); } }

			public static ColorHSY[] Triadic(float saturation, float luma)
			{
				return new ColorHSY[]
				{
					new ColorHSY(0f/360f, saturation, luma),
					new ColorHSY(120f/360f, saturation, luma),
					new ColorHSY(240f/360f, saturation, luma),
				};
			}

			public static ColorHSY[] triadicPure { get { return Triadic(1f, 0.5f); } }

			public static ColorHSY[] SplitComplementary(float angle, float saturation, float luma)
			{
				return new ColorHSY[]
				{
					new ColorHSY(0f/360f, saturation, luma),
					new ColorHSY((180f - angle)/360f, saturation, luma),
					new ColorHSY((180f + angle)/360f, saturation, luma),
				};
			}

			public static ColorHSY[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 0.5f); } }

			public static ColorHSY[] Tetratic(float angle, float saturation, float luma)
			{
				return new ColorHSY[]
				{
					new ColorHSY(0f/360f, saturation, luma),
					new ColorHSY(angle/360f, saturation, luma),
					new ColorHSY(180f/360f, saturation, luma),
					new ColorHSY((180f + angle)/360f, saturation, luma),
				};
			}

			public static ColorHSY[] tetraticPure { get { return Tetratic(30f, 1f, 0.5f); } }
		}

		public static class HCY
		{
			public static ColorHCY[] Complementary(float saturation, float luma)
			{
				return new ColorHCY[]
				{
					new ColorHCY(0f/360f, saturation, luma),
					new ColorHCY(180/360f, saturation, luma),
				};
			}

			public static ColorHCY[] complementaryPure { get { return Complementary(1f, 0.5f); } }

			public static ColorHCY[] Analogous(float angle, float saturation, float luma)
			{
				return new ColorHCY[]
				{
					new ColorHCY(0f/360f, saturation, luma),
					new ColorHCY(angle/360f, saturation, luma),
					new ColorHCY(-angle/360f, saturation, luma),
				};
			}

			public static ColorHCY[] analogousPure { get { return Analogous(30f, 1f, 0.5f); } }

			public static ColorHCY[] Triadic(float saturation, float luma)
			{
				return new ColorHCY[]
				{
					new ColorHCY(0f/360f, saturation, luma),
					new ColorHCY(120f/360f, saturation, luma),
					new ColorHCY(240f/360f, saturation, luma),
				};
			}

			public static ColorHCY[] triadicPure { get { return Triadic(1f, 0.5f); } }

			public static ColorHCY[] SplitComplementary(float angle, float saturation, float luma)
			{
				return new ColorHCY[]
				{
					new ColorHCY(0f/360f, saturation, luma),
					new ColorHCY((180f - angle)/360f, saturation, luma),
					new ColorHCY((180f + angle)/360f, saturation, luma),
				};
			}

			public static ColorHCY[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 0.5f); } }

			public static ColorHCY[] Tetratic(float angle, float saturation, float luma)
			{
				return new ColorHCY[]
				{
					new ColorHCY(0f/360f, saturation, luma),
					new ColorHCY(angle/360f, saturation, luma),
					new ColorHCY(180f/360f, saturation, luma),
					new ColorHCY((180f + angle)/360f, saturation, luma),
				};
			}

			public static ColorHCY[] tetraticPure { get { return Tetratic(30f, 1f, 0.5f); } }
		}

		#endregion
	}
}
