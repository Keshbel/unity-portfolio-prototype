using System;
using System.Collections.Generic;

namespace ExtractionRoom.Inventory
{
    public sealed class InventorySnapshot
    {
        private readonly IReadOnlyList<InventorySlot> slots;

        public InventorySnapshot(IEnumerable<InventorySlot> slots)
        {
            if (slots == null)
            {
                throw new ArgumentNullException(nameof(slots));
            }

            var slotArray = slots is InventorySlot[] sourceArray
                ? (InventorySlot[])sourceArray.Clone()
                : new List<InventorySlot>(slots).ToArray();
            this.slots = Array.AsReadOnly(slotArray);
        }

        public IReadOnlyList<InventorySlot> Slots => slots;
    }
}
