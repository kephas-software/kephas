// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataBehaviorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataBehaviorProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for providing data behaviors.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataBehaviorProvider
    {
        /// <summary>
        /// Gets the data behaviors of type <typeparamref name="TBehavior"/> for the provided type.
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// An enumeration of behaviors mathing the provided type.
        /// </returns>
        IEnumerable<TBehavior> GetDataBehaviors<TBehavior>(Type type);
    }
}