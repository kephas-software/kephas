// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContextFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IContextFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Interface for context factory.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IContextFactory
    {
        /// <summary>
        /// Creates a typed context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The new context.
        /// </returns>
        TContext CreateContext<TContext>(params object[] args)
            where TContext : class;
    }

    /// <summary>
    /// Extension methods for <see cref="IContextFactory"/>.
    /// </summary>
    public static class ContextFactoryExtensions
    {
        private static readonly MethodInfo CreateContextMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((IContextFactory)null).CreateContext<string>((object[])null));

        private static ConcurrentDictionary<IContextFactory, ILogManager> logManagerMap
            = new ConcurrentDictionary<IContextFactory, ILogManager>();

        /// <summary>
        /// Creates a typed context.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The new context.
        /// </returns>
        public static object CreateContext(this IContextFactory contextFactory, Type contextType, params object[] args)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));
            Requires.NotNull(contextType, nameof(contextType));

            var createContext = CreateContextMethod.MakeGenericMethod(contextType);
            return createContext.Call(contextFactory, new object[] { args });
        }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>
        /// The log manager.
        /// </returns>
        public static ILogManager GetLogManager(this IContextFactory contextFactory)
        {
            Requires.NotNull(contextFactory, nameof(contextFactory));

            if (contextFactory is ContextFactory factory)
            {
                return factory.LogManager;
            }

            return logManagerMap.GetOrAdd(contextFactory, _ => contextFactory.CreateContext<Context>()?.AmbientServices.LogManager);
        }
    }
}
