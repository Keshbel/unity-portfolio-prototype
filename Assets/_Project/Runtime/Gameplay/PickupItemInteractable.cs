using System;
using ExtractionRoom.Interaction;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using UnityEngine;
using VContainer;

namespace ExtractionRoom.Gameplay
{
    public sealed class PickupItemInteractable : InteractableBase
    {
        [SerializeField]
        private ItemId itemId;

        [SerializeField, Min(1)]
        private int count = 1;

        private IInventoryService inventoryService;

        public void Configure(ItemId id, int itemCount = 1)
        {
            itemId = id;
            count = itemCount;
        }

        [Inject]
        public void Construct(IInventoryService inventory)
        {
            inventoryService = inventory ?? throw new ArgumentNullException(nameof(inventory));
        }

        public override void Interact()
        {
            var result = inventoryService.AddItem(itemId, count);
            if (result.Succeeded)
            {
                Destroy(gameObject);
                return;
            }

            Debug.LogWarning($"Could not pick up {itemId}: {result.Status}.", this);
        }
    }
}
