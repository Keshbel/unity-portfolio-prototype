using System;
using System.Collections.Generic;

namespace ExtractionRoom.Items
{
    public sealed class ItemDefinitionProvider : IItemDefinitionProvider
    {
        private readonly Dictionary<ItemId, ItemDefinition> definitions = new();

        public ItemDefinitionProvider()
            : this(Array.Empty<ItemDefinition>())
        {
        }

        public ItemDefinitionProvider(IEnumerable<ItemDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            foreach (var definition in definitions)
            {
                if (definition == null)
                {
                    throw new ArgumentException("Item definitions cannot contain null values.", nameof(definitions));
                }

                if (!this.definitions.TryAdd(definition.Id, definition))
                {
                    throw new ArgumentException($"Duplicate item definition id: {definition.Id}.", nameof(definitions));
                }
            }
        }

        public bool TryGetDefinition(ItemId itemId, out ItemDefinition definition)
        {
            return definitions.TryGetValue(itemId, out definition);
        }
    }
}
