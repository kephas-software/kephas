// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenEntityActivator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate entity activator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities.Activation
{
    using System.Collections.Generic;

    using Kephas.Activation;
    using Kephas.Reflection;

    /// <summary>
    /// A llbl generate entity activator.
    /// </summary>
    public class LLBLGenEntityActivator : ActivatorBase, ILLBLGenEntityActivator
    {
        /// <summary>
        /// The model provider.
        /// </summary>
        private readonly ILLBLGenEntityModelProvider modelProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenEntityActivator"/> class.
        /// </summary>
        /// <param name="entityModelProvider">The entity model provider.</param>
        public LLBLGenEntityActivator(ILLBLGenEntityModelProvider entityModelProvider)
        {
            this.modelProvider = entityModelProvider;
        }

        /// <summary>Gets the supported implementation types.</summary>
        /// <remarks>
        /// The <see cref="T:Kephas.Activation.ActivatorBase" /> class provides always an empty list
        /// of implementation types. The inheritors should provide a proper list of
        /// supported implementation types annotated with <see cref="T:Kephas.Activation.ImplementationForAttribute" />,
        /// otherwise only non-abstract types will be resolved.
        /// </remarks>
        /// <returns>An enumeration of implementation types.</returns>
        protected override IEnumerable<ITypeInfo> GetImplementationTypes()
        {
            return this.modelProvider.GetModelTypeInfos();
        }
    }
}