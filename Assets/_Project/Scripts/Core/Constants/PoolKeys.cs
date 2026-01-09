namespace VampireSurvivor.Core.Constants
{
    // Centralized pool key constants. Must match PoolManager configs and ScriptableObject fields.
    public static class PoolKeys
    {
        // Enemies
        public const string EnemyBat = "enemy_bat";
        public const string EnemySkeleton = "enemy_skeleton";

        // Projectiles
        public const string ProjectileMagic = "projectile_magic";
        public const string ProjectileBomb = "projectile_bomb";

        // Add new pool keys here as you create more enemy/projectile types
    }
}
