using ExtractionRoom.Gameplay;

namespace ExtractionRoom.AI
{
    public sealed class EnemyAttackState : IEnemyState
    {
        private readonly EnemyStateMachine stateMachine;
        private readonly EnemyConfig config;
        private readonly EnemyRuntimeState runtimeState;
        private readonly EnemyPerception perception;
        private readonly IDamageService damageService;
        private float cooldownRemaining;

        public EnemyAttackState(
            EnemyStateMachine stateMachine,
            EnemyConfig config,
            EnemyRuntimeState runtimeState,
            EnemyPerception perception,
            IDamageService damageService)
        {
            this.stateMachine = stateMachine;
            this.config = config;
            this.runtimeState = runtimeState;
            this.perception = perception;
            this.damageService = damageService;
        }

        public void Enter()
        {
            cooldownRemaining = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (runtimeState.PlayerHealth.IsDead)
            {
                stateMachine.TransitionTo(stateMachine.Idle);
                return;
            }

            if (!perception.CanDetectPlayer(runtimeState))
            {
                stateMachine.TransitionTo(stateMachine.Patrol);
                return;
            }

            if (!perception.CanAttackPlayer(runtimeState))
            {
                stateMachine.TransitionTo(stateMachine.Chase);
                return;
            }

            cooldownRemaining -= deltaTime;
            if (cooldownRemaining <= 0f)
            {
                damageService.ApplyDamage(runtimeState.PlayerHealth, config.AttackDamage);
                cooldownRemaining = config.AttackCooldown;
            }
        }
    }
}
