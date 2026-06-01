using System;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using NUnit.Framework;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class DamageServiceTests
    {
        [Test]
        public void ApplyDamage_WithMissingHealth_Throws()
        {
            var damageService = new DamageService();

            Assert.Throws<ArgumentNullException>(() => damageService.ApplyDamage(null, 1));
        }

        [Test]
        public void ApplyDamage_WithZeroDamage_ReturnsUnappliedResult()
        {
            using var eventBus = new EventBus();
            using var health = new HealthModel(100, eventBus);
            var damageService = new DamageService();

            var result = damageService.ApplyDamage(health, 0);

            Assert.That(result.RequestedDamage, Is.Zero);
            Assert.That(result.AppliedDamage, Is.Zero);
            Assert.That(result.CurrentHealth, Is.EqualTo(100));
            Assert.That(result.WasApplied, Is.False);
            Assert.That(result.WasLethal, Is.False);
        }
    }
}
