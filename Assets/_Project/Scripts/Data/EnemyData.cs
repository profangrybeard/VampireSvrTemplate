using UnityEngine;

namespace VampireSurvivor.Data
{
    /// <summary>
    /// Defines an enemy type's stats and appearance.
    /// Create instances via Assets > Create > VampireSurvivor > Enemy Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "VampireSurvivor/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string EnemyName;
        [Tooltip("Key used by PoolManager to retrieve this enemy type")]
        public string PoolKey;

        [Header("Stats")]
        [Min(1)] public float MaxHealth = 10f;
        [Min(0)] public float MoveSpeed = 2f;
        [Min(0)] public float ContactDamage = 5f;

        [Header("Rewards")]
        [Min(0)] public int XPValue = 1;

        [Header("Visuals")]
        public Color Color = Color.red;
        public Vector2 Size = Vector2.one;

        [Header("Prefab Reference")]
        [Tooltip("The prefab to instantiate for this enemy type")]
        public GameObject Prefab;
    }
}
