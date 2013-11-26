namespace Tripod
{
    /// <summary>
    /// Synchronizes data state changes with an underlying data store.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commit all current data changes to the underlying data store.
        /// </summary>
        /// <returns>The number of data units whose values were modified after saving
        /// changes.</returns>
        int SaveChanges();

        /// <summary>
        /// Revert all current data changes to the last known state of the underlying data store.
        /// </summary>
        void DiscardChanges();
    }
}
