using System.Collections;
using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Core.Constants;
using VampireSurvivor.Data;
using VampireSurvivor.Entities.Enemies;
using VampireSurvivor.Events;
using VampireSurvivor.Systems.Arena;
using VampireSurvivor.Systems.Pooling;

namespace VampireSurvivor.Systems.Spawning
{
    // Manages wave-based enemy spawning. Respects arena bounds when ArenaGenerator is present.
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private Transform _playerTransform;

        private PoolManager _poolManager;
        private ArenaGenerator _arenaGenerator;
        private int _currentWaveIndex;
        private bool _isSpawning;
        private Coroutine _spawnCoroutine;

        public int CurrentWave => _currentWaveIndex + 1;
        public bool IsSpawning => _isSpawning;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            _poolManager = ServiceLocator.Get<PoolManager>();
            _arenaGenerator = ServiceLocator.Get<ArenaGenerator>();

            if (_playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag(Tags.Player);
                if (player != null)
                    _playerTransform = player.transform;
            }

            // Start first wave
            if (_config != null && _config.Waves != null && _config.Waves.Length > 0)
            {
                StartWave(0);
            }
        }

        private void OnEnable()
        {
            EventBus.OnWaveCompleted += HandleWaveCompleted;
        }

        private void OnDisable()
        {
            EventBus.OnWaveCompleted -= HandleWaveCompleted;
        }

        public void StartWave(int waveIndex)
        {
            if (_config == null || _config.Waves == null) return;
            if (waveIndex < 0 || waveIndex >= _config.Waves.Length) return;

            _currentWaveIndex = waveIndex;
            var waveData = _config.Waves[waveIndex];

            EventBus.Publish(new WaveStartedEvent(waveIndex + 1));

            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            _spawnCoroutine = StartCoroutine(SpawnWave(waveData));
        }

        private IEnumerator SpawnWave(WaveData waveData)
        {
            _isSpawning = true;

            // Initial delay
            yield return new WaitForSeconds(waveData.StartDelay);

            // Spawn each enemy entry
            foreach (var entry in waveData.Enemies)
            {
                if (entry.EnemyType == null) continue;

                for (int i = 0; i < entry.Count; i++)
                {
                    SpawnEnemy(entry.EnemyType, waveData.SpawnRadius);
                    yield return new WaitForSeconds(entry.SpawnDelay);
                }
            }

            _isSpawning = false;

            // Wait before completing wave
            yield return new WaitForSeconds(1f);

            EventBus.Publish(new WaveCompletedEvent(_currentWaveIndex + 1));
        }

        private void SpawnEnemy(EnemyData enemyData, float spawnRadius)
        {
            if (_playerTransform == null || _poolManager == null) return;

            Vector2 spawnPos = GetSpawnPosition(spawnRadius);

            var enemy = _poolManager.Get<Enemy>(enemyData.PoolKey, spawnPos);
            if (enemy != null)
            {
                enemy.Initialize(enemyData);

                // Set target for movement
                var movement = enemy.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.SetTarget(_playerTransform);
                }

                EventBus.Publish(new EnemySpawnedEvent(spawnPos, enemyData));
            }
        }

        private Vector2 GetSpawnPosition(float radius)
        {
            // If arena exists, spawn within arena bounds
            if (_arenaGenerator != null)
            {
                return GetArenaSpawnPosition();
            }

            // Fallback: Spawn in a ring around the player (off-screen)
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            return (Vector2)_playerTransform.position + offset;
        }

        // Gets a spawn position within the arena near edges.
        private Vector2 GetArenaSpawnPosition()
        {
            var (min, max) = _arenaGenerator.GetPlayableBounds();
            Vector2 playerPos = _playerTransform.position;

            // Try to spawn on the opposite side of the arena from the player
            // Pick a random edge (0=left, 1=right, 2=bottom, 3=top)
            int edge = Random.Range(0, 4);
            float padding = 1f; // Stay slightly inside the edge

            Vector2 spawnPos;
            switch (edge)
            {
                case 0: // Left edge
                    spawnPos = new Vector2(min.x + padding, Random.Range(min.y, max.y));
                    break;
                case 1: // Right edge
                    spawnPos = new Vector2(max.x - padding, Random.Range(min.y, max.y));
                    break;
                case 2: // Bottom edge
                    spawnPos = new Vector2(Random.Range(min.x, max.x), min.y + padding);
                    break;
                default: // Top edge
                    spawnPos = new Vector2(Random.Range(min.x, max.x), max.y - padding);
                    break;
            }

            return spawnPos;
        }

        private void HandleWaveCompleted(WaveCompletedEvent evt)
        {
            // Start next wave after delay
            if (_config != null && _currentWaveIndex + 1 < _config.Waves.Length)
            {
                StartCoroutine(StartNextWaveAfterDelay());
            }
        }

        private IEnumerator StartNextWaveAfterDelay()
        {
            yield return new WaitForSeconds(_config.TimeBetweenWaves);
            StartWave(_currentWaveIndex + 1);
        }

        // Force spawn a specific enemy type immediately.
        public void SpawnEnemyImmediate(EnemyData enemyData, Vector2 position)
        {
            if (_poolManager == null) return;

            var enemy = _poolManager.Get<Enemy>(enemyData.PoolKey, position);
            if (enemy != null)
            {
                enemy.Initialize(enemyData);

                var movement = enemy.GetComponent<EnemyMovement>();
                if (movement != null && _playerTransform != null)
                {
                    movement.SetTarget(_playerTransform);
                }
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<EnemySpawner>();
        }
    }
}
