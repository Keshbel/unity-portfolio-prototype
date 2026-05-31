using System;
using R3;

namespace ExtractionRoom.Gameplay
{
    public interface IHealth : IDisposable
    {
        int MaxHealth { get; }

        int CurrentHealth { get; }

        ReadOnlyReactiveProperty<int> CurrentHealthObservable { get; }

        bool IsDead { get; }

        DamageResult ApplyDamage(int damage);

        int Heal(int amount);
    }
}
