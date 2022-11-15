// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisposableQueryable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Linq;

/// <summary>
/// A queryable and disposable instance.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IDisposableQueryable<out T> : IQueryable<T>, IDisposable
{
}