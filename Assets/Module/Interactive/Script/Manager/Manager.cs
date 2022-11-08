using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;


namespace Interact
{
	public class Manager : MonoBehaviour
	{
		static Manager m_instance;

		public static Manager instance
		{
			get
			{
				if (m_instance == null)
					m_instance = FindObjectOfType<Manager>();
				return m_instance;
			}
		}
		IEnumerator Start()
		{
			m_instance = this;
			PlayingData.Create();
			LoadCenter.instance.Init();
			Store.instance.Init();
			Sound.sound.Init();
			InterfaceRoot.instance.Init();
			SceneHandle.instance.Init();
			UIGameSettingPage.Open(true);
			Application.targetFrameRate = 60;
			yield return new WaitForEndOfFrame();


			//bool loadcenter = false;
			//LoadCenter.instance.Download((complete) => { if(complete) loadcenter = true; });
			//while(!loadcenter) yield return new WaitForEndOfFrame();
			SceneHandle.instance.ApplyPlist();

			Init();
		}
		public static bool IsReady = false;
		public void Init()
		{
			IsReady = true;
			InterfaceRoot.instance.OnReady();
			Debug.Log("Init Done");
		}
	}
}