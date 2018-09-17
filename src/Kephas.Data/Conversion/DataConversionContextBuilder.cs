// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContextBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data conversion context builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// A data conversion context builder.
    /// </summary>
    public class DataConversionContextBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionContextBuilder"/> class.
        /// </summary>
        /// <param name="conversionContext">Context for the data conversion.</param>
        public DataConversionContextBuilder(IDataConversionContext conversionContext)
        {
            Requires.NotNull(conversionContext, nameof(conversionContext));

            this.ConversionContext = conversionContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConversionContextBuilder"/> class.
        /// </summary>
        /// <param name="dataSpace">The data space.</param>
        /// <param name="conversionService">The data conversion service.</param>
        public DataConversionContextBuilder(IDataSpace dataSpace, IDataConversionService conversionService)
        {
            Requires.NotNull(dataSpace, nameof(dataSpace));
            Requires.NotNull(conversionService, nameof(conversionService));

            this.ConversionContext = new DataConversionContext(conversionService, dataSpace);
        }

        /// <summary>
        /// Gets a context for the conversion.
        /// </summary>
        /// <value>
        /// The conversion context.
        /// </value>
        public IDataConversionContext ConversionContext { get; }

        /// <summary>
        /// Merges the provided data conversion context into the existing one, overwriting the values.
        /// </summary>
        /// <param name="dataConversionContext">The source context for the data conversion.</param>
        /// <returns>
        /// This <see cref="DataConversionContextBuilder"/>.
        /// </returns>
        public DataConversionContextBuilder Merge(IDataConversionContext dataConversionContext)
        {
            if (dataConversionContext == null)
            {
                return this;
            }

            this.ConversionContext.Merge(dataConversionContext);

            return this;
        }

        /// <summary>
        /// Uses the provided type for the target root object if the conversion needs to create or identify it.
        /// </summary>
        /// <param name="rootTargetType">The type of the target root object.</param>
        /// <returns>
        /// This <see cref="DataConversionContextBuilder"/>.
        /// </returns>
        public DataConversionContextBuilder WithRootTargetType(Type rootTargetType)
        {
            Requires.NotNull(rootTargetType, nameof(rootTargetType));

            this.ConversionContext.RootTargetType = rootTargetType;

            return this;
        }

        /// <summary>
        /// Uses the provided type for the source root object if the conversion needs to identify it.
        /// </summary>
        /// <param name="rootSourceType">The type of the source root object.</param>
        /// <returns>
        /// This <see cref="DataConversionContextBuilder"/>.
        /// </returns>
        public DataConversionContextBuilder WithRootSourceType(Type rootSourceType)
        {
            Requires.NotNull(rootSourceType, nameof(rootSourceType));

            this.ConversionContext.RootSourceType = rootSourceType;

            return this;
        }

        /// <summary>
        /// Sets a value indicating whether the conversion should throw an exception on error, or swallow them.
        /// </summary>
        /// <param name="throwOnError">True to throw on error, false to swallow the errors (optional). Default is <c>true</c>.</param>
        /// <returns>
        /// This <see cref="DataConversionContextBuilder"/>.
        /// </returns>
        public DataConversionContextBuilder ThrowOnError(bool throwOnError = true)
        {
            this.ConversionContext.ThrowOnError = throwOnError;

            return this;
        }
    }
}