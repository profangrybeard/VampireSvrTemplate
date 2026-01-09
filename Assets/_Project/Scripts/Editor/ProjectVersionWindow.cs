using UnityEditor;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Editor
{
    public class ProjectVersionWindow : EditorWindow
    {
        [MenuItem("VampireSurvivor/Project Version")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectVersionWindow>("Project Version");
            window.minSize = new Vector2(300, 150);
            window.maxSize = new Vector2(400, 200);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.LabelField("VampireSurvivor Template", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.LabelField("Version:", ProjectVersion.FullVersion);
            EditorGUILayout.LabelField("Release Date:", ProjectVersion.ReleaseDate);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Release Notes:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(ProjectVersion.ReleaseNotes, MessageType.Info);

            GUILayout.Space(10);
            if (GUILayout.Button("Copy Version to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = ProjectVersion.FullVersion;
                Debug.Log($"Copied version: {ProjectVersion.FullVersion}");
            }
        }
    }
}
