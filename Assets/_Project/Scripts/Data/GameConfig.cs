using UnityEngine;

namespace VampireSurvivor.Data
{
    // Global game configuration.
    [CreateAssetMenu(fileName = "GameConfig", menuName = "VampireSurvivor/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Player Settings")]
        [Min(0)] public float PlayerMoveSpeed = 5f;
        [Min(1)] public float PlayerMaxHealth = 100f;

        [Header("Camera Settings")]
        [Min(0)] public float CameraFollowSpeed = 5f;
        [Min(1)] public float CameraOrthoSize = 8f;

        [Header("Game Bounds")]
        public Vector2 PlayAreaSize = new Vector2(50f, 50f);

        [Header("Starting Equipment")]
        public WeaponData StartingWeapon;

        [Header("Wave Progression")]
        public WaveData[] Waves;
        [Min(0)] public float TimeBetweenWaves = 5f;
    }
}
