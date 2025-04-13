// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuerySqlString.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text;
using Kephas.Entities.StoredProcedures;
using Kephas.Reflection;

namespace Kephas.Entities;

/// <summary>
/// SQL string for query.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class QuerySqlString<T> : FormattableString
{
    private readonly IList<KeyValuePair<string, object?>> args;
    private string? format;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySqlString{T}"/> class.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public QuerySqlString(ISpArgs args)
    {
        this.args = args.GetType()
            .GetPublicInstanceProperties()
            .Select(p => new KeyValuePair<string, object?>(p.Name, p.GetValue(args)))
            .ToList();
    }

    /// <summary>Returns the argument at the specified index position.</summary>
    /// <param name="index">The index of the argument. Its value can range from zero to one less than the value of <see cref="P:System.FormattableString.ArgumentCount" />.</param>
    /// <returns>The argument.</returns>
    public override object? GetArgument(int index) => args[index].Value;

    /// <summary>Returns an object array that contains one or more objects to format.</summary>
    /// <returns>An object array that contains one or more objects to format.</returns>
    public override object?[] GetArguments() => args.Select(a => a.Value).ToArray();

    /// <summary>Returns the string that results from formatting the composite format string along with its arguments by using the formatting conventions of a specified culture.</summary>
    /// <param name="formatProvider">An object that provides culture-specific formatting information.</param>
    /// <returns>A result string formatted by using the conventions of <paramref name="formatProvider" />.</returns>
    public override string ToString(IFormatProvider? formatProvider)
    {
        var storageAttr = typeof(T).GetCustomAttribute<StoredProcedureAttribute>();
        var sqlBuilder = new StringBuilder();
        var storageName = storageAttr is null ? typeof(T).Name : storageAttr.Name;
        var storageKind = storageAttr?.StorageKind ?? StorageKind.StoredProcedure;

        sqlBuilder
            .Append(storageName)
            .Append(QuerySqlStringHelper.Brackets[storageKind].open);

        foreach (var (_, value) in args)
        {
            sqlBuilder
                .Append(FormatValue(value))
                .Append(',');
        }

        if (args.Count > 0)
        {
            sqlBuilder.Length--;
        }

        sqlBuilder
            .Append(QuerySqlStringHelper.Brackets[storageKind].close);

        return sqlBuilder.ToString();
    }

    /// <summary>Gets the number of arguments to be formatted.</summary>
    /// <returns>The number of arguments to be formatted.</returns>
    public override int ArgumentCount => args.Count;

    /// <summary>Returns the composite format string.</summary>
    /// <returns>The composite format string.</returns>
    public override string Format => format ??= BuildFormat();

    private string BuildFormat()
    {
        var sqlBuilder = new StringBuilder();
        var storageAttr = typeof(T).GetCustomAttribute<StoredProcedureAttribute>();
        var storageName = storageAttr is null ? typeof(T).Name : storageAttr.Name;
        var storageKind = storageAttr?.StorageKind ?? StorageKind.StoredProcedure;

        sqlBuilder
            .Append(storageName)
            .Append(QuerySqlStringHelper.Brackets[storageKind].open);

        if (args.Count > 0)
        {
            for (var i = 0; i < args.Count; i++)
            {
                sqlBuilder.Append($"{{{i}}},");
            }

            sqlBuilder.Length--;
        }

        sqlBuilder
            .Append(QuerySqlStringHelper.Brackets[storageKind].close);

        return sqlBuilder.ToString();
    }

    private string? FormatValue(object? value)
    {
        return value switch
        {
            null => "NULL",
            bool boolValue => boolValue ? "1" : "0",
            string strValue => $"'{strValue.Replace("'", "''")}'",
            _ => value.ToString(),
        };
    }
}