namespace VampireSurvivor.Interfaces
{
    /// <summary>
    /// Contract for objects managed by the object pooling system.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is retrieved from the pool and activated.
        /// Use this to reset state for reuse.
        /// </summary>
        void OnSpawnFromPool();

        /// <summary>
        /// Called when the object is returned to the pool and deactivated.
        /// Use this to clean up references and stop coroutines.
        /// </summary>
        void OnReturnToPool();
    }
}
