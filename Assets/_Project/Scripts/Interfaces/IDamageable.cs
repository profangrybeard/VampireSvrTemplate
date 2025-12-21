namespace VampireSurvivor.Interfaces
{
    /// <summary>
    /// Contract for any entity that can receive damage.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
