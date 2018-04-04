// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryConversionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query conversion context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    /// <summary>
    /// A client query conversion context.
    /// </summary>
    public class ClientQueryConversionContext : DataOperationContext, IClientQueryConversionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQueryConversionContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public ClientQueryConversionContext(IDataContext dataContext)
            : base(dataContext)
        {
        }
    }
}