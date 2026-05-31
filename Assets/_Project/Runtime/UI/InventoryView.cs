using ExtractionRoom.Inventory;
using UnityEngine;

namespace ExtractionRoom.UI
{
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private InventorySlotView[] slots = System.Array.Empty<InventorySlotView>();

        public void Configure(InventorySlotView[] inventorySlots)
        {
            slots = inventorySlots;
        }

        public void Display(InventorySnapshot snapshot)
        {
            for (var index = 0; index < slots.Length; index++)
            {
                if (index < snapshot.Slots.Count)
                {
                    var slot = snapshot.Slots[index];
                    slots[index].Display(slot.ItemId.ToString(), slot.Count);
                }
                else
                {
                    slots[index].DisplayEmpty();
                }
            }
        }
    }
}
