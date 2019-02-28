// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseKeyId.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base key identifier class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models.Identifiers
{
    /// <summary>
    /// A base key identifier.
    /// </summary>
    internal abstract class BaseKeyId : BaseId
    {
        public string Name { get; set; }
        public string Group { get; set; }
    }
}