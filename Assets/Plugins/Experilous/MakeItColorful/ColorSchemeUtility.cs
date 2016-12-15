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

		public static ColorHCV Create(float baseHue, ColorHCV template, float chromaBias)
		{
			return new ColorHCV(template.h + baseHue, template.c, template.v, template.a).GetValid(chromaBias).GetCanonical();
		}

		public static ColorHCV[] Create(float baseHue, ColorHCV[] template)
		{
			return Create(baseHue, template, new ColorHCV[template.Length]);
		}

		public static ColorHCV[] Create(float baseHue, ColorHCV[] template, float chromaBias)
		{
			return Create(baseHue, template, new ColorHCV[template.Length], chromaBias);
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

		public static ColorHCV[] Create(float baseHue, ColorHCV[] template, ColorHCV[] scheme, float chromaBias)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i], chromaBias);
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

		public static Color[] Create(float baseHue, ColorHCV[] template, Color[] scheme, float chromaBias)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i], chromaBias);
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

		public static ColorHCL Create(float baseHue, ColorHCL template, float chromaBias)
		{
			return new ColorHCL(template.h + baseHue, template.c, template.l, template.a).GetValid(chromaBias).GetCanonical();
		}

		public static ColorHCL[] Create(float baseHue, ColorHCL[] template)
		{
			return Create(baseHue, template, new ColorHCL[template.Length]);
		}

		public static ColorHCL[] Create(float baseHue, ColorHCL[] template, float chromaBias)
		{
			return Create(baseHue, template, new ColorHCL[template.Length], chromaBias);
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

		public static ColorHCL[] Create(float baseHue, ColorHCL[] template, ColorHCL[] scheme, float chromaBias)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i], chromaBias);
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

		public static Color[] Create(float baseHue, ColorHCL[] template, Color[] scheme, float chromaBias)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i], chromaBias);
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

		public static ColorHCY Create(float baseHue, ColorHCY template, float chromaBias)
		{
			return new ColorHCY(template.h + baseHue, template.c, template.y, template.a).GetValid(chromaBias).GetCanonical();
		}

		public static ColorHCY[] Create(float baseHue, ColorHCY[] template)
		{
			return Create(baseHue, template, new ColorHCY[template.Length]);
		}

		public static ColorHCY[] Create(float baseHue, ColorHCY[] template, float chromaBias)
		{
			return Create(baseHue, template, new ColorHCY[template.Length], chromaBias);
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

		public static ColorHCY[] Create(float baseHue, ColorHCY[] template, ColorHCY[] scheme, float chromaBias)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i], chromaBias);
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

		public static Color[] Create(float baseHue, ColorHCY[] template, Color[] scheme, float chromaBias)
		{
			if (template == null) throw new System.ArgumentNullException("template");
			if (template == null) throw new System.ArgumentNullException("template");
			if (template.Length != scheme.Length) throw new System.ArgumentException("The color scheme array must be the same length as the template array.", "scheme");
			for (int i = 0; i < template.Length; ++i)
			{
				scheme[i] = Create(baseHue, template[i], chromaBias);
			}
			return scheme;
		}

		#endregion

		public static class Templates
		{
			public static class HSV
			{
				public static ColorHSV[] Complementary(float saturation, float value)
				{
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, saturation, value),
						new ColorHSV(180f/360f, saturation, value),
					};
				}

				public static ColorHSV[] complementaryPure { get { return Complementary(1f, 1f); } }

				public static ColorHSV[] complementaryMix1
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 1f, 1f),
						new ColorHSV(0f/360f, 0.75f, 0.25f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(180f/360f, 1f, 0.25f),
						new ColorHSV(180f/360f, 1f, 0.75f),
					};
				} }

				public static ColorHSV[] complementaryMix2
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 0.8f, 0.8f),
						new ColorHSV(0f/360f, 0.6f, 1f),
						new ColorHSV(0f/360f, 0.3f, 1f),
						new ColorHSV(180f/360f, 1f, 0.6f),
						new ColorHSV(180f/360f, 1f, 0.3f),
					};
				} }

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

				public static ColorHSV[] analogousMix1
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 1f, 1f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(30f/360f, 1f, 0.5f),
						new ColorHSV(30f/360f, 0.5f, 0.25f),
						new ColorHSV(330f/360f, 0.75f, 0.75f),
					};
				} }

				public static ColorHSV[] analogousMix2
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 0.8f, 0.8f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(0f/360f, 1f, 0.5f),
						new ColorHSV(30f/360f, 0.25f, 0.75f),
						new ColorHSV(330f/360f, 0.75f, 0.25f),
					};
				} }

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

				public static ColorHSV[] triadicMix1
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 1f, 1f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(120f/360f, 0.75f, 0.5f),
						new ColorHSV(120f/360f, 0.25f, 0.25f),
						new ColorHSV(240f/360f, 0.75f, 0.75f),
					};
				} }

				public static ColorHSV[] triadicMix2
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 0.8f, 0.8f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(0f/360f, 0.75f, 0.5f),
						new ColorHSV(120f/360f, 0.4f, 0.6f),
						new ColorHSV(240f/360f, 0.5f, 0.25f),
					};
				} }

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

				public static ColorHSV[] splitComplementaryMix1
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 1f, 1f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(150f/360f, 0.75f, 0.25f),
						new ColorHSV(150f/360f, 0.25f, 0.25f),
						new ColorHSV(210f/360f, 1f, 0.75f),
					};
				} }

				public static ColorHSV[] splitComplementaryMix2
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 0.8f, 0.8f),
						new ColorHSV(0f/360f, 0.5f, 1f),
						new ColorHSV(0f/360f, 0.75f, 0.25f),
						new ColorHSV(150f/360f, 0.75f, 0.75f),
						new ColorHSV(210f/360f, 0.5f, 0.25f),
					};
				} }

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

				public static ColorHSV[] tetraticMix1
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 1f, 1f),
						new ColorHSV(30f/360f, 0.5f, 1f),
						new ColorHSV(180f/360f, 0.75f, 0.5f),
						new ColorHSV(180f/360f, 0.5f, 0.25f),
						new ColorHSV(210f/360f, 1f, 1f),
					};
				} }

				public static ColorHSV[] tetraticMix2
				{ get {
					return new ColorHSV[]
					{
						new ColorHSV(0f/360f, 0.8f, 0.8f),
						new ColorHSV(0f/360f, 0.25f, 1f),
						new ColorHSV(30f/360f, 0.75f, 0.25f),
						new ColorHSV(150f/360f, 0.25f, 0.75f),
						new ColorHSV(210f/360f, 0.5f, 0.25f),
					};
				} }
			}

			public static class HCV
			{
				public static ColorHCV[] Complementary(float chroma, float value)
				{
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, chroma, value),
						new ColorHCV(180f/360f, chroma, value),
					};
				}

				public static ColorHCV[] complementaryPure { get { return Complementary(1f, 1f); } }

				public static ColorHCV[] complementaryMix1
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 1f, 1f),
						new ColorHCV(0f/360f, 0.25f, 0.25f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(180f/360f, 0.25f, 0.25f),
						new ColorHCV(180f/360f, 0.75f, 0.75f),
					};
				} }

				public static ColorHCV[] complementaryMix2
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 0.8f, 0.8f),
						new ColorHCV(0f/360f, 0.6f, 1f),
						new ColorHCV(0f/360f, 0.3f, 1f),
						new ColorHCV(180f/360f, 1f, 0.6f),
						new ColorHCV(180f/360f, 1f, 0.3f),
					};
				} }

				public static ColorHCV[] Analogous(float angle, float chroma, float value)
				{
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, chroma, value),
						new ColorHCV(angle/360f, chroma, value),
						new ColorHCV(-angle/360f, chroma, value),
					};
				}

				public static ColorHCV[] analogousPure { get { return Analogous(30f, 1f, 1f); } }

				public static ColorHCV[] analogousMix1
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 1f, 1f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(30f/360f, 0.5f, 0.5f),
						new ColorHCV(30f/360f, 0.25f, 0.25f),
						new ColorHCV(330f/360f, 0.75f, 0.75f),
					};
				} }

				public static ColorHCV[] analogousMix2
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 0.8f, 0.8f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(0f/360f, 0.5f, 0.5f),
						new ColorHCV(30f/360f, 0.25f, 0.75f),
						new ColorHCV(330f/360f, 0.25f, 0.25f),
					};
				} }

				public static ColorHCV[] Triadic(float chroma, float value)
				{
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, chroma, value),
						new ColorHCV(120f/360f, chroma, value),
						new ColorHCV(240f/360f, chroma, value),
					};
				}

				public static ColorHCV[] triadicPure { get { return Triadic(1f, 1f); } }

				public static ColorHCV[] triadicMix1
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 1f, 1f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(120f/360f, 0.5f, 0.5f),
						new ColorHCV(120f/360f, 0.25f, 0.25f),
						new ColorHCV(240f/360f, 0.75f, 0.75f),
					};
				} }

				public static ColorHCV[] triadicMix2
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 0.8f, 0.8f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(0f/360f, 0.5f, 0.5f),
						new ColorHCV(120f/360f, 0.4f, 0.6f),
						new ColorHCV(240f/360f, 0.25f, 0.25f),
					};
				} }

				public static ColorHCV[] SplitComplementary(float angle, float chroma, float value)
				{
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, chroma, value),
						new ColorHCV((180f - angle)/360f, chroma, value),
						new ColorHCV((180f + angle)/360f, chroma, value),
					};
				}

				public static ColorHCV[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 1f); } }

				public static ColorHCV[] splitComplementaryMix1
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 1f, 1f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(150f/360f, 0.5f, 0.5f),
						new ColorHCV(150f/360f, 0.25f, 0.25f),
						new ColorHCV(210f/360f, 0.75f, 0.75f),
					};
				} }

				public static ColorHCV[] splitComplementaryMix2
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 0.8f, 0.8f),
						new ColorHCV(0f/360f, 0.5f, 1f),
						new ColorHCV(0f/360f, 0.25f, 0.25f),
						new ColorHCV(150f/360f, 0.5f, 0.75f),
						new ColorHCV(210f/360f, 0.25f, 0.25f),
					};
				} }

				public static ColorHCV[] Tetratic(float angle, float chroma, float value)
				{
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, chroma, value),
						new ColorHCV(angle/360f, chroma, value),
						new ColorHCV(180f/360f, chroma, value),
						new ColorHCV((180f + angle)/360f, chroma, value),
					};
				}

				public static ColorHCV[] tetraticPure { get { return Tetratic(30f, 1f, 1f); } }

				public static ColorHCV[] tetraticMix1
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 1f, 1f),
						new ColorHCV(30f/360f, 0.5f, 1f),
						new ColorHCV(180f/360f, 0.75f, 0.5f),
						new ColorHCV(180f/360f, 0.5f, 0.25f),
						new ColorHCV(210f/360f, 1f, 1f),
					};
				} }

				public static ColorHCV[] tetraticMix2
				{ get {
					return new ColorHCV[]
					{
						new ColorHCV(0f/360f, 0.8f, 0.8f),
						new ColorHCV(0f/360f, 0.25f, 1f),
						new ColorHCV(30f/360f, 0.75f, 0.25f),
						new ColorHCV(150f/360f, 0.25f, 0.75f),
						new ColorHCV(210f/360f, 0.5f, 0.25f),
					};
				} }
			}

			public static class HSL
			{
				public static ColorHSL[] Complementary(float saturation, float lightness)
				{
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, saturation, lightness),
						new ColorHSL(180f/360f, saturation, lightness),
					};
				}

				public static ColorHSL[] complementaryPure { get { return Complementary(1f, 0.5f); } }

				public static ColorHSL[] complementaryMix1
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.25f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(180f/360f, 0.5f, 0.125f),
						new ColorHSL(180f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSL[] complementaryMix2
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.7f),
						new ColorHSL(0f/360f, 0.3f, 0.9f),
						new ColorHSL(180f/360f, 0.5f, 0.3f),
						new ColorHSL(180f/360f, 0.3f, 0.1f),
					};
				} }

				public static ColorHSL[] Analogous(float angle, float saturation, float lightness)
				{
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, saturation, lightness),
						new ColorHSL(angle/360f, saturation, lightness),
						new ColorHSL(-angle/360f, saturation, lightness),
					};
				}

				public static ColorHSL[] analogousPure { get { return Analogous(30f, 1f, 0.5f); } }

				public static ColorHSL[] analogousMix1
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(30f/360f, 0.5f, 0.25f),
						new ColorHSL(30f/360f, 0.25f, 0.125f),
						new ColorHSL(330f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSL[] analogousMix2
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(0f/360f, 0.5f, 0.25f),
						new ColorHSL(30f/360f, 0.25f, 0.5f),
						new ColorHSL(330f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHSL[] Triadic(float saturation, float lightness)
				{
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, saturation, lightness),
						new ColorHSL(120f/360f, saturation, lightness),
						new ColorHSL(240f/360f, saturation, lightness),
					};
				}

				public static ColorHSL[] triadicPure { get { return Triadic(1f, 0.5f); } }

				public static ColorHSL[] triadicMix1
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(120f/360f, 0.5f, 0.25f),
						new ColorHSL(120f/360f, 0.25f, 0.125f),
						new ColorHSL(240f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSL[] triadicMix2
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(0f/360f, 0.5f, 0.25f),
						new ColorHSL(120f/360f, 0.25f, 0.5f),
						new ColorHSL(240f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHSL[] SplitComplementary(float angle, float saturation, float lightness)
				{
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, saturation, lightness),
						new ColorHSL((180f - angle)/360f, saturation, lightness),
						new ColorHSL((180f + angle)/360f, saturation, lightness),
					};
				}

				public static ColorHSL[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 0.5f); } }

				public static ColorHSL[] splitComplementaryMix1
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(150f/360f, 0.5f, 0.25f),
						new ColorHSL(150f/360f, 0.25f, 0.125f),
						new ColorHSL(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSL[] splitComplementaryMix2
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(0f/360f, 0.5f, 0.25f),
						new ColorHSL(150f/360f, 0.25f, 0.5f),
						new ColorHSL(210f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHSL[] Tetratic(float angle, float saturation, float lightness)
				{
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, saturation, lightness),
						new ColorHSL(angle/360f, saturation, lightness),
						new ColorHSL(180f/360f, saturation, lightness),
						new ColorHSL((180f + angle)/360f, saturation, lightness),
					};
				}

				public static ColorHSL[] tetraticPure { get { return Tetratic(30f, 1f, 0.5f); } }

				public static ColorHSL[] tetraticMix1
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(30f/360f, 0.25f, 0.75f),
						new ColorHSL(180f/360f, 0.5f, 0.25f),
						new ColorHSL(180f/360f, 0.25f, 0.125f),
						new ColorHSL(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSL[] tetraticMix2
				{ get {
					return new ColorHSL[]
					{
						new ColorHSL(0f/360f, 0.75f, 0.5f),
						new ColorHSL(0f/360f, 0.5f, 0.75f),
						new ColorHSL(30f/360f, 0.5f, 0.25f),
						new ColorHSL(150f/360f, 0.25f, 0.5f),
						new ColorHSL(210f/360f, 0.25f, 0.125f),
					};
				} }
			}

			public static class HCL
			{
				public static ColorHCL[] Complementary(float chroma, float lightness)
				{
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, chroma, lightness),
						new ColorHCL(180f/360f, chroma, lightness),
					};
				}

				public static ColorHCL[] complementaryPure { get { return Complementary(1f, 0.5f); } }

				public static ColorHCL[] complementaryMix1
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.25f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(180f/360f, 0.125f, 0.125f),
						new ColorHCL(180f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCL[] complementaryMix2
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.7f),
						new ColorHCL(0f/360f, 0.25f, 0.9f),
						new ColorHCL(180f/360f, 0.5f, 0.3f),
						new ColorHCL(180f/360f, 0.25f, 0.1f),
					};
				} }

				public static ColorHCL[] Analogous(float angle, float chroma, float lightness)
				{
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, chroma, lightness),
						new ColorHCL(angle/360f, chroma, lightness),
						new ColorHCL(-angle/360f, chroma, lightness),
					};
				}

				public static ColorHCL[] analogousPure { get { return Analogous(30f, 1f, 0.5f); } }

				public static ColorHCL[] analogousMix1
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(30f/360f, 0.5f, 0.25f),
						new ColorHCL(30f/360f, 0.25f, 0.125f),
						new ColorHCL(330f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCL[] analogousMix2
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(0f/360f, 0.5f, 0.25f),
						new ColorHCL(30f/360f, 0.25f, 0.5f),
						new ColorHCL(330f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHCL[] Triadic(float chroma, float lightness)
				{
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, chroma, lightness),
						new ColorHCL(120f/360f, chroma, lightness),
						new ColorHCL(240f/360f, chroma, lightness),
					};
				}

				public static ColorHCL[] triadicPure { get { return Triadic(1f, 0.5f); } }

				public static ColorHCL[] triadicMix1
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(120f/360f, 0.5f, 0.25f),
						new ColorHCL(120f/360f, 0.25f, 0.125f),
						new ColorHCL(240f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCL[] triadicMix2
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(0f/360f, 0.5f, 0.25f),
						new ColorHCL(120f/360f, 0.25f, 0.5f),
						new ColorHCL(240f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHCL[] SplitComplementary(float angle, float chroma, float lightness)
				{
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, chroma, lightness),
						new ColorHCL((180f - angle)/360f, chroma, lightness),
						new ColorHCL((180f + angle)/360f, chroma, lightness),
					};
				}

				public static ColorHCL[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 0.5f); } }

				public static ColorHCL[] splitComplementaryMix1
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(150f/360f, 0.5f, 0.25f),
						new ColorHCL(150f/360f, 0.25f, 0.125f),
						new ColorHCL(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCL[] splitComplementaryMix2
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(0f/360f, 0.5f, 0.25f),
						new ColorHCL(150f/360f, 0.25f, 0.5f),
						new ColorHCL(210f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHCL[] Tetratic(float angle, float chroma, float lightness)
				{
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, chroma, lightness),
						new ColorHCL(angle/360f, chroma, lightness),
						new ColorHCL(180f/360f, chroma, lightness),
						new ColorHCL((180f + angle)/360f, chroma, lightness),
					};
				}

				public static ColorHCL[] tetraticPure { get { return Tetratic(30f, 1f, 0.5f); } }

				public static ColorHCL[] tetraticMix1
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(30f/360f, 0.25f, 0.75f),
						new ColorHCL(180f/360f, 0.5f, 0.25f),
						new ColorHCL(180f/360f, 0.25f, 0.125f),
						new ColorHCL(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCL[] tetraticMix2
				{ get {
					return new ColorHCL[]
					{
						new ColorHCL(0f/360f, 0.75f, 0.5f),
						new ColorHCL(0f/360f, 0.5f, 0.75f),
						new ColorHCL(30f/360f, 0.5f, 0.25f),
						new ColorHCL(150f/360f, 0.25f, 0.5f),
						new ColorHCL(210f/360f, 0.25f, 0.125f),
					};
				} }
			}

			public static class HSY
			{
				public static ColorHSY[] Complementary(float saturation, float luma)
				{
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, saturation, luma),
						new ColorHSY(180f/360f, saturation, luma),
					};
				}

				public static ColorHSY[] complementaryPure { get { return Complementary(1f, 0.5f); } }

				public static ColorHSY[] complementaryMix1
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.25f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(180f/360f, 0.5f, 0.125f),
						new ColorHSY(180f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSY[] complementaryMix2
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.7f),
						new ColorHSY(0f/360f, 0.3f, 0.9f),
						new ColorHSY(180f/360f, 0.5f, 0.3f),
						new ColorHSY(180f/360f, 0.3f, 0.1f),
					};
				} }

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

				public static ColorHSY[] analogousMix1
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(30f/360f, 0.5f, 0.25f),
						new ColorHSY(30f/360f, 0.25f, 0.125f),
						new ColorHSY(330f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSY[] analogousMix2
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(0f/360f, 0.5f, 0.25f),
						new ColorHSY(30f/360f, 0.25f, 0.5f),
						new ColorHSY(330f/360f, 0.25f, 0.125f),
					};
				} }

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

				public static ColorHSY[] triadicMix1
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(120f/360f, 0.5f, 0.25f),
						new ColorHSY(120f/360f, 0.25f, 0.125f),
						new ColorHSY(240f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSY[] triadicMix2
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(0f/360f, 0.5f, 0.25f),
						new ColorHSY(120f/360f, 0.25f, 0.5f),
						new ColorHSY(240f/360f, 0.25f, 0.125f),
					};
				} }

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

				public static ColorHSY[] splitComplementaryMix1
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(150f/360f, 0.5f, 0.25f),
						new ColorHSY(150f/360f, 0.25f, 0.125f),
						new ColorHSY(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSY[] splitComplementaryMix2
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(0f/360f, 0.5f, 0.25f),
						new ColorHSY(150f/360f, 0.25f, 0.5f),
						new ColorHSY(210f/360f, 0.25f, 0.125f),
					};
				} }

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

				public static ColorHSY[] tetraticMix1
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(30f/360f, 0.25f, 0.75f),
						new ColorHSY(180f/360f, 0.5f, 0.25f),
						new ColorHSY(180f/360f, 0.25f, 0.125f),
						new ColorHSY(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHSY[] tetraticMix2
				{ get {
					return new ColorHSY[]
					{
						new ColorHSY(0f/360f, 0.75f, 0.5f),
						new ColorHSY(0f/360f, 0.5f, 0.75f),
						new ColorHSY(30f/360f, 0.5f, 0.25f),
						new ColorHSY(150f/360f, 0.25f, 0.5f),
						new ColorHSY(210f/360f, 0.25f, 0.125f),
					};
				} }
			}

			public static class HCY
			{
				public static ColorHCY[] Complementary(float chroma, float luma)
				{
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, chroma, luma),
						new ColorHCY(180f/360f, chroma, luma),
					};
				}

				public static ColorHCY[] complementaryPure { get { return Complementary(1f, 0.5f); } }

				public static ColorHCY[] complementaryMix1
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.25f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(180f/360f, 0.125f, 0.125f),
						new ColorHCY(180f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCY[] complementaryMix2
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.7f),
						new ColorHCY(0f/360f, 0.25f, 0.9f),
						new ColorHCY(180f/360f, 0.5f, 0.3f),
						new ColorHCY(180f/360f, 0.25f, 0.1f),
					};
				} }

				public static ColorHCY[] Analogous(float angle, float chroma, float luma)
				{
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, chroma, luma),
						new ColorHCY(angle/360f, chroma, luma),
						new ColorHCY(-angle/360f, chroma, luma),
					};
				}

				public static ColorHCY[] analogousPure { get { return Analogous(30f, 1f, 0.5f); } }

				public static ColorHCY[] analogousMix1
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(30f/360f, 0.5f, 0.25f),
						new ColorHCY(30f/360f, 0.25f, 0.125f),
						new ColorHCY(330f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCY[] analogousMix2
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(0f/360f, 0.5f, 0.25f),
						new ColorHCY(30f/360f, 0.25f, 0.5f),
						new ColorHCY(330f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHCY[] Triadic(float chroma, float luma)
				{
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, chroma, luma),
						new ColorHCY(120f/360f, chroma, luma),
						new ColorHCY(240f/360f, chroma, luma),
					};
				}

				public static ColorHCY[] triadicPure { get { return Triadic(1f, 0.5f); } }

				public static ColorHCY[] triadicMix1
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(120f/360f, 0.5f, 0.25f),
						new ColorHCY(120f/360f, 0.25f, 0.125f),
						new ColorHCY(240f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCY[] triadicMix2
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(0f/360f, 0.5f, 0.25f),
						new ColorHCY(120f/360f, 0.25f, 0.5f),
						new ColorHCY(240f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHCY[] SplitComplementary(float angle, float chroma, float luma)
				{
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, chroma, luma),
						new ColorHCY((180f - angle)/360f, chroma, luma),
						new ColorHCY((180f + angle)/360f, chroma, luma),
					};
				}

				public static ColorHCY[] splitComplementaryPure { get { return SplitComplementary(30f, 1f, 0.5f); } }

				public static ColorHCY[] splitComplementaryMix1
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(150f/360f, 0.5f, 0.25f),
						new ColorHCY(150f/360f, 0.25f, 0.125f),
						new ColorHCY(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCY[] splitComplementaryMix2
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(0f/360f, 0.5f, 0.25f),
						new ColorHCY(150f/360f, 0.25f, 0.5f),
						new ColorHCY(210f/360f, 0.25f, 0.125f),
					};
				} }

				public static ColorHCY[] Tetratic(float angle, float chroma, float luma)
				{
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, chroma, luma),
						new ColorHCY(angle/360f, chroma, luma),
						new ColorHCY(180f/360f, chroma, luma),
						new ColorHCY((180f + angle)/360f, chroma, luma),
					};
				}

				public static ColorHCY[] tetraticPure { get { return Tetratic(30f, 1f, 0.5f); } }

				public static ColorHCY[] tetraticMix1
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(30f/360f, 0.25f, 0.75f),
						new ColorHCY(180f/360f, 0.5f, 0.25f),
						new ColorHCY(180f/360f, 0.25f, 0.125f),
						new ColorHCY(210f/360f, 0.75f, 0.5f),
					};
				} }

				public static ColorHCY[] tetraticMix2
				{ get {
					return new ColorHCY[]
					{
						new ColorHCY(0f/360f, 0.75f, 0.5f),
						new ColorHCY(0f/360f, 0.5f, 0.75f),
						new ColorHCY(30f/360f, 0.5f, 0.25f),
						new ColorHCY(150f/360f, 0.25f, 0.5f),
						new ColorHCY(210f/360f, 0.25f, 0.125f),
					};
				} }
			}
		}
	}
}
