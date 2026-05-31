using UnityEngine;

namespace ExtractionRoom.AI
{
    public sealed class EnemyPatrolState : IEnemyState
    {
        private const float ArrivalDistance = 0.1f;

        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyRuntimeState runtimeState;
        private readonly EnemyPerception perception;

        public EnemyPatrolState(
            EnemyStateMachine stateMachine,
            EnemyRuntimeState runtimeState,
            EnemyPerception perception)
        {
            this.stateMachine = stateMachine;
            this.runtimeState = runtimeState;
            this.perception = perception;
        }

        public void Enter()
        {
        }

        public void Tick(float deltaTime)
        {
            if (perception.CanDetectPlayer(runtimeState))
            {
                stateMachine.TransitionTo(stateMachine.Chase);
                return;
            }

            if (runtimeState.PatrolPoints.Length == 0)
            {
                stateMachine.TransitionTo(stateMachine.Idle);
                return;
            }

            var destination = runtimeState.PatrolPoints[runtimeState.PatrolPointIndex];
            runtimeState.Destination = destination;
            runtimeState.ShouldMove = true;

            var offset = destination - runtimeState.EnemyPosition;
            offset.y = 0f;
            if (offset.sqrMagnitude <= ArrivalDistance * ArrivalDistance)
            {
                runtimeState.PatrolPointIndex =
                    (runtimeState.PatrolPointIndex + 1) % runtimeState.PatrolPoints.Length;
                stateMachine.TransitionTo(stateMachine.Idle);
            }
        }
    }
}
