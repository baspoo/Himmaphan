using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UISpinDragEvent : MonoBehaviour
{
	public int index = 0;
	public float sensitive = 0.1f;
	void Start()
	{
		isCan = true;
	}


	bool isCan = false;
	float timeRun;
	float timeMax = 0.1f;
    private void Update()
    {
		if (isCan)
			return;

		if (timeRun < timeMax)
			timeRun += Time.deltaTime;

		else
		{
			isCan = true;
			timeRun = 0.0f;
		}

	}

    void OnDrag(Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

		if (!isCan)
			return;

		if (Mathf.Abs(delta.x) < sensitive)
			return;


		isCan = false;
		if (delta.x < 0)
		{
			index--;
			//Debug.Log("Drag Left");
			OnLeft?.Execute();
		}
		else
		{
			index++;
			//Debug.Log("Drag Right");
			OnRight?.Execute();
		}
	}

	public EventDelegate OnLeft;
	public EventDelegate OnRight;
}