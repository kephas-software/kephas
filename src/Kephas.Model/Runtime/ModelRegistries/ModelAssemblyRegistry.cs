namespace Kephas.Model.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Hosting;
    using Kephas.Model.Runtime.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Registry reading the <see cref="ModelAssemblyAttribute"/> and providing the types
    /// exported by the attribute.
    /// </summary>
    public class ModelAssemblyRegistry : IRuntimeModelRegistry
    {
        /// <summary>
        /// The hosting environment.
        /// </summary>
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAssemblyRegistry"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        public ModelAssemblyRegistry(IHostingEnvironment hostingEnvironment)
        {
            Contract.Requires(hostingEnvironment != null);

            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Gets the runtime elements from the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of runtime elements.
        /// </returns>
        public async Task<IEnumerable<object>> GetRuntimeElementsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var assemblies = await this.hostingEnvironment.GetAppAssembliesAsync(cancellationToken).WithServerThreadingContext();
            var eligibleAssemblyPairs =
                (from kv in 
                    from a in assemblies
                    select new KeyValuePair<Assembly, IList<ModelAssemblyAttribute>>(
                            a,
                            a.GetCustomAttributes<ModelAssemblyAttribute>().ToList())
                where kv.Value.Count > 0
                select kv).ToList();

            var types = new HashSet<Type>();
            foreach (var kv in eligibleAssemblyPairs)
            {
                var assembly = kv.Key;

                // first of all add all explicitely given types.
                var attrs = kv.Value;
                foreach (var attr in attrs.Where(attr => attr.ModelTypes != null && attr.ModelTypes.Length > 0))
                {
                    types.AddRange(attr.ModelTypes);
                }

                // then add the types indicated by their namespace.
                var allTypesAttribute = attrs.FirstOrDefault(a => a.ModelTypes == null && a.ModelNamespaces == null);
                if (allTypesAttribute != null)
                {
                    // if no model types or namespaces are indicated, simply add all
                    // exported types from the assembly with no further processing
                    types.AddRange(assembly.GetLoadableExportedTypes());
                }
                else
                {
                    // add only the types from the provided namespaces
                    var allTypes = assembly.GetLoadableExportedTypes().ToList();
                    var namespaces = new HashSet<string>(attrs.Where(a => a.ModelNamespaces != null && a.ModelNamespaces.Length > 0).SelectMany(a => a.ModelNamespaces));
                    var namespacePatterns = namespaces.Select(n => n + ".").ToList();
                    types.AddRange(allTypes.Where(t => namespaces.Contains(t.Namespace) || namespacePatterns.Any(p => t.Namespace.StartsWith(p))));
                }
            }

            return types;
        }
    }
}