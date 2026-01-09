using UnityEngine;
using VampireSurvivor.Events;

namespace VampireSurvivor.Core
{
    // Central game state manager. Handles pause, game over, and restart.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject _gameOverUI;

        public bool IsGameOver { get; private set; }
        public bool IsPaused { get; private set; }
        public float GameTime { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            ServiceLocator.Register(this);
        }

        private void OnEnable()
        {
            EventBus.OnPlayerDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            EventBus.OnPlayerDeath -= HandlePlayerDeath;
        }

        private void Start()
        {
            Debug.Log($"<color=cyan>[VampireSurvivor]</color> {ProjectVersion.FullVersion} ({ProjectVersion.ReleaseDate})");

            IsGameOver = false;
            IsPaused = false;
            GameTime = 0f;
            Time.timeScale = 1f;

            if (_gameOverUI != null)
                _gameOverUI.SetActive(false);
        }

        private void Update()
        {
            if (!IsGameOver && !IsPaused)
            {
                GameTime += Time.deltaTime;
            }
        }

        private void HandlePlayerDeath(PlayerDeathEvent evt)
        {
            TriggerGameOver();
        }

        public void TriggerGameOver()
        {
            if (IsGameOver) return;

            IsGameOver = true;
            Time.timeScale = 0f;

            if (_gameOverUI != null)
                _gameOverUI.SetActive(true);

            EventBus.Publish(new GameOverEvent());
        }

        public void TogglePause()
        {
            if (IsGameOver) return;

            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0f : 1f;
            EventBus.Publish(new GamePausedEvent(IsPaused));
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            EventBus.ClearAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                ServiceLocator.Unregister<GameManager>();
                Instance = null;
            }
        }
    }
}
