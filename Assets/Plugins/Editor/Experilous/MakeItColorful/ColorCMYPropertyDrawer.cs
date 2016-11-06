/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Property drawer for the ColorCMY color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorCMY))]
	public class ColorCMYPropertyDrawer : PropertyDrawer
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
			var c = property.FindPropertyRelative("c");
			var m = property.FindPropertyRelative("m");
			var y = property.FindPropertyRelative("y");
			var a = property.FindPropertyRelative("a");
			var cmy = new ColorCMY(c.floatValue, m.floatValue, y.floatValue, a.floatValue);
			cmy = EditorGUI.ColorField(position, cmy);
			c.floatValue = cmy.c;
			m.floatValue = cmy.m;
			y.floatValue = cmy.y;
			a.floatValue = cmy.a;
			EditorGUI.EndProperty();
		}
	}
}
