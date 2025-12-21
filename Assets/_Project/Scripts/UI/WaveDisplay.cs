using TMPro;
using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Events;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Displays current wave number.
    /// </summary>
    public class WaveDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private string _format = "Wave {0}";

        private void OnEnable()
        {
            EventBus.OnWaveStarted += HandleWaveStarted;
        }

        private void OnDisable()
        {
            EventBus.OnWaveStarted -= HandleWaveStarted;
        }

        private void Start()
        {
            UpdateDisplay(1);
        }

        private void HandleWaveStarted(WaveStartedEvent evt)
        {
            UpdateDisplay(evt.WaveNumber);
        }

        private void UpdateDisplay(int waveNumber)
        {
            if (_waveText != null)
            {
                _waveText.text = string.Format(_format, waveNumber);
            }
        }
    }
}
