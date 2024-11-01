using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Kephas.Web.Mvc;

/// <summary>
/// Contract for a provider of application parts.
/// </summary>
public interface IApplicationPartsProvider
{
    /// <summary>
    /// Gets the application parts.
    /// </summary>
    /// <returns>An enumeration of application parts.</returns>
    IEnumerable<ApplicationPart> GetApplicationParts();
}