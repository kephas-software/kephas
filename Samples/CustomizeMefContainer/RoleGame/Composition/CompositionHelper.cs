namespace RoleGame.Composition
{
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Reflection;

    public static class CompositionHelper
    {
        public static IConventionsBuilder GetConventions(ITypeLoader typeLoader)
        {
            var registrars =
                typeLoader.GetLoadableExportedTypes(Assembly.GetExecutingAssembly())
                    .Where(t => typeof(IConventionRegistrar).IsAssignableFrom(t) && t.IsClass)
                    .Select(t => (IConventionRegistrar)t.AsRuntimeTypeInfo().CreateInstance())
                    .ToList();

            var conventions = new MefConventionsBuilder();
            var mefConventions = conventions.GetConventionBuilder();
            registrars.ForEach(r => r.RegisterConventions(mefConventions));

            return conventions;
        }
    }
}