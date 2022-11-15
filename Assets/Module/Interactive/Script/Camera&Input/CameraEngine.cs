using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactive.CameraControl
{
	public class CameraEngine : MonoBehaviour
	{
		static CameraEngine m_instance;
		public static CameraEngine instance
		{
			get
			{
				if (m_instance == null)
					m_instance = FindObjectOfType<CameraEngine>();
				return m_instance;
			}
		}
		public static void OnEnableEngine(bool enable)
		{
			IsStopCameraEngine = enable;
		}




		static bool stopInput = false;
		public static bool IsCameraInput
		{
			get
			{
				return UIHover.Hover && !stopInput;
			}
		}


		public static bool IsStopCameraEngine { get; private set; }





		public Camera maincamera;
		public Transform rootCamera;
		public Transform rootEffect;
		public CameraCtr cameraCtr;
		//public CameraAnimation cameraanim;
		public UserInput input;
		//public Minimap minimap;
		public float smoothspeed;

		private void Awake()
		{
			Init();
		}
		public void Init()
		{
			m_instance = this;
			cameraCtr.Init();
			input.Init();
			//minimap.Init();
		}

		CameraFollowType mCameraFollowType;
		public enum CameraFollowType
		{
			None, Island, Player
		}

		Transform m_target;
		Player.PlayerClient m_client;
		public void OnCameraFollow(Transform target)
		{
			m_target = target;
			cameraCtr.OnReset();
			input.onInput = null;
		}
		public void OnCameraFollowCharacter(Player.PlayerClient client, bool reset = false)
		{
			m_client = client;
			m_target = client.transform;
			cameraCtr.SetupLimitIndex(0, reset);
			mCameraFollowType = CameraFollowType.Player;
			input.onInput = (hit, location, held) =>
			{
				if (input.warp) client.pMovement.JumpTo(location, held);
				else client.pMovement.MoveTo(location, held);
			};
		}
		public void OnCameraCenterCharacter(bool enable)
		{
			if (enable)
			{
				//-> Handle Move Camera To Near Center Character
				input.enabled = false;
				cameraCtr.IsIgnoreZoom = true;
				cameraCtr.OnToNearCenter();
				m_client?.avatarObj.LookAt(maincamera.transform);
				m_client?.pInterface.OnVisible(false);
			}
			else
			{
				//-> Return To Defualt
				input.enabled = true;
				cameraCtr.IsIgnoreZoom = false;
				cameraCtr.OnReset();
				m_client?.avatarObj.LookAt(null);
				m_client?.pInterface.OnVisible(true);
			}
		}







		public void OnForceSnap()
		{
			if (m_target != null)
				rootCamera.position = m_target.position;
		}
		private void Update()
		{
			if (CameraEngine.IsStopCameraEngine)
				return;

			if (m_target != null)
			{
				rootCamera.position = Vector3.Slerp(rootCamera.position, m_target.position, Time.deltaTime * smoothspeed);
			}
		}


	}
}
