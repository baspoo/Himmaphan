using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseService : MonoBehaviour
{
    static FirebaseService m_Instance;
    public static FirebaseService Instance{
        get {
            if(m_Instance == null) 
            {
                var fireBase = new GameObject("FirebaseService");
                m_Instance = fireBase.AddComponent<FirebaseService>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }




    bool m_init = false;
    public void Init(System.Action complete) 
    {
        if (m_init)
        {
            complete?.Invoke();
        }
        else 
        {
            StartCoroutine(DoInit(complete));
        }
    }
    IEnumerator DoInit(System.Action complete) 
    {
        yield return new WaitForEndOfFrame();
        m_init = true;
        complete?.Invoke();
    }















}
