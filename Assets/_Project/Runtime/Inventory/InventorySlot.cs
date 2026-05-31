using ExtractionRoom.Items;

namespace ExtractionRoom.Inventory
{
    public readonly struct InventorySlot
    {
        public InventorySlot(ItemId itemId, int count)
        {
            ItemId = itemId;
            Count = count;
        }

        public ItemId ItemId { get; }

        public int Count { get; }
    }
}
