using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ray3d_obj : MonoBehaviour {
    public void reStart() { Start(); }
	void Start () {
		//layerMask = 1 << LayerMask.NameToLayer(LayerMask_);
		layerMask = ~(1 << LayerMask.NameToLayer(LayerMask_));
	}
    int layerMask = 0;
    public string LayerMask_;
    public List<Vector3> Diractions;
	Vector3 ray;
    public Vector3 TargetStart;
	public bool isParentThisTransform;
	public Color debugColor;
	RaycastHit hit;
    public float rang ;



	bool ishit(Vector3 dir) 
	{
		if(LayerMask_.isnull()) return Physics.Raycast(TargetStart, dir, out hit, rang);
		else return Physics.Raycast(TargetStart, dir, out hit, rang, layerMask);
	}

	void Update () {

		foreach (var Dir in Diractions)
		{
			var Diraction = Dir;
			if (isParentThisTransform)
			{
				TargetStart = this.transform.position;
				Diraction = transform.TransformDirection(Diraction);
			}
			ray = transform.TransformDirection(Diraction);
			Debug.DrawRay(TargetStart, Diraction * rang, debugColor);
			if (ishit(Diraction))
			{

				if (hit.collider.gameObject != null)
				{
					//Intersec collider
					if (_objTargetHit != null)
						if (_objTargetHit != hit.collider.gameObject)
							isHit = false;

					_objTargetHit = hit.collider.gameObject;
					if (!isHit)
						if (OnHit != null)
						{
							OnHit?.Invoke(_objTargetHit);
						}
					if (OnStay != null)
						OnStay?.Invoke(_objTargetHit);
				}
				isHit = true;
			}
			else
			{
				if (isHit)
				{
					if (OnOut != null)
						OnOut?.Invoke();
				}
				_objTargetHit = null;
				isHit = false;
			}

		}


	}
    public GameObject _objTargetHit;
    public System.Action<GameObject> OnHit;
	public System.Action OnOut;
	public System.Action<GameObject> OnStay;
    public bool isHit;




}
