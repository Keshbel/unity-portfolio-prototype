using System;
using ExtractionRoom.Gameplay;
using UnityEngine;

namespace ExtractionRoom.AI
{
    public sealed class EnemyStateMachine
    {
        private readonly EnemyRuntimeState runtimeState;
        private IEnemyState currentState;

        public EnemyStateMachine(
            EnemyConfig config,
            EnemyRuntimeState runtimeState,
            IDamageService damageService)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            var perception = new EnemyPerception(config);
            Idle = new EnemyIdleState(this, config, runtimeState, perception);
            Patrol = new EnemyPatrolState(this, runtimeState, perception);
            Chase = new EnemyChaseState(this, runtimeState, perception);
            Attack = new EnemyAttackState(this, config, runtimeState, perception, damageService);
            TransitionTo(Idle);
        }

        public IEnemyState CurrentState => currentState;

        public EnemyIdleState Idle { get; }

        public EnemyPatrolState Patrol { get; }

        public EnemyChaseState Chase { get; }

        public EnemyAttackState Attack { get; }

        public void Tick(float deltaTime, Vector3 enemyPosition, Vector3 playerPosition)
        {
            runtimeState.EnemyPosition = enemyPosition;
            runtimeState.PlayerPosition = playerPosition;
            runtimeState.ShouldMove = false;
            currentState.Tick(deltaTime);
        }

        public void TransitionTo(IEnemyState nextState)
        {
            if (nextState == null || ReferenceEquals(currentState, nextState))
            {
                return;
            }

            currentState = nextState;
            currentState.Enter();
        }
    }
}
