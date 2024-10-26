using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Runtime;
using Serilog.Events;
using Serilog.Expressions;

namespace Kephas.Extensions.Logging.Formatting;

internal class FormatMessageResolver : NameResolver
{
    private static readonly MessageInterpolationFormatter MessageFormatter = new ();
    private static IAppRuntime _appRuntime = new AppRuntime();
    private static Lazy<string> _lazyAppInstanceId = new(ResolveAppInstanceId, true);
    private static Lazy<string> _lazyAppName = new(ResolveAppName, true);
    private static Lazy<string> _lazyAppVersion = new(ResolveAppVersion, true);
    private static Lazy<string> _lazyEnv = new(ResolveEnv, true);

    public override bool TryResolveFunctionName(string name, [MaybeNullWhen(false)] out MethodInfo implementation)
    {
        if (base.TryResolveFunctionName(name, out implementation))
        {
            return true;
        }

        implementation = GetType().GetMethod(name);
        return implementation != null;
    }

    public static LogEventPropertyValue? FormatMessage(LogEventPropertyValue? formatValue, LogEventPropertyValue? propertiesValue)
    {
        if (formatValue is not ScalarValue formatScalar)
        {
            throw new ArgumentException($"The resolver {nameof(formatValue)} must be the unformatted message.", nameof(formatValue));
        }

        if (propertiesValue is not StructureValue propertiesStructure)
        {
            throw new ArgumentException($"The resolver {nameof(propertiesValue)} must be the properties structure.", nameof(propertiesValue));
        }

        var result = MessageFormatter.Format(
            formatScalar.Value?.ToString() ?? string.Empty,
            new DynamicLogEventProperty(propertiesStructure));
        return new ScalarValue(result);
    }

    public static LogEventPropertyValue? GetAppVersion(LogEventPropertyValue? appVersion)
    {
        return appVersion ?? new ScalarValue(_lazyAppVersion.Value);
    }

    public static LogEventPropertyValue? GetAppName(LogEventPropertyValue? application)
    {
        return application ?? new ScalarValue(_lazyAppName.Value);
    }

    public static LogEventPropertyValue? GetAppInstanceId(LogEventPropertyValue? application)
    {
        return application ?? new ScalarValue(_lazyAppInstanceId.Value);
    }

    public static LogEventPropertyValue? GetEnv(LogEventPropertyValue? environment)
    {
        return environment ?? new ScalarValue(_lazyEnv.Value);
    }

    internal static void SetAppRuntime(IAppRuntime appRuntime)
    {
        if (_appRuntime == appRuntime)
        {
            return;
        }

        _appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
        ResetResolvers();
    }

    private static void ResetResolvers()
    {
        _lazyAppName = new(ResolveAppName, true);
        _lazyAppVersion = new(ResolveAppVersion, true);
        _lazyAppInstanceId = new(ResolveAppInstanceId, true);
        _lazyEnv = new(ResolveEnv, true);
    }

    internal static string ResolveAppName() => _appRuntime.GetAppName();

    internal static string ResolveAppInstanceId() => _appRuntime.GetAppInstanceId();

    internal static string ResolveAppVersion() => _appRuntime.GetAppVersion();

    internal static string ResolveEnv() => _appRuntime.GetEnvironment();
}