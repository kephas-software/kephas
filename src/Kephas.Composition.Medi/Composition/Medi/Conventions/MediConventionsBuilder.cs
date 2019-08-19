// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Conventions
{
    using System;

    using Kephas.Composition.Conventions;
    using Kephas.Diagnostics.Contracts;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions builder for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class MediConventionsBuilder : IConventionsBuilder, IMediServiceCollectionProvider
    {
        private readonly IServiceCollection serviceCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediConventionsBuilder"/> class.
        /// </summary>
        public MediConventionsBuilder()
            : this(new ServiceCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediConventionsBuilder"/> class.
        /// </summary>
        /// <param name="serviceCollection">Collection of services.</param>
        public MediConventionsBuilder(IServiceCollection serviceCollection)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));

            this.serviceCollection = serviceCollection;
        }

        public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
        {
            throw new NotImplementedException();
        }

        public IPartConventionsBuilder ForTypesMatching(Predicate<Type> typePredicate)
        {
            throw new NotImplementedException();
        }

        public IPartConventionsBuilder ForType(Type type)
        {
            throw new NotImplementedException();
        }

        public IPartBuilder ForInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public IPartBuilder ForInstanceFactory(Type type, Func<ICompositionContext, object> factory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets service collection.
        /// </summary>
        /// <returns>
        /// The service collection.
        /// </returns>
        public IServiceCollection GetServiceCollection() => this.serviceCollection;
    }
}