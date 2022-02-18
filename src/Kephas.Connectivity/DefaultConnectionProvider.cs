// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConnectionProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Kephas.Logging;
using Kephas.Services;

namespace Kephas.Connectivity;

/// <summary>
/// The default connection provider.
/// </summary>
/// <seealso cref="IConnectionProvider" />
public class DefaultConnectionProvider : Loggable, IConnectionProvider
{
    private readonly IDictionary<string, Lazy<IConnectionFactory, ConnectionFactoryMetadata>> factoryFactories =
        new Dictionary<string, Lazy<IConnectionFactory, ConnectionFactoryMetadata>>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultConnectionProvider"/> class.
    /// </summary>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="factoryFactories">The factory factories.</param>
    public DefaultConnectionProvider(
        IContextFactory contextFactory,
        ICollection<Lazy<IConnectionFactory, ConnectionFactoryMetadata>> factoryFactories)
        : base(contextFactory)
    {
        contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        factoryFactories = factoryFactories ?? throw new ArgumentNullException(nameof(factoryFactories));

        this.ContextFactory = contextFactory;

        factoryFactories
            .Order()
            .ForEach(f =>
            {
                f.Metadata.ConnectionKind.ForEach(
                        l =>
                        {
                            if (!this.factoryFactories.ContainsKey(l))
                            {
                                this.factoryFactories.Add(l, f);
                            }
                        });
            });
    }

    /// <summary>
    /// Gets the context factory.
    /// </summary>
    /// <value>
    /// The context factory.
    /// </value>
    protected IContextFactory ContextFactory { get; }

    /// <summary>
    /// Creates the connection configured through the connection options.
    /// </summary>
    /// <param name="options">The options for connection configuration.</param>
    /// <returns>
    /// The newly created connection.
    /// </returns>
    public IConnection CreateConnection(Action<IConnectionContext> options)
    {
        options = options ?? throw new ArgumentNullException(nameof(options));

        var context = this.CreateConnectionContext(options);
        var factory = this.SelectConnectionFactory(context);

        return factory.Value.CreateConnection(context);
    }

    /// <summary>
    /// Selects the connection factory based on the arguments set in the context.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <returns>
    /// The <see cref="IConnectionFactory"/> together with its metadata, or <c>null</c> if not found.
    /// </returns>
    protected virtual Lazy<IConnectionFactory, ConnectionFactoryMetadata> SelectConnectionFactory(IConnectionContext connectionContext)
    {
        var connectionKind = connectionContext.Kind ?? connectionContext.Host?.Scheme;
        var factory = connectionKind == null ? null : this.factoryFactories.TryGetValue(connectionKind);
        return factory
            ?? throw new ConnectivityException($"Cannot create a connection for '{connectionKind}'");
    }

    /// <summary>
    /// Creates the scripting context.
    /// </summary>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <returns>
    /// A new instance implementing <see cref="IConnectionContext"/>.
    /// </returns>
    protected virtual IConnectionContext CreateConnectionContext(Action<IConnectionContext>? optionsConfig = null)
    {
        var context = this.ContextFactory.CreateContext<ConnectionContext>();

        optionsConfig?.Invoke(context);
        return context;
    }
}
