using System;
using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Interfaces;

namespace VampireSurvivor.Systems.Pooling
{
    /// <summary>
    /// Central registry for all object pools. Configure pools in the Inspector.
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        [Serializable]
        public class PoolConfig
        {
            [Tooltip("Unique identifier for this pool")]
            public string Key;
            public GameObject Prefab;
            [Min(1)] public int InitialSize = 20;
        }

        [SerializeField] private PoolConfig[] _poolConfigs;

        private readonly Dictionary<string, object> _pools = new();
        private readonly Dictionary<string, GameObject> _prefabs = new();

        private void Awake()
        {
            ServiceLocator.Register(this);
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var config in _poolConfigs)
            {
                if (string.IsNullOrEmpty(config.Key) || config.Prefab == null)
                {
                    Debug.LogWarning($"PoolManager: Invalid pool config (empty key or null prefab)");
                    continue;
                }

                if (_pools.ContainsKey(config.Key))
                {
                    Debug.LogWarning($"PoolManager: Duplicate pool key '{config.Key}'");
                    continue;
                }

                var poolable = config.Prefab.GetComponent<IPoolable>();
                if (poolable == null)
                {
                    Debug.LogWarning($"PoolManager: Prefab '{config.Prefab.name}' does not implement IPoolable");
                    continue;
                }

                _prefabs[config.Key] = config.Prefab;
                CreatePool(config.Key, config.Prefab, config.InitialSize);
            }
        }

        private void CreatePool(string key, GameObject prefab, int initialSize)
        {
            // Get the component type that implements IPoolable
            var poolableComponent = prefab.GetComponent<IPoolable>() as Component;
            if (poolableComponent == null) return;

            var componentType = poolableComponent.GetType();
            var poolType = typeof(ObjectPool<>).MakeGenericType(componentType);

            // Create pool: ObjectPool<T>(T prefab, int initialSize, Transform parent)
            var pool = Activator.CreateInstance(poolType, poolableComponent, initialSize, transform);
            _pools[key] = pool;
        }

        /// <summary>
        /// Get a typed pool by its key.
        /// </summary>
        public ObjectPool<T> GetPool<T>(string key) where T : Component, IPoolable
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return pool as ObjectPool<T>;
            }
            Debug.LogWarning($"PoolManager: No pool found with key '{key}'");
            return null;
        }

        /// <summary>
        /// Get an instance from the specified pool.
        /// </summary>
        public T Get<T>(string key, Vector3 position, Quaternion rotation) where T : Component, IPoolable
        {
            var pool = GetPool<T>(key);
            return pool?.Get(position, rotation);
        }

        /// <summary>
        /// Get an instance from the specified pool.
        /// </summary>
        public T Get<T>(string key, Vector3 position) where T : Component, IPoolable
        {
            return Get<T>(key, position, Quaternion.identity);
        }

        /// <summary>
        /// Return an instance to its pool.
        /// </summary>
        public void Return<T>(string key, T instance) where T : Component, IPoolable
        {
            var pool = GetPool<T>(key);
            pool?.Return(instance);
        }

        /// <summary>
        /// Return all instances in a specific pool.
        /// </summary>
        public void ReturnAll<T>(string key) where T : Component, IPoolable
        {
            var pool = GetPool<T>(key);
            pool?.ReturnAll();
        }

        /// <summary>
        /// Return all instances in all pools.
        /// </summary>
        public void ReturnAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                var returnAllMethod = pool.GetType().GetMethod("ReturnAll");
                returnAllMethod?.Invoke(pool, null);
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<PoolManager>();
        }
    }
}
