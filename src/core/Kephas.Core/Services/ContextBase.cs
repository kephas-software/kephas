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
    using System.Linq.Expressions;
    using System.Security.Principal;

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

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        /// <value>
        /// The authenticated user.
        /// </value>
        public IIdentity AuthenticatedUser { get; set; }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return ((IDynamicMetaObjectProvider)this.Data).GetMetaObject(parameter);
        }
    }
}