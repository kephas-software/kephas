using Kephas.Runtime;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions.Core;

namespace Kephas.Extensions.Serilog;

/// <summary>
/// Options for configuring Serilog.
/// </summary>
public record SerilogOptions
{
    /// <summary>
    /// Default value for the maximum destructuring depth.
    /// </summary>
    public static readonly int DefaultMaxDestructuringDepth = 5;

    /// <summary>
    /// Default value for the maximum collection count on destructuring.
    /// </summary>
    public static readonly int DefaultMaxCollectionCount = 10;
    
    /// <summary>
    /// The application runtime.
    /// </summary>
    public IAppRuntime? AppRuntime { get; set; }
    
    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public IConfiguration? Configuration { get; set; }
    
    /// <summary>
    /// The logging level switch.
    /// </summary>
    public LoggingLevelSwitch? LevelSwitch { get; set; }

    /// <summary>
    /// The maximum destructuring depth.
    /// </summary>
    public int MaxDestructuringDepth { get; set; } = DefaultMaxDestructuringDepth;

    /// <summary>
    /// The maximum number of elements to destructure from a collection.
    /// </summary>
    public int MaxCollectionCount { get; set; } = DefaultMaxCollectionCount;

    /// <summary>
    /// The action to be called upon logging internal errors.
    /// </summary>
    public Action<string>? SelfLog { get; set; }

    /// <summary>
    /// Gets or sets the destructuring options for exceptions.
    /// </summary>
    /// <remarks>
    /// Make sure that, when providing custom destructuring options,
    /// circular references in exceptions are handled properly and, also,
    /// large objects like in the case of Entity Framework. See here for more details:
    /// https://github.com/datalust/serilog-sinks-seq/issues/98
    /// </remarks>
    public IDestructuringOptions? ExceptionDestructuringOptions { get; set; }

    /// <summary>
    /// Value indicating whether the exception detail enricher should be used.  
    /// </summary>
    public bool UseExceptionDetailsEnricher { get; set; } = true;

    /// <summary>
    /// Action to be invoked before configuring the logger.
    /// </summary>
    public Action<IServiceProvider, LoggerConfiguration>? PreConfigure { get; set; }

    /// <summary>
    /// Action to be invoked after configuring the logger.
    /// </summary>
    public Action<IServiceProvider, LoggerConfiguration>? PostConfigure { get; set; }
};