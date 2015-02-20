// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A base implementtion for contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Dynamic;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public abstract class ContextBase : IContext
    {
        /// <summary>
        /// The custom values.
        /// </summary>
        private readonly dynamic data = new ExpandoObject();

        /// <summary>
        /// Gets the custom values.
        /// </summary>
        /// <value>
        /// The custom values.
        /// </value>
        public dynamic Data
        {
            get { return this.data; }
        }
    }
}