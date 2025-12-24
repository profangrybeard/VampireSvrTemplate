namespace VampireSurvivor.Interfaces
{
    // Contract for objects managed by the object pooling system.
    public interface IPoolable
    {
        // Called when retrieved from the pool.
        void OnSpawnFromPool();

        // Called when returned to the pool.
        void OnReturnToPool();
    }
}
