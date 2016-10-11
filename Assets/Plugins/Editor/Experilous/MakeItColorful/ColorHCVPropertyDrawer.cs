﻿/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Property drawer for the ColorHCV color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHCV))]
	public class ColorHCVPropertyDrawer : PropertyDrawer
	{
		/// <summary>
		/// Draws the GUI for the color property.
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the property GUI.</param>
		/// <param name="property">The SerializedProperty to make the custom GUI for.</param>
		/// <param name="label">The label of this property.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var h = property.FindPropertyRelative("h");
			var c = property.FindPropertyRelative("c");
			var v = property.FindPropertyRelative("v");
			var a = property.FindPropertyRelative("a");
			var hcy = new ColorHCV(h.floatValue, c.floatValue, v.floatValue, a.floatValue);
			hcy = EditorGUI.ColorField(position, hcy);
			h.floatValue = hcy.h;
			c.floatValue = hcy.c;
			v.floatValue = hcy.v;
			a.floatValue = hcy.a;
			EditorGUI.EndProperty();
		}
	}
}