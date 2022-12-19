// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullServiceException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Exception thrown by null services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Services.Resources;

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
            : base(string.Format(AbstractionsStrings.NullServiceExceptionMessage, serviceType.FullName))
        {
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="inner">The inner exception.</param>
        public NullServiceException(Type serviceType, Exception inner)
            : base(string.Format(AbstractionsStrings.NullServiceExceptionMessage, serviceType.FullName), inner)
        {
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException" /> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        public NullServiceException(object serviceInstance)
            : base(string.Format(
                AbstractionsStrings.NullServiceExceptionMessage,
                (serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance))).GetType().FullName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullServiceException" /> class.
        /// </summary>
        /// <param name="serviceInstance">The service instance.</param>
        /// <param name="inner">The inner exception.</param>
        public NullServiceException(object serviceInstance, Exception inner)
            : base(
                string.Format(
                    AbstractionsStrings.NullServiceExceptionMessage,
                    (serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance))).GetType().FullName),
                inner)
        {
        }
    }
}