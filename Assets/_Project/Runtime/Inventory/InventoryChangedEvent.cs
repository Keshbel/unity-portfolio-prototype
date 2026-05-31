namespace ExtractionRoom.Inventory
{
    public readonly struct InventoryChangedEvent
    {
        public InventoryChangedEvent(InventorySnapshot previousSnapshot, InventorySnapshot currentSnapshot)
        {
            PreviousSnapshot = previousSnapshot;
            CurrentSnapshot = currentSnapshot;
        }

        public InventorySnapshot PreviousSnapshot { get; }

        public InventorySnapshot CurrentSnapshot { get; }
    }
}
