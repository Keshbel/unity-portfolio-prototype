namespace ExtractionRoom.Items
{
    public interface IItemDefinitionProvider
    {
        bool TryGetDefinition(ItemId itemId, out ItemDefinition definition);
    }
}
