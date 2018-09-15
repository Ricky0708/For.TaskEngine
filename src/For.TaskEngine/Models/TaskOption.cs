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
        /// millisecond
        /// </summary>
        public int Interval { get; set; } = 0;
        /// <summary>
        /// limit retry times
        /// </summary>
        public int RetryTimes { get; set; } = 0;
        /// <summary>
        /// millisecond
        /// </summary>
        public int RetryInterval { get; set; } = 0;
        /// <summary>
        /// job processed result
        /// </summary>
        public Action<object> JobResult { get; set; } = (obj) => { };
        /// <summary>
        /// before start
        /// </summary>
        public Func<bool> BeforeStart { get; set; } = () => true;
        /// <summary>
        /// started
        /// </summary>
        public Action StartProcess { get; set; } = () => { };
        /// <summary>
        /// before call cencel
        /// </summary>
        public Func<bool> BeforeCallCancel { get; set; } = () => true;
        /// <summary>
        /// called cancel
        /// </summary>
        public Action CancelCalled { get; set; } = () => { };
        /// <summary>
        /// cancelded
        /// </summary>
        public Action AfterCanceled { get; set; } = () => { };
        /// <summary>
        /// called when out of limit retry times
        /// </summary>
        public Action<System.Exception> AfterExceptionCanceled { get; set; } = (ex) => { };
        /// <summary>
        /// on retry
        /// </summary>
        public Action<int> OnRetry { get; set; } = (times) => { };
    }
}
