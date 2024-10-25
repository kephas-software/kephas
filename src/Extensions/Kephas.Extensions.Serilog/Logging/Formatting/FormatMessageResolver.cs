using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Runtime;
using Serilog.Events;
using Serilog.Expressions;

namespace Kephas.Logging.Formatting;

internal class FormatMessageResolver : NameResolver
{
    private static readonly MessageInterpolationFormatter MessageFormatter = new ();
    private static IAppRuntime? _appRuntime;
    private static Lazy<string> _lazyAppInstanceId = new(ResolveAppInstanceId, true);
    private static Lazy<string> _lazyAppName = new(ResolveAppName, true);
    private static Lazy<string> _lazyAppVersion = new(ResolveAppVersion, true);
    private static Lazy<string> _lazyEnvironment = new(ResolveEnvironment, true);
    private static Assembly? entryAssembly;

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

    public static LogEventPropertyValue? GetEnvironment(LogEventPropertyValue? environment)
    {
        return environment ?? new ScalarValue(_lazyEnvironment.Value);
    }

    internal static void SetAppRuntime(IAppRuntime appRuntime)
    {
        _appRuntime = appRuntime;
        var appEntryAssembly = appRuntime.GetEntryAssembly();
        if (appEntryAssembly is not null)
        {
            SetEntryAssembly(appEntryAssembly);
        }
        else
        {
            ResetResolvers();
        }
    }

    internal static void SetEntryAssembly(Assembly? assembly)
    {
        entryAssembly = assembly;
        ResetResolvers();
    }

    private static void ResetResolvers()
    {
        _lazyAppName = new(ResolveAppName, true);
        _lazyAppVersion = new(ResolveAppVersion, true);
        _lazyAppInstanceId = new(ResolveAppInstanceId, true);
        _lazyEnvironment = new(ResolveEnvironment, true);
    }

    internal static string ResolveAppName()
    {
        var appName = _appRuntime?.GetAppName();
        if (appName is not null)
        {
            return appName;
        }

        var referenceAssembly = entryAssembly ?? Assembly.GetEntryAssembly();
        return IAppRuntime.GetAppNameFromAssembly(referenceAssembly) ?? "unknown";
    }

    internal static string ResolveAppInstanceId()
    {
        var appInstanceId = _appRuntime?.GetAppInstanceId();
        if (appInstanceId is not null)
        {
            return appInstanceId;
        }

        var referenceAssembly = entryAssembly ?? Assembly.GetEntryAssembly();
        return IAppRuntime.GetAppInstanceIdFromAssembly(referenceAssembly);
    }

    internal static string ResolveAppVersion()
    {
        var appVersion = _appRuntime?.GetAppVersion();
        if (appVersion is not null)
        {
            return appVersion;
        }

        var referenceAssembly = entryAssembly ?? Assembly.GetEntryAssembly();
        return IAppRuntime.GetAppVersionFromAssembly(referenceAssembly) ?? "0.0.0";
    }

    internal static string ResolveEnvironment()
    {
        var environment = _appRuntime?.GetEnvironment();
        return environment ?? "unknown";
    }
}