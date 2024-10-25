using System.Text.RegularExpressions;
using Kephas.Dynamic;

namespace Kephas.Logging.Formatting;

internal class MessageInterpolationFormatter 
{
    /// <summary>
    /// Formats the provided data using the template and writes the output to the writer.
    /// </summary>
    /// <param name="template">The template used in formatting.</param>
    /// <param name="model">The data used for formatting.</param>
    /// <returns>The formatted string.</returns>
    public string Format(string template, IDynamic model)
    {
        using var writer = new StringWriter();
        
        var crtIndex = 0;
        var matches = Regex.Matches(template, "{([^{}]*)");
        foreach (var match in matches.OfType<Match>())
        {
            if (match.Index > crtIndex)
            {
                writer.Write(template[crtIndex..match.Index]);
            }

            var value = GetFormattedValue(model, match.Groups[1].Value);
            if (value is not null)
            {
                writer.Write(value);
            }

            crtIndex = match.Index + match.Length + 1;
        }

        if (template.Length > crtIndex)
        {
            writer.Write(template[crtIndex..]);
        }

        return writer.GetStringBuilder().ToString();
    }

    private static string? GetFormattedValue(IDynamic model, string valueInfo)
    {
        var colonIndex = valueInfo.IndexOf(':');
        var propertyPath = colonIndex >= 0 ? valueInfo[..colonIndex] : valueInfo;
        var value = GetValue(model, propertyPath);
        if (value is null)
        {
            return null;
        }

        if (colonIndex >= 0 && value is IFormattable formattableValue)
        {
            return formattableValue.ToString(valueInfo[(colonIndex + 1)..], null);
        }

        return value.ToString();
    }

    private static object? GetValue(object? obj, string propertyPath) =>
        obj is null
            ? null
            : GetValue(obj, propertyPath, 0, propertyPath);

    private static object? GetValue(object obj, string propertyPath, int position, string originalPath)
    {
        while (true)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                throw new ArgumentException(
                    $"The property path cannot be empty (position {position}, path '{originalPath}').",
                    nameof(propertyPath));
            }

            var dotIndex = propertyPath.IndexOf('.');
            var propName = dotIndex > 0 ? propertyPath[..dotIndex] : propertyPath;
            try
            {
                var value = obj.ToDynamic()[propName];
                if (value is null || dotIndex < 0)
                {
                    return value;
                }

                obj = value;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"The property name '{propName}' cannot be resolved for type '{obj.GetType()}' (position {position}, path '{originalPath}').",
                    nameof(propertyPath),
                    ex);
            }

            propertyPath = propertyPath[(dotIndex + 1)..];
            position = position + dotIndex + 1;
        }
    }
}