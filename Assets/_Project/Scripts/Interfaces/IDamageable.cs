namespace VampireSurvivor.Interfaces
{
    // Contract for any entity that can receive damage.
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }
}
