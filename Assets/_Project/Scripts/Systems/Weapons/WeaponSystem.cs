using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Core.Constants;
using VampireSurvivor.Data;
using VampireSurvivor.Entities.Projectiles;
using VampireSurvivor.Systems.Pooling;

namespace VampireSurvivor.Systems.Weapons
{
    /// <summary>
    /// Manages weapon firing for the player. Attach to player or child object.
    /// </summary>
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private WeaponData _weaponData;
        [SerializeField] private Transform _firePoint;

        private float _fireCooldown;
        private Transform _playerTransform;
        private PoolManager _poolManager;

        // Cache for enemy finding
        private static readonly List<Transform> _enemyCache = new();

        private void Start()
        {
            _playerTransform = transform.root;
            _poolManager = ServiceLocator.Get<PoolManager>();
        }

        private void Update()
        {
            if (_weaponData == null) return;

            _fireCooldown -= Time.deltaTime;

            if (_fireCooldown <= 0)
            {
                Fire();
                _fireCooldown = 1f / _weaponData.FireRate;
            }
        }

        private void Fire()
        {
            Vector2 baseDirection = GetTargetDirection();
            if (baseDirection == Vector2.zero) return;

            Vector3 spawnPos = _firePoint != null ? _firePoint.position : transform.position;

            // Fire projectiles with spread
            if (_weaponData.ProjectileCount == 1)
            {
                SpawnProjectile(spawnPos, baseDirection);
            }
            else
            {
                float startAngle = -_weaponData.SpreadAngle / 2f;
                float angleStep = _weaponData.SpreadAngle / (_weaponData.ProjectileCount - 1);

                for (int i = 0; i < _weaponData.ProjectileCount; i++)
                {
                    float angle = startAngle + angleStep * i;
                    Vector2 direction = RotateVector(baseDirection, angle);
                    SpawnProjectile(spawnPos, direction);
                }
            }
        }

        private void SpawnProjectile(Vector3 position, Vector2 direction)
        {
            if (_poolManager == null) return;

            var projectile = _poolManager.Get<Projectile>(
                _weaponData.ProjectilePoolKey,
                position,
                Quaternion.identity
            );

            if (projectile != null)
            {
                projectile.Initialize(_weaponData, direction, _weaponData.ProjectilePoolKey);
            }
        }

        private Vector2 GetTargetDirection()
        {
            switch (_weaponData.Targeting)
            {
                case WeaponTargeting.NearestEnemy:
                    return GetDirectionToNearestEnemy();

                case WeaponTargeting.RandomEnemy:
                    return GetDirectionToRandomEnemy();

                case WeaponTargeting.RandomDirection:
                    return Random.insideUnitCircle.normalized;

                case WeaponTargeting.MouseDirection:
                    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    return ((Vector2)mouseWorld - (Vector2)transform.position).normalized;

                case WeaponTargeting.AllDirections:
                    return Vector2.right; // Base direction, spread handles the rest

                default:
                    return Vector2.right;
            }
        }

        private Vector2 GetDirectionToNearestEnemy()
        {
            RefreshEnemyCache();

            if (_enemyCache.Count == 0)
            {
                // No enemies, fire in player's facing direction
                var playerController = _playerTransform.GetComponent<Entities.Player.PlayerController>();
                return playerController != null ? playerController.FacingDirection : Vector2.right;
            }

            Transform nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var enemy in _enemyCache)
            {
                if (enemy == null) continue;

                float dist = Vector2.Distance(transform.position, enemy.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }

            if (nearest != null)
            {
                return ((Vector2)nearest.position - (Vector2)transform.position).normalized;
            }

            return Vector2.right;
        }

        private Vector2 GetDirectionToRandomEnemy()
        {
            RefreshEnemyCache();

            if (_enemyCache.Count == 0)
            {
                return Random.insideUnitCircle.normalized;
            }

            var randomEnemy = _enemyCache[Random.Range(0, _enemyCache.Count)];
            if (randomEnemy != null)
            {
                return ((Vector2)randomEnemy.position - (Vector2)transform.position).normalized;
            }

            return Vector2.right;
        }

        private static void RefreshEnemyCache()
        {
            _enemyCache.Clear();
            var enemies = GameObject.FindGameObjectsWithTag(Tags.Enemy);
            foreach (var enemy in enemies)
            {
                if (enemy.activeInHierarchy)
                {
                    _enemyCache.Add(enemy.transform);
                }
            }
        }

        private static Vector2 RotateVector(Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            );
        }

        /// <summary>
        /// Equip a new weapon at runtime.
        /// </summary>
        public void EquipWeapon(WeaponData weapon)
        {
            _weaponData = weapon;
            _fireCooldown = 0;
        }
    }
}
