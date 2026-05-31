using ExtractionRoom.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class HealthView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        [SerializeField]
        private DamageFeedbackView damageFeedback;

        private int? previousHealth;

        public void Configure(Text text, DamageFeedbackView feedback)
        {
            label = text;
            damageFeedback = feedback;
        }

        public void Display(int currentHealth, int maximumHealth)
        {
            label.text = $"Health: {currentHealth}/{maximumHealth}";
            if (previousHealth.HasValue && currentHealth < previousHealth.Value)
            {
                damageFeedback.Play();
            }

            previousHealth = currentHealth;
        }
    }
}
