using ExtractionRoom.Items;

namespace ExtractionRoom.Inventory
{
    public readonly struct InventoryOperationResult
    {
        public InventoryOperationResult(ItemId itemId, int requestedCount, int affectedCount, InventoryOperationStatus status)
        {
            ItemId = itemId;
            RequestedCount = requestedCount;
            AffectedCount = affectedCount;
            Status = status;
        }

        public ItemId ItemId { get; }

        public int RequestedCount { get; }

        public int AffectedCount { get; }

        public InventoryOperationStatus Status { get; }

        public bool Succeeded => Status == InventoryOperationStatus.Success;
    }
}
