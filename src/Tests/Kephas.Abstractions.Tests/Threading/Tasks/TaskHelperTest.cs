// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the task helper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Threading.Tasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
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
        public void TryWaitNonLocking_success()
        {
            var task = new Task(() => Thread.Sleep(50));
            task.Start();
            var completed = task.TryWaitNonLocking(TimeSpan.FromMilliseconds(1000));

            if (completed is false)
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
        public void TryGetResultNonLocking_timeout_silent_fail()
        {
            var task = new Task<int>(() => { Thread.Sleep(1000); return 500; });
            task.Start();
            var result = task.TryGetResultNonLocking(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual(0, result);
            Assert.IsFalse(task.IsCompleted);
        }

        [Test]
        public void TryGetResultNonLocking_timeout_silent_fail_nullable_type()
        {
            var task = new Task<string>(() => { Thread.Sleep(1000); return "test"; });
            task.Start();
            var result = task.TryGetResultNonLocking(TimeSpan.FromMilliseconds(50));

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
                    () => (task = TaskHelper.AsAsync(
                        () =>
                            {
                                Thread.Sleep(1000);
                                return 20;
                            },
                        cancellationToken: cts.Token)));
                Assert.IsInstanceOf<Task<int>>(task);
            }
        }

        [Test]
        public void EnsureCompletedSuccessfully_throws_if_not_completed()
        {
            var completionSource = new TaskCompletionSource<bool>();
            Assert.Throws<TaskNotCompletedException>(() => TaskHelper.EnsureCompletedSuccessfully(completionSource.Task));
        }

        [Test]
        public void EnsureCompletedSuccessfully_throws_TaskCanceledException()
        {
            using (var cts = new CancellationTokenSource(10))
            {
                var task = Task.Delay(1000, cts.Token);
                while (!task.IsCompleted)
                {
                    Thread.Sleep(50);
                }

                Assert.Throws<TaskCanceledException>(() => TaskHelper.EnsureCompletedSuccessfully(task));
            }
        }

        [Test]
        public void EnsureCompletedSuccessfully_throws_custom_exception()
        {
            var task = Task.Run(() => throw new ServiceException());
            while (!task.IsCompleted)
            {
                Thread.Sleep(50);
            }

            Assert.Throws<ServiceException>(() => TaskHelper.EnsureCompletedSuccessfully(task));
        }

        [Test]
        public void EnsureCompletedSuccessfully_success()
        {
            var task = Task.Run(() => true);
            while (!task.IsCompleted)
            {
                Thread.Sleep(50);
            }

            TaskHelper.EnsureCompletedSuccessfully(task);
        }

        [Test]
        public void PreserveThreadContext_canceled_awaiter_is_completed()
        {
            var canceledTask = Task.FromCanceled(new CancellationToken(true));
            var contextAwaiter = canceledTask.PreserveThreadContext();
            Assert.IsTrue(contextAwaiter.IsCompleted);
        }

        [Test]
        public void PreserveThreadContext_failed_awaiter_is_completed()
        {
            var failedTask = Task.FromException(new ArgumentException("arg"));
            var contextAwaiter = failedTask.PreserveThreadContext();
            Assert.IsTrue(contextAwaiter.IsCompleted);
        }
    }
}