using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGroup : MonoBehaviour {
	public int ID;
	public GameObject pObj;
	public int Amount;
	public Transform Transforms;
	public List<PoolObj> PoolObjs = new List<PoolObj>();

	public void init( Transform root = null)
	{
		if (pObj != null) 
		{
			ID = pObj.GetInstanceID ();
			name = "pool group ["+ pObj.name +"]";
			transform.parent = (root!=null)? root : PoolManager.getTransform;
			transform.localPosition = Vector3.zero;
			this.Transforms = transform;
		}
	}


	public PoolObj FindAvalible(){
		foreach (PoolObj p in PoolObjs)
			if ( Service.GameObj.isObjectNotNull(p) )
			if (!p.isActive)
				return p;
		return null;
	}

	public void DeactiveAll()
	{
		foreach (PoolObj p in PoolObjs)
			if (Service.GameObj.isObjectNotNull(p))
				p.Deactive();
	}
	public void ReturnToGroupTransOrigin()
	{
		foreach (PoolObj p in PoolObjs)
			if (Service.GameObj.isObjectNotNull(p))
			{
				p.ReturnToOrigin();
			}
	}


	public void RefreshList(){
		List<PoolObj> temp = new List<PoolObj> ();
		foreach (PoolObj p in PoolObjs)
			if (p != null)
				temp.Add (p);
		PoolObjs = temp;
	}
}
