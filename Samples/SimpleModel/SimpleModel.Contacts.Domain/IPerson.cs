// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerson.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a physical person.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleModel.Contacts.Domain
{
    /// <summary>
    /// Defines a physical person.
    /// </summary>
    public interface IPerson : IContact
    {
        string PersonalUniqueNumber { get; set; }
    }
}