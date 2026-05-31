using ExtractionRoom.Player;
using R3;
using UnityEngine;

namespace ExtractionRoom.Interaction
{
    public sealed class InteractionController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInputReader inputReader;

        [SerializeField]
        private Transform rayOrigin;

        [SerializeField, Min(0.1f)]
        private float interactionDistance = 3f;

        [SerializeField, Min(0f)]
        private float interactionRadius = 0.35f;

        private readonly ReactiveProperty<InteractionPromptData?> currentPrompt = new();
        private IInteractable currentInteractable;

        public ReadOnlyReactiveProperty<InteractionPromptData?> CurrentPromptObservable => currentPrompt;

        public void Configure(PlayerInputReader reader, Transform origin)
        {
            inputReader = reader;
            rayOrigin = origin;
        }

        private void Awake()
        {
            inputReader ??= GetComponent<PlayerInputReader>();
            rayOrigin ??= transform;
        }

        private void Update()
        {
            UpdateTarget();

            if (inputReader.ConsumeInteractPressed())
            {
                currentInteractable?.Interact();
            }
        }

        private void OnDestroy()
        {
            currentPrompt.Dispose();
        }

        private void UpdateTarget()
        {
            var nextInteractable = TryGetInteractable(out var interactable) ? interactable : null;
            if (ReferenceEquals(nextInteractable, currentInteractable))
            {
                return;
            }

            currentInteractable = nextInteractable;
            currentPrompt.Value = nextInteractable?.Prompt;
        }

        private bool TryGetInteractable(out IInteractable interactable)
        {
            if (TryFindNearestInteractable(
                    Physics.RaycastAll(rayOrigin.position, rayOrigin.forward, interactionDistance),
                    out interactable))
            {
                return true;
            }

            return TryFindNearestInteractable(
                Physics.SphereCastAll(rayOrigin.position, interactionRadius, rayOrigin.forward, interactionDistance),
                out interactable);
        }

        private static bool TryFindNearestInteractable(RaycastHit[] hits, out IInteractable interactable)
        {
            interactable = null;
            var nearestDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                var candidate = hit.collider.GetComponentInParent<IInteractable>();
                if (candidate != null && hit.distance < nearestDistance)
                {
                    nearestDistance = hit.distance;
                    interactable = candidate;
                }
            }

            return interactable != null;
        }
    }
}
