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
    /// <param name="razorEngineContext">The razor engine context.</param>
    /// <returns>
    /// The Razor project engine.
    /// </returns>
    public RazorProjectEngine CreateProjectEngine(
        RazorProjectFileSystem fileSystem,
        IRazorProjectEngineContext razorEngineContext)
    {
        var projectEngine = RazorProjectEngine.Create(
            RazorConfiguration.Default,
            fileSystem,
            builder =>
            {
                builder
                    .SetNamespace(razorEngineContext.RootNamespace ?? $"Generated_{Guid.NewGuid():N}")
                    .SetBaseType($"Kephas.Templating.Razor.TemplatePage<{razorEngineContext.ModelType?.FullName ?? "object"}>")
                    .ConfigureClass((document, @class) =>
                    {
                        @class.ClassName = Path.GetFileNameWithoutExtension(document.Source.FilePath);
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

                razorEngineContext.Configure?.Invoke(builder);

                builder.AddDefaultImports(@"
@using System
@using System.Threading.Tasks
");
            });
        return projectEngine;
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