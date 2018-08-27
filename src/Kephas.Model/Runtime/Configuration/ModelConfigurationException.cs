// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelConfigurationException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model configuration exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Configuration
{
    using System;

    /// <summary>
    /// Exception for signalling model configuration errors.
    /// </summary>
    public class ModelConfigurationException : ModelException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConfigurationException"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="messageTemplate">The message template.</param>
        public ModelConfigurationException(string memberName, string containerName, string messageTemplate)
            : base(GetMemberNotFoundMessage(memberName, containerName, messageTemplate))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConfigurationException"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="inner">The inner exception.</param>
        public ModelConfigurationException(string memberName, string containerName, string messageTemplate, Exception inner)
            : base(GetMemberNotFoundMessage(memberName, containerName, messageTemplate), inner)
        {
        }

        private static string GetMemberNotFoundMessage(string memberName, string containerName, string messageTemplate)
        {
            return string.Format(messageTemplate, memberName, containerName);
        }
    }
}