using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class RuntimeBtn
{


	public enum pram { None, String, Double, Transform, MonoBehaviour }

	public RuntimeBtn(System.Action<RuntimeBtn> action)
	{
		this.Action = action;
		this.BtnName = "Run";
	}
	public RuntimeBtn(string btnName, System.Action<RuntimeBtn> action)
	{
		this.Action = action;
		this.BtnName = btnName;
	}
	public pram Prams;
	public System.Action<RuntimeBtn> Action;
	public string BtnName;
	public string String;
	public double Double;
	public Transform Transform;
	public MonoBehaviour MonoBehaviour;

#if UNITY_EDITOR
	public GameObject Gameobject => (GameObject)Selection.activeObject;
#else
		public GameObject Gameobject => Transform.gameObject;	
#endif


}





#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RuntimeBtn))]
public class RuntimeBtnDrawer : PropertyDrawer
{
	int nextwidth=0;
	int pramboxsize = 150;
	void Reposition(){ nextwidth = 0; }
	Rect Position( Rect position , int width)
	{
		Rect R = new Rect(position.x	+ nextwidth	, position.y, width	, position.height);
		nextwidth += width + 5;
		return R;
	}
	public string pathKeep;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		Reposition ();



		

		if (GUI.Button(Position(position, 100), property.FindPropertyRelative("BtnName").stringValue))
		{
			var targetObject = property.serializedObject.targetObject;
			var targetObjectClassType = targetObject.GetType();
			var field = targetObjectClassType.GetField(property.propertyPath);
			if (field != null)
			{
				var value = field.GetValue(targetObject);
				RuntimeBtn g = (RuntimeBtn)value;
				g.Action?.Invoke(g);
			}
		}

		//	public enum pram { String, Double, Obj, MonoBehaviour }
		EditorGUI.PropertyField(Position(position, 60), property.FindPropertyRelative("Prams"), GUIContent.none);
		var pram = (RuntimeBtn.pram)property.FindPropertyRelative("Prams").enumValueIndex;
        switch (pram)
        {
            case RuntimeBtn.pram.String:
				EditorGUI.PropertyField(Position(position, pramboxsize), property.FindPropertyRelative("String"), GUIContent.none);
				break;
            case RuntimeBtn.pram.Double:
				EditorGUI.PropertyField(Position(position, pramboxsize), property.FindPropertyRelative("Double"), GUIContent.none);
				break;
            case RuntimeBtn.pram.Transform:
				EditorGUI.PropertyField(Position(position, pramboxsize), property.FindPropertyRelative("Transform"), GUIContent.none);
				break;
            case RuntimeBtn.pram.MonoBehaviour:
				EditorGUI.PropertyField(Position(position, pramboxsize), property.FindPropertyRelative("MonoBehaviour"), GUIContent.none);
				break;
            default:
                break;
        }

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
#endif
















