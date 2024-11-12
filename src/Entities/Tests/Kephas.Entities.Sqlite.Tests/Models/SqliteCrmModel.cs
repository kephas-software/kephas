// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteCrmModel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Kephas.Entities;
using Kephas.Entities.Tests.Models;

[assembly: GenerateEntityContext("SqliteCrm", typeof(Person), typeof(Country))]

namespace Kephas.Entities.Tests.Models;

public abstract record EntityBase : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
}

public record Person : EntityBase
{
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [MaxLength(2)]
    public string? Initials { get; set; }

    [Range(0, 150, MinimumIsExclusive = false)]
    public int Age { get; set; }
}

public record Country : EntityBase
{
    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}