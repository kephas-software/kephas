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
    using System.Collections.Generic;

    /// <summary>
    /// A base implementtion for contexts.
    /// </summary>
    public abstract class ContextBase : IContext
    {
        /// <summary>
        /// The custom values.
        /// </summary>
        private readonly IDictionary<string, object> customValues = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="System.Object"/> with the specified key.</returns>
        public virtual object this[string key]
        {
            get
            {
                object value;
                if (this.customValues.TryGetValue(key, out value))
                {
                    return value;
                }

                return null;
            }

            set
            {
                this.customValues[key] = value;
            }
        }
    }
}