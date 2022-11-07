using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimHandle : MonoBehaviour
{

    public Animation Anim;






    [SerializeField] List<SimpleCallBack> SimpleCallBacks;
    [System.Serializable]
    public class SimpleCallBack
    {
        public enum SpwanType { 
            create, poolUIEff
        }
        public string callbackname;
        public GameObject spwan;
        public SpwanType spwanType;
        public AnimationClip anim;
        public AudioClip audio;
        public Transform enable;
        public Transform disable;
        public EventDelegate eventDele;
    }
    public void OnSimpleCallback(string callbackname)
    {
        var callback = SimpleCallBacks.Find(x => x.callbackname == callbackname);
        if (callback != null)
        {
            if (callback.spwan != null)
            {
                Debug.Log(callback.spwanType);
                if (callback.spwanType == SimpleCallBack.SpwanType.create)
                    callback.spwan.Create(transform);
                if (callback.spwanType == SimpleCallBack.SpwanType.poolUIEff) 
                {
                   callback.spwan.Pool(InterfaceRoot.instance.transform, 4.5f);
                }
               
            }
            if (callback.enable != null) callback.enable.gameObject.SetActive(true);
            if (callback.disable != null) callback.disable.gameObject.SetActive(false);
            if (callback.anim != null) Anim.Play(callback.anim.name);
            if (callback.audio != null) callback.audio.Play();
            if (callback.eventDele != null) callback.eventDele.Execute();
        }
    }






    [HideInInspector] public TaskService.Function TriggerEvent = new TaskService.Function();
    public void OnTriggerEvent(string eventName) 
    {
        if(Service.GameObj.isObjectNotNull(gameObject))
            TriggerEvent.call(eventName);
    }








    public LoopHandle loopHandle;
    [System.Serializable]
    public class LoopHandle
    {
        [Tooltip("loopclip : ถ้าไม่ใส่จะใช้ตัว default / ถ้าใส่ไว้จะใช้ clip อันนี้")]
        public AnimationClip loopclip;
        public bool isLoop;
    }

    float savepoint;
    float savespeed;
    string playingName
    {
        get
        {
            if (loopHandle.loopclip != null)
                return loopHandle.loopclip.name;
            else
                return Anim.clip.name;
        }
    }
    public void LoopBegin()
    {
        loopHandle.isLoop = true;
        savepoint = Anim[playingName].time;
    }
    public void LoopEnd()
    {
        if (loopHandle.isLoop)
            Anim[playingName].time = savepoint;
    }
    public void OnUnLoop()
    {
        loopHandle.isLoop = false;
    }
    public void OnPause()
    {
        savespeed = Anim[playingName].speed;
        Anim[playingName].speed = 0.0f;
    }
    public void OnContinue()
    {
        loopHandle.isLoop = false;
        Anim[playingName].speed = savespeed;
    }





}
