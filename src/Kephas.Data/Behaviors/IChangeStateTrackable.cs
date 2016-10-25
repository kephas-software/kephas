namespace Kephas.Data.Behaviors
{
    /// <summary>
    /// Enumerates the entity change state.
    /// </summary>
    public enum ChangeState
    {
        /// <summary>
        /// The entity is not changed.
        /// </summary>
        NotChanged,

        /// <summary>
        /// The entity is added.
        /// </summary>
        Added,

        /// <summary>
        /// The entity is changed.
        /// </summary>
        Changed,

        /// <summary>
        /// The entity is added or updated.
        /// </summary>
        AddedOrChanged,

        /// <summary>
        /// The entity is deleted.
        /// </summary>
        Deleted,
    }

    /// <summary>
    /// Contract for an entity's ability of being tracked with respect to its change state.
    /// </summary>
    public interface IChangeStateTrackable
    {
        /// <summary>
        /// Gets or sets the change state of the entity.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        ChangeState ChangeState { get; set; }
    }
}