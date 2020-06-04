// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementConfiguratorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base configurator for model elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Configuration;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Resources;
    using Kephas.Reflection;

    /// <summary>
    /// Base configurator for model elements.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TRuntimeElement">Type of the runtime element.</typeparam>
    /// <typeparam name="TConfigurator">The type of the configurator.</typeparam>
    public abstract class RuntimeModelElementConfiguratorBase<TElement, TRuntimeElement, TConfigurator> : IRuntimeModelElementConfigurator<TElement, TRuntimeElement>
        where TElement : INamedElement
        where TConfigurator : IRuntimeModelElementConfigurator<TElement, TRuntimeElement>
    {
        /// <summary>
        /// The configs.
        /// </summary>
        private readonly IList<Action<IModelConstructionContext, TElement>> configs = new List<Action<IModelConstructionContext, TElement>>();

        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        void IElementConfigurator.Configure(IModelConstructionContext constructionContext, INamedElement element)
        {
            this.Configure(constructionContext, (TElement)element);
        }

        /// <summary>
        /// Configures the model element provided.
        /// </summary>
        /// <param name="constructionContext">The construction context.</param>
        /// <param name="element">The model element to be configured.</param>
        public virtual void Configure(IModelConstructionContext constructionContext, TElement element)
        {
            foreach (var config in this.configs)
            {
                config(constructionContext, element);
            }
        }

        /// <summary>
        /// Adds a member to the configured element.
        /// </summary>
        /// <param name="member">The member to be added.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        public virtual TConfigurator AddMember(INamedElement member)
        {
            Requires.NotNull(member, nameof(member));

            this.AddConfiguration((context, element) =>
                {
                    if (element is IConstructibleElement constructibleElement)
                    {
                        constructibleElement.AddMember(member);
                    }
                    else
                    {
                        throw new NonConstructibleElementException(element);
                    }
                });

            return (TConfigurator)(object)this;
        }

        /// <summary>
        /// Adds a member out of the runtime element to the configured model element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// This configurator.
        /// </returns>
        public virtual TConfigurator AddMember(object runtimeElement)
        {
            Requires.NotNull(runtimeElement, nameof(runtimeElement));

            if (runtimeElement is INamedElement namedElement)
            {
                return this.AddMember(namedElement);
            }

            this.AddConfiguration((context, element) =>
                {
                    if (element is IConstructibleElement constructibleElement)
                    {
                        var annotation = context.RuntimeModelElementFactory.TryCreateModelElement(context, runtimeElement);
                        if (annotation == null)
                        {
                            throw new ModelConstructionException(string.Format(Strings.RuntimeModelElementConfiguratorBase_AddAttribute_CannotCreateModelElement_Exception, runtimeElement.GetType()));
                        }

                        constructibleElement.AddMember(annotation);
                    }
                    else
                    {
                        throw new NonConstructibleElementException(element);
                    }
                });

            return (TConfigurator)(object)this;
        }

        /// <summary>
        /// Adds an attribute to the configured element.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// A TConfigurator.
        /// </returns>
        public virtual TConfigurator AddAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var attr = Activator.CreateInstance(typeof(TAttribute));
            return this.AddMember(attr);
        }

        /// <summary>
        /// Adds a member to the configured element.
        /// </summary>
        /// <param name="member">The member to be added.</param>
        /// <returns>
        /// An IRuntimeModelElementConfigurator.
        /// </returns>
        IRuntimeModelElementConfigurator IRuntimeModelElementConfigurator.AddMember(INamedElement member)
        {
            return this.AddMember(member);
        }

        /// <summary>
        /// Adds a member out of the runtime element to the configured model element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// An IRuntimeModelElementConfigurator.
        /// </returns>
        IRuntimeModelElementConfigurator IRuntimeModelElementConfigurator.AddMember(object runtimeElement)
        {
            return this.AddMember(runtimeElement);
        }

        /// <summary>
        /// Adds an element configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        protected void AddConfiguration(Action<IModelConstructionContext, TElement> config)
        {
            this.configs.Add(config);
        }
    }
}