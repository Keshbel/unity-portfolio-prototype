namespace ExtractionRoom.Gameplay
{
    public readonly struct DamageResult
    {
        public DamageResult(int requestedDamage, int appliedDamage, int previousHealth, int currentHealth)
        {
            RequestedDamage = requestedDamage;
            AppliedDamage = appliedDamage;
            PreviousHealth = previousHealth;
            CurrentHealth = currentHealth;
        }

        public int RequestedDamage { get; }

        public int AppliedDamage { get; }

        public int PreviousHealth { get; }

        public int CurrentHealth { get; }

        public bool WasApplied => AppliedDamage > 0;

        public bool WasLethal => PreviousHealth > 0 && CurrentHealth == 0;
    }
}
