using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactive.Player
{
    public class pSfx : MonoBehaviour
    {
        public AudioClip audio;
        public AudioSource audioSource;
        public float pitchRandom;
        public float time;
        public float timeMax;

        bool ready = false;
        PlayerClient client;
        public void Init(PlayerClient client)
        {
            this.client = client;
            ready = true;
            enabled = true;
            audioSource.enabled = true;
        }
        public void Stop( )
        {
            ready = false;
        }


        private void Update()
        {
            if (!ready || client == null)
                return;

            if (client.pMovement.currentCharacterState != pMovement.CharacterState.Moving)
                return;

            if (time < timeMax)
                time += Time.deltaTime;
            else 
            {
                time = 0.0f;
                audioSource.PlayOneShot(audio);
                audioSource.pitch = 1 + Random.RandomRange(-pitchRandom, pitchRandom);
            }
        }


    }
}
