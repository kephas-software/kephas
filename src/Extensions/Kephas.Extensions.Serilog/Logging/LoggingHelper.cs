using Kephas.Logging.Formatting;
using Kephas.Runtime;

namespace Kephas.Logging;

/// <summary>
/// Helper methods for logging.
/// </summary>
public static class LoggingHelper
{
    private static readonly object FallbackLogLock = new();

    /// <summary>
    /// Gets or sets the fallback log path.
    /// </summary>
    public static string FallbackLogPath { get; set; } = Path.GetTempPath();

    /// <summary>
    /// Sets the application runtime.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    public static void SetAppRuntime(IAppRuntime appRuntime) =>
        FormatMessageResolver.SetAppRuntime(appRuntime ?? throw new ArgumentNullException(nameof(appRuntime)));

    /// <summary>
    /// Write the fallback log error.
    /// </summary>
    /// <remarks>
    /// This method does not fail. Any errors are swallowed so that logging itself should not be reason for runtime exceptions.
    /// </remarks>
    /// <param name="message">The message to be logged.</param>
    /// <param name="appRuntime">Optional. The application runtime used to get application level information.</param>
    public static void WriteFallbackLogError(string message, IAppRuntime? appRuntime = null)
    {
        try
        {
            var localNow = DateTime.Now;
            appRuntime ??= new AppRuntime();
            var filePath = Path.Combine(FallbackLogPath, $"internal-{appRuntime.GetAppInstanceId()}.{localNow:yyyyMMdd}.json");
            var application = appRuntime.GetAppName();
            var appVersion = appRuntime.GetAppVersion();
            var appInstanceId = appRuntime.GetAppInstanceId();
            var environment = appRuntime.GetEnvironment();
            var hostname = appRuntime.GetMachineName();
            var jsonMessage = $$"""
                                {"time":"{{localNow.ToUniversalTime():s}}","appName":"{{application}}","appVersion":"{{appVersion}}","appInstanceId":"{{appInstanceId}}","env":"{{environment}}","hostname":"{{hostname}}","level":"Error","threadId":"{{Environment.CurrentManagedThreadId}}","message":"{{message.Replace("\\", "\\\\").Replace("\"", "\\\"")}}"}
                                """;

            // see here for the lock requirement
            // https://stackoverflow.com/questions/4809843/net-2-0-file-appendalltext-thread-safe-implementation
            lock (FallbackLogLock)
            {
                File.AppendAllLines(filePath, [jsonMessage]);
            }
        }
        catch
        {
            // fail silently, as no one can listen to this, only the loving God...
        }
    }
}