// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostedServiceAction.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Hosting;

internal record HostedServiceAction(
    Type Type,
    IList<Type> Dependencies,
    Func<Task> Action,
    string ActionName,
    ILogger Logger)
{
    public bool CanProceed() => Dependencies.Count == 0;

    public async Task Proceed(Func<Type, Task> next)
    {
        try
        {
            await Action();
            await next(Type);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Errors occured during {ServiceAction} of {ServiceType}. Further processing is stopped.", ActionName, Type);
            throw;
        }
    }

    public void NotifyCompleted(IEnumerable<Type> completed)
    {
        foreach (var item in completed)
        {
            Dependencies.Remove(item);
        }
    }
}