// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Model;

using System.Transactions;
using Kephas.Services.Builder;
using Kephas.Model;
using Kephas.Model.Runtime;
using Kephas.Model.Runtime.Configuration;
using Kephas.Services;
using NSubstitute;

/// <summary>
/// Extension methods for testing models.
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Gets the model registry for the provided element types.
    /// </summary>
    /// <param name="elements">The elements.</param>
    /// <returns>The <see cref="IRuntimeModelRegistry"/>.</returns>
    public static IRuntimeModelRegistry GetModelRegistry(this IEnumerable<Type> elements)
    {
        var registry = Substitute.For<IRuntimeModelRegistry>();
        registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<object>>(elements));
        return registry;
    }

    /// <summary>
    /// Adds the model elements to the <see cref="IAmbientServices"/> as a new <see cref="IRuntimeModelRegistry"/>.
    /// </summary>
    /// <param name="servicesBuilder">The service collection builder.</param>
    /// <param name="elements">The elements.</param>
    /// <returns>The provided service collection builder.</returns>
    public static IAppServiceCollectionBuilder WithModelElements(this IAppServiceCollectionBuilder servicesBuilder, IEnumerable<Type> elements)
    {
        servicesBuilder.AmbientServices.Add(_ => elements.GetModelRegistry(), b => b.Singleton().AllowMultiple());

        return servicesBuilder;
    }

    /// <summary>
    /// Adds the model elements to the <see cref="IAmbientServices"/> as a new <see cref="IRuntimeModelRegistry"/>.
    /// </summary>
    /// <param name="servicesBuilder">The service collection builder.</param>
    /// <param name="elements">The elements.</param>
    /// <returns>The provided service collection builder.</returns>
    public static IAppServiceCollectionBuilder WithModelElements(this IAppServiceCollectionBuilder servicesBuilder, params Type[] elements)
        => WithModelElements(servicesBuilder, (IEnumerable<Type>)elements);

    /// <summary>
    /// Adds the model element configurator to the <see cref="IAmbientServices"/>.
    /// </summary>
    /// <typeparam name="TConfigurator">The configurator type.</typeparam>
    /// <param name="servicesBuilder">The service collection builder.</param>
    /// <returns>The provided service collection builder.</returns>
    public static IAppServiceCollectionBuilder WithModelElementConfigurator<TConfigurator>(this IAppServiceCollectionBuilder servicesBuilder)
        where TConfigurator : IRuntimeModelElementConfigurator
    {
        servicesBuilder.Providers.Add(new ConfiguratorAppServiceInfoProvider<TConfigurator>());

        return servicesBuilder;
    }

    private class ConfiguratorAppServiceInfoProvider<TConfigurator> : IAppServiceInfoProvider
        where TConfigurator : IRuntimeModelElementConfigurator
    {
        public IEnumerable<ServiceDeclaration> GetAppServices()
        {
            yield return new ServiceDeclaration(
                typeof(TConfigurator),
                this.GetInterfaceType());
        }

        private Type GetInterfaceType()
        {
            return typeof(TConfigurator)
                .GetInterfaces()
                .Single(t =>
                    t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IRuntimeModelElementConfigurator<,>));
        }
    }
}