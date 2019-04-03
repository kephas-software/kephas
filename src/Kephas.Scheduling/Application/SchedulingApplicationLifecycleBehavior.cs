// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingApplicationLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scheduling application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Runtime;
    using Kephas.Scheduling.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A scheduling application lifecycle behavior.
    /// </summary>
    [OverridePriority(Priority.High - 100)]
    public class SchedulingApplicationLifecycleBehavior : AppLifecycleBehaviorBase
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            RuntimeTypeInfo.RegisterFactory(new SchedulingTypeInfoFactory());

            return base.BeforeAppInitializeAsync(appContext, cancellationToken);
        }
    }
}