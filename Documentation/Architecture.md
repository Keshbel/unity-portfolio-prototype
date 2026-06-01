# Architecture

## Current State

The repository contains the initial dependency injection foundation, small
health, damage, inventory, and objective domain models, a placeholder playable
scene, an explicit enemy AI state machine, and a reactive UGUI HUD.

## Intended Boundaries

Runtime code belongs to `Project.Runtime`. Editor-only tooling belongs to
`Project.Editor`, which references `Project.Runtime`. EditMode and PlayMode
tests have separate assemblies and reference `Project.Runtime`.

The runtime folders reserve small, explicit areas for the vertical slice:

| Folder | Intended responsibility |
| --- | --- |
| `Core` | Shared domain primitives and application contracts |
| `DI` | VContainer composition roots and registrations |
| `Gameplay` | Plain C# gameplay models and application services |
| `Player` | Player-facing domain logic and Unity adapters |
| `Interaction` | Interaction contracts and implementations |
| `Inventory` | Plain C# inventory state and rules |
| `Items` | Item definitions and configuration adapters |
| `AI` | Minimal extraction-room AI behavior |
| `Objectives` | Objective state and completion rules |
| `UI` | UGUI Views, Presenters, and Binders |
| `Presentation` | LitMotion-driven visual feedback |

## Dependency Rules

- Use VContainer as the composition root. Do not use global singletons.
- Keep gameplay and runtime state in plain C# classes where possible.
- Use `ScriptableObject` assets for configuration only.
- Keep `MonoBehaviour` classes thin and Unity-facing.
- Keep gameplay logic out of UI Views.
- Use Presenter or Binder classes between services and Views.
- Use R3 for observation and bindings, not as a replacement for domain logic.
- Use UniTask for valuable async workflows and pass `CancellationToken`.
- Use LitMotion only for presentation. Gameplay state must not wait on tweens.

## Composition Root

`GameLifetimeScope` is the VContainer composition root for the vertical slice.
It registers the typed `IEventBus`, the `IGameStateMachine`, the
`IDamageService`, the `IGameInitializationService`, the
`ISceneLoadingService`, the `IItemDefinitionProvider`, the
`IInventoryService`, the `IObjectiveService`, and the `GameEntryPoint`. The
scope is intentionally the single place where runtime services are wired
together, so dependencies stay explicit and testable.

`GameLifetimeScope` is a thin Unity-facing component attached to the bootstrap
scene. It explicitly injects the scene adapters configured by the editor
bootstrap tool. Runtime services remain plain C# classes and do not discover
each other through global singletons or scene searches.

`GameEntryPoint` is registered as a VContainer entry point. When its VContainer
async start lifecycle runs, it awaits `IGameInitializationService` and advances
the state machine from `Bootstrapping` to `Playing` only after initialization
completes.

## Async Initialization And Scene Flow

`GameInitializationService` demonstrates a small UniTask initialization
workflow with a cancellable non-blocking demo delay. `GameEntryPoint`
implements VContainer's `IAsyncStartable`, so scope disposal cancels work
through the token supplied by VContainer. Cancellation is handled without
transitioning the game to `Playing`, while unexpected failures are logged.

`SceneLoadingService` exposes Unity scene loading as an explicit
`ISceneLoadingService` dependency and awaits the Unity `AsyncOperation` through
UniTask. The service accepts a `CancellationToken`; it does not use coroutines
or block the main thread.

## Initial Game State Flow

```text
Bootstrapping -> Playing -> Won
                         -> Lost
```

Duplicate transitions are ignored, and `Won` and `Lost` are terminal states.
Successful transitions update an R3 read-only reactive property and publish a
typed `GameStateChangedEvent` through the event bus.

## Gameplay Domain Models

`HealthModel` is a plain C# runtime model. It owns health state, clamps damage
and healing, exposes current health through an R3 read-only reactive property,
and publishes typed health and death events. `DamageService` applies damage
through the `IHealth` contract and returns a `DamageResult` without knowing
about scene objects, controllers, UI, or animations.

Future `MonoBehaviour` components should remain thin Unity-facing adapters or
views. They may forward Unity callbacks into domain services and present model
state, but gameplay rules stay in testable plain C# classes. This keeps the
health and damage rules verifiable in EditMode without loading a scene.

## Item Configuration And Inventory State

`ItemDefinition` assets contain configuration only: stable id, display name,
item type, maximum stack size, and optional icon. Runtime item counts never
live in `ScriptableObject` assets. `ItemDefinitionProvider` resolves config by
id and handles unknown ids without throwing.

`InventoryService` owns runtime slots in plain C# memory. It applies
fixed-capacity stacking and removal rules, returns explicit operation results,
exposes immutable snapshots through an R3 read-only reactive property, and
publishes typed `InventoryChangedEvent` messages. It has no UI or scene-object
dependencies, so its behavior is verifiable in EditMode.

## Objective Flow

`ObjectiveService` is a plain C# orchestrator for the compact extraction flow:

```text
Collect 3 Fuses -> Activate Generator -> Reach Extraction Zone -> Completed
```

The service observes typed `InventoryChangedEvent`, `GeneratorActivatedEvent`,
and `ExtractionReachedEvent` messages through `IEventBus`. Inventory, future
generator adapters, and future extraction-zone adapters do not need direct
references to the objective service. When extraction completes, the objective
service asks `IGameStateMachine` to transition from `Playing` to `Won`.

Current objective text and progress are exposed as R3 read-only reactive
properties. The service publishes typed objective change and completion events,
but it does not directly control UI or scene objects. Future presenters can
bind the observable state to a View without moving gameplay rules into UGUI.

## Player And Interaction Flow

The placeholder scene uses a small top-down `CharacterController` player.
`PlayerInputReader` reads movement and interaction input through the Unity Input
System. `PlayerController` applies movement and rotates the capsule toward its
travel direction.

`InteractionController` performs a short forward raycast and exposes the
current `InteractionPromptData` through an R3 read-only reactive property.
When interaction input is pressed, it calls the targeted `IInteractable`.
Interactables remain narrow Unity adapters:

- `PickupItemInteractable` asks `IInventoryService` to add an item.
- `GeneratorInteractable` publishes `GeneratorActivatedEvent`.
- `ExtractionZone` publishes `ExtractionReachedEvent` only when the objective
  flow has reached extraction.

These adapters do not update UI, objective progress, or game state directly.
`PlayerHealthBinder` connects a player-owned `HealthModel` to the scene and
transitions the state machine to `Lost` when that model publishes its death
event.

## Placeholder Scene Tooling

`ExtractionRoom/BootstrapPrototypeScene` creates the small placeholder room,
item and enemy configuration assets, dependency-injection roots, a simple enemy
with patrol points, a simple UGUI HUD, and the build-settings entry. It uses
Unity primitives and built-in UGUI resources only. It is intentionally an
editor bootstrap utility rather than production level-authoring infrastructure.

## Main Scene Composition

`Assets/_Project/Scenes/MainPrototype.unity` is the playable vertical slice and
the only enabled build-settings scene. Its `CompositionRoot` owns the
`GameLifetimeScope` serialized configuration for item definitions and explicit
scene injection roots.

The room contains a player capsule, top-down camera, reactive HUD canvas, three
blue Fuse pickups, a green Medkit pickup, a yellow Keycard pickup, a dark
generator with an activation light, an orange extraction trigger, and one red
enemy capsule. Walls, crates, utility pipes, lamps, platform markers, floor,
labels, and extraction markers use Unity primitives or built-in resources so
the repository remains asset-license safe and easy to inspect. A curated CC0
Quaternius content set adds sci-fi wall details, pipes, props, a medkit visual,
and the enemy slime visual without changing gameplay ownership.

The lightweight content pass stays inside scene composition and presentation
components. It does not move gameplay decisions into decorative objects. The
generator light observes the existing activation event, while objective
progress remains owned by the domain service.

## Reactive UGUI HUD

The HUD uses a small View + Presenter split. `HealthView`, `ObjectiveView`,
`InventoryView`, `InteractionPromptView`, and `EndGameView` are passive UGUI
components. They expose display methods and serialized widget references, but
they do not mutate gameplay services or decide when game rules have completed.

`HudPresenter` owns R3 subscriptions for player health, inventory snapshots,
objective text, and interaction prompts. `GameStatePresenter` observes the
game-state machine and selects the win or lose presentation. The presenters
dispose their subscriptions when the owning `HudView` is destroyed.

`GameLifetimeScope` explicitly binds the scene HUD after the player-facing
adapters have been configured. This keeps service dependencies visible at the
composition root and avoids global singletons, scene searches, and gameplay
logic inside UGUI Views.

## Presentation Layer Boundaries

LitMotion tweening is restricted to Views and components under `Presentation`.
`PresentationTweenHelper` centralizes short fades, scale pulses, and color
flashes while linking motion handles to Unity component lifetimes so destroyed
objects cancel their active tween work.

The interaction prompt fades in and out, inventory slots pulse when their
displayed count increases, health damage flashes the health label,
`EndGameView` fades in, and `GeneratorActivationView` pulses the generator when
it observes `GeneratorActivatedEvent`.

These effects are optional presentation feedback. Domain services, gameplay
models, and the game-state machine do not reference LitMotion. No objective,
damage, inventory, or state transition waits for a tween callback.

## Enemy AI State Machine

The placeholder enemy uses an explicit `EnemyStateMachine` with small
`EnemyIdleState`, `EnemyPatrolState`, `EnemyChaseState`, and `EnemyAttackState`
classes. Each state owns one readable branch of the behavior flow. For this
single-enemy prototype, explicit states are easier to inspect and test than a
behavior-tree framework.

`EnemyConfig` is a `ScriptableObject` asset containing tuning values only:
health, movement speed, ranges, damage, cooldown, and patrol wait time. Mutable
state such as the current patrol point and movement destination lives in the
plain C# `EnemyRuntimeState`.

`EnemyAIController` is the thin Unity adapter. It forwards positions and frame
time into the state machine, then applies the requested movement to its
transform. Attacks call `IDamageService` against the player health model. The
enemy does not reference UI, objective state, or global singletons.

## Namespace Convention

Assembly definition roots establish these namespace prefixes:

- Runtime: `ExtractionRoom`
- Editor: `ExtractionRoom.Editor`
- EditMode tests: `ExtractionRoom.Tests.EditMode`
- PlayMode tests: `ExtractionRoom.Tests.PlayMode`

Runtime feature code should use focused namespaces such as
`ExtractionRoom.Inventory` and `ExtractionRoom.Objectives`.
