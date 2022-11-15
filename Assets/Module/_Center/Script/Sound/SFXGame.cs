using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SFXGame : MonoBehaviour
{

    static SFXGame m_instance;
    public static SFXGame instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = ((GameObject)Resources.Load("Sound/SFXGame")).GetComponent<SFXGame>();
            }
            return m_instance;
        }
    }
    public AudioClip sfx_click;
    public AudioClip sfx_select;
    public AudioClip sfx_submit;
    public AudioClip sfx_back;
    public AudioClip sfx_movepage;




}




























