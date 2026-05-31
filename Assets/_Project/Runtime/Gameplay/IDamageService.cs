namespace ExtractionRoom.Gameplay
{
    public interface IDamageService
    {
        DamageResult ApplyDamage(IHealth health, int damage);
    }
}
