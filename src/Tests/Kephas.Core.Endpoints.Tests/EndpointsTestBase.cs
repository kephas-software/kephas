namespace Kephas.Core.Endpoints.Tests;

using System.Reflection;
using Kephas.Services.Builder;
using Kephas.Testing;

public abstract class EndpointsTestBase : TestBase
{
    protected IServiceProvider BuildServiceProvider(
        IEnumerable<Assembly>? assemblies = null,
        IEnumerable<Type>? parts = null)
    {
        var builder = this.CreateServicesBuilder();
        if (assemblies is not null)
        {
            builder.WithAssemblies(assemblies);
        }

        if (parts is not null)
        {
            builder.WithParts(parts);
        }

        return builder.BuildWithDependencyInjection();
    }
}