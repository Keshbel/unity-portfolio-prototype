using System;
using ExtractionRoom.Core;
using R3;

namespace ExtractionRoom.Gameplay
{
    public sealed class HealthModel : IHealth
    {
        private readonly IEventBus eventBus;
        private readonly ReactiveProperty<int> currentHealth;
        private bool hasPublishedDeath;

        public HealthModel(int maxHealth, IEventBus eventBus)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Maximum health must be greater than zero.");
            }

            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            MaxHealth = maxHealth;
            currentHealth = new ReactiveProperty<int>(maxHealth);
        }

        public int MaxHealth { get; }

        public int CurrentHealth => currentHealth.Value;

        public ReadOnlyReactiveProperty<int> CurrentHealthObservable => currentHealth;

        public bool IsDead => CurrentHealth == 0;

        public DamageResult ApplyDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            }

            var previousHealth = CurrentHealth;
            var nextHealth = Math.Max(0, previousHealth - damage);

            SetCurrentHealth(nextHealth);

            return new DamageResult(damage, previousHealth - nextHealth, previousHealth, nextHealth);
        }

        public int Heal(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount cannot be negative.");
            }

            var previousHealth = CurrentHealth;
            var nextHealth = (int)Math.Min(MaxHealth, (long)previousHealth + amount);

            SetCurrentHealth(nextHealth);

            return nextHealth - previousHealth;
        }

        public void Dispose()
        {
            currentHealth.Dispose();
        }

        private void SetCurrentHealth(int nextHealth)
        {
            if (nextHealth == CurrentHealth)
            {
                return;
            }

            var previousHealth = CurrentHealth;
            currentHealth.Value = nextHealth;
            eventBus.Publish(new HealthChangedEvent(this, previousHealth, nextHealth));

            if (nextHealth == 0 && !hasPublishedDeath)
            {
                hasPublishedDeath = true;
                eventBus.Publish(new EntityDiedEvent(this));
            }
        }
    }
}
