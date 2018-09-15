using For.TaskEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace For.TaskEngine.ConsoleTest
{
    public class StandardTestJob : IJob
    {
        public static List<string> lst = new List<string>();
        public object DoJob()
        {
            lock (lst)
            {
                lst.Clear();
                for (int i = 0; i < 100; i++)
                {
                    lst.Add("A'");
                }
                return lst.Count();
            }
        }
    }
    public class ExceptionTestJob : IJob
    {
        public static List<string> lst = new List<string>();
        public object DoJob()
        {
            lst.Clear();
            for (int i = 0; i < 100; i++)
            {
                lst.Add("A'");
            }
            return lst[100];
        }
    }
}
