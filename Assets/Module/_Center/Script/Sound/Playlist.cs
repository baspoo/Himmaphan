using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Playlist : MonoBehaviour
{


    static Playlist m_instance;
    public static Playlist instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = ((GameObject)Resources.Load("Sound/Playlist")).GetComponent<Playlist>();
            }
            return m_instance;
        }
    }





}


































[System.Serializable]
public class SoundData
{
    public string path;
    public AudioClip clip;

    public AudioClip getclip
    {
        get
        {
            if (!Application.isPlaying)
                return reload(path);
            if (clip == null)
            {
                if (!string.IsNullOrEmpty(path))
                    clip = reload(path);
            }
            return clip;
        }
    }
    AudioClip reload(string path)
    {
        return Resources.Load($"Sound/" + path) as AudioClip;
    }
    public void Play(bool unmuteBackground = false )
    {
        Sound.Play( getclip , unmuteBackground );
    }
}



#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SoundData))]
public class IngredientDrawer : PropertyDrawer
{
    int nextwidth = 0;
    void Reposition() { nextwidth = 0; }
    Rect Position(Rect position, int width)
    {
        Rect R = new Rect(position.x + nextwidth, position.y, width, position.height);
        nextwidth += width + 5;
        return R;
    }
    public string pathKeep;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        Reposition();
        //EditorGUI.PropertyField(Position(position, 35), property.FindPropertyRelative("clip"), GUIContent.none);
        EditorGUI.PropertyField(Position(position, 180), property.FindPropertyRelative("path"), GUIContent.none);
        var obj = Resources.Load($"Sound/" + property.FindPropertyRelative("path").stringValue) as AudioClip;

        if (obj != null)
        {
            if (GUI.Button(Position(position, 25), EditorGUIUtility.FindTexture("Animation.Play")))
            {
                Sound.sound.Sfx.PlayOneShot(obj);
            }
            if (GUI.Button(Position(position, 25), EditorGUIUtility.FindTexture("animationvisibilitytoggleon@2x")))
            {
                EditorGUIUtility.PingObject(obj);
            }
        }


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
#endif
