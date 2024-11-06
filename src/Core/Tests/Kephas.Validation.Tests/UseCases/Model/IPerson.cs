// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerson.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.UseCases.Model;

public interface IPerson
{
    public DateTime? Birthday { get; set; }
}
