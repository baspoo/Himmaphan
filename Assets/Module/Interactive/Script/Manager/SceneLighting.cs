using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class SceneLighting : MonoBehaviour
{


    public void Init() { 
    
    }




    [Header("Object")]
    public Light light;
    public UnityEngine.Rendering.Volume volume;
    public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData universalAdditionalCameraData;

    [Header("Setup")]
    public LightData Island;
    public LightData World;
    [System.Serializable]
    public class LightData
    {
        [Header("Camera")]
        public float farClipPlane;
        public bool useOcclusionCulling;
        [Header("Lighting")]
        public Vector3 rotate;
        public float intensity;
        public Color color;
        [Header("Rendering")]
        public UnityEngine.Rendering.VolumeProfile profile;
        
        public Material skybox;
        [Header("Fog")]
        public FogMode fogMode;
        public float fogVolume;
        public float[] fogLinear = new float[2] { 0 , 100};
        public Color fogColor;
        [Header("Bgm")]
        public AudioClip bgm;
    }

    Material m_matsky;
    public void DoApplyLighting(LightData lightData) 
    {

        if (m_matsky != null)
        {
            Destroy(m_matsky);
        }
        m_matsky = new Material(lightData.skybox);

        SceneHandle.instance.cameraManager.mainCamera.useOcclusionCulling = lightData.useOcclusionCulling;
        SceneHandle.instance.cameraManager.mainCamera.farClipPlane = lightData.farClipPlane;
        volume.profile = lightData.profile;
        light.transform.eulerAngles = lightData.rotate;
        light.intensity = lightData.intensity;
        light.color = lightData.color;
        RenderSettings.skybox = m_matsky;// lightData.skybox;
        RenderSettings.fogColor = lightData.fogColor;
        RenderSettings.fogMode = lightData.fogMode;
        if (lightData.fogMode == FogMode.Exponential)
        {
            RenderSettings.fogDensity = lightData.fogVolume;
        }
        if (lightData.fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = lightData.fogLinear[0];
            RenderSettings.fogEndDistance = lightData.fogLinear[1];
        }
        Sound.sound.PlayBGM(lightData.bgm);
    }
    private void Update()
    {
        //RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.4f);
        if(m_matsky!=null)
            m_matsky.SetFloat("_Rotation", Time.time * 0.4f);
    }






    public void Blur(bool isBlur)
    {
        //UIConsole.instance?.OnHide(isBlur);
        SceneHandle.instance.cameraManager.dragCamera.CanRotate = !isBlur;
        Beautify.Universal.BeautifySettings.settings.blurIntensity.overrideState = isBlur;
    }






}
