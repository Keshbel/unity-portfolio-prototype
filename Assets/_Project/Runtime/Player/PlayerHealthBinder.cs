using System;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using UnityEngine;
using VContainer;

namespace ExtractionRoom.Player
{
    public sealed class PlayerHealthBinder : MonoBehaviour
    {
        [SerializeField, Min(1)]
        private int maximumHealth = 100;

        private IGameStateMachine gameStateMachine;
        private IDisposable deathSubscription;

        public IHealth Health { get; private set; }

        [Inject]
        public void Construct(IEventBus eventBus, IGameStateMachine stateMachine)
        {
            gameStateMachine = stateMachine;
            Health = new HealthModel(maximumHealth, eventBus);
            deathSubscription = eventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
        }

        private void OnDestroy()
        {
            deathSubscription?.Dispose();
            Health?.Dispose();
        }

        private void OnEntityDied(EntityDiedEvent eventData)
        {
            if (ReferenceEquals(eventData.Health, Health))
            {
                gameStateMachine.TryTransitionTo(GameState.Lost);
            }
        }
    }
}
