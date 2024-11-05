// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicLogEventProperty.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Dynamic;
using Serilog.Events;

namespace Kephas.Extensions.Logging.Formatting;

internal class DynamicLogEventProperty(StructureValue properties) : IDynamic
{
    private static readonly string SequenceStart = "[ "; 
    private static readonly string SequenceEnd = " ]"; 
    private static readonly string DictionaryStart = "{ "; 
    private static readonly string DictionaryEnd = " }"; 
    private static readonly string DictionaryKeyValueSeparator = ": "; 
    private static readonly string ItemSeparator = ", "; 

    private readonly IReadOnlyList<LogEventProperty> properties = properties.Properties;

    /// <summary>
    /// Gets the dynamic keys to access the content of this object.
    /// </summary>
    /// <returns>An enumeration of keys.</returns>
    public IEnumerable<string> GetDynamicKeys()
    {
        return properties.Select(p => p.Name);
    }

    /// <summary>
    /// Gets a value indicating whether the object contains the provided key.
    /// </summary>
    /// <param name="key">The key to be checked.</param>
    /// <returns><c>true</c> if the key is found, <c>false</c> otherwise.</returns>
    public bool ContainsKey(string key) => properties.Any(p => p.Name == key);

    /// <summary>
    /// Gets or sets the dynamic value.
    /// </summary>
    /// <param name="key">The value key.</param>
    public object? this[string key]
    {
        get
        {
            var propValue = properties.FirstOrDefault(p => p.Name == key)?.Value;
            if (propValue is null)
            {
                return $"!!! '{key}' is not found !!!";
            }

            return ToString(propValue);
        }
        set => throw new NotSupportedException();
    }

    private static string? ToString(LogEventPropertyValue propValue) =>
        propValue switch
        {
            ScalarValue scalar => ToString(scalar),
            SequenceValue sequence => ToString(sequence),
            DictionaryValue dictionary => ToString(dictionary),
            _ => propValue.ToString()
        };

    private static string ToString(DictionaryValue dictionary)
    {
        var sb = new StringBuilder(DictionaryStart);
        foreach (var e in dictionary.Elements)
        {
            sb.Append(ToString(e.Key))
                .Append(DictionaryKeyValueSeparator)
                .Append(ToString(e.Value))
                .Append(ItemSeparator);
        }

        if (sb.Length > DictionaryStart.Length)
        {
            sb.Length -= ItemSeparator.Length;
        }

        sb.Append(DictionaryEnd);

        return sb.ToString();
    }

    private static string ToString(SequenceValue sequence)
    {
        var sb = new StringBuilder(SequenceStart);
        foreach (var e in sequence.Elements)
        {
            sb.Append(ToString(e))
                .Append(ItemSeparator);
        }

        if (sb.Length > SequenceStart.Length)
        {
            sb.Length -= ItemSeparator.Length;
        }

        sb.Append(SequenceEnd);

        return sb.ToString();
    }

    private static string? ToString(ScalarValue scalar)
    {
        return scalar.Value?.ToString();
    }
}
