using System;
using UnityEngine;

namespace VampireSurvivor.Data
{
    [Serializable]
    public class EnemySpawnEntry
    {
        public EnemyData EnemyType;
        [Min(1)] public int Count = 5;
        [Tooltip("Delay between each individual spawn")]
        [Min(0)] public float SpawnDelay = 0.5f;
    }

    /// <summary>
    /// Defines a spawn wave's composition and timing.
    /// Create instances via Assets > Create > VampireSurvivor > Wave Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWave", menuName = "VampireSurvivor/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Wave Settings")]
        public string WaveName;
        [Tooltip("Delay before this wave begins")]
        [Min(0)] public float StartDelay = 2f;

        [Header("Enemies")]
        public EnemySpawnEntry[] Enemies;

        [Header("Spawn Settings")]
        [Tooltip("Distance from player where enemies spawn")]
        [Min(1)] public float SpawnRadius = 12f;
        public bool SpawnFromOffscreen = true;
    }
}
