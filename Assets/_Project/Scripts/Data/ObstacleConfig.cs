using UnityEngine;

namespace VampireSurvivor.Data
{
    // Configuration for procedural obstacle generation.
    [CreateAssetMenu(fileName = "ObstacleConfig", menuName = "VampireSurvivor/ObstacleConfig")]
    public class ObstacleConfig : ScriptableObject
    {
        [Header("Obstacle Count")]
        [Tooltip("Number of obstacle clusters to spawn")]
        [SerializeField] private int _obstacleCount = 10;

        [Header("Obstacle Size")]
        [Tooltip("Minimum size of each obstacle in tiles")]
        [SerializeField] private Vector2Int _minObstacleSize = new Vector2Int(1, 1);

        [Tooltip("Maximum size of each obstacle in tiles")]
        [SerializeField] private Vector2Int _maxObstacleSize = new Vector2Int(2, 2);

        [Header("Appearance")]
        [Tooltip("Color of obstacle tiles (darker than walls for contrast)")]
        [SerializeField] private Color _obstacleColor = new Color(0.4f, 0.25f, 0.15f);

        [Header("Placement Rules")]
        [Tooltip("Minimum distance from arena center (keeps player spawn clear)")]
        [SerializeField] private int _minDistanceFromCenter = 3;

        [Tooltip("Minimum distance from arena edges (keeps spawn areas clear)")]
        [SerializeField] private int _edgePadding = 3;

        // Public accessors
        public int ObstacleCount => _obstacleCount;
        public Vector2Int MinObstacleSize => _minObstacleSize;
        public Vector2Int MaxObstacleSize => _maxObstacleSize;
        public Color ObstacleColor => _obstacleColor;
        public int MinDistanceFromCenter => _minDistanceFromCenter;
        public int EdgePadding => _edgePadding;

        // Gets a random obstacle size within the configured range.
        public Vector2Int GetRandomSize()
        {
            return new Vector2Int(
                Random.Range(_minObstacleSize.x, _maxObstacleSize.x + 1),
                Random.Range(_minObstacleSize.y, _maxObstacleSize.y + 1)
            );
        }
    }
}
