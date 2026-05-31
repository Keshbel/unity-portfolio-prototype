using System;
using UnityEngine;

namespace ExtractionRoom.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField]
        private HealthView healthView;

        [SerializeField]
        private ObjectiveView objectiveView;

        [SerializeField]
        private InventoryView inventoryView;

        [SerializeField]
        private InteractionPromptView interactionPromptView;

        [SerializeField]
        private EndGameView endGameView;

        private IDisposable presenters;

        public HealthView HealthView => healthView;

        public ObjectiveView ObjectiveView => objectiveView;

        public InventoryView InventoryView => inventoryView;

        public InteractionPromptView InteractionPromptView => interactionPromptView;

        public EndGameView EndGameView => endGameView;

        public void Configure(
            HealthView health,
            ObjectiveView objective,
            InventoryView inventory,
            InteractionPromptView interactionPrompt,
            EndGameView endGame)
        {
            healthView = health;
            objectiveView = objective;
            inventoryView = inventory;
            interactionPromptView = interactionPrompt;
            endGameView = endGame;
        }

        public void Bind(IDisposable ownedPresenters)
        {
            presenters?.Dispose();
            presenters = ownedPresenters;
        }

        private void OnDestroy()
        {
            presenters?.Dispose();
        }
    }
}
