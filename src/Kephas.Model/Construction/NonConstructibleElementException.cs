// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonConstructibleElementException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the non constructible element exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction
{
    using System;

    using Kephas.Model.Resources;

    /// <summary>
    /// Exception for signalling non-constructible element errors.
    /// </summary>
    public class NonConstructibleElementException : ModelConstructionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonConstructibleElementException"/> class.
        /// </summary>
        /// <param name="element">The non-constructible element.</param>
        public NonConstructibleElementException(INamedElement element)
            : this(element, GetNonConstructibleElementDefaultMessage(element))
        {
            this.Element = element;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonConstructibleElementException"/> class.
        /// </summary>
        /// <param name="element">The non-constructible element.</param>
        /// <param name="message">The message.</param>
        public NonConstructibleElementException(INamedElement element, string message)
            : base(message)
        {
            this.Element = element;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonConstructibleElementException"/> class.
        /// </summary>
        /// <param name="element">The non-constructible element.</param>
        /// <param name="inner">The inner exception.</param>
        public NonConstructibleElementException(INamedElement element, Exception inner)
            : this(element, GetNonConstructibleElementDefaultMessage(element), inner)
        {
            this.Element = element;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonConstructibleElementException"/> class.
        /// </summary>
        /// <param name="element">The non-constructible element.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public NonConstructibleElementException(INamedElement element, string message, Exception inner)
            : base(message, inner)
        {
            this.Element = element;
        }

        /// <summary>
        /// Gets the non-constructible element.
        /// </summary>
        /// <value>
        /// The non-constructible element.
        /// </value>
        public INamedElement Element { get; }

        /// <summary>
        /// Gets non constructible element default message.
        /// </summary>
        /// <param name="element">The non-constructible element.</param>
        /// <returns>
        /// The non constructible element default message.
        /// </returns>
        private static string GetNonConstructibleElementDefaultMessage(INamedElement element)
        {
            return string.Format(Strings.NonConstructibleElementException_Message, element.GetType());
        }
    }
}