using System;
using UnityEngine;

namespace ServiceInternal.Job
{
    /// <summary>
    /// A class to manage and subscribe for multiple action callbacks. Run jobs (action) simultaneously.
    /// </summary>
    public class SimultaneousJobManager : JobManager
    {
        public SimultaneousJobManager() { }
        public SimultaneousJobManager(bool startImmediately) : base(startImmediately) { }

        public override Job AddJob(string jobName, Action<Action> action, Action callback = null)
        {
            var job = base.AddJob(jobName, action, callback);
            if (StartImmediately || IsRunning)
            {
                job.Run();
                IsRunning = true;
            }

            return job;
        }

        public override Job AddJob<T>(string jobName, Action<Action<T>> action, Action<T> callback = null)
        {
            var job = base.AddJob(jobName, action, callback);
            if (StartImmediately || IsRunning)
            {
                job.Run();
                IsRunning = true;
            }

            return job;
        }

        public override void Run()
        {
            base.Run();
            foreach (var job in jobs)
            {
                job.Run();
            }
        }

        protected override void OnActionComplete()
        {
            jobCompleteCount++;
            //Debug.Log($"jobLeft = {jobCount - jobCompleteCount}");
            if (jobCompleteCount == jobCount)
            {
                IsRunning = false;
                onFinish?.Invoke();
                Clear();
            }
        }
    }

}