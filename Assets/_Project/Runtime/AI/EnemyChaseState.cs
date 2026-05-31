namespace ExtractionRoom.AI
{
    public sealed class EnemyChaseState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyRuntimeState runtimeState;
        private readonly EnemyPerception perception;

        public EnemyChaseState(
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
            if (!perception.CanDetectPlayer(runtimeState))
            {
                stateMachine.TransitionTo(stateMachine.Patrol);
                return;
            }

            if (perception.CanAttackPlayer(runtimeState))
            {
                stateMachine.TransitionTo(stateMachine.Attack);
                return;
            }

            runtimeState.Destination = runtimeState.PlayerPosition;
            runtimeState.ShouldMove = true;
        }
    }
}
