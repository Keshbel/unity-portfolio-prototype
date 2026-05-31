namespace ExtractionRoom.Objectives
{
    public readonly struct ObjectiveState
    {
        public ObjectiveState(ObjectiveStep step, string text, ObjectiveProgress progress)
        {
            Step = step;
            Text = text;
            Progress = progress;
        }

        public ObjectiveStep Step { get; }

        public string Text { get; }

        public ObjectiveProgress Progress { get; }

        public bool IsCompleted => Step == ObjectiveStep.Completed;
    }
}
