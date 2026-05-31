namespace ExtractionRoom.Interaction
{
    public interface IInteractable
    {
        InteractionPromptData Prompt { get; }

        void Interact();
    }
}
