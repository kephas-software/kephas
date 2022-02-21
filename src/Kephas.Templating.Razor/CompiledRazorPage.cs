// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompiledRazorPage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using System.Reflection;

/// <summary>
/// A compiled Razor page.
/// </summary>
public class CompiledRazorPage : ICompiledRazorPage
{
    private readonly Lazy<Assembly> lazyAssembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledRazorPage"/> class.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <param name="assemblyByteCode">The assembly byte code.</param>
    public CompiledRazorPage(ITemplate template, MemoryStream assemblyByteCode)
    {
        assemblyByteCode = assemblyByteCode ?? throw new ArgumentNullException(nameof(assemblyByteCode));
        this.Template = template ?? throw new ArgumentNullException(nameof(template));

        this.lazyAssembly = new Lazy<Assembly>(() => Assembly.Load(assemblyByteCode.ToArray()));
    }

    /// <summary>
    /// Gets the associated template.
    /// </summary>
    public ITemplate Template { get; }

    /// <summary>
    /// Renders the template asynchronously using the provided model.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="model">The model.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    public Task RenderAsync<T>(T model, TextWriter writer, CancellationToken cancellationToken = default)
    {
        var assembly = this.lazyAssembly.Value;
        var templatePageType = assembly.ExportedTypes.FirstOrDefault(t =>
            typeof(ITemplatePage<T>).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
        if (templatePageType == null)
        {
            throw new TemplatingException(Razor.Resources.Strings.CompiledRazorPage_RenderAsync_Exception.FormatWith(this.Template.Name, typeof(ITemplatePage<T>)));
        }

        var page = (ITemplatePage<T>)Activator.CreateInstance(templatePageType)!;
        return page.RenderAsync(model, writer, cancellationToken);
    }
}