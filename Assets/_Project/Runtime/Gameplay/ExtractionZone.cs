using ExtractionRoom.Core;
using ExtractionRoom.Objectives;
using ExtractionRoom.Player;
using UnityEngine;
using VContainer;

namespace ExtractionRoom.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public sealed class ExtractionZone : MonoBehaviour
    {
        private IEventBus eventBus;
        private IObjectiveService objectiveService;
        private bool hasPublishedExtraction;

        [Inject]
        public void Construct(IEventBus bus, IObjectiveService objectives)
        {
            eventBus = bus;
            objectiveService = objectives;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasPublishedExtraction ||
                objectiveService.CurrentState.Step != ObjectiveStep.ReachExtraction ||
                other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            hasPublishedExtraction = true;
            eventBus.Publish(new ExtractionReachedEvent());
        }
    }
}
