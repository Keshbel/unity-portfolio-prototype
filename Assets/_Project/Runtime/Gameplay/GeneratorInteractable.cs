using ExtractionRoom.Core;
using ExtractionRoom.Interaction;
using UnityEngine;
using VContainer;

namespace ExtractionRoom.Gameplay
{
    public sealed class GeneratorInteractable : InteractableBase
    {
        private IEventBus eventBus;
        private bool isActivated;

        [Inject]
        public void Construct(IEventBus bus)
        {
            eventBus = bus;
        }

        public override void Interact()
        {
            if (isActivated)
            {
                return;
            }

            isActivated = true;
            eventBus.Publish(new GeneratorActivatedEvent());
        }
    }
}
