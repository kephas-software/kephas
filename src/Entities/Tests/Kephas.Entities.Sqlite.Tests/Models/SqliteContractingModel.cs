// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteCrmModel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Kephas.Entities;
using Kephas.Entities.Tests.Models;

[assembly: GenerateEntityContext("SqliteContracting", typeof(Contract), typeof(Invoice))]

namespace Kephas.Entities.Tests.Models;

public record Contract : EntityBase
{
    [Required]
    public string Number { get; set; } = null!;

    [Required]
    public string Customer { get; set; } = null!;
}

public record Invoice : EntityBase
{
    public int Number { get; set; }

    [Range(0, int.MaxValue, MinimumIsExclusive = true)]
    public decimal Value { get; set; }
}