using System.Collections.Generic;
using ExtractionRoom.Core;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using NUnit.Framework;
using UnityEngine;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class InventoryServiceTests
    {
        private readonly List<ItemDefinition> definitions = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var definition in definitions)
            {
                Object.DestroyImmediate(definition);
            }

            definitions.Clear();
        }

        [Test]
        public void AddItem_ToEmptyInventory_AddsSlot()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Fuse, ItemType.Fuse, 3));

            var result = inventory.AddItem(ItemId.Fuse, 1);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.AffectedCount, Is.EqualTo(1));
            Assert.That(inventory.Snapshot.Slots.Count, Is.EqualTo(1));
            Assert.That(inventory.Snapshot.Slots[0].ItemId, Is.EqualTo(ItemId.Fuse));
            Assert.That(inventory.Snapshot.Slots[0].Count, Is.EqualTo(1));
        }

        [Test]
        public void AddItem_WhenStackHasSpace_StacksItem()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Medkit, ItemType.Medkit, 3));
            inventory.AddItem(ItemId.Medkit, 1);

            inventory.AddItem(ItemId.Medkit, 2);

            Assert.That(inventory.Snapshot.Slots.Count, Is.EqualTo(1));
            Assert.That(inventory.Snapshot.Slots[0].Count, Is.EqualTo(3));
        }

        [Test]
        public void AddItem_WhenCountExceedsMaxStack_CreatesAdditionalSlot()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Medkit, ItemType.Medkit, 2));

            inventory.AddItem(ItemId.Medkit, 3);

            Assert.That(inventory.Snapshot.Slots.Count, Is.EqualTo(2));
            Assert.That(inventory.Snapshot.Slots[0].Count, Is.EqualTo(2));
            Assert.That(inventory.Snapshot.Slots[1].Count, Is.EqualTo(1));
        }

        [Test]
        public void RemoveItem_WhenEnoughItemsExist_RemovesRequestedCount()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Fuse, ItemType.Fuse, 3));
            inventory.AddItem(ItemId.Fuse, 3);

            var result = inventory.RemoveItem(ItemId.Fuse, 2);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.AffectedCount, Is.EqualTo(2));
            Assert.That(inventory.GetItemCount(ItemId.Fuse), Is.EqualTo(1));
        }

        [Test]
        public void HasItem_ReportsWhetherRequestedCountExists()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Keycard, ItemType.Keycard, 1));
            inventory.AddItem(ItemId.Keycard, 1);

            Assert.That(inventory.HasItem(ItemId.Keycard), Is.True);
            Assert.That(inventory.HasItem(ItemId.Keycard, 2), Is.False);
        }

        [Test]
        public void GetItemCount_SumsMatchingStacks()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Fuse, ItemType.Fuse, 2));

            inventory.AddItem(ItemId.Fuse, 3);

            Assert.That(inventory.GetItemCount(ItemId.Fuse), Is.EqualTo(3));
        }

        [Test]
        public void AddItem_WhenInventoryIsFull_ReturnsFailureWithoutChangingState()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(
                eventBus,
                1,
                CreateDefinition(ItemId.Keycard, ItemType.Keycard, 1),
                CreateDefinition(ItemId.Fuse, ItemType.Fuse, 1));
            inventory.AddItem(ItemId.Keycard, 1);

            var result = inventory.AddItem(ItemId.Fuse, 1);

            Assert.That(result.Status, Is.EqualTo(InventoryOperationStatus.InventoryFull));
            Assert.That(result.AffectedCount, Is.Zero);
            Assert.That(inventory.GetItemCount(ItemId.Keycard), Is.EqualTo(1));
            Assert.That(inventory.GetItemCount(ItemId.Fuse), Is.Zero);
        }

        [Test]
        public void AddItem_WithInvalidCount_ReturnsFailure()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Fuse, ItemType.Fuse, 3));

            var result = inventory.AddItem(ItemId.Fuse, 0);

            Assert.That(result.Status, Is.EqualTo(InventoryOperationStatus.InvalidCount));
            Assert.That(inventory.Snapshot.Slots, Is.Empty);
        }

        [Test]
        public void AddItem_WithMissingDefinition_ReturnsFailure()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus);

            var result = inventory.AddItem(ItemId.Fuse, 1);

            Assert.That(result.Status, Is.EqualTo(InventoryOperationStatus.MissingItemDefinition));
            Assert.That(inventory.Snapshot.Slots, Is.Empty);
        }

        [Test]
        public void AddItem_WhenStateChanges_PublishesSnapshotAndEvent()
        {
            using var eventBus = new EventBus();
            using var inventory = CreateInventory(eventBus, CreateDefinition(ItemId.Fuse, ItemType.Fuse, 3));
            InventoryChangedEvent? publishedEvent = null;
            using var subscription = eventBus.Subscribe<InventoryChangedEvent>(value => publishedEvent = value);

            inventory.AddItem(ItemId.Fuse, 1);

            Assert.That(inventory.SnapshotObservable.CurrentValue, Is.SameAs(inventory.Snapshot));
            Assert.That(publishedEvent?.PreviousSnapshot.Slots, Is.Empty);
            Assert.That(publishedEvent?.CurrentSnapshot, Is.SameAs(inventory.Snapshot));
        }

        [Test]
        public void TryGetDefinition_WithMissingId_ReturnsFalse()
        {
            var provider = new ItemDefinitionProvider();

            var found = provider.TryGetDefinition(ItemId.Fuse, out var definition);

            Assert.That(found, Is.False);
            Assert.That(definition, Is.Null);
        }

        private InventoryService CreateInventory(EventBus eventBus, params ItemDefinition[] itemDefinitions)
        {
            return CreateInventory(eventBus, InventoryService.DefaultCapacity, itemDefinitions);
        }

        private static InventoryService CreateInventory(
            EventBus eventBus,
            int capacity,
            params ItemDefinition[] itemDefinitions)
        {
            return new InventoryService(capacity, new ItemDefinitionProvider(itemDefinitions), eventBus);
        }

        private ItemDefinition CreateDefinition(ItemId itemId, ItemType itemType, int maxStack)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            definition.Initialize(itemId, itemId.ToString(), itemType, maxStack);
            definitions.Add(definition);
            return definition;
        }
    }
}
