using UnityEngine;

public class LabelNumberRun : MonoBehaviour {
	public UILabel ui_text;
	public int CountRunning;
	public float TimeingPerCount;
	public string textBegin;
	public string textEnd;
	public EventDelegate OnFinish;
	int currentCount = 0;
	float currentTime = 0.0f;
	int m_to,m_from;
	int valuePlus = 0;
	int currentValue = 0;
	int m_countRunning = 0;

	bool isRunning = false;
	public int GetCurrentValue {
		get{ return currentValue; }
	}
	public void RunStartByCurrentValue (  int to ) {
		Run (currentValue,to);
	}
	public void Run ( int from , int to ) {
		int difference =  to - from;
		currentValue = from;
		m_to = to;
		m_from = from;

		if (Mathf.Abs (to - from) < CountRunning) {
			valuePlus = difference > 0 ? 1 : -1;
			m_countRunning = Mathf.Abs (to - from);
		}
		else {
			valuePlus = difference / CountRunning;
			m_countRunning = CountRunning;
		}


		currentCount = 0;
		currentTime = 0.0f;
		isRunning = true;
		SetText ( );
	}
	void SetText (  ) {
		if (currentCount == m_countRunning) {
			ui_text.text = textBegin+m_to.ToString ("#,##0")+textEnd;
			isRunning = false;
			if (OnFinish != null)
				OnFinish.Execute ();
		}
		else {
			ui_text.text = textBegin+currentValue.ToString ("#,##0")+textEnd;
		}
	}
	void Update () {
		if (isRunning) {
			if (currentTime < TimeingPerCount)
				currentTime += Time.deltaTime;
			else {
				currentTime = 0.0f;
				currentCount++;
				currentValue += valuePlus;
				SetText ();
			}
		}
	}


	public bool isRunTestInAwake;
	void OnEnable(){
		if(isRunTestInAwake)Run (0,2200);
	}



}
