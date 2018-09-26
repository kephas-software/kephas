namespace Kephas.Model.Tests.Models.ConfiguratorModel
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Model.Runtime.Configuration;

    public interface INamed
    {
        string Name { get; set; }
    }

    public interface IDerivedNamed : INamed
    {
    }

    public interface IOverrideName : INamed
    {
        new string Name { get; set; }
    }

    public interface IOtherNamed
    {
        string Name { get; set; }
    }

    public class NamedConfigurator : ClassifierConfigurator<INamed>
    {
        public NamedConfigurator()
        {
            this.WithProperty(nameof(INamed.Name), cfg => cfg.AddAttribute<RequiredAttribute>());
        }
    }

    public class DerivedNamedConfigurator : ClassifierConfigurator<IDerivedNamed>
    {
        public DerivedNamedConfigurator()
        {
            this.WithProperty(nameof(IDerivedNamed.Name), cfg => cfg.AddAttribute<RequiredAttribute>());
        }
    }
}