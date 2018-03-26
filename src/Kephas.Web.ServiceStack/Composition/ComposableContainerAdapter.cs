// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposableContainerAdapter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the composable container adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Composition
{
    using Kephas.Composition;

    /// <summary>
    ///     A composable container adapter.
    /// </summary>
    public class ComposableContainerAdapter : IComposableContainerAdapter
    {
        /// <summary>
        /// The composition context.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComposableContainerAdapter" /> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public ComposableContainerAdapter(ICompositionContext compositionContext)
        {
            this.compositionContext = compositionContext;
        }

        /// <summary>
        ///     Tries to resolve the service with the provided type.
        /// </summary>
        /// <typeparam name="T">The part service type.</typeparam>
        /// <returns>
        ///     The service or <c>null</c>.
        /// </returns>
        public T TryResolve<T>()
        {
            return this.compositionContext.TryGetExport<T>();
        }

        /// <summary>
        ///     Resolve the service with the provided type.
        /// </summary>
        /// <typeparam name="T">The part service type.</typeparam>
        /// <returns>
        ///     The service.
        /// </returns>
        public T Resolve<T>()
        {
            return this.compositionContext.GetExport<T>();
        }
    }
}
