using UnityEngine;
using UnityEngine.UI;

namespace HealthSystem
{
    public class HealthFillView : HealthView
    {
        [SerializeField]
        private Image fillImage;

        protected override void Refresh()
        {
            if (CurrentHealth == null || fillImage == null)
                return;

            if (CurrentHealth.MaxHealth <= 0)
            {
                fillImage.fillAmount = 0;
                return;
            }

            fillImage.fillAmount = Mathf.Clamp01(CurrentHealth.CurrHealth / CurrentHealth.MaxHealth);
        }
    }
}
