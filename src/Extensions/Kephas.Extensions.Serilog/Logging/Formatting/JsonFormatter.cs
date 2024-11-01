using System.Text;
using Serilog.Expressions;
using Serilog.Templates;

namespace Kephas.Extensions.Logging.Formatting;

/// <summary>
/// This formatter takes care of formatting the Serilog JSON output
/// as specified in the Log Format and Naming for Projects (https://atari:8443/wiki/display/DevOpPVS/Log+Format+and+Naming+for+Projects). 
/// </summary>
public class JsonFormatter : ExpressionTemplate
{
    private static readonly string Template = BuildTemplate();

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFormatter"/> class.
    /// </summary>
    public JsonFormatter()
        : this(new FormatMessageResolver())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFormatter"/> class.
    /// </summary>
    protected internal JsonFormatter(NameResolver resolver)
        : base(Template, nameResolver: resolver)
    {
    }

    private static string BuildTemplate()
    {
        const string separator = ", ";
        var sb = new StringBuilder("{ { ")
            .Append("time: ToString(UtcDateTime(@t), 'yyyy-MM-dd HH:mm:ss.fffZ')").Append(separator)
            .Append("appName: GetAppName(AppName)").Append(separator)
            .Append("appVersion: GetAppVersion(AppVersion)").Append(separator)
            .Append("appInstanceId: GetAppInstanceId(AppInstanceId)").Append(separator)
            .Append("env: GetEnv(Environment)").Append(separator)
            .Append("statusCode: StatusCode").Append(separator)
            .Append("client-ip: ClientIP").Append(separator)
            .Append("userName: UserName").Append(separator)
            .Append("path: Path").Append(separator)
            .Append("hostName: MachineName").Append(separator)
            .Append("level: @l").Append(separator)
            .Append("thread: ThreadId").Append(separator)
            .Append("className: SourceContext").Append(separator)
            .Append("correlation: CorrelationId").Append(separator)
            .Append("message: FormatMessage(@mt, @p)").Append(separator)
            .Append("stackTrace: @x").Append(separator)
            .Append("..@p").Append(separator)
            .Append("AppName: undefined()").Append(separator)
            .Append("AppVersion: undefined()").Append(separator)
            .Append("AppInstanceId: undefined()").Append(separator)
            .Append("Environment: undefined()").Append(separator)
            .Append("StatusCode: undefined()").Append(separator)
            .Append("ClientIP: undefined()").Append(separator)
            .Append("UserName: undefined()").Append(separator)
            .Append("Path: undefined()").Append(separator)
            .Append("MachineName: undefined()").Append(separator)
            .Append("ThreadId: undefined()").Append(separator)
            .Append("SourceContext: undefined()").Append(separator)
            .Append("CorrelationId: undefined()").Append(separator);
        sb.Length -= separator.Length;
        sb .AppendLine("} }");
        return sb.ToString();
    }
}