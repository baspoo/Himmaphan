
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lines : MonoBehaviour
{
	[System.Serializable]
	public class LineData
	{
		public string Name;
		public Vector3 Position;
		public Transform Transform;
		public bool IsUsing;

		public void Snap(Transform trans)
		{
			if (Transform == null) 
			{
				trans.ResetTransform();
				trans.localPosition = Position;
			}
            else 
			{
				trans.parent = Transform;
				trans.ResetTransform();
			}
		}
	}
	public List<LineData> LineDatas = new List<LineData>();
	public LineData GetLine(string Name) => LineDatas.Find(x=>x.Name == Name);
	public void ClearUsing() => LineDatas.ForEach(x=> { x.IsUsing = false; });
	public void Using(string Name) => LineDatas.ForEach(x => { if(x.Name == Name) x.IsUsing = true; });
	public void Unusing(string Name) => LineDatas.ForEach(x => { if (x.Name == Name) x.IsUsing = false; });
	public bool IsCanUsing(string Name) {
		var find = LineDatas.Find(x=>x.Name == Name);
		if (find != null)
			return !find.IsUsing;
		return false;
	}
	//public Vector3? GetPosition(string Name)
	//{
	//	var find = LineDatas.Find(x => x.Name == Name);
	//	if (find != null)
	//		return find.Position;
	//	return null;
	//}
	public List<LineData> GetUnusing(string[] Names)
	{
		var result = new List<LineData>();
		foreach (var path in Names) 
		{
			var line = GetLine(path);
			if(line!=null && !line.IsUsing) 
				result.Add(line);
		}
		return result;
	}




}


#if UNITY_EDITOR
[CustomEditor(typeof(Lines))]
public class LinesInspector : Editor {

	private void OnSceneGUI () {
		Lines line = target as Lines;
		Transform handleTransform = line.transform;
		Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
		foreach (var p in line.LineDatas) 
		{
			Vector3 path = handleTransform.TransformPoint(p.Position);
			Handles.color = Color.white;
			EditorGUI.BeginChangeCheck();
			path = Handles.DoPositionHandle(path, handleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(line, "Move Point");
				EditorUtility.SetDirty(line);
				p.Position = handleTransform.InverseTransformPoint(path);
			}
			Handles.Label(path, p.Name );
		}
	}
}
#endif