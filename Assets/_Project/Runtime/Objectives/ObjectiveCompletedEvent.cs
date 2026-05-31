namespace ExtractionRoom.Objectives
{
    public readonly struct ObjectiveCompletedEvent
    {
        public ObjectiveCompletedEvent(ObjectiveState completedObjective)
        {
            CompletedObjective = completedObjective;
        }

        public ObjectiveState CompletedObjective { get; }
    }
}
