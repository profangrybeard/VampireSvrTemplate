using UnityEngine;

namespace VampireSurvivor.Data
{
    /// <summary>
    /// Data-driven configuration for procedural arena generation.
    /// Create assets via: Assets > Create > VampireSurvivor > ArenaConfig
    /// </summary>
    [CreateAssetMenu(fileName = "ArenaConfig", menuName = "VampireSurvivor/ArenaConfig")]
    public class ArenaConfig : ScriptableObject
    {
        [Header("Arena Size")]
        [Tooltip("Width and height of the arena in tiles")]
        [SerializeField] private Vector2Int _arenaSize = new Vector2Int(30, 30);

        [Header("Wall Settings")]
        [Tooltip("Thickness of the border walls in tiles")]
        [SerializeField] private int _wallThickness = 1;

        [Header("Tile Colors")]
        [Tooltip("Color of the ground/floor tiles")]
        [SerializeField] private Color _groundColor = new Color(0.2f, 0.2f, 0.25f);

        [Tooltip("Color of the wall tiles")]
        [SerializeField] private Color _wallColor = new Color(0.5f, 0.3f, 0.2f);

        [Header("Spawn Settings")]
        [Tooltip("Minimum distance from walls for enemy spawning")]
        [SerializeField] private int _spawnPadding = 2;

        // Public accessors
        public Vector2Int ArenaSize => _arenaSize;
        public int WallThickness => _wallThickness;
        public Color GroundColor => _groundColor;
        public Color WallColor => _wallColor;
        public int SpawnPadding => _spawnPadding;

        /// <summary>
        /// Gets the playable area bounds (inside the walls).
        /// Returns min and max world positions.
        /// </summary>
        public (Vector2 min, Vector2 max) GetPlayableBounds()
        {
            // Arena is centered at origin
            float halfWidth = _arenaSize.x / 2f;
            float halfHeight = _arenaSize.y / 2f;

            // Playable area is inside the walls
            float padding = _wallThickness + _spawnPadding;

            Vector2 min = new Vector2(-halfWidth + padding, -halfHeight + padding);
            Vector2 max = new Vector2(halfWidth - padding, halfHeight - padding);

            return (min, max);
        }

        /// <summary>
        /// Gets a random spawn position within the playable area.
        /// </summary>
        public Vector2 GetRandomSpawnPosition()
        {
            var (min, max) = GetPlayableBounds();
            return new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
            );
        }

        /// <summary>
        /// Checks if a position is within the playable area.
        /// </summary>
        public bool IsWithinBounds(Vector2 position)
        {
            var (min, max) = GetPlayableBounds();
            return position.x >= min.x && position.x <= max.x &&
                   position.y >= min.y && position.y <= max.y;
        }
    }
}
