// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVersionComparer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Versioning
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract for version comparer objects capable of sorting and determining the equality of
    /// <see cref="SemanticVersion"/> objects.
    /// </summary>
    public interface IVersionComparer : IEqualityComparer<SemanticVersion>, IComparer<SemanticVersion>
    {
    }
}