using UnityEngine;
using UnityEngine.UI;
using VampireSurvivor.Core;
using VampireSurvivor.Events;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Simple health bar UI that responds to player damage events.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private Gradient _healthGradient;

        private void OnEnable()
        {
            EventBus.OnPlayerDamaged += HandlePlayerDamaged;
        }

        private void OnDisable()
        {
            EventBus.OnPlayerDamaged -= HandlePlayerDamaged;
        }

        private void HandlePlayerDamaged(PlayerDamagedEvent evt)
        {
            float percent = evt.CurrentHealth / evt.MaxHealth;
            UpdateHealthBar(percent);
        }

        public void UpdateHealthBar(float percent)
        {
            percent = Mathf.Clamp01(percent);

            if (_fillImage != null)
            {
                _fillImage.fillAmount = percent;

                if (_healthGradient != null)
                {
                    _fillImage.color = _healthGradient.Evaluate(percent);
                }
            }
        }
    }
}
