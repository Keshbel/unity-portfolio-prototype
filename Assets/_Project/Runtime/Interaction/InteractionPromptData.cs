namespace ExtractionRoom.Interaction
{
    public readonly struct InteractionPromptData
    {
        public InteractionPromptData(string actionText)
        {
            ActionText = actionText;
        }

        public string ActionText { get; }
    }
}
