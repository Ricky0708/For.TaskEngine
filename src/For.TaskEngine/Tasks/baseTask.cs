using For.TaskEngine.Interfaces;
using For.TaskEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace For.TaskEngine.Tasks
{
    public abstract class baseTask<T> where T : IJob
    {
        private readonly TaskOption _baseOption;
        protected Task _task;
        protected CancellationTokenSource tokenSource;
        protected CancellationToken token;
        protected readonly T _job;
        protected int _currentRetry = 0;

        public TaskStatus Status => _task.Status;
        public long TaskID => _task.Id;
        public baseTask(T job, TaskOption baseOption)
        {
            _baseOption = baseOption;
            _job = job;
            _task = CreateTask();
        }

        #region public method   

        /// <summary>
        /// <see cref="ITask.Start(int, long)"/>
        /// </summary>
        /// <param name="retryTimes"></param>
        /// <param name="interval"></param>
        public void Start()
        {
            if (_task.Status == TaskStatus.RanToCompletion || _task.Status == TaskStatus.Created || _task.Status == TaskStatus.Faulted)
            {
                _currentRetry = 0;
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                token.Register(() =>
                {
                    CancelCalled();
                });
                _task = CreateTask();
                if (BeforeStart()) _task.Start();
                AfterStarted();
            }
        }

        /// <summary>
        /// <see cref="ITask.Stop"/>
        /// </summary>
        public void Stop()
        {
            if (_task.Status == TaskStatus.Running && BeforeCallCancel()) tokenSource.Cancel();
        }

        #endregion 

        #region abstract

        /// <summary>
        /// use job to create a task
        /// </summary>
        protected abstract int NextInterval();
        protected abstract int RetryInterval();
        protected abstract Tuple<bool, int> CheckRetry();

        #endregion

        #region overridable check and log

        protected void JobResult(object obj) => _baseOption.JobResult(obj);
        protected bool BeforeStart() => _baseOption.BeforeStart();
        protected void AfterStarted() => _baseOption.AfterStarted();
        protected bool BeforeCallCancel() => _baseOption.BeforeCallCancel();
        protected void CancelCalled() => _baseOption.CancelCalled();
        protected void AfterCanceled() => _baseOption.AfterCanceled();
        protected void AfterExceptionCanceled(System.Exception ex) => _baseOption.AfterExceptionCanceled(ex);
        protected void OnRetry(int times) => _baseOption.OnRetry(times);

        #endregion

        #region private method

        private Task CreateTask()
        {
            return new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();
                        JobResult(_job.DoJob());
                        SpinWait.SpinUntil(() => false, NextInterval());
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (System.Exception ex)
                    {
                        var isRetry = CheckRetry();
                        if (!isRetry.Item1)
                        {
                            AfterExceptionCanceled(ex);
                            throw;
                        }
                        OnRetry(isRetry.Item2);
                        SpinWait.SpinUntil(() => false, RetryInterval());
                    }
                }
                AfterCanceled();
            }, token);
        }

        #endregion
    }
}
