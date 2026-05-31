using System;
using ExtractionRoom.Items;
using R3;

namespace ExtractionRoom.Inventory
{
    public interface IInventoryService : IDisposable
    {
        int Capacity { get; }

        InventorySnapshot Snapshot { get; }

        ReadOnlyReactiveProperty<InventorySnapshot> SnapshotObservable { get; }

        InventoryOperationResult AddItem(ItemId itemId, int count);

        InventoryOperationResult RemoveItem(ItemId itemId, int count);

        bool HasItem(ItemId itemId, int count = 1);

        int GetItemCount(ItemId itemId);
    }
}
