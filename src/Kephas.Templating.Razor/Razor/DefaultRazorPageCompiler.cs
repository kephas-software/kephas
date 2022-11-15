// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRazorPageCompiler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Application;
using Kephas.Collections;
using Kephas.ExceptionHandling;
using Kephas.Operations;
using Kephas.Services;
using Kephas.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// The default implementation of the <see cref="IRazorPageCompiler"/> service.
/// </summary>
[OverridePriority(Priority.Low)]
public class DefaultRazorPageCompiler : IRazorPageCompiler
{
    private readonly IRazorProjectFileSystemProvider projectProvider;
    private readonly IRazorProjectEngineFactory engineFactory;
    private readonly IRazorPageGenerator pageGenerator;
    private readonly IMetadataReferenceManager metadataReferenceManager;
    private readonly IAppRuntime appRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRazorPageCompiler"/> class.
    /// </summary>
    /// <param name="projectProvider">The project provider.</param>
    /// <param name="engineFactory">The engine factory.</param>
    /// <param name="pageGenerator">The page generator.</param>
    /// <param name="metadataReferenceManager">The metadata reference manager.</param>
    /// <param name="appRuntime">The application runtime.</param>
    public DefaultRazorPageCompiler(
        IRazorProjectFileSystemProvider projectProvider,
        IRazorProjectEngineFactory engineFactory,
        IRazorPageGenerator pageGenerator,
        IMetadataReferenceManager metadataReferenceManager,
        IAppRuntime appRuntime)
    {
        this.projectProvider = projectProvider ?? throw new ArgumentNullException(nameof(projectProvider));
        this.engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
        this.pageGenerator = pageGenerator ?? throw new ArgumentNullException(nameof(pageGenerator));
        this.metadataReferenceManager = metadataReferenceManager ?? throw new ArgumentNullException(nameof(metadataReferenceManager));
        this.appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
    }

    /// <summary>
    /// Compiles the template asynchronously.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="template">The template.</param>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result yielding the operation result.</returns>
    public async Task<IOperationResult<ICompiledRazorPage>> CompileTemplateAsync<TModel>(
        ITemplate template,
        ITemplateProcessingContext context,
        CancellationToken cancellationToken = default)
    {
        IOperationResult<ICompiledRazorPage> compileResult = new OperationResult<ICompiledRazorPage>();

        string? tempProjectPath = null;
        try
        {
            var rootNamespace = GetRootNamespace(context);
            var (projectFileSystem, basePath) = await this.projectProvider.GetRazorProjectFileSystemAsync(template, context, cancellationToken).PreserveThreadContext();
            tempProjectPath = basePath;
            context.RootNamespace(rootNamespace)
                .ModelType(typeof(TModel));
            var projectEngine = this.engineFactory.CreateProjectEngine(projectFileSystem, context);

            var genResults = projectEngine.FileSystem.EnumerateItems(basePath)
                .Select(item => this.pageGenerator.GenerateRazorPage(projectEngine, item, context))
                .ToList();
            genResults.ForEach(r => compileResult.MergeMessages(r));
            if (compileResult.HasErrors())
            {
                return compileResult.Fail(new TemplatingException("Errors occurred while generating the C# code."));
            }

            return this.CompileTemplate(
                genResults.Select(r => r.Value).Where(r => r is not null),
                template,
                context,
                compileResult,
                cancellationToken);
        }
        finally
        {
            if (tempProjectPath != null && Directory.Exists(tempProjectPath))
            {
                Directory.Delete(tempProjectPath, true);
            }
        }
    }

    private IOperationResult<ICompiledRazorPage> CompileTemplate(
        IEnumerable<RazorPageGeneratorResult> generatorResults,
        ITemplate template,
        ITemplateProcessingContext context,
        IOperationResult<ICompiledRazorPage> compileResult,
        CancellationToken cancellationToken)
    {
        var syntaxTrees = generatorResults
            .Select(r => CSharpSyntaxTree.ParseText(r.GeneratedCode, path: r.FilePath, cancellationToken: cancellationToken))
            .ToArray();

        var compilation = CSharpCompilation.Create(
            context.RootNamespace(),
            syntaxTrees,
            this.metadataReferenceManager.Resolve(this.appRuntime.GetAppAssemblies()),
            new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));

        var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream, cancellationToken: cancellationToken);

        emitResult.Diagnostics.ForEach(m =>
        {
            if (m.Severity is DiagnosticSeverity.Error)
            {
                compileResult.MergeException(new OperationException(m.ToString())
                {
                    Severity = m.Severity switch
                    {
                        DiagnosticSeverity.Error => SeverityLevel.Error,
                        DiagnosticSeverity.Warning => SeverityLevel.Warning,
                        _ => SeverityLevel.Info
                    },
                });
            }
            else if (m.Severity is not DiagnosticSeverity.Hidden)
            {
                compileResult.MergeMessage(m.ToString());
            }
        });

        if (compileResult.HasErrors())
        {
            return compileResult.Fail(new Exception("Errors occured during compilation."));
        }

        memoryStream.Position = 0;
        return compileResult.Value(new CompiledRazorPage(template, memoryStream, ownsStream: true)).Complete();
    }

    private static string GetRootNamespace(ITemplateProcessingContext context)
    {
        return context.RootNamespace() ?? "Kephas.Templating.Razor.GeneratedPages";
    }
}