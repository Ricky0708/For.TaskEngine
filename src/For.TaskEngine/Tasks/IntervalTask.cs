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

        public IntervalTask(T job, TaskOption option) : base(job, option)
        {
            _option = option;
        }

        protected override Tuple<bool, int> CheckRetry()
        {
            return Tuple.Create(_currentRetry++ < _option.RetryTimes, _currentRetry);
        }
        protected override int NextInterval()
        {
            return _option.Interval;
        }
        protected override int RetryInterval()
        {
            return _option.RetryInterval;
        }


    }
}
