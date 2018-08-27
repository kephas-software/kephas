// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementNotFoundException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Exception indicating that a requested element was not found.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System;

    using Kephas.Model.Resources;

    /// <summary>
    /// Exception indicating that a requested element was not found.
    /// </summary>
    public class ElementNotFoundException : ModelException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotFoundException"/> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="containerName">Name of the container.</param>
        public ElementNotFoundException(string memberName, string containerName)
            : base(GetMemberNotFoundMessage(memberName, containerName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotFoundException" /> class.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="inner">The inner exception.</param>
        public ElementNotFoundException(string memberName, string containerName, Exception inner)
            : base(GetMemberNotFoundMessage(memberName, containerName), inner)
        {
        }

        private static string GetMemberNotFoundMessage(string memberName, string containerName)
        {
            return string.Format(Strings.ElementNotFoundInMembers, memberName, containerName);
        }
    }
}