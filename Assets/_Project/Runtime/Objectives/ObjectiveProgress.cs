namespace ExtractionRoom.Objectives
{
    public readonly struct ObjectiveProgress
    {
        public ObjectiveProgress(int current, int target)
        {
            Current = current;
            Target = target;
        }

        public int Current { get; }

        public int Target { get; }

        public bool IsComplete => Current >= Target;
    }
}
