using UnityEngine;
using System.Collections;


public enum MoveMatType
{
	x=0,
	y=1
}
public class MatreralMoveSet_ : MonoBehaviour {

	public MoveMatType MoveMatType_;
	public Material mat;
	public LineRenderer link;
	public float speed = 2.0f;
	public bool move = false;
	float offset;
	float x=0.0f;
	float y=0.0f;

	public bool isRandom = false;
	public float[] speeds;
	public bool isRandomOffset = false;
	public bool isOnce = false;
	public bool isMainTextureOffset = false;
	Renderer render;

	void Start () {
		if(MoveMatType_ == MoveMatType.x)x=1.0f;
		if(MoveMatType_ == MoveMatType.y)y=1.0f;


		render = gameObject.GetComponent<Renderer>();


		if (mat == null) {
			mat = render.material;
		}


		if (isRandom) 
		{
			
			if (speeds.Length == 2)
			{
				speed = Random.Range(-speeds[0], speeds[1]);
			}
			else 
			{
				speed = Random.Range(-speed, speed);
			}
		}
		if (isRandomOffset) 
		{
			offsetcustom.x = Random.Range(0.0f,1.0f);
			offsetcustom.y = Random.Range(0.0f, 1.0f);
		}
	}

	Vector2 offsetcustom = Vector2.zero;


	void Update () {
		if(move){

			if(render!=null)
			if (render.material != mat)
				mat = render.material;

			if (link != null)
				mat = link.material;


			offset += Time.deltaTime *speed;
			if(offset>=1.0f) 
			{ 
				offset= offset-1.0f; 
			}
			if(isMainTextureOffset) mat.mainTextureOffset = new Vector2(offset * x + offsetcustom.x, offset * y + offsetcustom.y);
			else mat.SetTextureOffset(GetOffset, new Vector2(offset*x + offsetcustom.x , offset* y + offsetcustom.y));

			if (isOnce) 
			{
				move = false;
			}

		}
	}


	


	public void Set (	float _x , float _y) {
		if (mat == null)
			mat = gameObject.GetComponent<Renderer> ().material;
		mat.SetTextureOffset(GetOffset, new Vector2(_x,_y));
	}


	public string OffsetKey;
	string GetOffset {

		get { return (string.IsNullOrEmpty(OffsetKey) ? "_MainTex" : OffsetKey); }
	}

}
