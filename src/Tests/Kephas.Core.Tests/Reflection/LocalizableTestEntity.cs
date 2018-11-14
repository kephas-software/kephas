// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableTestEntity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the localizable test entity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;

    /// <summary>
    /// The localizable test entity.
    /// </summary>
    [TypeDisplay(Name = "LocalizableTestEntity-Name", Description = "LocalizableTestEntity-Description")]
    public class LocalizableTestEntity
    {
        [Display(Name = "Id-Name", ShortName = "Id-ShortName", Prompt = "Id-Prompt", Description = "Id-Description")]
        public int Id { get; set; }
    }
}