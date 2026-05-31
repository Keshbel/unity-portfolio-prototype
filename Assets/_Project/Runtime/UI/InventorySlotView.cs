using UnityEngine;
using UnityEngine.UI;

namespace ExtractionRoom.UI
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        public void Configure(Text text)
        {
            label = text;
        }

        public void Display(string itemId, int count)
        {
            label.text = $"{itemId} x{count}";
        }

        public void DisplayEmpty()
        {
            label.text = "-";
        }
    }
}
