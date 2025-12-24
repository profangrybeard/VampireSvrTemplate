using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using VampireSurvivor.Core;
using VampireSurvivor.Core.Constants;
using VampireSurvivor.Data;

namespace VampireSurvivor.Systems.Arena
{
    /// <summary>
    /// Procedurally generates a rectangular arena with ground tiles, wall borders,
    /// and random interior obstacles. Creates all required components at runtime.
    ///
    /// Attach to an empty GameObject in the scene. Generates arena on Awake
    /// so it's ready before other systems (like EnemySpawner, PathfindingManager) initialize.
    /// </summary>
    public class ArenaGenerator : MonoBehaviour
    {
        [SerializeField] private ArenaConfig _config;
        [SerializeField] private ObstacleConfig _obstacleConfig;

        private Grid _grid;
        private Tilemap _groundTilemap;
        private Tilemap _wallTilemap;
        private TilemapCollider2D _wallCollider;

        // Track obstacle positions for pathfinding and spawn validation
        private HashSet<Vector2Int> _obstaclePositions = new();

        public ArenaConfig Config => _config;
        public ObstacleConfig ObstacleConfig => _obstacleConfig;
        public IReadOnlyCollection<Vector2Int> ObstaclePositions => _obstaclePositions;

        private void Awake()
        {
            // Register with ServiceLocator so other systems can access arena bounds
            ServiceLocator.Register(this);

            if (_config == null)
            {
                Debug.LogError("ArenaGenerator: No ArenaConfig assigned!");
                return;
            }

            CreateGridAndTilemaps();
            GenerateArena();
            GenerateObstacles();
        }

        /// <summary>
        /// Creates the Grid and Tilemap GameObjects with required components.
        /// </summary>
        private void CreateGridAndTilemaps()
        {
            // Create Grid (parent for all tilemaps)
            GameObject gridObj = new GameObject("Arena_Grid");
            gridObj.transform.SetParent(transform);
            _grid = gridObj.AddComponent<Grid>();
            _grid.cellSize = Vector3.one; // 1 unit per cell

            // Create Ground Tilemap (rendered below walls)
            GameObject groundObj = new GameObject("Ground_Tilemap");
            groundObj.transform.SetParent(gridObj.transform);
            _groundTilemap = groundObj.AddComponent<Tilemap>();
            var groundRenderer = groundObj.AddComponent<TilemapRenderer>();
            groundRenderer.sortingOrder = 0; // Back layer

            // Create Wall Tilemap (rendered above ground)
            GameObject wallObj = new GameObject("Wall_Tilemap");
            wallObj.transform.SetParent(gridObj.transform);

            // Set physics layer - requires Layer 11 "Wall" to exist in Project Settings
            int wallLayer = LayerMask.NameToLayer(Layers.Names.Wall);
            if (wallLayer == -1)
            {
                Debug.LogError("ArenaGenerator: Layer 'Wall' not found! Add Layer 11 'Wall' in Edit > Project Settings > Tags and Layers");
                wallLayer = 0; // Fallback to default
            }
            wallObj.layer = wallLayer;
            _wallTilemap = wallObj.AddComponent<Tilemap>();
            var wallRenderer = wallObj.AddComponent<TilemapRenderer>();
            wallRenderer.sortingOrder = 1; // Above ground

            // Add collider to walls - blocks Player and Enemy
            _wallCollider = wallObj.AddComponent<TilemapCollider2D>();

            // Add Rigidbody2D for collision (static body)
            var wallRb = wallObj.AddComponent<Rigidbody2D>();
            wallRb.bodyType = RigidbodyType2D.Static;

            // Add CompositeCollider2D for better performance
            // Merges individual tile colliders into one optimized polygon
            var composite = wallObj.AddComponent<CompositeCollider2D>();
            _wallCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        }

        /// <summary>
        /// Generates the arena tiles based on config settings.
        /// </summary>
        private void GenerateArena()
        {
            // Create tiles using factory
            // Ground tiles have no collision, wall tiles have collision
            var groundTile = TileFactory.CreateColoredTile(_config.GroundColor, hasCollision: false);
            var wallTile = TileFactory.CreateColoredTile(_config.WallColor, hasCollision: true);

            int width = _config.ArenaSize.x;
            int height = _config.ArenaSize.y;
            int wallThickness = _config.WallThickness;

            // Calculate bounds (centered at origin)
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            // Fill entire arena area
            for (int x = -halfWidth; x < halfWidth; x++)
            {
                for (int y = -halfHeight; y < halfHeight; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);

                    // Check if this cell is on the border (wall)
                    bool isWall = x < -halfWidth + wallThickness ||
                                  x >= halfWidth - wallThickness ||
                                  y < -halfHeight + wallThickness ||
                                  y >= halfHeight - wallThickness;

                    if (isWall)
                    {
                        _wallTilemap.SetTile(pos, wallTile);
                    }
                    else
                    {
                        _groundTilemap.SetTile(pos, groundTile);
                    }
                }
            }

            Debug.Log($"Arena generated: {width}x{height} tiles, wall thickness: {wallThickness}");
        }

        /// <summary>
        /// Generates random obstacles inside the arena.
        /// Obstacles avoid the center (player spawn) and edges (enemy spawn).
        /// Weighted to distribute evenly across cardinal directions.
        /// </summary>
        private void GenerateObstacles()
        {
            if (_obstacleConfig == null || _obstacleConfig.ObstacleCount <= 0)
            {
                return; // No obstacles configured
            }

            // Create obstacle tile (same as wall but different color)
            var obstacleTile = TileFactory.CreateColoredTile(_obstacleConfig.ObstacleColor, hasCollision: true);

            int width = _config.ArenaSize.x;
            int height = _config.ArenaSize.y;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            // Define valid placement zone (inside walls, away from center and edges)
            int edgePadding = _config.WallThickness + _obstacleConfig.EdgePadding;
            int centerClear = _obstacleConfig.MinDistanceFromCenter;

            // Track obstacles per quadrant: 0=North, 1=East, 2=South, 3=West
            int[] quadrantCounts = new int[4];

            int obstaclesPlaced = 0;
            int maxAttempts = _obstacleConfig.ObstacleCount * 10; // Prevent infinite loops
            int attempts = 0;

            while (obstaclesPlaced < _obstacleConfig.ObstacleCount && attempts < maxAttempts)
            {
                attempts++;

                // Get random obstacle size
                Vector2Int size = _obstacleConfig.GetRandomSize();

                // Pick quadrant weighted toward least-populated
                int quadrant = PickWeightedQuadrant(quadrantCounts);

                // Get position bounds for selected quadrant
                int minX, maxX, minY, maxY;
                GetQuadrantBounds(quadrant, halfWidth, halfHeight, edgePadding, size,
                    out minX, out maxX, out minY, out maxY);

                if (maxX <= minX || maxY <= minY) continue; // Quadrant too small

                int posX = Random.Range(minX, maxX + 1);
                int posY = Random.Range(minY, maxY + 1);

                // Check if too close to center
                float distFromCenter = Mathf.Sqrt(posX * posX + posY * posY);
                if (distFromCenter < centerClear) continue;

                // Check if overlaps existing obstacles
                bool overlaps = false;
                for (int ox = 0; ox < size.x && !overlaps; ox++)
                {
                    for (int oy = 0; oy < size.y && !overlaps; oy++)
                    {
                        Vector2Int checkPos = new Vector2Int(posX + ox, posY + oy);
                        if (_obstaclePositions.Contains(checkPos))
                        {
                            overlaps = true;
                        }
                    }
                }

                if (overlaps) continue;

                // Place the obstacle
                for (int ox = 0; ox < size.x; ox++)
                {
                    for (int oy = 0; oy < size.y; oy++)
                    {
                        Vector3Int tilePos = new Vector3Int(posX + ox, posY + oy, 0);
                        _wallTilemap.SetTile(tilePos, obstacleTile);
                        _obstaclePositions.Add(new Vector2Int(posX + ox, posY + oy));
                    }
                }

                quadrantCounts[quadrant]++;
                obstaclesPlaced++;
            }

            Debug.Log($"Obstacles generated: {obstaclesPlaced} (N:{quadrantCounts[0]} E:{quadrantCounts[1]} S:{quadrantCounts[2]} W:{quadrantCounts[3]})");
        }

        /// <summary>
        /// Picks a quadrant weighted toward those with fewer obstacles.
        /// </summary>
        private int PickWeightedQuadrant(int[] counts)
        {
            // Find min count
            int minCount = int.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                if (counts[i] < minCount) minCount = counts[i];
            }

            // Collect quadrants at or near minimum (within 1)
            List<int> candidates = new List<int>(4);
            for (int i = 0; i < 4; i++)
            {
                if (counts[i] <= minCount + 1)
                    candidates.Add(i);
            }

            return candidates[Random.Range(0, candidates.Count)];
        }

        /// <summary>
        /// Gets position bounds for a quadrant. 0=North, 1=East, 2=South, 3=West.
        /// </summary>
        private void GetQuadrantBounds(int quadrant, int halfWidth, int halfHeight, int edgePadding,
            Vector2Int size, out int minX, out int maxX, out int minY, out int maxY)
        {
            // Full valid range
            int fullMinX = -halfWidth + edgePadding;
            int fullMaxX = halfWidth - edgePadding - size.x;
            int fullMinY = -halfHeight + edgePadding;
            int fullMaxY = halfHeight - edgePadding - size.y;

            switch (quadrant)
            {
                case 0: // North (positive Y)
                    minX = fullMinX; maxX = fullMaxX;
                    minY = 0; maxY = fullMaxY;
                    break;
                case 1: // East (positive X)
                    minX = 0; maxX = fullMaxX;
                    minY = fullMinY; maxY = fullMaxY;
                    break;
                case 2: // South (negative Y)
                    minX = fullMinX; maxX = fullMaxX;
                    minY = fullMinY; maxY = 0;
                    break;
                case 3: // West (negative X)
                    minX = fullMinX; maxX = 0;
                    minY = fullMinY; maxY = fullMaxY;
                    break;
                default:
                    minX = fullMinX; maxX = fullMaxX;
                    minY = fullMinY; maxY = fullMaxY;
                    break;
            }
        }

        /// <summary>
        /// Checks if a tile position contains an obstacle.
        /// </summary>
        public bool IsObstacle(Vector2Int tilePosition)
        {
            return _obstaclePositions.Contains(tilePosition);
        }

        /// <summary>
        /// Converts world position to tile position.
        /// </summary>
        public Vector2Int WorldToTile(Vector2 worldPosition)
        {
            Vector3Int cell = _grid.WorldToCell(worldPosition);
            return new Vector2Int(cell.x, cell.y);
        }

        /// <summary>
        /// Gets a random position within the playable area (inside walls).
        /// </summary>
        public Vector2 GetRandomSpawnPosition()
        {
            return _config.GetRandomSpawnPosition();
        }

        /// <summary>
        /// Gets the playable bounds of the arena.
        /// </summary>
        public (Vector2 min, Vector2 max) GetPlayableBounds()
        {
            return _config.GetPlayableBounds();
        }

        /// <summary>
        /// Checks if a position is within the playable area.
        /// </summary>
        public bool IsWithinBounds(Vector2 position)
        {
            return _config.IsWithinBounds(position);
        }

        /// <summary>
        /// Clamps a position to stay within playable bounds.
        /// Useful for camera clamping or preventing movement outside arena.
        /// </summary>
        public Vector2 ClampToBounds(Vector2 position)
        {
            var (min, max) = GetPlayableBounds();
            return new Vector2(
                Mathf.Clamp(position.x, min.x, max.x),
                Mathf.Clamp(position.y, min.y, max.y)
            );
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ArenaGenerator>();
        }
    }
}
