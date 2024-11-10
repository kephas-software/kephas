namespace Kephas.Model.Tests.Models.ConflictingBaseMembersModel
{
    using Kephas.Model.AttributedModel;

    [Mixin]
    public interface IIdentifiable
    {
        int Id { get; set; }
    }

    [Mixin]
    public interface INamed : IIdentifiable
    {
        string Name { get; set; }
    }

    public class EntityBase : IIdentifiable
    {
        public int Id { get; set; }
    }

    public class NamedEntityBase : EntityBase, INamed
    {
        public string Name { get; set; }
    }
}