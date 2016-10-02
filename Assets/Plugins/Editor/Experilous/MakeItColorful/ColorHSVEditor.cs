﻿/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Property drawer for the ColorHSV color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHSV))]
	public class ColorHSVEditor : PropertyDrawer
	{
		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var h = property.FindPropertyRelative("h");
			var s = property.FindPropertyRelative("s");
			var v = property.FindPropertyRelative("v");
			var a = property.FindPropertyRelative("a");
			var hcy = new ColorHSV(h.floatValue, s.floatValue, v.floatValue, a.floatValue);
			hcy = EditorGUI.ColorField(position, hcy);
			h.floatValue = hcy.h;
			s.floatValue = hcy.s;
			v.floatValue = hcy.v;
			a.floatValue = hcy.a;
			EditorGUI.EndProperty();
		}
	}
}
