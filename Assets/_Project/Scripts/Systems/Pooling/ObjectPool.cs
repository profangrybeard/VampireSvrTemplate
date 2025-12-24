using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Interfaces;

namespace VampireSurvivor.Systems.Pooling
{
    // Generic object pool for any Component that implements IPoolable.
    public class ObjectPool<T> where T : Component, IPoolable
    {
        private readonly Queue<T> _available = new();
        private readonly HashSet<T> _inUse = new();
        private readonly T _prefab;
        private readonly Transform _parent;

        public int CountAvailable => _available.Count;
        public int CountInUse => _inUse.Count;
        public int CountTotal => CountAvailable + CountInUse;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            Prewarm(initialSize);
        }

        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateInstance();
            }
        }

        private T CreateInstance()
        {
            T instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            _available.Enqueue(instance);
            return instance;
        }

        // Get an instance from the pool at the specified position and rotation.
        public T Get(Vector3 position, Quaternion rotation)
        {
            T instance = _available.Count > 0 ? _available.Dequeue() : CreateInstance();

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            instance.OnSpawnFromPool();
            _inUse.Add(instance);

            return instance;
        }

        // Get an instance from the pool at the specified position.
        public T Get(Vector3 position)
        {
            return Get(position, Quaternion.identity);
        }

        // Return an instance to the pool.
        public void Return(T instance)
        {
            if (!_inUse.Contains(instance)) return;

            instance.OnReturnToPool();
            instance.gameObject.SetActive(false);
            _inUse.Remove(instance);
            _available.Enqueue(instance);
        }

        // Return all active instances to the pool.
        public void ReturnAll()
        {
            // Copy to avoid modifying collection during iteration
            var inUseList = new List<T>(_inUse);
            foreach (var instance in inUseList)
            {
                Return(instance);
            }
        }

        // Destroy all pooled instances.
        public void Clear()
        {
            ReturnAll();
            while (_available.Count > 0)
            {
                var instance = _available.Dequeue();
                if (instance != null)
                {
                    Object.Destroy(instance.gameObject);
                }
            }
        }
    }
}
