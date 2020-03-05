/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace MakeIt.Colorful
{
	/// <summary>
	/// Property drawer for the ColorHSL color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHSL))]
	public class ColorHSLPropertyDrawer : PropertyDrawer
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
			var s = property.FindPropertyRelative("s");
			var l = property.FindPropertyRelative("l");
			var a = property.FindPropertyRelative("a");
			var hcy = new ColorHSL(h.floatValue, s.floatValue, l.floatValue, a.floatValue);
			hcy = EditorGUI.ColorField(position, hcy);
			h.floatValue = hcy.h;
			s.floatValue = hcy.s;
			l.floatValue = hcy.l;
			a.floatValue = hcy.a;
			EditorGUI.EndProperty();
		}
	}
}
