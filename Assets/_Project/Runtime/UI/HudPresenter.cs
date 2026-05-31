using System;
using System.Collections.Generic;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Interaction;
using ExtractionRoom.Inventory;
using ExtractionRoom.Objectives;
using R3;

namespace ExtractionRoom.UI
{
    public sealed class HudPresenter : IDisposable
    {
        private readonly List<IDisposable> subscriptions = new();

        public HudPresenter(
            IHealth playerHealth,
            IInventoryService inventoryService,
            IObjectiveService objectiveService,
            InteractionController interactionController,
            HudView view)
        {
            if (playerHealth == null)
            {
                throw new ArgumentNullException(nameof(playerHealth));
            }

            if (inventoryService == null)
            {
                throw new ArgumentNullException(nameof(inventoryService));
            }

            if (objectiveService == null)
            {
                throw new ArgumentNullException(nameof(objectiveService));
            }

            if (interactionController == null)
            {
                throw new ArgumentNullException(nameof(interactionController));
            }

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            subscriptions.Add(playerHealth.CurrentHealthObservable.Subscribe(
                health => view.HealthView.Display(health, playerHealth.MaxHealth)));
            subscriptions.Add(inventoryService.SnapshotObservable.Subscribe(view.InventoryView.Display));
            subscriptions.Add(objectiveService.CurrentObjectiveTextObservable.Subscribe(view.ObjectiveView.Display));
            subscriptions.Add(interactionController.CurrentPromptObservable.Subscribe(view.InteractionPromptView.Display));
        }

        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();
        }
    }
}
