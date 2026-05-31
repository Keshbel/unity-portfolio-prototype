namespace ExtractionRoom.AI
{
    public sealed class EnemyIdleState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyConfig config;
        private readonly EnemyRuntimeState runtimeState;
        private readonly EnemyPerception perception;
        private float elapsedTime;

        public EnemyIdleState(
            EnemyStateMachine stateMachine,
            EnemyConfig config,
            EnemyRuntimeState runtimeState,
            EnemyPerception perception)
        {
            this.stateMachine = stateMachine;
            this.config = config;
            this.runtimeState = runtimeState;
            this.perception = perception;
        }

        public void Enter()
        {
            elapsedTime = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (perception.CanDetectPlayer(runtimeState))
            {
                stateMachine.TransitionTo(stateMachine.Chase);
                return;
            }

            elapsedTime += deltaTime;
            if (elapsedTime >= config.PatrolWaitTime && runtimeState.PatrolPoints.Length > 0)
            {
                stateMachine.TransitionTo(stateMachine.Patrol);
            }
        }
    }
}
