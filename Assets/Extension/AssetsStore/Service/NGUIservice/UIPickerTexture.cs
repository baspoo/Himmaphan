using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIPickerTexture : MonoBehaviour
{



	public Vector2 mPos;
	[SerializeField] Transform root;
    [SerializeField] UITexture texture;
	[SerializeField] UIWidget selectionWidget;

	public UnityAction<Vector2> OnUIClick;

	// Update is called once per frame
	void Update()
    {
        
    }

	UICamera mCam;
	void Start()
	{
		mCam = UICamera.FindCameraForLayer(gameObject.layer);
	}


	
	void OnPress(bool pressed) { if (enabled && pressed && UICamera.currentScheme != UICamera.ControlScheme.Controller) Sample(); }
	//void OnDrag(Vector2 delta) { if (enabled) Sample(); }
	void OnPan(Vector2 delta)
	{
		if (enabled)
		{
			mPos.x = Mathf.Clamp01(mPos.x + delta.x);
			mPos.y = Mathf.Clamp01(mPos.y + delta.y);
			Select(mPos);
		}
	}
	void Sample()
	{
		var pos = root.InverseTransformPoint(UICamera.lastWorldPosition);
		var corners = texture.localCorners;
		mPos.x = Mathf.Clamp01((pos.x - corners[0].x) / (corners[2].x - corners[0].x));
		mPos.y = Mathf.Clamp01((pos.y - corners[0].y) / (corners[2].y - corners[0].y));

		if (selectionWidget != null)
		{
			pos.x = Mathf.Lerp(corners[0].x, corners[2].x, mPos.x);
			pos.y = Mathf.Lerp(corners[0].y, corners[2].y, mPos.y);
			pos = root.TransformPoint(pos);
			selectionWidget.transform.OverlayPosition(pos, mCam.cachedCamera);
		}

		OnUIClick?.Invoke(selectionWidget.transform.position);
	}
	public void Select(Vector2 v)
	{
		v.x = Mathf.Clamp01(v.x);
		v.y = Mathf.Clamp01(v.y);
		mPos = v;

		if (selectionWidget != null)
		{
			Vector3[] corners = texture.localCorners;
			v.x = Mathf.Lerp(corners[0].x, corners[2].x, mPos.x);
			v.y = Mathf.Lerp(corners[0].y, corners[2].y, mPos.y);
			v = root.TransformPoint(v);
			selectionWidget.transform.OverlayPosition(v, mCam.cachedCamera);
		}

		OnUIClick?.Invoke(selectionWidget.transform.position);
	}


}
