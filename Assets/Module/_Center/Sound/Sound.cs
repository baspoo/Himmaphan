using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif


public class Sound : MonoBehaviour {

#if UNITY_EDITOR
	[MenuItem("Utility/Sound/Playlist")]
	public static void MenuSetup2() {
		Selection.activeObject = Playlist.instance;
		//((GameObject)Selection.activeObject).transform.hideFlags = HideFlags.HideInInspector;
		AssetDatabase.OpenAsset(Selection.activeObject);
	}
#endif


	
	
	static Sound m_sound;
	public static Sound sound {
		get {
			if (m_sound == null) 
			{
				m_sound = FindObjectOfType<Sound>();
			}
			return m_sound;
		}
	}














	static float def_bgmvolume;
	static float def_sxfvolume;
	public void Init() 
	{
		def_bgmvolume = Bgm.volume;
		def_sxfvolume = Bgm.volume;

		is_sfx_mute = !IsSfx;
		is_bgm_mute = !IsBgm;
	}






	public static bool IsSfx 
	{
		get 
		{
			return PlayerPrefs.GetInt("sound.sfx") == 0;
		}
		set 
		{
			is_sfx_mute = !value;
			PlayerPrefs.SetInt("sound.sfx", value ? 0 : 1);
		}
	}
	public static bool IsBgm
	{
		get
		{
			return PlayerPrefs.GetInt("sound.bgm") == 0;
		}
		set
		{
			is_bgm_mute = !value;
			if (value)
				sound?.ResumeBGM();
			else
				sound?.StopBGM();
			PlayerPrefs.SetInt("sound.bgm", value ? 0 : 1);
		}
	}













	public static bool is_bgm_mute { get; private set; }
	public static bool is_sfx_mute { get; private set; }
	public AudioSource Sfx;
	public AudioSource Bgm;


	static float lasttime_sound;
	static AudioClip last_clip;

	static soundcheck m_soundcheck = null;
	public class soundcheck{
		public float time;
		public List<AudioClip> clips;
	}
	public void PlayBGM( AudioClip clip )
	{
		Bgm.clip = clip;
		if (!pause)
		{
			Bgm.Stop();
			if(!is_bgm_mute) 
				Bgm.Play();
		}
	}

	bool pause;
	public void ResumeBGM()
	{
		if (!Bgm.isPlaying)
		{
			Bgm.Stop();
			if (!is_bgm_mute) 
				Bgm.Play();
		}
		pause = false;
		Bgm.mute = false;
	}
	public void StopBGM()
	{
		Bgm.Stop();
		pause = true;
		Bgm.mute = true;
	}



	public static void Play( AudioClip clip , bool unmuteBackground = false)
	{
		if (Time.time < 1)
			return;

		if (clip == null)
			return;


		//Debug.Log($"clip : {clip.name}");

		if (!unmuteBackground)
		{
			if (InterfaceRoot.instance != null)
			{
				if (InterfaceRoot.instance.IsbackgroundClossing)
					return;
			}
		}




		if (m_soundcheck != null) 
		{
			if (m_soundcheck.time == Time.time) 
			{
				if (m_soundcheck.clips.Contains(clip))
				{
					return;
				}
			}
			else m_soundcheck = null;
		}

		if (m_soundcheck == null)
		{
			m_soundcheck = new soundcheck() { time = Time.time };
			m_soundcheck.clips = new List<AudioClip>();
		}
		m_soundcheck.clips.Add(clip);

		if (!is_sfx_mute)
			sound.Sfx.PlayOneShot (clip);
		lasttime_sound = Time.time;
		last_clip = clip;
	}






	public static AudioSource CreatedAudio(AudioClip _sound , float duration = 0.0f ){
		var audioSource = CreatedAudio (_sound,duration,false,Sound.sound.transform);
		return audioSource;
	}
	static List<AudioSource> AudioSources = new List<AudioSource> ();
	public static AudioSource CreatedAudio(AudioClip _sound , float duration , bool loop , Transform transform ){

		if (transform == null)
			return null;

		AudioSource Sfx = transform.gameObject.AddComponent<AudioSource>();

		//-----------------------------------------------------------------------------
		// Update List AudioSources.
		//-----------------------------------------------------------------------------
		List<AudioSource> newlist = new List<AudioSource> ();
		newlist.Add (Sfx);
		foreach(AudioSource a in AudioSources){
			if (a != null)
				newlist.Add (a);
		}
		AudioSources = newlist;



		Sfx.clip = _sound;
		Sfx.volume = m_sound.Sfx.volume;
		Sfx.loop = loop;
		Sfx.Play();
		if (duration != 0.0f) {
			if(Sfx!=null)
				Service.Timmer.Wait (duration,()=>{
					Destroy(Sfx);
			});
		}
		return Sfx;
	}
	public static void ClearAndStopAllCreatedAudio() {
		foreach (AudioSource a in AudioSources)
		{
			if (a != null)
				Destroy(a);
		}
		AudioSources.Clear();
	}








	
	public static void OnResetBgmVolume( ){
		m_sound.Bgm.volume = def_bgmvolume;
	}

	public static void OnDimBgmVolume(float volume = 0.1f)
	{
		m_sound.Bgm.volume = volume;
	}





}
