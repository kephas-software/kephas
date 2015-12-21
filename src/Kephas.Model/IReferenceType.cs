// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReferenceType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A complex type is a classifier which has as instances compelex values containing of multiple properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    /// <summary>
    /// A reference type represents values that can be identified and referenced.
    /// The identification is based on some property providing (usually) a persistable ID.
    /// </summary>
    /// <remarks>
    /// Reference types could be entities in a database, report or process instances.
    /// Note that they are semantically slightly different from the reference types in programming languages,
    /// meaning that some reference types in programming languages, like <see cref="string"/> will be 
    /// value types instead of reference types.
    /// </remarks>
    public interface IReferenceType : IClassifier
    {
    }
}