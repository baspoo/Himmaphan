using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Collections.Generic;
using System.Text;

namespace Center
{
    public class UIDebuging : UIBase
    {
        static UIDebuging instance;
        public static UIDebuging Open()
        {
            if (instance) instance.OnClose();
            if (!instance)
            {
                instance = CreatePage<UIDebuging>(Center.Store.instance.page.prefab_debug);
                instance.Init();
            }
            return instance;
        }
        private void OnEnable()
        {
            Init();
        }

        bool init = false;
        public void Init()
        {
            if (init)
                return;

            init = true;
            systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
            mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        }
        public void OnBtnClose()
        {
            init = false;
            systemMemoryRecorder.Dispose();
            gcMemoryRecorder.Dispose();
            mainThreadTimeRecorder.Dispose();
            OnClose();
        }

        public UILabel textDebug;
        ProfilerRecorder systemMemoryRecorder;
        ProfilerRecorder gcMemoryRecorder;
        ProfilerRecorder mainThreadTimeRecorder;

        float run = 0.0f;
        float max = 0.5f;
        void Update()
        {
            if (run < max)
            {
                run += Time.deltaTime;
                return;
            }


            run = 0.0f;
            string text = "";
            text += $"fps:{(int)1.0f / Time.deltaTime}\n";
            text += $"gc memory:{gcMemoryRecorder.LastValue / (1024 * 1024)} MB \n";
            text += $"system memory:{systemMemoryRecorder.LastValue / (1024 * 1024)} MB \n";
            textDebug.text = text;
        }





    }
}