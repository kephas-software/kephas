// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableHelperTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the queryable helper test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class QueryableHelperTest
    {
        [Test]
        public async Task CountAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.CountAsync();
            Assert.AreEqual(2, value);
        }

        [Test]
        public async Task CountAsync_non_async()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.CountAsync();
            Assert.AreEqual(2, value);
        }

        [Test]
        public async Task CountAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.CountAsync();
            Assert.AreEqual(2, value);
        }

        [Test]
        public async Task LongCountAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.LongCountAsync();
            Assert.AreEqual(2, value);
        }

        [Test]
        public async Task LongCountAsync_non_async()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.LongCountAsync();
            Assert.AreEqual(2, value);
        }

        [Test]
        public async Task LongCountAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.LongCountAsync();
            Assert.AreEqual(2, value);
        }

        [Test]
        public async Task SingleAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi" }.AsQueryable();
            var value = await queryable.SingleAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task SingleAsync_non_async()
        {
            var queryable = new[] { "Hi" }.AsQueryable();
            var value = await queryable.SingleAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task SingleAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi" }.AsQueryable());
            var value = await queryable.SingleAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task SingleAsync_non_async_predicate()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.SingleAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task SingleAsync_async_predicate()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.SingleAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }
        [Test]
        public async Task SingleOrDefaultAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi" }.AsQueryable();
            var value = await queryable.SingleOrDefaultAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task SingleOrDefaultAsync_non_async()
        {
            var queryable = new[] { "Hi" }.AsQueryable();
            var value = await queryable.SingleOrDefaultAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task SingleOrDefaultAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi" }.AsQueryable());
            var value = await queryable.SingleOrDefaultAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task SingleOrDefaultAsync_non_async_predicate()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.SingleOrDefaultAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task SingleOrDefaultAsync_async_predicate()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.SingleOrDefaultAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task FirstAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.FirstAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task FirstAsync_non_async()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.FirstAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task FirstAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.FirstAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task FirstAsync_non_async_predicate()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.FirstAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task FirstAsync_async_predicate()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.FirstAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task FirstOrDefaultAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.FirstOrDefaultAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task FirstOrDefaultAsync_non_async()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.FirstOrDefaultAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task FirstOrDefaultAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.FirstOrDefaultAsync();
            Assert.AreEqual("Hi", value);
        }

        [Test]
        public async Task FirstOrDefaultAsync_non_async_predicate()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.FirstOrDefaultAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task FirstOrDefaultAsync_async_predicate()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var value = await queryable.FirstOrDefaultAsync(e => e == "there");
            Assert.AreEqual("there", value);
        }

        [Test]
        public async Task AnyAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi", "there" }.AsQueryable();
            var any = await queryable.AnyAsync();
            Assert.IsTrue(any);
        }

        [Test]
        public async Task AnyAsync_non_async()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var any = await queryable.AnyAsync();
            Assert.IsTrue(any);
        }

        [Test]
        public async Task AnyAsync_async()
        {
            var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
            var any = await queryable.AnyAsync();
            Assert.IsTrue(any);
        }

        [Test]
        public async Task ToListAsync_non_generic()
        {
            var queryable = (IQueryable)new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.ToListAsync();
            Assert.IsInstanceOf<List<object>>(value);
            Assert.IsTrue(value.Contains("Hi"));
            Assert.IsTrue(value.Contains("there"));
        }

        [Test]
        public async Task ToListAsync_non_async()
        {
            var queryable = new[] { "Hi", "there" }.AsQueryable();
            var value = await queryable.ToListAsync();
            Assert.IsInstanceOf<List<string>>(value);
            Assert.IsTrue(value.Contains("Hi"));
            Assert.IsTrue(value.Contains("there"));
        }

        ////[Test]
        ////public async Task ToListAsync_async()
        ////{
        ////    var queryable = AsyncQueryProvider.AsAsyncQueryable(new[] { "Hi", "there" }.AsQueryable());
        ////    var value = await queryable.ToListAsync();
        ////    Assert.IsInstanceOf<List<string>>(value);
        ////    Assert.IsTrue(value.Contains("Hi"));
        ////    Assert.IsTrue(value.Contains("there"));
        ////}

        public class AsyncQuery<T> : IQueryable<T>
        {
            private readonly IAsyncQueryProvider queryProvider;

            private readonly IQueryable<T> nativeQuery;

            public AsyncQuery(IAsyncQueryProvider queryProvider, IQueryable<T> nativeQuery)
            {
                this.queryProvider = queryProvider;
                this.nativeQuery = nativeQuery;
            }

            /// <summary>Returns an enumerator that iterates through the collection.</summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<T> GetEnumerator()
            {
                return this.nativeQuery.GetEnumerator();
            }

            /// <summary>Returns an enumerator that iterates through a collection.</summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            /// <summary>Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable" />.</summary>
            /// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> that is associated with this instance of <see cref="T:System.Linq.IQueryable" />.</returns>
            public Expression Expression => this.nativeQuery.Expression;

            /// <summary>Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable" /> is executed.</summary>
            /// <returns>A <see cref="T:System.Type" /> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.</returns>
            public Type ElementType => this.nativeQuery.ElementType;

            /// <summary>Gets the query provider that is associated with this data source.</summary>
            /// <returns>The <see cref="T:System.Linq.IQueryProvider" /> that is associated with this data source.</returns>
            public IQueryProvider Provider => this.queryProvider;
        }

        public class AsyncQueryProvider : IAsyncQueryProvider
        {
            private readonly IQueryProvider nativeQueryProvider;

            public AsyncQueryProvider(IQueryProvider nativeQueryProvider)
            {
                this.nativeQueryProvider = nativeQueryProvider;
            }

            public static IQueryable<TElement> AsAsyncQueryable<TElement>(IQueryable<TElement> queryable)
            {
                var queryProvider = new AsyncQueryProvider(queryable.Provider);
                return queryProvider.CreateQuery<TElement>(queryable.Expression);
            }

            /// <summary>Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query represented by a specified expression tree.</summary>
            /// <returns>An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the specified expression tree.</returns>
            /// <param name="expression">An expression tree that represents a LINQ query.</param>
            public IQueryable CreateQuery(Expression expression)
            {
                var elementType = expression.Type.TryGetEnumerableItemType();
                var nativeQuery = this.nativeQueryProvider.CreateQuery(expression);
                var asyncQueryTypeInfo = typeof(AsyncQuery<>).MakeGenericType(elementType).AsRuntimeTypeInfo();
                var asyncQuery = (IQueryable)asyncQueryTypeInfo.CreateInstance(new object[] { this, nativeQuery });
                return asyncQuery;
            }

            /// <summary>Constructs an <see cref="T:System.Linq.IQueryable`1" /> object that can evaluate the query represented by a specified expression tree.</summary>
            /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> that can evaluate the query represented by the specified expression tree.</returns>
            /// <param name="expression">An expression tree that represents a LINQ query.</param>
            /// <typeparam name="TElement">The type of the elements of the <see cref="T:System.Linq.IQueryable`1" /> that is returned.</typeparam>
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                var nativeQuery = this.nativeQueryProvider.CreateQuery<TElement>(expression);
                return new AsyncQuery<TElement>(this, nativeQuery);
            }

            /// <summary>Executes the query represented by a specified expression tree.</summary>
            /// <returns>The value that results from executing the specified query.</returns>
            /// <param name="expression">An expression tree that represents a LINQ query.</param>
            public object Execute(Expression expression)
            {
                return this.nativeQueryProvider.Execute(expression);
            }

            /// <summary>Executes the strongly-typed query represented by a specified expression tree.</summary>
            /// <returns>The value that results from executing the specified query.</returns>
            /// <param name="expression">An expression tree that represents a LINQ query.</param>
            /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
            public TResult Execute<TResult>(Expression expression)
            {
                return this.nativeQueryProvider.Execute<TResult>(expression);
            }

            /// <summary>
            /// Asynchronously executes the query represented by a specified expression tree.
            /// </summary>
            /// <param name="expression"> An expression tree that represents a LINQ query. </param>
            /// <param name="cancellationToken">
            /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
            /// </param>
            /// <returns>
            /// A task that represents the asynchronous operation.
            /// The task result contains the value that results from executing the specified query.
            /// </returns>
            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.FromResult(this.Execute(expression));
            }

            /// <summary>
            /// Asynchronously executes the strongly-typed query represented by a specified expression tree.
            /// </summary>
            /// <typeparam name="TResult"> The type of the value that results from executing the query. </typeparam>
            /// <param name="expression"> An expression tree that represents a LINQ query. </param>
            /// <param name="cancellationToken">
            /// A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
            /// </param>
            /// <returns>
            /// A task that represents the asynchronous operation.
            /// The task result contains the value that results from executing the specified query.
            /// </returns>
            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.FromResult(this.Execute<TResult>(expression));
            }
        }
    }
}