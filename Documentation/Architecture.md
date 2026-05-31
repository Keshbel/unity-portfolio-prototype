# Architecture

## Current State

The repository currently contains the project foundation only. No gameplay
systems, scene composition roots, UI flows, or runtime services have been
implemented.

## Intended Boundaries

Runtime code belongs to `Project.Runtime`. Editor-only tooling belongs to
`Project.Editor`, which references `Project.Runtime`. EditMode and PlayMode
tests have separate assemblies and reference `Project.Runtime`.

The runtime folders reserve small, explicit areas for the vertical slice:

| Folder | Intended responsibility |
| --- | --- |
| `Core` | Shared domain primitives and application contracts |
| `DI` | VContainer composition roots and registrations |
| `Gameplay` | Game-flow orchestration |
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

## Namespace Convention

Assembly definition roots establish these namespace prefixes:

- Runtime: `ExtractionRoom`
- Editor: `ExtractionRoom.Editor`
- EditMode tests: `ExtractionRoom.Tests.EditMode`
- PlayMode tests: `ExtractionRoom.Tests.PlayMode`

Runtime feature code should use focused namespaces such as
`ExtractionRoom.Inventory` and `ExtractionRoom.Objectives`.
