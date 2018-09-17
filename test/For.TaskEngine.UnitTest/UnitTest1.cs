using For.TaskEngine.Interfaces;
using For.TaskEngine.Models;
using For.TaskEngine.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace For.TaskEngine.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestStandard()
        {
            var tasks = new List<baseTask<IJob>>();
            var job = new StandardTestJob();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(new IntervalTask<IJob>(job, 1000, new TaskOption
                {
                    RetryInterval = 1000,
                    RetryTimes = 5,
                    AfterExceptionCanceled = (ex) => throw ex,
                }));
            }

            foreach (var task in tasks)
            {
                task.Start();
            }


            while (true)
            {
                var count = 0;
                foreach (var task in tasks)
                {
                    if (task.Status == System.Threading.Tasks.TaskStatus.Running)
                    {
                        count++;
                    }
                }
                if (count == 20)
                {
                    break;
                }
            }
            Assert.AreEqual(StandardTestJob.lst.Count, 100);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestArgumentOutOfRangeException()
        {
            var tasks = new List<baseTask<IJob>>();
            var job = new ArgumentOutOfRangeExceptionTestJob();
            var flag = true;
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(new IntervalTask<IJob>(job, 0, new TaskOption
                {
                    RetryInterval = 0,
                    RetryTimes = 5,
                    AfterExceptionCanceled = (ex) =>
                    {
                        if (ex is ArgumentOutOfRangeException)
                        {
                            flag = false;
                        }
                    },
                }));
            }

            foreach (var task in tasks)
            {
                task.Start();
            }

            var count = 0;
            while (flag)
            {
                count++;
                System.Threading.Thread.Sleep(1000);
                if (count > 100)
                {
                    throw new System.Exception("Should have exception in task");
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestException()
        {
            var tasks = new List<baseTask<IJob>>();
            var job = new ArgumentOutOfRangeExceptionTestJob();
            var flag = true;
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(new IntervalTask<IJob>(job, new TaskOption
                {
                    Interval = 0,
                    RetryInterval = 0,
                    RetryTimes = 5,
                    AfterExceptionCanceled = (ex) =>
                    {
                            flag = false;
                    },
                }));
            }

            foreach (var task in tasks)
            {
                task.Start();
            }

            var count = 0;
            while (flag)
            {
                count++;
                System.Threading.Thread.Sleep(1000);
                if (count > 100)
                {
                    break;
                }
                if (!flag)
                {
                    throw new Exception();
                }
            }
        }
    }
}
