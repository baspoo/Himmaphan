using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GameSetting : MonoBehaviour
{
    static GameSetting m_instance;
    public static GameSetting instance{
        get {
            if (m_instance == null)
                m_instance = FindObjectOfType<GameSetting>();
            return m_instance;
        }
    }














}



