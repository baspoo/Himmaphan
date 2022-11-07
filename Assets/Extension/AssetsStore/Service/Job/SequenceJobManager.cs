using System;
using UnityEngine;

namespace ServiceInternal.Job
{
    /// <summary>
    /// A class to manage and subscribe for multiple action callbacks. Run jobs (action) in sequence, one at a time.
    /// </summary>
    public class SequenceJobManager : JobManager
    {
        public SequenceJobManager() { }
        public SequenceJobManager(bool startImmediately) : base(startImmediately) { }

        public override Job AddJob(string jobName, Action<Action> action, Action callback = null)
        {
            var job = base.AddJob(jobName, action, callback);
            if (StartImmediately && !IsRunning)
            {
                job.Run();
                IsRunning = true;
            }

            return job;
        }

        public override Job AddJob<T>(string jobName, Action<Action<T>> action, Action<T> callback = null)
        {
            var job = base.AddJob(jobName, action, callback);
            if (StartImmediately && !IsRunning)
            {
                job.Run();
                IsRunning = true;
            }

            return job;
        }

        public override void Run()
        {
            if (jobCompleteCount >= jobs.Count)
                return;


            base.Run();
            var job = jobs[jobCompleteCount];

            Debug.Log("Job : " + job.stackname);
            job.Run();
        }

        protected override void OnActionComplete()
        {
            jobCompleteCount++;
            //Debug.Log($"jobLeft = {jobCount - jobCompleteCount}");
            if (jobCompleteCount == jobCount)
            {
                Debug.Log("Job : Done");
                IsRunning = false;
                onFinish?.Invoke();
                Clear();
            }
            else
            {
                Run();
            }
        }
    }
}