// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableTestEntity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the localizable test entity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The localizable test entity.
    /// </summary>
    [Display(Name = "LocalizableTestEntity-Name", Description = "LocalizableTestEntity-Description")]
    public class LocalizableTestEntityWithOverloads : LocalizableTestEntity
    {
        [Display(Name = "New-Id-Name", ShortName = "New-Id-ShortName", Prompt = "New-Id-Prompt", Description = "New-Id-Description")]
        public new int Id { get; set; }
    }
}