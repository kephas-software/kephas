// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionConverterMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expression converter metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// An expression converter metadata.
    /// </summary>
    public class ExpressionConverterMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionConverterMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ExpressionConverterMetadata(IDictionary<string, object?> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Operator = (string)metadata.TryGetValue(nameof(this.Operator));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionConverterMetadata"/> class.
        /// </summary>
        /// <param name="operator">The operator.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public ExpressionConverterMetadata(string @operator, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            Requires.NotNullOrEmpty(@operator, nameof(@operator));

            this.Operator = @operator;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public string Operator { get; }
    }
}