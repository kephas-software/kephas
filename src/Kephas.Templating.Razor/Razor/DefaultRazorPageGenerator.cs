// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRazorPageGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Collections;
using Kephas.ExceptionHandling;
using Kephas.Logging;using Kephas.Operations;
using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// The default implementation of the <see cref="IRazorPageGenerator"/>.
/// </summary>
[OverridePriority(Priority.Low)]
public class DefaultRazorPageGenerator : Loggable, IRazorPageGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRazorPageGenerator"/> class.
    /// </summary>
    /// <param name="logManager">optional. The log manager.</param>
    public DefaultRazorPageGenerator(ILogManager? logManager = null)
        : base(logManager)
    {
    }

    /// <summary>
    /// Generates the razor page.
    /// </summary>
    /// <param name="projectEngine">The project engine.</param>
    /// <param name="projectItem">The project item.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <returns>An operation result yielding the generation result.</returns>
    public IOperationResult<RazorPageGeneratorResult> GenerateRazorPage(
        RazorProjectEngine projectEngine,
        RazorProjectItem projectItem,
        ITemplateProcessingContext processingContext)
    {
        var genResult = new OperationResult<RazorPageGeneratorResult>();
        var codeDocument = projectEngine.Process(projectItem);
        var cSharpDocument = codeDocument.GetCSharpDocument();
        if (cSharpDocument.Diagnostics.Any())
        {
            this.Logger.Error($"One or more parse errors encountered. This will not prevent the generator from continuing: {Environment.NewLine}{{diagnostics}}.", cSharpDocument.Diagnostics);

            var diagnostics = string.Join(Environment.NewLine, cSharpDocument.Diagnostics);
            cSharpDocument.Diagnostics.ForEach(m =>
            {
                if (m.Severity is RazorDiagnosticSeverity.Error)
                {
                    genResult.MergeException(new OperationException(m.GetMessage()) { Severity = SeverityLevel.Error });
                }
                else
                {
                    genResult.MergeMessage(m.GetMessage());
                }
            });
        }

        var generatedCodeFilePath = Path.ChangeExtension(projectItem.PhysicalPath, ".Designer.cs");
        return genResult
            .Value(new RazorPageGeneratorResult(generatedCodeFilePath, cSharpDocument.GeneratedCode))
            .Complete();
    }
}