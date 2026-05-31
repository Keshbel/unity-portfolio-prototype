namespace ExtractionRoom.Gameplay
{
    public readonly struct HealthChangedEvent
    {
        public HealthChangedEvent(IHealth health, int previousHealth, int currentHealth)
        {
            Health = health;
            PreviousHealth = previousHealth;
            CurrentHealth = currentHealth;
        }

        public IHealth Health { get; }

        public int PreviousHealth { get; }

        public int CurrentHealth { get; }
    }
}
