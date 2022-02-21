// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplatingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Logging;
using Kephas.Operations;
using Kephas.Templating.AttributedModel;

/// <summary>
/// Templating engine using the cshtml format.
/// </summary>
/// <seealso cref="Kephas.Templating.ITemplatingEngine" />
[TemplateKind("cshtml")]
public class RazorTemplatingEngine : Loggable, ITemplatingEngine
{
    private readonly IMetadataReferenceManager metadataReferenceManager;
    private readonly IRazorProjectFileSystemProvider projectProvider;
    private readonly IRazorProjectEngineFactory projectEngineFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="RazorTemplatingEngine" /> class.
    /// </summary>
    /// <param name="metadataReferenceManager">The metadata reference manager.</param>
    /// <param name="projectProvider">The project provider.</param>
    /// <param name="projectEngineFactory">The project engine factory.</param>
    /// <param name="logManager">The log manager.</param>
    public RazorTemplatingEngine(
        IMetadataReferenceManager metadataReferenceManager,
        IRazorProjectFileSystemProvider projectProvider,
        IRazorProjectEngineFactory projectEngineFactory,
        ILogManager? logManager = null)
        : base(logManager)
    {
        this.metadataReferenceManager = metadataReferenceManager ?? throw new ArgumentNullException(nameof(metadataReferenceManager));
        this.projectProvider = projectProvider ?? throw new ArgumentNullException(nameof(projectProvider));
        this.projectEngineFactory = projectEngineFactory ?? throw new ArgumentNullException(nameof(projectEngineFactory));
    }

    /// <summary>
    /// Processes the provided template asynchronously returning the processed output.
    /// </summary>
    /// <typeparam name="T">The type of the bound model.</typeparam>
    /// <param name="template">The template to be interpreted/executed.</param>
    /// <param name="model">Optional. The template model.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// A promise of the execution result.
    /// </returns>
    public Task<IOperationResult<object?>> ProcessAsync<T>(ITemplate template, T? model, ITemplateProcessingContext processingContext, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
