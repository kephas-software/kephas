// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTemplatingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests;

using Kephas.Operations;
using Kephas.Templating.AttributedModel;

[TemplateKind("test")]
public class TestTemplatingEngine : ITemplatingEngine
{
    public Task<IOperationResult<object?>> ProcessAsync<T>(
        ITemplate template,
        T? model,
        ITemplateProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IOperationResult<object?>>(
            new OperationResult<object?>("processed " + template.Name).Complete());
    }
}