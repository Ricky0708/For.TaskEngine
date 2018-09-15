using For.TaskEngine.Interfaces;
using For.TaskEngine.Models;
using For.TaskEngine.Tasks;
using System;
using System.Collections.Generic;

namespace For.TaskEngine.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<baseTask<IJob>>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(new IntervalTask<IJob>(new StandardTestJob(), new TaskOption
                {
                    Interval = 1000,
                    RetryInterval = 1000,
                    RetryTimes = 5,
                    BeforeStart = () => { Console.WriteLine("Before start"); return true; },
                    AfterStarted = () => Console.WriteLine("After started"),
                    BeforeCallCancel = () => { Console.WriteLine("Before call cancel"); return true; },
                    CancelCalled = () => Console.WriteLine("Called cancel"),
                    AfterCanceled = () => Console.WriteLine("Canceled"),
                    AfterExceptionCanceled = (ex) => Console.WriteLine(ex.Message),
                    JobResult = (obj) => Console.WriteLine(obj),
                    OnRetry = (times) => Console.WriteLine($"Retry {times}")
                }));
            }
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "start":
                        foreach (var task in tasks)
                        {
                            task.Start();
                        }
                        break;
                    case "stop":
                        foreach (var task in tasks)
                        {
                            task.Stop();
                        }
                        break;
                    case "status":
                        foreach (var task in tasks)
                        {
                            Console.WriteLine(task.TaskID.ToString(), task.Status);
                        }
                        break;
                    case "live":
                        var count = 0;
                        foreach (var task in tasks)
                        {
                            if (task.Status == System.Threading.Tasks.TaskStatus.Running)
                            {
                                count++;
                            }
                        }
                        Console.WriteLine($"-----{count}-------");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
