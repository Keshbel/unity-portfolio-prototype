using System;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using NUnit.Framework;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class HealthModelTests
    {
        [Test]
        public void Constructor_StartsAtMaximumHealth()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);

            Assert.That(health.MaxHealth, Is.EqualTo(100));
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
            Assert.That(health.CurrentHealthObservable.CurrentValue, Is.EqualTo(100));
            Assert.That(health.IsDead, Is.False);
        }

        [Test]
        public void Constructor_WithInvalidMaximumHealth_Throws()
        {
            using var eventBus = new EventBus();

            Assert.Throws<ArgumentOutOfRangeException>(() => new HealthModel(0, eventBus));
        }

        [Test]
        public void ApplyDamage_ReducesHealth()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);

            health.ApplyDamage(25);

            Assert.That(health.CurrentHealth, Is.EqualTo(75));
        }

        [Test]
        public void ApplyDamage_ClampsHealthToZero()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);

            health.ApplyDamage(125);

            Assert.That(health.CurrentHealth, Is.Zero);
            Assert.That(health.IsDead, Is.True);
        }

        [Test]
        public void ApplyDamage_WithNegativeDamage_Throws()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);

            Assert.Throws<ArgumentOutOfRangeException>(() => health.ApplyDamage(-1));
        }

        [Test]
        public void Heal_RestoresHealth()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);
            health.ApplyDamage(50);

            var restoredHealth = health.Heal(20);

            Assert.That(restoredHealth, Is.EqualTo(20));
            Assert.That(health.CurrentHealth, Is.EqualTo(70));
        }

        [Test]
        public void Heal_ClampsHealthToMaximum()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);
            health.ApplyDamage(10);

            var restoredHealth = health.Heal(25);

            Assert.That(restoredHealth, Is.EqualTo(10));
            Assert.That(health.CurrentHealth, Is.EqualTo(100));
        }

        [Test]
        public void Heal_WithNegativeAmount_Throws()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);

            Assert.Throws<ArgumentOutOfRangeException>(() => health.Heal(-1));
        }

        [Test]
        public void ApplyDamage_WhenHealthReachesZero_PublishesDeathOnce()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);
            var deathEventCount = 0;
            using var subscription = eventBus.Subscribe<EntityDiedEvent>(_ => deathEventCount++);

            health.ApplyDamage(100);
            health.Heal(100);
            health.ApplyDamage(100);

            Assert.That(deathEventCount, Is.EqualTo(1));
        }

        [Test]
        public void ApplyDamage_PublishesHealthChangedEvent()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);
            HealthChangedEvent? publishedEvent = null;
            using var subscription = eventBus.Subscribe<HealthChangedEvent>(value => publishedEvent = value);

            health.ApplyDamage(25);

            Assert.That(publishedEvent?.Health, Is.SameAs(health));
            Assert.That(publishedEvent?.PreviousHealth, Is.EqualTo(100));
            Assert.That(publishedEvent?.CurrentHealth, Is.EqualTo(75));
        }

        [Test]
        public void DamageService_ApplyDamage_ReturnsExpectedResult()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);
            var damageService = new DamageService();

            var result = damageService.ApplyDamage(health, 125);

            Assert.That(result.RequestedDamage, Is.EqualTo(125));
            Assert.That(result.AppliedDamage, Is.EqualTo(100));
            Assert.That(result.PreviousHealth, Is.EqualTo(100));
            Assert.That(result.CurrentHealth, Is.Zero);
            Assert.That(result.WasApplied, Is.True);
            Assert.That(result.WasLethal, Is.True);
        }
    }
}
