using UnityEngine;

namespace VampireSurvivor.Data
{
    public enum WeaponTargeting
    {
        NearestEnemy,
        RandomEnemy,
        RandomDirection,
        MouseDirection,
        AllDirections
    }

    /// <summary>
    /// Defines a weapon's stats and behavior.
    /// Create instances via Assets > Create > VampireSurvivor > Weapon Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "VampireSurvivor/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string WeaponName;
        [Tooltip("Key used by PoolManager to retrieve projectiles")]
        public string ProjectilePoolKey;

        [Header("Firing")]
        [Tooltip("Shots per second")]
        [Min(0.1f)] public float FireRate = 1f;
        [Min(1)] public int ProjectileCount = 1;
        [Range(0, 360)] public float SpreadAngle = 0f;

        [Header("Projectile Stats")]
        [Min(0)] public float Damage = 10f;
        [Min(0)] public float ProjectileSpeed = 8f;
        [Min(0)] public float ProjectileLifetime = 2f;
        [Min(0)] public float ProjectileSize = 0.3f;
        [Tooltip("How many enemies can this projectile hit before despawning. 0 = infinite (pierce all)")]
        [Min(0)] public int PierceCount = 1;

        [Header("Targeting")]
        public WeaponTargeting Targeting = WeaponTargeting.NearestEnemy;

        [Header("Visuals")]
        public Color ProjectileColor = Color.yellow;

        [Header("Prefab Reference")]
        public GameObject ProjectilePrefab;
    }
}
