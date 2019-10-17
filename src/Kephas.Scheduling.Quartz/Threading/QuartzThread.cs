// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuartzThread.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quartz thread class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.Threading
{
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Support class used to handle threads
    /// </summary>
    public abstract class QuartzThread : IThreadRunnable
    {
        /// <summary>
        /// The instance of System.Threading.Thread.
        /// </summary>
        private readonly Thread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzThread"/> class.
        /// </summary>
        protected QuartzThread()
        {
            thread = new Thread(Run);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzThread"/> class.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        protected QuartzThread(string name)
        {
            thread = new Thread(Run);
            Name = name;
        }

        /// <summary>
        /// This method has no functionality unless the method is overridden
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        /// Causes the operating system to change the state of the current thread instance to ThreadState.Running
        /// </summary>
        public void Start()
        {
            thread.Start();
        }

        /// <summary>
        /// Interrupts a thread that is in the WaitSleepJoin thread state
        /// </summary>
        protected void Interrupt()
        {
            thread.Interrupt();
        }

        /// <summary>
        /// Gets or sets the name of the thread
        /// </summary>
        public string Name
        {
            get { return thread.Name; }
            protected set
            {
                thread.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a thread
        /// </summary>
        protected ThreadPriority Priority
        {
            get { return thread.Priority; }
            set { thread.Priority = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        protected bool IsBackground
        {
            set { thread.IsBackground = value; }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates
        /// </summary>
        public void Join()
        {
            thread.Join();
        }

        /// <summary>
        /// Obtain a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Thread[{0},{1},]", Name, Priority);
        }
    }
}