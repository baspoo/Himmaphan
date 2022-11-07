using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class FillScaleTexture : MonoBehaviour
{


#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FillScaleTexture))]
    [System.Serializable]
    public class FillScaleTextureUI : Editor
    {
        public FillScaleTexture m_tools { get { return ((GameObject)Selection.activeObject).GetComponent<FillScaleTexture>(); } }
        public override void OnInspectorGUI() {

                if (!Service.GameObj.isObjectNotNull(m_tools))
                {
                    return;
                }

            float v = EditorGUILayout.Slider(m_tools.value, 0.0f, 1.0f);
            if (v != m_tools.value) m_tools.value = v;
            m_tools.isAnim = EditorGUILayout.ToggleLeft("isAnim", m_tools.isAnim);
            if (m_tools.isAnim)
            {
                m_tools.Speed = EditorGUILayout.FloatField("Speed Anim", m_tools.Speed);
            }
        }
    }
#endif









    UITexture m_texture = null;
    public float value {
        get { return m_value; }
        set { OnChange(value); }
    }
    public bool isAnim = false;
    public float Speed = 1.0f;
    float m_value = 0.0f;
    int height;
    int width;
    int lastwidth;
    int min = 0;
    Vector4 border = new Vector4();
    Vector4 minborder = new Vector4();
    private bool Init()
    {
        if (m_texture == null) {
            m_texture = GetComponent<  UITexture >();
            if (m_texture != null)
            {
                width = m_texture.width;
                height = m_texture.height;
                if (m_texture.type == UIBasicSprite.Type.Sliced)
                {

                    //left   = x
                    //bot    = y
                    //right  = z
                    //top    = w
                    border = m_texture.border;
                    minborder = border;
                    minborder.x = 0.0f;
                    minborder.z = 0.0f;
                    min = (int) ( m_texture.border.x + m_texture.border.z);
                } 
            }
        }
        return (m_texture != null);
    }




    void OnChange( float percent = 0.0f )
    {


        if (m_texture != null && lastwidth != 0 && m_texture.width != lastwidth)
        {
            m_texture = null;
            Debug.Log("modify width");
        }

            if (!Init())
            return;
        if (percent >= 1.0f) percent = 1.0f;
        if (percent <= 0.0f) percent = 0.0f;
        a_min = m_value;
        a_max = percent;
        m_value = percent;
        InterfaceUpdate(m_value);
        a_time = 0.0f;
    }
    void InterfaceUpdate( float percent ) {

        m_texture.enabled = percent != 0.0f;

        int x = (int)(width * percent);
        if (x < min)
        {
            m_texture.border = minborder;
        }
        else
        {
            m_texture.border = border;
        }
        m_texture.width = x;
        lastwidth = m_texture.width;
    }





    float a_time = 0.0f;
    float a_max = 0.0f;
    float a_min = 0.0f;
    private void Update()
    {
        if (isAnim)
        {
            float a_v = Mathf.Lerp(a_min, a_max, a_time);
            a_time += Speed * Time.deltaTime;
            InterfaceUpdate(a_v);
        }
    }


}
