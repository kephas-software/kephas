// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteAllModelsEntityContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Kephas.DependencyInjection.Metadata;
using Kephas.Entities;
using Kephas.Entities.Tests.Models;

[assembly: GenerateEntityContext("SqliteAllModels", Priority.Low, typeof(Person), typeof(Country), typeof(Contract), typeof(Invoice))]
