// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullServiceException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Exception thrown by null services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Resources;

    /// <summary>
    /// Exception thrown by null services.
    /// </summary>
    public class NullServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        public NullServiceException(Type serviceType)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceType.FullName))
        {
            Contract.Requires(serviceType != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="inner">The inner exception.</param>
        public NullServiceException(Type serviceType, Exception inner)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceType.FullName), inner)
        {
            Contract.Requires(serviceType != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException" /> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        public NullServiceException(object serviceInstance)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceInstance.GetType().FullName))
        {
            Contract.Requires(serviceInstance != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException" /> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="inner">The inner exception.</param>
        public NullServiceException(object serviceInstance, Exception inner)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceInstance.GetType().FullName), inner)
        {
            Contract.Requires(serviceInstance != null);
        }
    }
}