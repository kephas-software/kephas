// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoPersistChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements a persist changes command for DTOs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.DTO
{
    using Kephas.Data.Commands;

    /// <summary>
    /// A persist changes command for DTOs.
    /// </summary>
    [DataContextType(typeof(DtoDataContext))]
    public class DtoPersistChangesCommand : PersistChangesCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DtoPersistChangesCommand"/> class.
        /// </summary>
        /// <remarks>
        /// The DTO entities do not need behaviors for persistence.
        /// </remarks>
        public DtoPersistChangesCommand()
            : base(new DtoDataContext.NoneDataBehaviorProvider())
        {
        }
    }
}