using UnityEngine;

namespace ExtractionRoom.AI
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Extraction Room/Enemy Config")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [SerializeField, Min(1)]
        private int maxHealth = 50;

        [SerializeField, Min(0f)]
        private float moveSpeed = 2.5f;

        [SerializeField, Min(0f)]
        private float detectionRange = 6f;

        [SerializeField, Min(0f)]
        private float attackRange = 1.5f;

        [SerializeField, Min(0)]
        private int attackDamage = 10;

        [SerializeField, Min(0f)]
        private float attackCooldown = 1f;

        [SerializeField, Min(0f)]
        private float patrolWaitTime = 1f;

        public int MaxHealth => maxHealth;

        public float MoveSpeed => moveSpeed;

        public float DetectionRange => detectionRange;

        public float AttackRange => attackRange;

        public int AttackDamage => attackDamage;

        public float AttackCooldown => attackCooldown;

        public float PatrolWaitTime => patrolWaitTime;

        public void Initialize(
            int health,
            float speed,
            float detection,
            float attack,
            int damage,
            float cooldown,
            float waitTime)
        {
            maxHealth = health;
            moveSpeed = speed;
            detectionRange = detection;
            attackRange = attack;
            attackDamage = damage;
            attackCooldown = cooldown;
            patrolWaitTime = waitTime;
        }
    }
}
