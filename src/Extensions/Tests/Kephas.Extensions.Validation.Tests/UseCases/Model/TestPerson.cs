// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestPerson.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Kephas.Extensions.Tests.UseCases.Model;

public record TestPerson(string Name, string Email, int Age, DateTime? Birthday) : IPerson
{
    [MinLength(2)]
    public string Name { get; set; } = Name;

    [EmailAddress]
    public string Email { get; set; } = Email;

    public int Age { get; set; } = Age;

    public DateTime? Birthday { get; set; } = Birthday;
};
