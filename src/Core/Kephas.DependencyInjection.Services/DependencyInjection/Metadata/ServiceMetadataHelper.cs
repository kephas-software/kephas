namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Helper class for service metadata.
/// </summary>
public static class ServiceMetadataHelper
{
    /// <summary>
    /// Gets the <see cref="IServiceMetadataProvider"/> for the <see paramref="genericType"/>.
    /// </summary>
    /// <param name="genericType">The generic type.</param>
    /// <returns>An instance of <see cref="IServiceMetadataProvider"/>.</returns>
    public static IServiceMetadataProvider GetGenericTypeMetadataProvider(Type genericType)
        => GenericTypeServiceMetadataProvider.GetProvider(genericType);
}