using Kephas.Runtime;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Kephas.Web.Mvc;

/// <summary>
/// The default implementation of the <see cref="IApplicationPartsProvider"/> service.
/// </summary>
public class ApplicationPartsProvider(IAppRuntime appRuntime) : IApplicationPartsProvider
{
    /// <summary>
    /// Gets the application parts.
    /// </summary>
    /// <returns>An enumeration of application parts.</returns>
    public IEnumerable<ApplicationPart> GetApplicationParts() =>
        appRuntime.GetAppAssemblies().Select(a => new AssemblyPart(a));
}