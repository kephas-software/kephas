// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the operation metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CalculatorConsole.Operations
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// An operation metadata.
    /// </summary>
    public class OperationMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the CalculatorConsole.Operations.OperationMetadata class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public OperationMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Operation = (string)metadata.TryGetValue(nameof(this.Operation), string.Empty);
            this.OperationName = (string)metadata.TryGetValue(nameof(this.OperationName), string.Empty);
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public string Operation { get; }

        /// <summary>
        /// Gets the name of the operation.
        /// </summary>
        /// <value>
        /// The name of the operation.
        /// </value>
        public string OperationName { get; }
    }
}