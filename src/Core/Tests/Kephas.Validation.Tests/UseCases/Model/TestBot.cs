// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBot.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.UseCases.Model;

public record TestBot(string? Producer, DateTime? Birthday) : IPerson
{
    public string? Producer { get; set; } = Producer;
    public DateTime? Birthday { get; set; } = Birthday;
}
