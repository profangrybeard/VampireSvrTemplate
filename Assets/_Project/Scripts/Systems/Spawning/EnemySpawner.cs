using System.Collections;
using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Data;
using VampireSurvivor.Entities.Enemies;
using VampireSurvivor.Events;
using VampireSurvivor.Systems.Pooling;

namespace VampireSurvivor.Systems.Spawning
{
    /// <summary>
    /// Manages wave-based enemy spawning.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameConfig _config;
        [SerializeField] private Transform _playerTransform;

        private PoolManager _poolManager;
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

            if (_playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
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
            // Spawn in a ring around the player (off-screen)
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            return (Vector2)_playerTransform.position + offset;
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

        /// <summary>
        /// Force spawn a specific enemy type immediately.
        /// </summary>
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
