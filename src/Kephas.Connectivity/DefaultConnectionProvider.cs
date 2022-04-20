// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConnectionProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

using Kephas.Collections;
using Kephas.Connectivity.Behaviors;
using Kephas.Injection;
using Kephas.Logging;
using Kephas.Security.Authentication;
using Kephas.Services;

/// <summary>
/// The default connection provider.
/// </summary>
/// <seealso cref="IConnectionProvider" />
public class DefaultConnectionProvider : Loggable, IConnectionProvider
{
    private readonly IDictionary<string, Lazy<IConnectionFactory, ConnectionFactoryMetadata>> factoryFactories =
        new Dictionary<string, Lazy<IConnectionFactory, ConnectionFactoryMetadata>>(StringComparer.OrdinalIgnoreCase);

    private readonly ICollection<IExportFactory<IConnectionCreateBehavior, ConnectionCreateBehaviorMetadata>> behaviorFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultConnectionProvider"/> class.
    /// </summary>
    /// <param name="injectableFactory">The injectable factory.</param>
    /// <param name="factoryFactories">The factory factories.</param>
    /// <param name="behaviorFactories">Optional. The behavior factories.</param>
    public DefaultConnectionProvider(
        IInjectableFactory injectableFactory,
        ICollection<Lazy<IConnectionFactory, ConnectionFactoryMetadata>> factoryFactories,
        ICollection<IExportFactory<IConnectionCreateBehavior, ConnectionCreateBehaviorMetadata>>? behaviorFactories = null)
        : base(injectableFactory)
    {
        injectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
        factoryFactories = factoryFactories ?? throw new ArgumentNullException(nameof(factoryFactories));

        this.behaviorFactories = behaviorFactories?.Order().ToList()
                                 ?? new List<IExportFactory<IConnectionCreateBehavior, ConnectionCreateBehaviorMetadata>>();

        this.InjectableFactory = injectableFactory;

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
    protected IInjectableFactory InjectableFactory { get; }

    /// <summary>
    /// Creates the connection configured through the connection options.
    /// </summary>
    /// <param name="host">The host URI.</param>
    /// <param name="credentials">Optional. The credentials. Although typically required, not all connections need credentials.</param>
    /// <param name="kind">Optional. The connection kind. If not provided, the host scheme is used.</param>
    /// <param name="options">Optional. Other options for connection configuration.</param>
    /// <returns>
    /// The newly created connection.
    /// </returns>
    public IConnection CreateConnection(Uri host, ICredentials? credentials = null, string? kind = null, Action<IConnectionContext>? options = null)
    {
        // do not dispose the context at this level, some connections need it.
        var context = this.CreateConnectionContext(host, credentials, kind, options);
        var factory = this.SelectConnectionFactory(context);

        var behaviors = this.SelectConnectionCreateBehaviors(context).ToList();
        try
        {
            behaviors.ForEach(b => b.BeforeCreate(context));
            context.Connection = factory.Value.CreateConnection(context);
        }
        catch (Exception ex)
        {
            context.Exception = ex;
        }

        behaviors.Reverse();
        behaviors.ForEach(b => b.AfterCreate(context));

        return context.Connection ?? throw context.Exception ?? new ConnectivityException("Connection could not be created.");
    }

    /// <summary>
    /// Selects the connection factory based on the arguments set in the context.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <returns>
    /// The <see cref="IConnectionFactory"/> together with its metadata, or <c>null</c> if not found.
    /// </returns>
    protected virtual IEnumerable<IConnectionCreateBehavior> SelectConnectionCreateBehaviors(IConnectionContext connectionContext)
    {
        var connectionKind = this.GetConnectionKind(connectionContext);
        var factories = connectionKind == null
            ? Enumerable.Empty<IConnectionCreateBehavior>()
            : this.behaviorFactories
                .Where(f => f.Metadata.ConnectionKind.Length == 0 || f.Metadata.ConnectionKind.Contains(connectionKind))
                .Select(f => f.CreateExportedValue());
        return factories;
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
        var connectionKind = this.GetConnectionKind(connectionContext);
        var factory = connectionKind == null ? null : this.factoryFactories.TryGetValue(connectionKind);
        return factory
            ?? throw new ConnectivityException($"Cannot create a connection for '{connectionKind}'");
    }

    /// <summary>
    /// Tries to get the connection kind.
    /// </summary>
    /// <param name="connectionContext">The connection context.</param>
    /// <returns>The connection kind or <c>null</c>.</returns>
    protected virtual string? GetConnectionKind(IConnectionContext connectionContext)
    {
        return connectionContext.Kind ?? connectionContext.Host?.Scheme;
    }

    /// <summary>
    /// Creates the scripting context.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="kind">The kind.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <returns>
    /// A new instance implementing <see cref="IConnectionContext" />.
    /// </returns>
    protected virtual IConnectionContext CreateConnectionContext(Uri host, ICredentials? credentials = null, string? kind = null, Action<IConnectionContext>? optionsConfig = null)
    {
        var context = this.InjectableFactory.Create<ConnectionContext>();

        context.Host = host;
        context.Credentials = credentials;
        context.Kind = kind ?? host.Scheme;

        optionsConfig?.Invoke(context);
        return context;
    }
}
