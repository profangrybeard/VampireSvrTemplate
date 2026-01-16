namespace VampireSurvivor.Core
{
    // Project version tracking for students.
    // Update these values when pushing significant changes.
    public static class ProjectVersion
    {
        public const int Major = 1;
        public const int Minor = 2;
        public const int Patch = 0;

        public const string VersionString = "1.2.0";
        public const string ReleaseDate = "2026-01-15";
        public const string ReleaseNotes = "Added data-driven movement types (Chase/Zigzag/Wander) with enum-based configuration";

        public static string FullVersion => $"v{Major}.{Minor}.{Patch}";
    }
}
