using System;
using UnityEngine;

namespace ServiceInternal.Job
{
    public class Job
    {
        public string stackname;
        public Action jobAction;
        public JobStatus jobStatus;

        public void Run()
        {
            if (jobStatus != JobStatus.Idle)
                return;

           // Debug.Log("Job starts");
            jobAction?.Invoke();
            jobStatus = JobStatus.Running;
        }
    }
}