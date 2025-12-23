namespace VampireSurvivor.Core.Constants
{
    /// <summary>
    /// Centralized tag constants. Use these with CompareTag() and
    /// FindGameObjectsWithTag() instead of magic strings.
    ///
    /// These must match the tags created in Unity's Tag Manager:
    /// Edit > Project Settings > Tags and Layers
    /// </summary>
    public static class Tags
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";

        // Unity built-in tags (for reference)
        public const string Untagged = "Untagged";
        public const string MainCamera = "MainCamera";
    }
}
