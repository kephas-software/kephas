// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableTestEntity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the localizable test entity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
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