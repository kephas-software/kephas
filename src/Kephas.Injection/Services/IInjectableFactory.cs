// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectableFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using System.Diagnostics.CodeAnalysis;

using Kephas.Logging;

/// <summary>
/// Factory of injectable instances.
/// </summary>
[SingletonAppServiceContract]
public interface IInjectableFactory
{
    /// <summary>
    /// Creates an injectable instance.
    /// </summary>
    /// <typeparam name="T">Type of the injectable.</typeparam>
    /// <param name="args">A variable-length parameters list containing arguments.</param>
    /// <returns>
    /// The new injectable instance.
    /// </returns>
    [return: NotNull]
    public T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(params object?[] args)
        where T : class
        => (T)this.Create(typeof(T), args);

    /// <summary>
    /// Creates an injectable instance.
    /// </summary>
    /// <param name="type">Type of the injectable.</param>
    /// <param name="args">A variable-length parameters list containing arguments.</param>
    /// <returns>
    /// The new injectable instance.
    /// </returns>
    public object Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, params object?[] args);

    /// <summary>
    /// Gets the log manager.
    /// </summary>
    /// <returns>
    /// The log manager.
    /// </returns>
    public ILogManager GetLogManager()
    {
        if (this is InjectableFactory factory)
        {
            return factory.LogManager;
        }

        return LoggingHelper.DefaultLogManager;
    }
}