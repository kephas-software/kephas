// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for collections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq.Expressions;

namespace Kephas.Collections;

/// <summary>
/// Extension methods for collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds a range of items to the collection.
    /// </summary>
    /// <typeparam name="T">The collection type.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="items">The items.</param>
    /// <returns>The provided collection for method chaining.</returns>
    public static T AddRange<T, TItem>(this T collection, IEnumerable<TItem>? items)
        where T : class, ICollection<TItem>
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (items == null || ReferenceEquals(collection, items))
        {
            return collection;
        }

        foreach (var item in items)
        {
            collection.Add(item);
        }

        return collection;
    }

    /// <summary>Returns the only element of a sequence asynchronously, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
    /// <param name="source">An <see cref="T:System.Collections.Generic.AsyncIEnumerable`1" /> to return the single element of.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
    /// <returns>A value task yielding the single element of the input sequence, or <see langword="default" />(<typeparamref name="T" />) if the sequence contains no elements.</returns>
    public static async ValueTask<T?> SingleOrDefaultAsync<T>(
        this IQueryable<T> source,
        CancellationToken cancellationToken = default)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));

        if (source is not IAsyncEnumerable<T> asyncEnumerable)
        {
            return source.SingleOrDefault();
        }
        
        await using var enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
        if (!await enumerator.MoveNextAsync())
        {
            return default;
        }

        var result = enumerator.Current;
        return await enumerator.MoveNextAsync()
            ? throw new InvalidOperationException("More than one element found in sequence.")
            : result;
    }

    /// <summary>Returns the only element of a sequence asynchronously, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
    /// <param name="source">An <see cref="T:System.Collections.Generic.AsyncIEnumerable`1" /> to return the single element of.</param>
    /// <param name="predicate">The predicate to filter the data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
    /// <returns>A value task yielding the single element of the input sequence, or <see langword="default" />(<typeparamref name="T" />) if the sequence contains no elements.</returns>
    public static ValueTask<T?> SingleOrDefaultAsync<T>(
        this IQueryable<T> source,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));

        return source.Where(predicate).SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>Returns the first element of a sequence asynchronously, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
    /// <param name="source">An <see cref="T:System.Collections.Generic.AsyncIEnumerable`1" /> to return the single element of.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
    /// <returns>A value task yielding the single element of the input sequence, or <see langword="default" />(<typeparamref name="T" />) if the sequence contains no elements.</returns>
    public static async ValueTask<T?> FirstOrDefaultAsync<T>(
        this IQueryable<T> source,
        CancellationToken cancellationToken = default)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));

        if (source is not IAsyncEnumerable<T> asyncEnumerable)
        {
            return source.FirstOrDefault();
        }
        
        await using var enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
        return await enumerator.MoveNextAsync()
            ? enumerator.Current
            : default;
    }

    /// <summary>Returns the first element of a sequence asynchronously, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
    /// <param name="source">An <see cref="T:System.Collections.Generic.AsyncIEnumerable`1" /> to return the single element of.</param>
    /// <param name="predicate">The predicate to filter the data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
    /// <returns>A value task yielding the single element of the input sequence, or <see langword="default" />(<typeparamref name="T" />) if the sequence contains no elements.</returns>
    public static ValueTask<T?> FirstOrDefaultAsync<T>(
        this IQueryable<T> source,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));

        return source.Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }
}