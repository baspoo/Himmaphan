using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SFXInteract : MonoBehaviour
{

    static SFXInteract m_instance;
    public static SFXInteract instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = ((GameObject)Resources.Load("Sound/SFXInteract")).GetComponent<SFXInteract>();
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



























