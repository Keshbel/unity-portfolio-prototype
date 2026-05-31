namespace ExtractionRoom.Objectives
{
    public readonly struct ObjectiveChangedEvent
    {
        public ObjectiveChangedEvent(ObjectiveState previousState, ObjectiveState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
        }

        public ObjectiveState PreviousState { get; }

        public ObjectiveState CurrentState { get; }
    }
}
