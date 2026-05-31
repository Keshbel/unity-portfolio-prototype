using ExtractionRoom.Core;
using ExtractionRoom.Player;
using NUnit.Framework;
using UnityEngine;

namespace ExtractionRoom.Tests.PlayMode
{
    public sealed class PlayerHealthBinderTests
    {
        [Test]
        public void PlayerDeath_TransitionsGameStateToLost()
        {
            var eventBus = new EventBus();
            var stateMachine = new GameStateMachine(eventBus);
            var player = new GameObject("Player");

            try
            {
                var binder = player.AddComponent<PlayerHealthBinder>();
                binder.Construct(eventBus, stateMachine);
                stateMachine.TryTransitionTo(GameState.Playing);

                binder.Health.ApplyDamage(binder.Health.MaxHealth);

                Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Lost));
            }
            finally
            {
                Object.DestroyImmediate(player);
                stateMachine.Dispose();
                eventBus.Dispose();
            }
        }
    }
}
