using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class HealthView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        public void Configure(Text text)
        {
            label = text;
        }

        public void Display(int currentHealth, int maximumHealth)
        {
            label.text = $"Health: {currentHealth}/{maximumHealth}";
        }
    }
}
