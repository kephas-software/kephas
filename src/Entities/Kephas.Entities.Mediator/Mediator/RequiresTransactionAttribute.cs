// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiresTransactionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities.Mediator;

/// <summary>
/// Attribute for annotating operations which require transactional context.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
public class RequiresTransactionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether the annotated operation requires
    /// a new transaction (<c>true</c>) or an existing one can be used (<c>false</c>).
    /// </summary>
    public bool StartNewTransaction { get; init; }
}
