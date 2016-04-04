// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerson.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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