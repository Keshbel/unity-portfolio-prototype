using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using VContainer;
using VContainer.Unity;

namespace ExtractionRoom.DI
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
            builder.Register<IDamageService, DamageService>(Lifetime.Singleton);
            builder.Register<IItemDefinitionProvider, ItemDefinitionProvider>(Lifetime.Singleton);
            builder.Register<IInventoryService, InventoryService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
