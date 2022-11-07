using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineProjection : MonoBehaviour
{

	//public Transform target;
	public Vector3 target;
	public LineRenderer line;
	public int lineCount = 10;
	public float degree = 10.0f;

	float minimumDistance = 0f;

	//public float distanceupdate;
	void Update()
	{
		updateLine();
	}


	float distance
	{
		get
		{
			return 1.0f;
		}
	}
	public void OnTarget(Transform target, float minDistance = 0f) {
		OnTarget(target.position, minDistance);
	}

	public void OnTarget(Vector3 target, float minDistance = 0f)
	{
		enabled = true;
		this.agent = null;
		this.target = target;
		minimumDistance = minDistance;
	}

	UnityEngine.AI.NavMeshAgent agent;
	public void OnTarget(UnityEngine.AI.NavMeshAgent agent)
	{
		enabled = true;
		this.target = Vector3.zero;
		this.agent = agent;
	}
	public void OnClose( )
	{
		this.agent = null;
		this.target = Vector3.zero;
	}


	Vector3 ParametricPos(float progress, float hight, Vector3 origin, Vector3 destination)
	{
		Vector3 pos = Vector3.Lerp(origin, destination, progress);
		pos.y = distance * (progress - progress * progress) * hight + progress * destination.y + (1 - progress) * origin.y;
		return pos;
	}




	void DrawPath(UnityEngine.AI.NavMeshAgent agent)
	{
		if (agent.path.corners.Length < 2) //if the path has 1 or no corners, there is no need
			return;

		line.positionCount = (agent.path.corners.Length); //set the array of positions to the amount of corners

		line.SetPosition(0, agent.transform.position);

		for (int i = 1; i < agent.path.corners.Length; i++)
		{
			line.SetPosition(i, agent.path.corners[i]); //go through each corner and set that to the line renderer's position
		}
	}


	void updateLine()
	{
		if (agent != null)
		{
			line.enabled = true;
			DrawPath(agent);
			return;
		}


		if (target == null || target == Vector3.zero) 
		{
			enabled = false;
			line.enabled = false;
			line.SetVertexCount(0);
			OnClose();
			return;
		}

		line.enabled = true;
		line.SetVertexCount(lineCount+1);
		for (int x = 0; x < lineCount; x++)
		{
			//Vector3 Position = ParametricPos((float)x / (float)line.positionCount, degree, transform.position, target.position);
			Vector3 Position = ParametricPos((float)x / (float)line.positionCount, degree, transform.position, target);
			line.SetPosition(x, Position);
		}
		//line.SetPosition(lineCount, target.position);
		line.SetPosition(lineCount, target);

		if(Vector3.Distance(this.target, transform.position) < minimumDistance)
        {
			target = Vector3.zero;
		}
		uplineDistance();
	}




	public float scaleTiling , maxLineTiling, minLineTiling;

	void uplineDistance() 
	{
		var dist = Vector3.Distance(this.target, transform.position)* scaleTiling;
		if (dist < minLineTiling) dist = minLineTiling;
		if (dist > maxLineTiling) dist = maxLineTiling;
		line.material.mainTextureScale = new Vector2( dist , 1.0f);
	}


}
