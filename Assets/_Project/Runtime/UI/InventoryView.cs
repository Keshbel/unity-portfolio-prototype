using ExtractionRoom.Inventory;
using UnityEngine;

namespace ExtractionRoom.UI
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private InventorySlotView[] slots = System.Array.Empty<InventorySlotView>();

        private int[] previousCounts = System.Array.Empty<int>();
        private bool hasDisplayedSnapshot;

        public void Configure(InventorySlotView[] inventorySlots)
        {
            slots = inventorySlots;
            previousCounts = new int[slots.Length];
        }

        private void Awake()
        {
            previousCounts = new int[slots.Length];
        }

        public void Display(InventorySnapshot snapshot)
        {
            if (previousCounts.Length != slots.Length)
            {
                previousCounts = new int[slots.Length];
            }

            for (var index = 0; index < slots.Length; index++)
            {
                if (index < snapshot.Slots.Count)
                {
                    var slot = snapshot.Slots[index];
                    var shouldPulse = hasDisplayedSnapshot && slot.Count > previousCounts[index];
                    slots[index].Display(slot.ItemId.ToString(), slot.Count, shouldPulse);
                    previousCounts[index] = slot.Count;
                }
                else
                {
                    slots[index].DisplayEmpty();
                    previousCounts[index] = 0;
                }
            }

            hasDisplayedSnapshot = true;
        }
    }
}
