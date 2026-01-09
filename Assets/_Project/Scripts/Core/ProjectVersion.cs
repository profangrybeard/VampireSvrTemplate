namespace VampireSurvivor.Core
{
    // Project version tracking for students.
    // Update these values when pushing significant changes.
    public static class ProjectVersion
    {
        public const int Major = 0;
        public const int Minor = 2;
        public const int Patch = 0;

        public const string VersionString = "0.2.0";
        public const string ReleaseDate = "2026-01-09";
        public const string ReleaseNotes = "Added weighted enemy spawn system (Sequential/Interleaved/PureWeighted modes)";

        public static string FullVersion => $"v{Major}.{Minor}.{Patch}";
    }
}
