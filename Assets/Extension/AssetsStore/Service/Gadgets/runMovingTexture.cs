using UnityEngine;
using System.Collections;

public class runMovingTexture : MonoBehaviour {

	float width_start;
	float hight_start;

	public UITexture _texture;
	void Start () {
		if(_texture==null)
			if( gameObject.GetComponent<UITexture>()!=null)
				_texture = gameObject.GetComponent<UITexture>();

		width_start = _texture.uvRect.width ;
		hight_start = _texture.uvRect.height ;

		_texture.uvRect = new Rect(0.5f,0.5f,width_start,hight_start);
	}

	float xsum;
	float ysum;
	public float Xins;
	public float Yins;
	void Update()
	{

		if(_texture!=null)
		{
			xsum+=Xins*Time.deltaTime;
			ysum+=Yins*Time.deltaTime;
			if(xsum>1)	xsum = xsum-1;
			if(ysum>1)	ysum = ysum-1;
			_texture.uvRect = new Rect(xsum,ysum,width_start,hight_start);
		}

	}


}
