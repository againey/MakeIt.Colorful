/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace Experilous.MakeItColorful
{
	/// <summary>
	/// Property drawer for the ColorHCY color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHCY))]
	public class ColorHCYPropertyDrawer : PropertyDrawer
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
			var y = property.FindPropertyRelative("y");
			var a = property.FindPropertyRelative("a");
			var hcy = new ColorHCY(h.floatValue, c.floatValue, y.floatValue, a.floatValue);
			hcy = EditorGUI.ColorField(position, hcy);
			h.floatValue = hcy.h;
			c.floatValue = hcy.c;
			y.floatValue = hcy.y;
			a.floatValue = hcy.a;
			EditorGUI.EndProperty();
		}
	}
}
