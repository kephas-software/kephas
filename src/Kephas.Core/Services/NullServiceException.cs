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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;

    /// <summary>
    /// Exception thrown by null services.
    /// </summary>
    public class NullServiceException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        public NullServiceException(Type serviceType)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceType.FullName))
        {
            Requires.NotNull(serviceType, nameof(serviceType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="inner">The inner exception.</param>
        public NullServiceException(Type serviceType, Exception inner)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceType.FullName), inner)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException" /> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        public NullServiceException(object serviceInstance)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceInstance.GetType().FullName))
        {
            Requires.NotNull(serviceInstance, nameof(serviceInstance));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException" /> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="inner">The inner exception.</param>
        public NullServiceException(object serviceInstance, Exception inner)
            : base(string.Format(Strings.NullServiceExceptionMessage, serviceInstance.GetType().FullName), inner)
        {
            Requires.NotNull(serviceInstance, nameof(serviceInstance));
        }
    }
}