using For.TaskEngine.Interfaces;
using For.TaskEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace For.TaskEngine.Tasks
{
    public class IntervalTask : baseTask
    {
        private readonly int _interval;
        private readonly TaskOption _option;

        /// <summary>
        /// initial task
        /// </summary>
        /// <param name="job"></param>
        /// <param name="interval">millisecond</param>
        /// <param name="option"></param>
        public IntervalTask(IJob job, int interval, TaskOption option) : base(job, option)
        {
            _option = option;
            _interval = interval;
        }

        /// <summary>
        /// <see cref="baseTask.CheckRetry"/>
        /// </summary>
        /// <returns></returns>
        protected override Tuple<bool, int> CheckRetry()
        {
            return Tuple.Create(_currentRetry++ < _option.RetryTimes, _currentRetry);
        }

        /// <summary>
        /// <see cref="baseTask.NextInterval"/>
        /// </summary>
        /// <returns></returns>
        protected override int NextInterval()
        {
            return _interval;
        }

        /// <summary>
        /// <see cref="baseTask.RetryInterval"/>
        /// </summary>
        /// <returns></returns>
        protected override int RetryInterval()
        {
            return _option.RetryInterval;
        }


    }
}
