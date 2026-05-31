using ExtractionRoom.AI;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using NUnit.Framework;
using UnityEngine;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class EnemyStateMachineTests
    {
        private EventBus eventBus;
        private HealthModel playerHealth;
        private EnemyConfig config;
        private EnemyRuntimeState runtimeState;
        private EnemyStateMachine stateMachine;

        [SetUp]
        public void SetUp()
        {
            eventBus = new EventBus();
            playerHealth = new HealthModel(100, eventBus);
            config = ScriptableObject.CreateInstance<EnemyConfig>();
            config.Initialize(50, 2.5f, 6f, 1.5f, 10, 1f, 1f);
            runtimeState = new EnemyRuntimeState(playerHealth, new[] { new Vector3(3f, 0f, 0f) });
            stateMachine = new EnemyStateMachine(config, runtimeState, new DamageService());
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(config);
            playerHealth.Dispose();
            eventBus.Dispose();
        }

        [Test]
        public void Constructor_StartsIdle()
        {
            Assert.That(stateMachine.CurrentState, Is.SameAs(stateMachine.Idle));
        }

        [Test]
        public void Idle_AfterWait_TransitionsToPatrol()
        {
            stateMachine.Tick(1f, Vector3.zero, new Vector3(20f, 0f, 0f));

            Assert.That(stateMachine.CurrentState, Is.SameAs(stateMachine.Patrol));
        }

        [Test]
        public void Patrol_WhenPlayerDetected_TransitionsToChase()
        {
            stateMachine.Tick(1f, Vector3.zero, new Vector3(20f, 0f, 0f));
            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(5f, 0f, 0f));

            Assert.That(stateMachine.CurrentState, Is.SameAs(stateMachine.Chase));
        }

        [Test]
        public void Chase_WhenPlayerInAttackRange_TransitionsToAttack()
        {
            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(5f, 0f, 0f));
            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(1f, 0f, 0f));

            Assert.That(stateMachine.CurrentState, Is.SameAs(stateMachine.Attack));
        }

        [Test]
        public void Attack_AppliesDamageThroughDamageService()
        {
            EnterAttackState();

            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(1f, 0f, 0f));

            Assert.That(playerHealth.CurrentHealth, Is.EqualTo(90));
        }

        [Test]
        public void Attack_WhenPlayerDies_StopsAttacking()
        {
            EnterAttackState();
            playerHealth.ApplyDamage(100);

            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(1f, 0f, 0f));

            Assert.That(stateMachine.CurrentState, Is.SameAs(stateMachine.Idle));
        }

        private void EnterAttackState()
        {
            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(1f, 0f, 0f));
            stateMachine.Tick(0.1f, Vector3.zero, new Vector3(1f, 0f, 0f));
            Assert.That(stateMachine.CurrentState, Is.SameAs(stateMachine.Attack));
        }
    }
}
