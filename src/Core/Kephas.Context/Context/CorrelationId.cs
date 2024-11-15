// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationId.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Context;

/// <summary>
/// The correlation identifier.
/// </summary>
public struct CorrelationId() : IFormattable
{
    private readonly Guid value = Guid.NewGuid();

    /// <summary>Returns the fully qualified type name of this instance.</summary>
    /// <returns>The fully qualified type name.</returns>
    public override string ToString() => value.ToString("N");

    /// <summary>Formats the value of the current instance using the specified format.</summary>
    /// <param name="format">The format to use.
    /// -or-
    /// A null reference (<see langword="Nothing" /> in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
    /// <param name="formatProvider">The provider to use to format the value.
    /// -or-
    /// A null reference (<see langword="Nothing" /> in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
    /// <returns>The value of the current instance in the specified format.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        value.ToString(format, formatProvider);
}