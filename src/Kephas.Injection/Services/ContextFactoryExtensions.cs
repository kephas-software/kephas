// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFactoryExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IContextFactory"/>.
    /// </summary>
    public static class ContextFactoryExtensions
    {
        private static readonly MethodInfo CreateContextMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((IContextFactory)null!).CreateContext<string>((object?[])null!));

        /// <summary>
        /// Creates a typed context.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The new context.
        /// </returns>
        public static object CreateContext(this IContextFactory contextFactory, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contextType, params object?[] args)
        {
            contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            contextType = contextType ?? throw new ArgumentNullException(nameof(contextType));

            var createContext = CreateContextMethod.MakeGenericMethod(contextType);
            return createContext.Call(contextFactory, new object[] { args })!;
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
            contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

            if (contextFactory is ContextFactory factory)
            {
                return factory.LogManager;
            }

            return LoggingHelper.DefaultLogManager;
        }
    }
}