using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false; // ï“èWÇ≈Ç´Ç»Ç≠Ç∑ÇÈ
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;  // å≥Ç…ñﬂÇ∑
	}
}
