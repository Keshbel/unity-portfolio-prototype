using UnityEngine;

namespace ExtractionRoom.Interaction
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string actionText = "Interact";

        public InteractionPromptData Prompt => new(actionText);

        public abstract void Interact();

        public void SetActionText(string value)
        {
            actionText = value;
        }
    }
}
