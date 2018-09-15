using For.TaskEngine.Interfaces;
using For.TaskEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace For.TaskEngine.Tasks
{
    public class IntervalTask<T> : baseTask<T> where T : IJob
    {
        private readonly TaskOption _option;

        /// <summary>
        /// initial task
        /// </summary>
        /// <param name="job"></param>
        /// <param name="option"></param>
        public IntervalTask(T job, TaskOption option) : base(job, option)
        {
            _option = option;
        }

        /// <summary>
        /// <see cref="baseTask{T}.CheckRetry"/>
        /// </summary>
        /// <returns></returns>
        protected override Tuple<bool, int> CheckRetry()
        {
            return Tuple.Create(_currentRetry++ < _option.RetryTimes, _currentRetry);
        }
        /// <summary>
        /// <see cref="baseTask{T}.NextInterval"/>
        /// </summary>
        /// <returns></returns>
        protected override int NextInterval()
        {
            return _option.Interval;
        }
        /// <summary>
        /// <see cref="baseTask{T}.RetryInterval"/>
        /// </summary>
        /// <returns></returns>
        protected override int RetryInterval()
        {
            return _option.RetryInterval;
        }


    }
}
