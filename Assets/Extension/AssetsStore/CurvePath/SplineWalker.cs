using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class SplineWalker : MonoBehaviour {


	public Transform locator;
	public BezierSpline spline;
	public BezierCurve curve;

	public float duration;

	public bool lookForward;
	public bool isAnimRunning;
	public Bezier.SplineWalkerMode mode;



	
	public float progress
    {
		get { return m_progress; }
		set { m_progress = value; UpdateLocator(); }
	}


	[SerializeField]
	[Range(0.0f, 1.0f)]
	float m_progress;


	float mPointProgress;

	public void ResetAnimation( ) 
	{
		m_progress = 0.0f;
		isAnimRunning = true;
	}



	bool goingForward;
	private void Update () {

		if (!isAnimRunning)
			return;

		if (goingForward) {
			m_progress += Time.deltaTime / duration;
			if (m_progress > 1f) 
			{
				if (mode == Bezier.SplineWalkerMode.Once) {
					m_progress = 1f;
				}
				else if (mode == Bezier.SplineWalkerMode.Loop) {
					m_progress -= 1f;
				}
				else {
					m_progress = 2f - m_progress;
					goingForward = false;
				}
				OnFinish?.Execute();
			}
		}
		else {
			m_progress -= Time.deltaTime / duration;
			if (m_progress < 0f) {
				m_progress = -m_progress;
				goingForward = true;
			}
		}




		UpdateLocator();



	}

	public EventDelegate OnFinish;

    public void UpdateLocator()
    {
		if (locator != null)
		{
			if (spline != null)
			{
				Vector3 position = spline.GetPoint(m_progress);
				locator.localPosition = position;
				if (lookForward)
				{
					locator.LookAt(position + spline.GetDirection(m_progress));
				}
			}
			if (curve != null)
			{
				Vector3 position = curve.GetPoint(m_progress);
				locator.localPosition = position;
				if (lookForward)
				{
					locator.LookAt(position + curve.GetDirection(m_progress));
				}
			}
		}
	}
}














#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SplineWalker))]
[System.Serializable]
public class SplineWalkerUI : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var splineWalker = (SplineWalker)target;
		splineWalker.UpdateLocator();
	}
}
#endif