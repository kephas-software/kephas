// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Testing.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kephas.Testing
{
    /// <summary>
    /// Provides testing primitives.
    /// </summary>
    public static class Test
    {
        /// <summary>
        /// Gets the test <see cref="ILogger{T}"/>.
        /// </summary>
        /// <returns>The test logger.</returns>
        public static ILogger<T> GetTestLogger<T>(LogLevel? minimumLevel = null) => new TestLogger<T>(minimumLevel: minimumLevel);

        /// <summary>
        /// Gets the test <see cref="ILogger{T}"/>.
        /// </summary>
        /// <returns>The test logger.</returns>
        public static ILogger<T> GetTestLogger<T>(StringBuilder? stringBuilder, LogLevel? minimumLevel = null) => new TestLogger<T>(stringBuilder, minimumLevel: minimumLevel);

        /// <summary>
        /// Creates a test instance implementing <see cref="IAppServiceCollection"/>.
        /// </summary>
        /// <returns>The <see cref="IAppServiceCollection"/>.</returns>
        public static IAppServiceCollection CreateTestServices()
        {
            return new AppServiceCollection();
        }

        /// <summary>
        /// Creates a test instance implementing <see cref="IAppServiceCollectionBuilder"/>.
        /// </summary>
        /// <param name="services">Optional. The service collection used to initialize the builder with.</param>
        /// <returns>The <see cref="IAppServiceCollectionBuilder"/>.</returns>
        public static IAppServiceCollectionBuilder CreateTestServicesBuilder(IAppServiceCollection? services = null)
        {
            return new AppServiceCollectionBuilder(services ?? CreateTestServices());
        }

        /// <summary>
        /// Gets the test lazy enumerator.
        /// </summary>
        /// <param name="services">The services</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <typeparam name="TMetadata">The metadata type.</typeparam>
        /// <returns>The test <see cref="ILazyEnumerable{TContract,TMetadata}"/>.</returns>
        public static ILazyEnumerable<T, TMetadata> GetTestLazyEnumerable<T, TMetadata>(params T[] services)
        {
            var lazyEnumerable = Substitute.For<ILazyEnumerable<T, TMetadata>>();

            lazyEnumerable.GetEnumerator()
                .Returns(services
                    .Select(s => new Lazy<T, TMetadata>(() => s, CreateMetadata<TMetadata>(null)))
                    .ToList()
                    .GetEnumerator());

            return lazyEnumerable;
        }

        private static TMetadata CreateMetadata<TMetadata>(IDictionary<string, object?>? metadata)
        {
            var typedMetadata = Activator.CreateInstance(typeof(TMetadata), metadata);
            return (TMetadata)typedMetadata!;
        }
    }
}
