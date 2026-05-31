namespace ExtractionRoom.Gameplay
{
    public readonly struct EntityDiedEvent
    {
        public EntityDiedEvent(IHealth health)
        {
            Health = health;
        }

        public IHealth Health { get; }
    }
}
