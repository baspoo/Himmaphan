using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHover : MonoBehaviour
{

    public static bool Hover 
    {
        get
        {
            return m_hover || m_hoverAlway;
        }
    }


    static bool m_hover;
    static bool m_hoverAlway;

    public static void ForceHoverAlway(bool hover) 
    {
        m_hoverAlway = hover;
    }

    [SerializeField] bool m_Hover;
    [SerializeField] bool m_HoverAlway;
    UIEventTrigger et;

    void Awake()
    {
        et = gameObject.AddComponent<UIEventTrigger>();
        et.onHoverOver.Add(new EventDelegate(()=> {
            m_hover = false;
        }));
        et.onHoverOut.Add(new EventDelegate(() => {
            m_hover = true;
        }));
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        m_Hover = m_hover;
        m_HoverAlway = m_hoverAlway;
        #endif
    }
}
