// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDestructuringPolicy.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Serilog.Core;
using Serilog.Events;

namespace Kephas.Extensions.Serilog.Destructurers;

/// <summary>
/// Destructuring policy for <see cref="Type"/>.
/// </summary>
public class TypeDestructuringPolicy : IDestructuringPolicy
{
    /// <inheritdoc />
    public bool TryDestructure(
        object value,
        ILogEventPropertyValueFactory propertyValueFactory,
        [NotNullWhen(true)] out LogEventPropertyValue? result)
    {
        if (value is not Type type)
        {
            result = null;
            return false;
        }
        
        result = new ScalarValue(type.FullName);
        return true;
    }
}