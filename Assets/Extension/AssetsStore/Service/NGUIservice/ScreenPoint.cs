using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPoint : MonoBehaviour
{

	#region SERVICE
	public static Vector3 GetWorldPositionToOtherCam(Camera maincam,Transform target, Camera othercam){
		Vector3 position = Vector3.zero;
		position = maincam.WorldToScreenPoint(target.position);
		position = othercam.ScreenToWorldPoint (position);
		return position;
	}
	public static Vector3 GetWorldPositionToNGUI(Camera maincam,Transform target, UIRoot root , UIAnchor anc = null){

		//** Setup Anc
		if (anc != null) {
			anc.side = UIAnchor.Side.BottomLeft;
			anc.runOnlyOnce = false;
			anc.enabled = true;
		}


		if (maincam == null)
			maincam = Camera.main;

		//** Position
		Vector3 position = Vector3.zero;
		position = maincam.WorldToScreenPoint(target.position);

		if (position.x * position.y * position.z < 0) return new Vector3(-10000f, -10000f, -10000f);

		//** Screen Percent
		Vector3 percent = Vector3.zero;
		percent.x = position.x / (float)Screen.width;
		percent.y = position.y / (float)Screen.height;

		//** Canvert to Panel Position
		Vector3 panelscreen = Vector3.zero;
		panelscreen.x  =  (float)Screen.width * root.pixelSizeAdjustment;
		panelscreen.y  =  (float)Screen.height * root.pixelSizeAdjustment;
		position.x = panelscreen.x * percent.x;
		position.y = panelscreen.y * percent.y;

		return position;
	}
	#endregion




	[Header("MainCamera")]
	public Camera cam;
	public Transform target;
    void Update()
    {
		NguiUpdate ();
	}


	[Header("NGUI")]
	public bool isNgui;
	public UIRoot root;
	public UIAnchor anc;
	public Transform move;
	public Vector3 NGUIMousePosition()
	{
		var pos = Input.mousePosition;
		pos.z = 0;
		pos = anc.uiCamera.ScreenToWorldPoint(pos);
		return pos;
	}
	public Vector3 NGUITargetPosition( Transform objectWorld3D )
	{
		moveBase.transform.localPosition = GetWorldPositionToNGUI(cam, objectWorld3D, root);
		return moveBase.transform.position;
	}
	void NguiUpdate()
	{
		if (!isNgui) return;

		if(move!=null && target != null) 
			move.transform.localPosition =  GetWorldPositionToNGUI(cam,target,root);

		if (Lookings.Count > 0) 
		{
			var remove = new List<Looking>();
			foreach (var look in Lookings) 
			{
				if ( look.isReady ) 
				{
					look.move.transform.localPosition = GetWorldPositionToNGUI(cam, look.target3D, root);
					look.uipage.position = look.move.transform.position;
					look.onUpdate?.Invoke();
				}
				else if (look.target3D == null || look.uipage == null)
					remove.Add(look);
			}
			if(remove.Count>0)
				foreach (var look in remove)
				{
					RemoveLooking(look);
				}
		}
	}








#if UNITY_EDITOR
	public RuntimeBtn Setup = new RuntimeBtn("UIAnchor", (r) => {


		var root = (GameObject)UnityEditor.Selection.activeObject;
		var screen = root.GetComponent<ScreenPoint>();
		if (screen != null) 
		{

			if (screen.anc == null)
				screen.anc = screen.gameObject.GetComponent<UIAnchor>();

			if (screen.anc == null && screen.gameObject.GetComponent<UIAnchor>() == null)
				screen.anc = screen.gameObject.AddComponent<UIAnchor>();

			if (screen.anc != null)
			{
				screen.anc.side = UIAnchor.Side.BottomLeft;
				screen.anc.runOnlyOnce = false;
				screen.anc.enabled = true;
			}
		}

	});
#endif

	List<Looking> Lookings = new List<Looking>();
	public class Looking {
		public bool ignore;
		public Transform uipage;
		public PoolObj move;
		public Transform target3D;
		public string name;
		public System.Action onUpdate;
		public bool isReady => (!ignore && uipage != null && move != null && target3D != null);

	}
	GameObject m_moveBase;
	GameObject moveBase
    {
		get {
			if (m_moveBase == null)
			{
				m_moveBase = new GameObject("moveBase");
				m_moveBase.transform.parent = transform;
			}
			return m_moveBase;
		}
	}
	public Looking AddLooking(Transform uipage, Transform target3D , string name = null) 
	{
		var looking = new Looking
		{
			uipage = uipage,
			move = PoolManager.SpawParent(moveBase, transform),
			target3D = target3D,
			name = name
		};
		Lookings.Add(looking);
		return looking;
	}
	public void RemoveLooking(Looking looking)
	{
		looking.move.Deactive();
		Lookings.Remove(looking);
	}
	public void RemoveLooking(string looking)
	{
		Lookings.FindAll(x => x.name == looking).ForEach(x => RemoveLooking(x));
	}
	public void RemoveLooking(Transform looking)
	{
		Lookings.RemoveAll(x=>x.uipage == looking);
		Lookings.RemoveAll(x => x.target3D == looking);
	}






























#if UNITY_EDITOR
	public class ScreenPointExample : MonoBehaviour
	{
		public bool lockMouse;
		public ScreenPoint ScreenPoint;
		public Transform page;
		public Transform target;
		ScreenPoint.Looking look;
		void Update()
		{
			//if (Input.GetKeyDown(KeyCode.A))
			//{
			//	look = ScreenPoint.AddLooking(page, target);

			//}
			//if (Input.GetKeyDown(KeyCode.S))
			//{
			//	ScreenPoint.RemoveLooking(look);

			//}
			//if (Input.GetKeyDown(KeyCode.G))
			//{
			//	page.transform.position = ScreenPoint.NGUITargetPosition(target);

			//}
			if (lockMouse)
			{
				page.transform.position = ScreenPoint.NGUIMousePosition();
			}
		}
	}
#endif





}



