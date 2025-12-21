using UnityEngine;
using UnityEngine.UI;
using VampireSurvivor.Core;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Game over screen with restart button.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;

        private void Start()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        private void OnRestartClicked()
        {
            var gameManager = ServiceLocator.Get<GameManager>();
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
            else
            {
                // Fallback
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
        }

        private void OnDestroy()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(OnRestartClicked);
            }
        }
    }
}
