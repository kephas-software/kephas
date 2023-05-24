// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.Services;

namespace Kephas.Pipelines;

/// <summary>
/// Metadata class for <see cref="IAsyncPipelineBehavior"/>.
/// </summary>
public class PipelineBehaviorMetadata : AppServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineBehaviorMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public PipelineBehaviorMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        this.TargetType = (Type?)metadata.TryGetValue(nameof(this.TargetType));
        this.OperationArgsType = (Type?)metadata.TryGetValue(nameof(this.OperationArgsType));
        this.ResultType = (Type?)metadata.TryGetValue(nameof(this.ResultType));
    }

    /// <summary>
    /// Gets or sets the target type.
    /// </summary>
    public Type? TargetType { get; set; }

    /// <summary>
    /// Gets or sets the operation arguments type.
    /// </summary>
    public Type? OperationArgsType { get; set; }

    /// <summary>
    /// Gets or sets the result type.
    /// </summary>
    public Type? ResultType { get; set; }
}