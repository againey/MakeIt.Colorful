/******************************************************************************\
* Copyright Andy Gainey                                                        *
\******************************************************************************/

using UnityEngine;
using UnityEditor;

namespace MakeIt.Colorful
{
	/// <summary>
	/// Property drawer for the ColorHSY color space struct.  Defers to the standard Unity Editor color field UI.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorHSY))]
	public class ColorHSYPropertyDrawer : PropertyDrawer
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
			var y = property.FindPropertyRelative("y");
			var a = property.FindPropertyRelative("a");
			var hcy = new ColorHSY(h.floatValue, s.floatValue, y.floatValue, a.floatValue);
			hcy = EditorGUI.ColorField(position, hcy);
			h.floatValue = hcy.h;
			s.floatValue = hcy.s;
			y.floatValue = hcy.y;
			a.floatValue = hcy.a;
			EditorGUI.EndProperty();
		}
	}
}
