using System;
using System.Collections.Generic;

namespace ServiceInternal.Job
{
    /// <summary>
    /// A class to manage and subscribe for multiple action callbacks. Call method 'Run' if not start immediately.
    /// </summary>
    public abstract partial class JobManager
    {

        public bool StartImmediately;

        public Action onFinish;

        protected int jobCount;
        protected int jobCompleteCount;
        protected List<Job> jobs = new List<Job>();


        public static List<JobManager> JobManagers = new List<JobManager>();



        public bool IsRunning { get; protected set; }

        public JobManager() { JobManagers.Add(this); }

        public JobManager(bool startImmediately)
        {
            StartImmediately = startImmediately;
            JobManagers.Add(this);
        }

        public void Run(Action onFinish)
        {
            if (jobCount == 0)
            {
                onFinish?.Invoke();
                return;
            }
            this.onFinish = onFinish;
            Run();
        }

        public virtual void Run()
        {
            IsRunning = true;
        }

        public void Clear()
        {
            jobs.Clear();
            jobCount = 0;
            jobCompleteCount = 0;
            onFinish = null;
            IsRunning = false;
        }

        public void ForceFinish()
        {
            onFinish?.Invoke();
            Clear();
        }

        public virtual Job AddJob(string jobName, Action<Action> action, Action callback = null)
        {
            Job job = new Job
            {
                stackname = jobName
            };
            job.jobAction = () => action(() =>
            {
                job.jobStatus = JobStatus.Done;
                callback?.Invoke();
                OnActionComplete();
            });
            jobs.Add(job);
            jobCount++;

            return job;
        }

        public virtual Job AddJob<T>(string jobName, Action<Action<T>> action, Action<T> callback = null)
        {
            Job job = new Job
            {
                stackname = jobName
            };
            job.jobAction = () => action(t =>
            {
                callback?.Invoke(t);
                OnActionComplete();
            });
            jobs.Add(job);
            jobCount++;

            return job;
        }

        protected abstract void OnActionComplete();
    }
}