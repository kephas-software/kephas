namespace Kephas.Data.Model.Tests.Models.KeyInheritanceModel
{
    using System;

    using Kephas.Data.Model.AttributedModel;

    [Key(new[] { nameof(Name) })]
    public interface IUniqueName
    {
        string Name { get; set; }
    }

    [NaturalKey(new[] { nameof(Guid) })]
    public interface IUniqueGuid : IUniqueName
    {
        Guid Guid { get; set; }
    }

    [Entity]
    public interface IPlugin : IUniqueGuid
    {
    }
}
