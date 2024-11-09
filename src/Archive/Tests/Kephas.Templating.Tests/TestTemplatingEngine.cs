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
    public Task<IOperationResult> ProcessAsync<T>(
        ITemplate template,
        T? model,
        TextWriter textWriter,
        ITemplateProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        textWriter.Write("processed " + template.Name);
        return Task.FromResult<IOperationResult>(new OperationResult().Complete());
    }

    public IOperationResult Process<T>(
        ITemplate template,
        T? model,
        TextWriter textWriter,
        ITemplateProcessingContext processingContext)
    {
        textWriter.Write("processed " + template.Name);
        return new OperationResult().Complete();
    }
}