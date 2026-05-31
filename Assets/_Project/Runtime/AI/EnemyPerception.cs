using UnityEngine;

namespace ExtractionRoom.AI
{
    public sealed class EnemyPerception
    {
        private readonly EnemyConfig config;

        public EnemyPerception(EnemyConfig config)
        {
            this.config = config;
        }

        public bool CanDetectPlayer(EnemyRuntimeState runtimeState)
        {
            return !runtimeState.PlayerHealth.IsDead
                && IsWithinRange(runtimeState, config.DetectionRange);
        }

        public bool CanAttackPlayer(EnemyRuntimeState runtimeState)
        {
            return !runtimeState.PlayerHealth.IsDead
                && IsWithinRange(runtimeState, config.AttackRange);
        }

        private static bool IsWithinRange(EnemyRuntimeState runtimeState, float range)
        {
            var offset = runtimeState.PlayerPosition - runtimeState.EnemyPosition;
            offset.y = 0f;
            return offset.sqrMagnitude <= range * range;
        }
    }
}
