namespace VampireSurvivor.Core.Constants
{
    /// <summary>
    /// Centralized pool key constants. Use these instead of magic strings
    /// to prevent typos and enable IDE autocomplete.
    ///
    /// These keys must match:
    /// 1. PoolManager's Pool Configs in the Inspector
    /// 2. ScriptableObject PoolKey/ProjectilePoolKey fields
    /// </summary>
    public static class PoolKeys
    {
        // Enemies
        public const string EnemyBat = "enemy_bat";

        // Projectiles
        public const string ProjectileMagic = "projectile_magic";

        // Add new pool keys here as you create more enemy/projectile types:
        // public const string EnemySkeleton = "enemy_skeleton";
        // public const string ProjectileFire = "projectile_fire";
    }
}
