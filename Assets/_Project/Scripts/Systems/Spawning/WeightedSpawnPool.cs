using System;
using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Data;

namespace VampireSurvivor.Systems.Spawning
{
    [Serializable]
    public class WeightedSpawnEntry
    {
        public EnemyData EnemyType;
        [Tooltip("Higher weight = more likely to spawn early. Weight is consumed as enemies spawn.")]
        [Min(1)] public int SpawnWeight = 10;
    }

    // Manages weighted random enemy selection with consumption.
    // Higher-weighted enemies spawn more frequently until their weight is depleted,
    // then lower-weighted enemies get their turn.
    public class WeightedSpawnPool
    {
        private readonly List<WeightedSpawnEntry> _entries;
        private readonly List<int> _remainingWeights;
        private int _totalRemainingWeight;
        private int _currentIndex;
        private readonly bool _useRoundRobin;

        public bool IsExhausted => _totalRemainingWeight <= 0;
        public int TotalRemainingWeight => _totalRemainingWeight;

        public WeightedSpawnPool(WeightedSpawnEntry[] entries, bool roundRobinBase = true)
        {
            _entries = new List<WeightedSpawnEntry>(entries);
            _remainingWeights = new List<int>();
            _useRoundRobin = roundRobinBase;
            _currentIndex = 0;

            Reset();
        }

        // Resets all weights to their original values.
        public void Reset()
        {
            _remainingWeights.Clear();
            _totalRemainingWeight = 0;

            foreach (var entry in _entries)
            {
                int weight = Mathf.Max(1, entry.SpawnWeight);
                _remainingWeights.Add(weight);
                _totalRemainingWeight += weight;
            }

            _currentIndex = 0;
        }

        // Gets the next enemy to spawn using weighted selection.
        // Returns null if all weights are exhausted.
        public EnemyData GetNext()
        {
            if (_entries.Count == 0 || _totalRemainingWeight <= 0)
                return null;

            if (_useRoundRobin)
            {
                return GetNextRoundRobinWeighted();
            }
            else
            {
                return GetNextPureWeighted();
            }
        }

        // Round-robin with weighting: cycles through types, but weighted types get picked more often.
        // Each enemy type is visited in order, but the weight determines how many times
        // it can be selected before moving to the next.
        private EnemyData GetNextRoundRobinWeighted()
        {
            // Find next valid entry with remaining weight
            int attempts = 0;
            while (attempts < _entries.Count)
            {
                if (_remainingWeights[_currentIndex] > 0)
                {
                    var entry = _entries[_currentIndex];
                    _remainingWeights[_currentIndex]--;
                    _totalRemainingWeight--;

                    // Move to next index for next call
                    _currentIndex = (_currentIndex + 1) % _entries.Count;

                    return entry.EnemyType;
                }

                // This entry is exhausted, try next
                _currentIndex = (_currentIndex + 1) % _entries.Count;
                attempts++;
            }

            return null;
        }

        // Pure weighted random: higher weights are more likely to be picked.
        private EnemyData GetNextPureWeighted()
        {
            if (_totalRemainingWeight <= 0)
                return null;

            int roll = UnityEngine.Random.Range(0, _totalRemainingWeight);
            int cumulative = 0;

            for (int i = 0; i < _entries.Count; i++)
            {
                cumulative += _remainingWeights[i];
                if (roll < cumulative)
                {
                    var entry = _entries[i];
                    _remainingWeights[i]--;
                    _totalRemainingWeight--;
                    return entry.EnemyType;
                }
            }

            // Fallback (shouldn't happen)
            return _entries[0].EnemyType;
        }

        // Preview what's available without consuming.
        public EnemyData PeekNext()
        {
            if (_entries.Count == 0 || _totalRemainingWeight <= 0)
                return null;

            if (_useRoundRobin)
            {
                // Find next valid entry
                int idx = _currentIndex;
                for (int i = 0; i < _entries.Count; i++)
                {
                    if (_remainingWeights[idx] > 0)
                        return _entries[idx].EnemyType;
                    idx = (idx + 1) % _entries.Count;
                }
                return null;
            }
            else
            {
                // Return highest weighted entry
                int maxWeight = 0;
                int maxIdx = 0;
                for (int i = 0; i < _entries.Count; i++)
                {
                    if (_remainingWeights[i] > maxWeight)
                    {
                        maxWeight = _remainingWeights[i];
                        maxIdx = i;
                    }
                }
                return _entries[maxIdx].EnemyType;
            }
        }

        // Gets remaining weight for a specific enemy type.
        public int GetRemainingWeight(EnemyData enemyType)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].EnemyType == enemyType)
                    return _remainingWeights[i];
            }
            return 0;
        }
    }
}
