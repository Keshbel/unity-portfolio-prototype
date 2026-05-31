using System;
using UnityEngine;

namespace ExtractionRoom.Items
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Extraction Room/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [field: SerializeField]
        public ItemId Id { get; private set; }

        [field: SerializeField]
        public string DisplayName { get; private set; } = string.Empty;

        [field: SerializeField]
        public ItemType ItemType { get; private set; }

        [field: SerializeField, Min(1)]
        public int MaxStack { get; private set; } = 1;

        [field: SerializeField]
        public Sprite Icon { get; private set; }

        public void Initialize(ItemId id, string displayName, ItemType itemType, int maxStack, Sprite icon = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
            }

            if (maxStack <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxStack), "Maximum stack must be greater than zero.");
            }

            Id = id;
            DisplayName = displayName;
            ItemType = itemType;
            MaxStack = maxStack;
            Icon = icon;
        }

        private void OnValidate()
        {
            MaxStack = Math.Max(1, MaxStack);
        }
    }
}
