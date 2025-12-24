namespace VampireSurvivor.Core.Constants
{
    // Centralized layer constants. Must match layers in Project Settings.
    public static class Layers
    {
        // Layer names (use with LayerMask.NameToLayer() or Inspector fields)
        public static class Names
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string PlayerProjectile = "PlayerProjectile";
            public const string Wall = "Wall";
        }

        // Layer indices (use with gameObject.layer comparisons)
        // These match the layer slots in Project Settings
        public static class Index
        {
            public const int Player = 8;
            public const int Enemy = 9;
            public const int PlayerProjectile = 10;
            public const int Wall = 11;
        }

        // Layer masks (use with Physics2D.Raycast, OverlapCircle, etc.)
        // Calculated as: 1 << layerIndex
        public static class Mask
        {
            public const int Player = 1 << Index.Player;
            public const int Enemy = 1 << Index.Enemy;
            public const int PlayerProjectile = 1 << Index.PlayerProjectile;
            public const int Wall = 1 << Index.Wall;

            // Combined masks for common queries
            public const int AllEnemies = Enemy;
            public const int AllProjectiles = PlayerProjectile;
            public const int Obstacles = Wall;
        }
    }
}
