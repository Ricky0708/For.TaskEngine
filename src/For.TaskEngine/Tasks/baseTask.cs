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
        private bool _isCanceled = false;

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
        /// start task
        /// </summary>
        public void Start()
        {
            if (_task.Status == TaskStatus.RanToCompletion || _task.Status == TaskStatus.Created || _task.Status == TaskStatus.Faulted)
            {
                _isCanceled = false;
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
        /// stop task
        /// </summary>
        public void Stop()
        {
            if (_task.Status == TaskStatus.Running && BeforeCallCancel()) tokenSource.Cancel();
            _isCanceled = true;
        }

        #endregion 

        #region abstract

        /// <summary>
        /// next interval
        /// </summary>
        protected abstract int NextInterval();

        /// <summary>
        /// retry interval
        /// </summary>
        /// <returns></returns>
        protected abstract int RetryInterval();

        /// <summary>
        /// check should keep retry and get current retry times
        /// </summary>
        /// <returns></returns>
        protected abstract Tuple<bool, int> CheckRetry();

        #endregion

        #region overridable check and log

        /// <summary>
        /// job processed result
        /// </summary>
        /// <param name="obj"></param>
        protected void JobResult(object obj) => _baseOption.JobResult(obj);
        /// <summary>
        /// before start
        /// </summary>
        /// <returns></returns>
        protected bool BeforeStart() => _baseOption.BeforeStart();
        /// <summary>
        /// started
        /// </summary>
        protected void AfterStarted() => _baseOption.AfterStarted();
        /// <summary>
        /// before call cancel
        /// </summary>
        /// <returns></returns>
        protected bool BeforeCallCancel() => _baseOption.BeforeCallCancel();
        /// <summary>
        /// call cancele
        /// </summary>
        protected void CancelCalled() => _baseOption.CancelCalled();
        /// <summary>
        /// canceled
        /// </summary>
        protected void AfterCanceled() => _baseOption.AfterCanceled();
        /// <summary>
        /// called when out of limit retry times
        /// </summary>
        /// <param name="ex"></param>
        protected void AfterExceptionCanceled(System.Exception ex) => _baseOption.AfterExceptionCanceled(ex);
        /// <summary>
        /// on retry
        /// </summary>
        /// <param name="times"></param>
        protected void OnRetry(int times) => _baseOption.OnRetry(times);

        #endregion

        #region private method

        private Task CreateTask()
        {
            return new Task(() =>
            {
                while (!_isCanceled)
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
