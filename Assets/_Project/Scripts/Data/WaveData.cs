using System;
using UnityEngine;
using VampireSurvivor.Systems.Spawning;

namespace VampireSurvivor.Data
{
    public enum SpawnMode
    {
        [Tooltip("Spawn enemies in order: all of type A, then all of type B, etc.")]
        Sequential,
        [Tooltip("Cycle through enemy types (A→B→C→A→B→C). Weight = how many spawns before removal.")]
        Interleaved,
        [Tooltip("Random roll each spawn. Weight = probability. High-weight enemies dominate early.")]
        PureWeighted
    }

    [Serializable]
    public class EnemySpawnEntry
    {
        public EnemyData EnemyType;
        [Min(1)] public int Count = 5;
        [Tooltip("Delay between each individual spawn")]
        [Min(0)] public float SpawnDelay = 0.5f;
    }

    // Defines a spawn wave's composition and timing.
    [CreateAssetMenu(fileName = "NewWave", menuName = "VampireSurvivor/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Wave Settings")]
        public string WaveName;
        [Tooltip("Delay before this wave begins")]
        [Min(0)] public float StartDelay = 2f;

        [Header("Spawn Mode")]
        public SpawnMode SpawnMode = SpawnMode.Sequential;

        [Header("Enemies (Sequential Mode)")]
        [Tooltip("Used when SpawnMode is Sequential")]
        public EnemySpawnEntry[] Enemies;

        [Header("Enemies (Weighted Modes)")]
        [Tooltip("Used when SpawnMode is Interleaved or PureWeighted")]
        public WeightedSpawnEntry[] WeightedEnemies;
        [Tooltip("Total enemies to spawn in weighted modes")]
        [Min(1)] public int TotalWeightedSpawns = 100;
        [Tooltip("Delay between spawns in weighted modes")]
        [Min(0)] public float WeightedSpawnDelay = 0.5f;

        [Header("Spawn Settings")]
        [Tooltip("Distance from player where enemies spawn")]
        [Min(1)] public float SpawnRadius = 12f;
        public bool SpawnFromOffscreen = true;
    }
}
