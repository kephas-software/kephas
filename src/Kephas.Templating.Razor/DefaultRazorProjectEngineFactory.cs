// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRazorProjectEngineFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;

/// <summary>
/// The default implementation of the <see cref="IRazorProjectEngineFactory"/>.
/// </summary>
/// <seealso cref="IRazorProjectEngineFactory" />
[OverridePriority(Priority.Low)]
public class DefaultRazorProjectEngineFactory : IRazorProjectEngineFactory
{
    /// <summary>
    /// Creates the project engine for the provided file system and with the given context.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <returns>
    /// The Razor project engine.
    /// </returns>
    public RazorProjectEngine CreateProjectEngine(
        RazorProjectFileSystem fileSystem,
        ITemplateProcessingContext processingContext)
    {
        var projectEngine = RazorProjectEngine.Create(
            RazorConfiguration.Default,
            fileSystem,
            builder =>
            {
                builder
                    .SetNamespace(GetTemplateRootNamespace(processingContext))
                    .SetBaseType(GetTemplateBaseTypeName(processingContext))
                    .ConfigureClass((document, @class) =>
                    {
                        @class.ClassName = GetTemplateClassName(processingContext, document);
                        @class.Modifiers.Clear();
                        @class.Modifiers.Add("public");
                    });

                NamespaceDirective.Register(builder);
                FunctionsDirective.Register(builder);
                InheritsDirective.Register(builder);
                SectionDirective.Register(builder);
                // InjectDirective.Register(builder);
                // ModelDirective.Register(builder);

                builder.Features.Add(new SuppressChecksumOptionsFeature());
                builder.Features.Add(new SuppressMetadataAttributesFeature());

                processingContext.ConfigureEngine()?.Invoke(builder);

                builder.AddDefaultImports(@"
@using System
@using System.Threading.Tasks
");
            });
        return projectEngine;
    }

    private static string GetTemplateClassName(ITemplateProcessingContext processingContext, RazorCodeDocument document)
    {
        return Path.GetFileNameWithoutExtension(document.Source.FilePath);
    }

    private static string GetTemplateBaseTypeName(ITemplateProcessingContext processingContext)
    {
        var baseTypeName = processingContext.BaseTypeName() ?? "Kephas.Templating.Razor.TemplatePage<{0}>";
        return string.Format(baseTypeName, processingContext.ModelType()?.FullName ?? "object");
    }

    private static string GetTemplateRootNamespace(ITemplateProcessingContext processingContext)
    {
        return processingContext.RootNamespace() ?? $"Generated_{Guid.NewGuid():N}";
    }

    private class SuppressChecksumOptionsFeature : RazorEngineFeatureBase, IConfigureRazorCodeGenerationOptionsFeature
    {
        public int Order { get; set; }

        public void Configure(RazorCodeGenerationOptionsBuilder options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.SuppressChecksum = true;
        }
    }

    private class SuppressMetadataAttributesFeature : RazorEngineFeatureBase, IConfigureRazorCodeGenerationOptionsFeature
    {
        public int Order { get; set; }

        public void Configure(RazorCodeGenerationOptionsBuilder options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.SuppressMetadataAttributes = true;
        }
    }
}