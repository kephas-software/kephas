// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableQueryable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Linq;

using System.Collections;
using System.Linq.Expressions;

/// <summary>
/// A disposable wrapper for a Queryable.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <seealso cref="IDisposableQueryable{T}" />
public class DisposableQueryable<T> : IDisposableQueryable<T>
{
    private readonly IQueryable<T> queryable;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableQueryable{T}"/> class.
    /// </summary>
    /// <param name="queryable">The queryable.</param>
    public DisposableQueryable(IQueryable<T> queryable)
    {
        this.queryable = queryable ?? throw new ArgumentNullException(nameof(queryable));
    }

    /// <summary>Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.</summary>
    /// <returns>A <see cref="T:System.Type" /> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.</returns>
    public Type ElementType => this.queryable.ElementType;

    /// <summary>Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable" />.</summary>
    /// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this instance of <see cref="T:System.Linq.IQueryable" />.</returns>
    public Expression Expression => this.queryable.Expression;

    /// <summary>Gets the query provider that is associated with this data source.</summary>
    /// <returns>The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data source.</returns>
    public IQueryProvider Provider => this.queryable.Provider;

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator() => this.queryable.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            (this.queryable as IDisposable)?.Dispose();
        }
    }
}