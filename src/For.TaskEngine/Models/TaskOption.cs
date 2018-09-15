using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace For.TaskEngine.Models
{
    public class TaskOption
    {
        /// <summary>
        /// ms
        /// </summary>
        public int Interval { get; set; } = 0;
        /// <summary>
        /// times
        /// </summary>
        public int RetryTimes { get; set; } = 0;
        public int RetryInterval { get; set; } = 0;
        /// <summary>
        /// task start message
        public Action<object> JobResult { get; set; } = (obj) => { };
        /// </summary>
        public Func<bool> BeforeStart { get; set; } = () => true;
        public Action AfterStarted { get; set; } = () => { };
        public Func<bool> BeforeCallCancel { get; set; } = () => true;
        public Action CancelCalled { get; set; } = () => { };
        public Action AfterCanceled { get; set; } = () => { };
        public Action<System.Exception> AfterExceptionCanceled { get; set; } = (ex) => { };
        public Action<int> OnRetry { get; set; } = (times) => { };
    }
}
