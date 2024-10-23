// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Pipelines;

/// <summary>
/// Metadata class for <see cref="IAsyncPipeline{TTarget,TOperationArgs,TResult}"/>.
/// </summary>
public class PipelineMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public PipelineMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        TargetType = (Type?)metadata.TryGetValue(nameof(TargetType));
        OperationArgsType = (Type?)metadata.TryGetValue(nameof(OperationArgsType));
        ResultType = (Type?)metadata.TryGetValue(nameof(ResultType));
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
