// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scheduling type information factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Runtime
{
    using System;

    using Kephas.Runtime;
    using Kephas.Runtime.Factories;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Triggers;

    /// <summary>
    /// A scheduling type information factory.
    /// </summary>
    public class SchedulingTypeInfoFactory : RuntimeTypeInfoFactoryBase
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="args">Additional arguments.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public override IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, params object[] args)
        {
            var type = reflectInfo;
            if (typeof(IJob).IsAssignableFrom(type))
            {
                return new RuntimeJobInfo(registry, type);
            }

            if (typeof(ITrigger).IsAssignableFrom(type))
            {
                return new RuntimeTriggerInfo(registry, type);
            }

            return null;
        }
    }
}