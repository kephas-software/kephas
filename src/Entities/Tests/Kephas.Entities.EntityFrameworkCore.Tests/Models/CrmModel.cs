// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmModel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities;
using Kephas.Entities.Tests.Models;

[assembly: GenerateEntityContext("Crm", typeof(Person), typeof(Country))]

namespace Kephas.Entities.Tests.Models;

public abstract record EntityBase : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
}

public record Person : EntityBase
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int Age { get; set; }
}

public record Country : EntityBase
{
    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}