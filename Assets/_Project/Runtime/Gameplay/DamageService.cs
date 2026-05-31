using System;

namespace ExtractionRoom.Gameplay
{
    public sealed class DamageService : IDamageService
    {
        public DamageResult ApplyDamage(IHealth health, int damage)
        {
            if (health == null)
            {
                throw new ArgumentNullException(nameof(health));
            }

            return health.ApplyDamage(damage);
        }
    }
}
