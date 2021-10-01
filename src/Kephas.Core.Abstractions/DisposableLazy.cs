// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableLazy.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Threading;

    /// <summary>
    /// A disposable lazy value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class DisposableLazy<T> : Lazy<T>, IDisposable
    {
        private readonly Action? dispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazy{T}"/> class.
        /// </summary>
        /// <param name="factory">The factory providing the value and a dispose callback.</param>
        /// <param name="dispose">Optional. The dispose callback. If not provided, the produced value will be disposed.</param>
        public DisposableLazy(Func<T> factory, Action? dispose = null)
            : base(factory)
        {
            this.dispose = dispose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazy{T}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="isThreadSafe">True to make this instance thread safe.</param>
        /// <param name="dispose">Optional. The dispose callback. If not provided, the produced value will be disposed.</param>
        public DisposableLazy(Func<T> factory, bool isThreadSafe, Action? dispose = null)
            : base(factory, isThreadSafe)
        {
            this.dispose = dispose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazy{T}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="threadSafetyMode">Indicates the thread safety mode.</param>
        /// <param name="dispose">Optional. The dispose callback. If not provided, the produced value will be disposed.</param>
        public DisposableLazy(Func<T> factory, LazyThreadSafetyMode threadSafetyMode, Action? dispose = null)
            : base(factory, threadSafetyMode)
        {
            this.dispose = dispose;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <footer>
        /// <a href="https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable.Dispose?view=netcore-5.0">`IDisposable.Dispose` on docs.microsoft.com</a>
        /// </footer>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dispose != null)
                {
                    this.dispose();
                }
                else
                {
                    (this.Value as IDisposable)?.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// A disposable lazy value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    public class DisposableLazy<T, TMetadata> : Lazy<T, TMetadata>, IDisposable
    {
        private readonly Action? dispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazy{T,TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="dispose">Optional. The dispose callback. If not provided, the produced value will be disposed.</param>
        public DisposableLazy(Func<T> factory, TMetadata metadata, Action? dispose = null)
            : base(factory, metadata)
        {
            this.dispose = dispose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazy{T,TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="isThreadSafe">True to make this instance thread safe.</param>
        /// <param name="dispose">Optional. The dispose callback. If not provided, the produced value will be disposed.</param>
        public DisposableLazy(Func<T> factory, TMetadata metadata, bool isThreadSafe, Action? dispose = null)
            : base(factory, metadata, isThreadSafe)
        {
            this.dispose = dispose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazy{T,TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="threadSafetyMode">Indicates the thread safety mode.</param>
        /// <param name="dispose">Optional. The dispose callback. If not provided, the produced value will be disposed.</param>
        public DisposableLazy(Func<T> factory, TMetadata metadata, LazyThreadSafetyMode threadSafetyMode, Action? dispose = null)
            : base(factory, metadata, threadSafetyMode)
        {
            this.dispose = dispose;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <footer>
        /// <a href="https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable.Dispose?view=netcore-5.0">`IDisposable.Dispose` on docs.microsoft.com</a>
        /// </footer>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dispose != null)
                {
                    this.dispose();
                }
                else
                {
                    (this.Value as IDisposable)?.Dispose();
                }
            }
        }
    }
}