// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Conventions
{
    using System;

    using global::Autofac;

    using Kephas.Composition.Conventions;

    /// <summary>
    /// An Autofac conventions builder.
    /// </summary>
    public class AutofacConventionsBuilder : IConventionsBuilder, IAutofacContainerBuilderProvider
    {
        private readonly ContainerBuilder containerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConventionsBuilder"/> class.
        /// </summary>
        public AutofacConventionsBuilder()
            : this(new ContainerBuilder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConventionsBuilder"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacConventionsBuilder(ContainerBuilder containerBuilder)
        {
            this.containerBuilder = containerBuilder;
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
        /// Gets the container builder.
        /// </summary>
        /// <returns>
        /// The container builder.
        /// </returns>
        public ContainerBuilder GetContainerBuilder() => this.containerBuilder;
    }
}