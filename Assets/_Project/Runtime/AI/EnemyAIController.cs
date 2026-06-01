using System;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Player;
using UnityEngine;
using VContainer;

namespace ExtractionRoom.AI
{
    public sealed class EnemyAIController : MonoBehaviour
    {
        [SerializeField]
        private EnemyConfig config;

        [SerializeField]
        private Transform[] patrolPoints = System.Array.Empty<Transform>();

        private EnemyRuntimeState runtimeState;
        private EnemyStateMachine stateMachine;
        private Transform player;
        private IHealth health;

        public IEnemyState CurrentState => stateMachine?.CurrentState;

        public IHealth Health => health;

        public void Configure(EnemyConfig enemyConfig, Transform[] points)
        {
            config = enemyConfig ?? throw new ArgumentNullException(nameof(enemyConfig));
            patrolPoints = points ?? throw new ArgumentNullException(nameof(points));
        }

        [Inject]
        public void Construct(IDamageService damageService, IEventBus eventBus, PlayerHealthBinder playerHealthBinder)
        {
            if (damageService == null)
            {
                throw new ArgumentNullException(nameof(damageService));
            }

            if (eventBus == null)
            {
                throw new ArgumentNullException(nameof(eventBus));
            }

            if (playerHealthBinder == null)
            {
                throw new ArgumentNullException(nameof(playerHealthBinder));
            }

            if (config == null)
            {
                throw new MissingReferenceException("Enemy config is not assigned.");
            }

            if (playerHealthBinder.Health == null)
            {
                throw new InvalidOperationException("Player health must be constructed before enemy AI.");
            }

            player = playerHealthBinder.transform;
            health = new HealthModel(config.MaxHealth, eventBus);
            var positions = new Vector3[patrolPoints.Length];
            for (var index = 0; index < patrolPoints.Length; index++)
            {
                if (patrolPoints[index] == null)
                {
                    throw new MissingReferenceException($"Enemy patrol point at index {index} is missing.");
                }

                positions[index] = patrolPoints[index].position;
            }

            runtimeState = new EnemyRuntimeState(playerHealthBinder.Health, positions);
            stateMachine = new EnemyStateMachine(config, runtimeState, damageService);
        }

        private void Update()
        {
            if (stateMachine == null || health.IsDead)
            {
                return;
            }

            stateMachine.Tick(Time.deltaTime, transform.position, player.position);
            if (!runtimeState.ShouldMove)
            {
                return;
            }

            var destination = runtimeState.Destination;
            destination.y = transform.position.y;
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                config.MoveSpeed * Time.deltaTime);
            var direction = destination - transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.forward = direction.normalized;
            }
        }

        private void OnDestroy()
        {
            health?.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            if (config == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.DetectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.AttackRange);
            Gizmos.color = Color.cyan;
            foreach (var point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.15f);
                }
            }
        }
    }
}
