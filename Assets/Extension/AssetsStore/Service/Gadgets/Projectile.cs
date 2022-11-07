using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public delegate void onCallback ();
	onCallback m_callback;
	Vector3 m_origin_position;
	Vector3 m_destination_position;
	public float startAngle = 0;
	public float endAngle = 0;
	public float m_speed;
	public float m_height;
	public float m_progress;
	public bool isRotateAngle = true;
	public bool isAutoDestory = true;
	public GameObject particle;

	public float Progress{	get{ return m_progress; }}
	[TextArea]
	public string status;	

	public static Projectile AddProjectile(Transform origin,Transform destination,float speed = 0.02f,float height = 20f, bool isRotateAngle  = true , onCallback callback = null){
		Projectile projectile = origin.gameObject.AddComponent<Projectile> ();
		projectile.m_origin_position = origin.position;
		projectile.m_destination_position = destination.position;
		projectile.m_speed = speed;
		projectile.m_height = height;
		projectile.m_progress = 0;
		projectile.isRotateAngle = isRotateAngle;
		projectile.m_callback = callback;
		return projectile;
	}


	public void Letgo( Transform target , onCallback callback = null){
		m_progress = 0.0f;
		m_origin_position = transform.position;
		m_destination_position = target.position;
		m_callback = callback;
		enabled = true;
	}
	public void Letgo( Vector3 target, onCallback callback = null)
	{
		m_progress = 0.0f;
		m_origin_position = transform.position;
		m_destination_position = target;
		m_callback = callback;
		enabled = true;
	}


	bool isPause = false;
    public void Pause(bool isPause)
    {
        this.isPause = isPause;
    }


    Vector3 ParametricPos(float t) {
		var pos = Vector3.Lerp(m_origin_position, m_destination_position, t);
		pos.y = 4*(t -t*t)*m_height + t*m_destination_position.y + (1-t)*m_origin_position.y;
		return pos;
	}
	void FixedUpdate () {

        if (isPause) return;
		//**----- Transform
		if (m_progress > 1) {
			status = "Progress: FINISHED!!";
			if(m_callback!=null) 	m_callback ();
			if(isAutoDestory) 		Destroy (this);
			enabled = false;
			if (particle != null)
				PoolManager.Spawn(particle,transform,2.0f);
		} 
		else 
		{
			transform.position = ParametricPos(m_progress);
			m_progress += m_speed*Time.deltaTime;
			status = "Progress: " + m_progress.ToString ("##.##");
		}



		//**----- Rotation
		if (isRotateAngle) {
			Vector3 op = m_origin_position;
			op.y = 0;
			Vector3 dp = m_destination_position;
			dp.y = 0;
			// Projection of object on line between origin and destination
			var projection = Vector3.Project (transform.position - m_origin_position, op - dp) + op;
			// Distance to origin
			var dstOrigin = Vector3.Distance (op, projection);
			// Distance to destination
			var dstDestination = Vector3.Distance (dp, projection);
			// Parameter
			float t = (dstDestination + dstOrigin == 0 ? 0 : (dstOrigin / (dstDestination + dstOrigin)));
			// The angle at current t
			float angle = Mathf.Lerp (startAngle, endAngle, t);

			// Rotate the object by the angle, then make sure it also faces destination
			transform.rotation = Quaternion.LookRotation (dp - op) * Quaternion.Euler (angle, 0, 0);
		}






	}

}
