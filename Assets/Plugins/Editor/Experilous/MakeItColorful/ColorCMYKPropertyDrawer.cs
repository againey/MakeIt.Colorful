/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Property drawer for the ColorCMYK color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorCMYK))]
	public class ColorCMYKPropertyDrawer : PropertyDrawer
	{
		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var c = property.FindPropertyRelative("c");
			var m = property.FindPropertyRelative("m");
			var y = property.FindPropertyRelative("y");
			var k = property.FindPropertyRelative("k");
			var a = property.FindPropertyRelative("a");
			var cmy = new ColorCMYK(c.floatValue, m.floatValue, y.floatValue, k.floatValue, a.floatValue);
			cmy = EditorGUI.ColorField(position, cmy);
			c.floatValue = cmy.c;
			m.floatValue = cmy.m;
			y.floatValue = cmy.y;
			k.floatValue = cmy.k;
			a.floatValue = cmy.a;
			EditorGUI.EndProperty();
		}
	}
}
