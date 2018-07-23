//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Canti.Utilities
{
    /// <summary>
    /// Utility class for QueuedWorker instances
    /// </summary>
    public struct Work
    {
        public Delegate Delegate { get; set; }
        public object Object { get; set; }
        public object[] Params { get; set; }
        public Work(Delegate Delegate, object Object, params object[] Params)
        {
            this.Delegate = Delegate;
            this.Object = Object;
            this.Params = Params;
        }
    }

    /// <summary>
    /// Runs a series of delegate tasks/jobs in order of appearance
    /// </summary>
    class QueuedWorker
    {
        /// <summary>
        /// Error ahandler that is called should an exception occur
        /// </summary>
        public EventHandler OnError { get; set; }

        // Private variables
        private Queue<Work> WorkQueue { get; set; }
        private bool Paused { get; set; }
        private bool Stopped { get; set; }
        private Task previousTask = Task.FromResult(true);
        private object key = new object();
        private Thread Worker;

        /// <summary>
        /// Adds a job to the worker queue
        /// </summary>
        public Task Add(Action action)
        {
            lock (key)
            {
                previousTask = previousTask.ContinueWith(t => action()
                    , CancellationToken.None
                    , TaskContinuationOptions.None
                    , TaskScheduler.Default);
                return previousTask;
            }
        }

        /// <summary>
        /// Adds a job to the worker queue
        /// </summary>
        public Task<T> Add<T>(Func<T> work)
        {
            lock (key)
            {
                var task = previousTask.ContinueWith(t => work()
                    , CancellationToken.None
                    , TaskContinuationOptions.None
                    , TaskScheduler.Default);
                previousTask = task;
                return task;
            }
        }

        /// <summary>
        /// Adds a job to the worker queue
        /// </summary>
        public void Add(Delegate Delegate)
        {
            WorkQueue.Enqueue(new Work(Delegate, null, null));
        }
        /// <summary>
        /// Adds a job to the worker queue
        /// </summary>
        public void Add(Delegate Delegate, object Sender)
        {
            WorkQueue.Enqueue(new Work(Delegate, Sender, null));
        }
        /// <summary>
        /// Adds a job to the worker queue
        /// </summary>
        public void Add(Delegate Delegate, object Sender, params object[] Params)
        {
            WorkQueue.Enqueue(new Work(Delegate, Sender, Params));
        }

        /// <summary>
        /// Clears all queued jobs
        /// </summary>
        public void Clear()
        {
            WorkQueue.Clear();
        }

        /// <summary>
        /// Starts the work queue
        /// </summary>
        public void Start()
        {
            Stopped = false;
            Paused = false;
            Worker = new Thread(delegate ()
            {
                while (!Stopped)
                {
                    try
                    {
                        Work Job = WorkQueue.Dequeue();
                        Job.Delegate?.Method?.Invoke(Job.Object, Job.Params);
                    }
                    catch (Exception e)
                    {
                        OnError?.Invoke(e, EventArgs.Empty);
                    }
                    while (Paused) { }
                }
            });
            Worker.Start();
        }

        /// <summary>
        /// Pauses a worker after the current job is completed
        /// </summary>
        public void Pause()
        {
            Paused = true;
        }

        /// <summary>
        /// Resumes a paused worker
        /// </summary>
        public void Resume()
        {
            Paused = false;
        }

        /// <summary>
        /// Stops all work after the current job is completed, then clears the job queue
        /// </summary>
        public void Stop()
        {
            Stopped = true;
            WorkQueue.Clear();
        }

        /// <summary>
        /// Aborts all work, including the current job, then clears the job queue
        /// </summary>
        public void Abort()
        {
            Worker.Abort();
            WorkQueue.Clear();
            Stopped = true;
        }
    }
}
