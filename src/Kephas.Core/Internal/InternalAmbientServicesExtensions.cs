// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the internal ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
{
    using Kephas.Composition;

    internal static class InternalAmbientServicesExtensions
    {
        internal static ICompositionContext AsCompositionContext(this IAmbientServices ambientServices)
        {
            const string AsCompositionContextKey = "__AsCompositionContext";
            if (ambientServices[AsCompositionContextKey] is ICompositionContext compositionContext)
            {
                return compositionContext;
            }

            compositionContext = ambientServices.ToCompositionContext();
            ambientServices[AsCompositionContextKey] = compositionContext;
            return compositionContext;
        }

    }
}