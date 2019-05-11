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
        public async Task WithTimeout_timeout_exception()
        {
            var task = new Task(() => Thread.Sleep(1000));
            task.Start();
            try
            {
                await task.WithTimeout(TimeSpan.FromMilliseconds(200));
            }
            catch (TaskTimeoutException tex)
            {
                Assert.AreSame(task, tex.Task);
            }
        }

        [Test]
        public async Task WithTimeout_success()
        {
            var task = new Task(() => { });
            task.Start();
            await task.WithTimeout(TimeSpan.FromMilliseconds(200));
        }

        [Test]
        public async Task WithTimeout_result_timeout_exception()
        {
            var task = new Task<int>(() => { Thread.Sleep(1000); return 1000; });
            task.Start();
            try
            {
                await task.WithTimeout(TimeSpan.FromMilliseconds(200));
            }
            catch (TaskTimeoutException tex)
            {
                Assert.AreSame(task, tex.Task);
            }
        }

        [Test]
        public async Task WithTimeout_result_success()
        {
            var task = new Task<int>(() => 1000);
            task.Start();
            var result = await task.WithTimeout(TimeSpan.FromMilliseconds(1000));
            Assert.AreEqual(1000, result);
        }

        [Test]
        public async Task WaitNonLocking_timeout_exception()
        {
            var task = new Task(() => Thread.Sleep(1000));
            task.Start();
            try
            {
                task.WaitNonLocking(TimeSpan.FromMilliseconds(200));
            }
            catch (TaskTimeoutException tex)
            {
                Assert.AreSame(task, tex.Task);
            }
        }

        [Test]
        public void WaitNonLocking_custom_exception()
        {
            var task = new Task(() => throw new InvalidOperationException("must throw!"));
            task.Start();
            Assert.Throws<InvalidOperationException>(() => task.WaitNonLocking(TimeSpan.FromMilliseconds(10000)));
        }

        [Test]
        public void GetResultNonLocking_timeout_exception()
        {
            var task = new Task<int>(() => { Thread.Sleep(1000); return 1000; });
            task.Start();
            Assert.Throws<TaskTimeoutException>(() => task.GetResultNonLocking(TimeSpan.FromMilliseconds(200)));
        }

        [Test]
        public void WaitNonLocking_success()
        {
            var task = new Task(() => Thread.Sleep(50));
            task.Start();
            task.WaitNonLocking(TimeSpan.FromMilliseconds(1000), throwOnTimeout: false);

            if (!task.IsCompleted)
            {
                Assert.Inconclusive("Normally, waiting for 1 sec. for tasks scheduled for 50 ms should not fail, but in a test environment this can be inconclusive.");
            }
        }

        [Test]
        public void GetResultNonLocking_success()
        {
            var task = new Task<int>(() => { Thread.Sleep(50); return 1000; });
            task.Start();
            var result = task.GetResultNonLocking(TimeSpan.FromMilliseconds(1000));

            Assert.AreEqual(1000, result);
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void GetResultNonLocking_success_nullable_type()
        {
            var task = new Task<string>(() => { Thread.Sleep(50); return "test"; });
            task.Start();
            var result = task.GetResultNonLocking(TimeSpan.FromMilliseconds(500));

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

        [Test]
        public void AsAsync_action_timeout_10_of_1000()
        {
            Assert.ThrowsAsync<TaskTimeoutException>(
                () => TaskHelper.AsAsync(() => Thread.Sleep(1000), 10));
        }

        [Test]
        public void AsAsync_action_timeout_100_of_1000()
        {
            Assert.ThrowsAsync<TaskTimeoutException>(
                () => TaskHelper.AsAsync(() => Thread.Sleep(1000), 100));
        }

        [Test]
        public void AsAsync_action_canceled()
        {
            using (var cts = new CancellationTokenSource(10))
            {
                Assert.ThrowsAsync<TaskCanceledException>(
                    () => TaskHelper.AsAsync(() => Thread.Sleep(1000), cancellationToken: cts.Token));
            }
        }

        [Test]
        public void AsAsync_func_timeout_10_of_1000()
        {
            Task task = null;
            Assert.ThrowsAsync<TaskTimeoutException>(
                () => task = TaskHelper.AsAsync(
                    () =>
                    {
                        Thread.Sleep(1000);
                        return 20;
                    },
                    10));
            Assert.IsInstanceOf<Task<int>>(task);
        }

        [Test]
        public void AsAsync_func_timeout_100_of_1000()
        {
            Task task = null;
            Assert.ThrowsAsync<TaskTimeoutException>(
                () => task = TaskHelper.AsAsync(
                          () =>
                              {
                                  Thread.Sleep(1000);
                                  return 20;
                              },
                          100));
            Assert.IsInstanceOf<Task<int>>(task);
        }

        [Test]
        public void AsAsync_func_canceled()
        {
            using (var cts = new CancellationTokenSource(10))
            {
                Task task = null;
                Assert.ThrowsAsync<TaskCanceledException>(
                    () => task = TaskHelper.AsAsync(
                        () =>
                            {
                                Thread.Sleep(1000);
                                return 20;
                            },
                        cancellationToken: cts.Token));
                Assert.IsInstanceOf<Task<int>>(task);
            }
        }
    }
}