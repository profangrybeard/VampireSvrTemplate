using UnityEngine;
using UnityEngine.UI;
using VampireSurvivor.Core;
using VampireSurvivor.Events;

namespace VampireSurvivor.UI
{
    // Health bar UI that responds to player damage events.
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

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
            }
        }
    }
}
