using System;
using System.Collections.Generic;
using ExtractionRoom.Core;
using ExtractionRoom.Items;
using R3;

namespace ExtractionRoom.Inventory
{
    public sealed class InventoryService : IInventoryService
    {
        public const int DefaultCapacity = 5;

        private readonly IItemDefinitionProvider itemDefinitionProvider;
        private readonly IEventBus eventBus;
        private readonly List<InventorySlot> slots = new();
        private readonly ReactiveProperty<InventorySnapshot> snapshot;

        public InventoryService(IItemDefinitionProvider itemDefinitionProvider, IEventBus eventBus)
            : this(DefaultCapacity, itemDefinitionProvider, eventBus)
        {
        }

        public InventoryService(int capacity, IItemDefinitionProvider itemDefinitionProvider, IEventBus eventBus)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
            }

            Capacity = capacity;
            this.itemDefinitionProvider = itemDefinitionProvider ?? throw new ArgumentNullException(nameof(itemDefinitionProvider));
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            snapshot = new ReactiveProperty<InventorySnapshot>(CreateSnapshot());
        }

        public int Capacity { get; }

        public InventorySnapshot Snapshot => snapshot.Value;

        public ReadOnlyReactiveProperty<InventorySnapshot> SnapshotObservable => snapshot;

        public InventoryOperationResult AddItem(ItemId itemId, int count)
        {
            if (count <= 0)
            {
                return CreateResult(itemId, count, 0, InventoryOperationStatus.InvalidCount);
            }

            if (!itemDefinitionProvider.TryGetDefinition(itemId, out var definition))
            {
                return CreateResult(itemId, count, 0, InventoryOperationStatus.MissingItemDefinition);
            }

            if (GetAvailableCapacity(itemId, definition.MaxStack) < count)
            {
                return CreateResult(itemId, count, 0, InventoryOperationStatus.InventoryFull);
            }

            var previousSnapshot = Snapshot;
            var remainingCount = count;

            for (var index = 0; index < slots.Count && remainingCount > 0; index++)
            {
                var slot = slots[index];
                if (slot.ItemId != itemId || slot.Count >= definition.MaxStack)
                {
                    continue;
                }

                var addedCount = Math.Min(definition.MaxStack - slot.Count, remainingCount);
                slots[index] = new InventorySlot(itemId, slot.Count + addedCount);
                remainingCount -= addedCount;
            }

            while (remainingCount > 0)
            {
                var addedCount = Math.Min(definition.MaxStack, remainingCount);
                slots.Add(new InventorySlot(itemId, addedCount));
                remainingCount -= addedCount;
            }

            PublishSnapshot(previousSnapshot);
            return CreateResult(itemId, count, count, InventoryOperationStatus.Success);
        }

        public InventoryOperationResult RemoveItem(ItemId itemId, int count)
        {
            if (count <= 0)
            {
                return CreateResult(itemId, count, 0, InventoryOperationStatus.InvalidCount);
            }

            if (!itemDefinitionProvider.TryGetDefinition(itemId, out _))
            {
                return CreateResult(itemId, count, 0, InventoryOperationStatus.MissingItemDefinition);
            }

            if (GetItemCount(itemId) < count)
            {
                return CreateResult(itemId, count, 0, InventoryOperationStatus.InsufficientItems);
            }

            var previousSnapshot = Snapshot;
            var remainingCount = count;

            for (var index = slots.Count - 1; index >= 0 && remainingCount > 0; index--)
            {
                var slot = slots[index];
                if (slot.ItemId != itemId)
                {
                    continue;
                }

                var removedCount = Math.Min(slot.Count, remainingCount);
                var nextCount = slot.Count - removedCount;
                remainingCount -= removedCount;

                if (nextCount == 0)
                {
                    slots.RemoveAt(index);
                }
                else
                {
                    slots[index] = new InventorySlot(itemId, nextCount);
                }
            }

            PublishSnapshot(previousSnapshot);
            return CreateResult(itemId, count, count, InventoryOperationStatus.Success);
        }

        public bool HasItem(ItemId itemId, int count = 1)
        {
            return count > 0 && GetItemCount(itemId) >= count;
        }

        public int GetItemCount(ItemId itemId)
        {
            var totalCount = 0;

            foreach (var slot in slots)
            {
                if (slot.ItemId == itemId)
                {
                    totalCount += slot.Count;
                }
            }

            return totalCount;
        }

        public void Dispose()
        {
            snapshot.Dispose();
        }

        private int GetAvailableCapacity(ItemId itemId, int maxStack)
        {
            var availableCapacity = (Capacity - slots.Count) * maxStack;

            foreach (var slot in slots)
            {
                if (slot.ItemId == itemId)
                {
                    availableCapacity += maxStack - slot.Count;
                }
            }

            return availableCapacity;
        }

        private void PublishSnapshot(InventorySnapshot previousSnapshot)
        {
            var currentSnapshot = CreateSnapshot();
            snapshot.Value = currentSnapshot;
            eventBus.Publish(new InventoryChangedEvent(previousSnapshot, currentSnapshot));
        }

        private InventorySnapshot CreateSnapshot()
        {
            return new InventorySnapshot(slots);
        }

        private static InventoryOperationResult CreateResult(
            ItemId itemId,
            int requestedCount,
            int affectedCount,
            InventoryOperationStatus status)
        {
            return new InventoryOperationResult(itemId, requestedCount, affectedCount, status);
        }
    }
}
