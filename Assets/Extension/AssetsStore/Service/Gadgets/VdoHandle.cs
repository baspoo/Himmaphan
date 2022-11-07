using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VdoHandle : MonoBehaviour
{
    public string vdoName;
    public VideoPlayer vidplayer;
    // Start is called before the first frame update
    void Start()
    {

    }
    bool isplay = false;
    void Play()
    {
        isplay = true;
        vidplayer.url = $"{Application.streamingAssetsPath}/Vdo/{vdoName}";
        vidplayer.Play();
        vidplayer.isLooping = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            if(!isplay) Play();
        }
    }
}
