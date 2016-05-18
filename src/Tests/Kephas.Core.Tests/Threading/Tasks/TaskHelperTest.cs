namespace Kephas.Core.Tests.Threading.Tasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    using NUnit.Framework;

    [TestFixture]
    public class TaskHelperTest
    {
        [Test]
        public void WaitNonLocking_timeout_exception()
        {
            var task = new Task(() => Thread.Sleep(1000));
            task.Start();
            Assert.Throws<TimeoutException>(() => task.WaitNonLocking(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void GetResultNonLocking_timeout_exception()
        {
            var task = new Task<int>(() => { Thread.Sleep(1000); return 1000; });
            task.Start();
            Assert.Throws<TimeoutException>(() => task.GetResultNonLocking(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void WaitNonLocking_success()
        {
            var task = new Task(() => Thread.Sleep(50));
            task.Start();
            task.WaitNonLocking(TimeSpan.FromMilliseconds(600));

            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void GetResultNonLocking_success()
        {
            var task = new Task<int>(() => { Thread.Sleep(50); return 1000; });
            task.Start();
            var result = task.GetResultNonLocking(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(1000, result);
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void GetResultNonLocking_success_nullable_type()
        {
            var task = new Task<string>(() => { Thread.Sleep(50); return "test"; });
            task.Start();
            var result = task.GetResultNonLocking(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("test", result);
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void GetResultNonLocking_timeout_silent_fail()
        {
            var task = new Task<int>(() => { Thread.Sleep(1000); return 500; });
            task.Start();
            var result = task.GetResultNonLocking(TimeSpan.FromMilliseconds(50), throwOnTimeout: false);

            Assert.AreEqual(0, result);
            Assert.IsFalse(task.IsCompleted);
        }

        [Test]
        public void GetResultNonLocking_timeout_silent_fail_nullable_type()
        {
            var task = new Task<string>(() => { Thread.Sleep(1000); return "test"; });
            task.Start();
            var result = task.GetResultNonLocking(TimeSpan.FromMilliseconds(50), throwOnTimeout: false);

            Assert.IsNull(result);
            Assert.IsFalse(task.IsCompleted);
        }
    }
}