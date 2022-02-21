// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterpolationTemplatingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Interpolation;

using System.Text.RegularExpressions;
using Kephas.Operations;
using Kephas.Services;
using Kephas.Templating.AttributedModel;
using Kephas.Threading.Tasks;

/// <summary>
/// Templating engine for string interpolation.
/// </summary>
[TemplateKind(Interpolation)]
[ServiceName(Interpolation)]
public class InterpolationTemplatingEngine : ITemplatingEngine
{
    /// <summary>
    /// The interpolation templating kind.
    /// </summary>
    public const string Interpolation = "interpolation";

    /// <summary>
    /// Processes the provided template asynchronously returning the processed output.
    /// </summary>
    /// <typeparam name="T">The type of the bound model.</typeparam>
    /// <param name="template">The template to be interpreted/executed.</param>
    /// <param name="model">Optional. The template model.</param>
    /// <param name="textWriter">The text writer for the output.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// A promise of the execution result.
    /// </returns>
    public async Task<IOperationResult> ProcessAsync<T>(
        ITemplate template,
        T? model,
        TextWriter textWriter,
        ITemplateProcessingContext processingContext,
        CancellationToken cancellationToken = default)
    {
        textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));

        var opResult = new OperationResult<object?>();
        var content = await template.GetContentAsync(cancellationToken).PreserveThreadContext();

        var expandoModel = model?.ToExpando();
        var crtIndex = 0;
        var matches = Regex.Matches(content, "{([^{}]*)");
        foreach (Match match in matches)
        {
            if (match.Index > crtIndex)
            {
                textWriter.Write(content[crtIndex..match.Index]);
            }

            var value = expandoModel?[match.Groups[1].Value];
            if (value is not null)
            {
                textWriter.Write(value);
            }

            crtIndex = match.Index + match.Length + 1;
        }

        if (content.Length > crtIndex)
        {
            textWriter.Write(content[crtIndex..]);
        }

        return opResult.Complete();
    }
}