namespace Nexu.Shared.Model
{
    public enum AccountStatus
    {
        // TODO: Define
        /// <summary>
        /// Active (initial) status.
        /// </summary>
        Active,

        /// <summary>
        /// Intermediate state between <see cref="Active" /> and <see cref="Invalidated" />.
        /// Some actions will be performed and scheduled tasks will be run, but no new content
        /// can be created.
        /// </summary>
        Restricted,

        /// <summary>
        /// Invalidated/deactivated by the system because of billing or other types of issues
        /// </summary>
        Invalidated,

        /// <summary>
        /// Manually Inactive by the user.
        /// </summary>
        Inactive,

        /// <summary>
        /// Permanently deleted.
        /// </summary>
        Deleted
    }
}
