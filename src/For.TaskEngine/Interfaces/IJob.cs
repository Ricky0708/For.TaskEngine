using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace For.TaskEngine.Interfaces
{
    public interface IJob
    {
        /// <summary>
        /// process a job
        /// </summary>
        /// <returns></returns>
        object DoJob();
    }
}
