// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpArgsModelFinalizingConvention.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.StoredProcedures;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Kephas.Entities.Conventions;

/// <summary>
/// Convention for marking <see cref="ISpArgs"/> keyless.
/// </summary>
public class SpArgsModelFinalizingConvention : IModelFinalizingConvention
{
    /// <summary>Called when a model is being finalized.</summary>
    /// <param name="modelBuilder">The builder for the model.</param>
    /// <param name="context">Additional information associated with convention execution.</param>
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes()
                     .Where(et => typeof(ISpArgs).IsAssignableFrom(et.ClrType)))
        {
            entityType.SetIsKeyless(true);
        }
    }
}