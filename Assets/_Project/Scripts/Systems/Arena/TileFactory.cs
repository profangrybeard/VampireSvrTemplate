using UnityEngine;
using UnityEngine.Tilemaps;

namespace VampireSurvivor.Systems.Arena
{
    // Factory for creating colored tiles at runtime.
    public static class TileFactory
    {
        // Creates a solid-colored tile programmatically.
        public static Tile CreateColoredTile(Color color, bool hasCollision = false, int pixelsPerUnit = 1)
        {
            // Create a 1x1 pixel texture
            Texture2D texture = new Texture2D(pixelsPerUnit, pixelsPerUnit);
            texture.filterMode = FilterMode.Point; // Crisp pixels, no smoothing

            // Fill with solid color
            Color[] pixels = new Color[pixelsPerUnit * pixelsPerUnit];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();

            // Create sprite from texture
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, pixelsPerUnit, pixelsPerUnit),
                new Vector2(0.5f, 0.5f), // Pivot at center
                pixelsPerUnit
            );

            // Create and configure tile
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = Color.white; // Sprite already has color baked in

            // Set collider type - Grid means TilemapCollider2D will generate a box collider
            tile.colliderType = hasCollision ? Tile.ColliderType.Grid : Tile.ColliderType.None;

            return tile;
        }

        // Creates a standard ground tile (dark gray).
        public static Tile CreateGroundTile()
        {
            return CreateColoredTile(new Color(0.2f, 0.2f, 0.25f)); // Dark blue-gray
        }

        // Creates a standard wall tile (brown/rust).
        public static Tile CreateWallTile()
        {
            return CreateColoredTile(new Color(0.5f, 0.3f, 0.2f)); // Brown
        }
    }
}
