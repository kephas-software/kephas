// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceCodeNotSupportedException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the source code not supported exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using System;

    using Kephas.Scripting.Resources;

    /// <summary>
    /// Exception for signalling source code not supported errors.
    /// </summary>
    public class SourceCodeNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeNotSupportedException"/> class.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="supportedCodeTypes">A variable-length parameters list containing supported code
        ///                                  types.</param>
        public SourceCodeNotSupportedException(IScript script, params Type[] supportedCodeTypes)
            : base(GetMessage(script, supportedCodeTypes))
        {
        }

        private static string GetMessage(IScript script, Type[] supportedCodeTypes)
        {
            return Strings.SourceCodeTypeNotSupported_Exception.FormatWith(script?.SourceCode?.GetType(), supportedCodeTypes.JoinWith(", "));
        }
    }
}
