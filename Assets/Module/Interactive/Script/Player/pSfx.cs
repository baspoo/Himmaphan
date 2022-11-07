using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class pSfx : MonoBehaviour
{
    public AudioClip audio;
    public AudioSource audioSource;
    public float pitchRandom;
    public float time;
    public float timeMax;

    bool ready = false;
    [SerializeField] Movement movement;


    private void Update()
    {
        if (movement.currentCharacterState != Movement.CharacterState.Moving)
            return;

        if (time < timeMax)
            time += Time.deltaTime;
        else
        {
            time = 0.0f;
            if (!Sound.is_sfx_mute)
            {
                audioSource.PlayOneShot(audio);
                audioSource.pitch = 1 + Random.RandomRange(-pitchRandom, pitchRandom);
            }
        }
    }


}
