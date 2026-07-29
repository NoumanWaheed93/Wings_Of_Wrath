using TMPro;
using UnityEngine;

namespace HealthSystem
{
    public class HealthTextView : HealthView
    {
        [SerializeField]
        private TMP_Text healthText;

        protected override void Refresh()
        {
            if (CurrentHealth == null || healthText == null)
                return;

            healthText.text = Mathf.CeilToInt(CurrentHealth.CurrHealth) + "/" + Mathf.CeilToInt(CurrentHealth.MaxHealth);
        }
    }
}
