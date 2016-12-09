// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestExport.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the test export class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Core.Composition
{
    using System;

    using Kephas.Composition;

    /// <summary>
    /// A test export.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class TestExport<TService> : IExport<TService>
    {
        /// <summary>
        /// The lazy value.
        /// </summary>
        private readonly Lazy<TService> lazyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExport{TService}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public TestExport(Func<Tuple<TService, Action>> factory)
        {
            this.lazyValue = new Lazy<TService>(() => factory().Item1);
        }

        /// <summary>
        /// The value.
        /// </summary>
        public TService Value => this.lazyValue.Value;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// A test export with metadata.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
    public class TestExport<TService, TMetadata> : IExport<TService, TMetadata>
    {
        /// <summary>
        /// The lazy value.
        /// </summary>
        private readonly Lazy<TService> lazyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestExport{TService,TMetadata}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="metadata">The metadata.</param>
        public TestExport(Func<Tuple<TService, Action>> factory, TMetadata metadata)
        {
            this.Metadata = metadata;
            this.lazyValue = new Lazy<TService>(() => factory().Item1);
        }

        /// <summary>
        /// The value.
        /// </summary>
        public TService Value => this.lazyValue.Value;

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public TMetadata Metadata { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}