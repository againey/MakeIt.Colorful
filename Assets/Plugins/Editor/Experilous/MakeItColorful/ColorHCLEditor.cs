/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Property drawer for the ColorHCL color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHCL))]
	public class ColorHCLEditor : PropertyDrawer
	{
		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var h = property.FindPropertyRelative("h");
			var c = property.FindPropertyRelative("c");
			var l = property.FindPropertyRelative("l");
			var a = property.FindPropertyRelative("a");
			var hcy = new ColorHCL(h.floatValue, c.floatValue, l.floatValue, a.floatValue);
			hcy = EditorGUI.ColorField(position, hcy);
			h.floatValue = hcy.h;
			c.floatValue = hcy.c;
			l.floatValue = hcy.l;
			a.floatValue = hcy.a;
			EditorGUI.EndProperty();
		}
	}
}
