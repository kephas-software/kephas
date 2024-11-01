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
            .Append("Time: ToString(UtcDateTime(@t), 'yyyy-MM-dd HH:mm:ss.fffZ')").Append(separator)
            .Append("Level: @l").Append(separator)
            .Append("Env: GetEnv(Environment)").Append(separator)
            .Append("Message: FormatMessage(@mt, @p)").Append(separator)
            .Append("AppName: GetAppName(AppName)").Append(separator)
            .Append("AppVersion: GetAppVersion(AppVersion)").Append(separator)
            .Append("AppInstanceId: GetAppInstanceId(AppInstanceId)").Append(separator)
            .Append("ClassName: SourceContext").Append(separator)
            .Append("StackTrace: @x").Append(separator)
            .Append("..@p").Append(separator)
            .Append("SourceContext: undefined()").Append(separator)
            .Append("Environment: undefined()").Append(separator);
        sb.Length -= separator.Length;
        sb .AppendLine("} }");
        return sb.ToString();
    }
}