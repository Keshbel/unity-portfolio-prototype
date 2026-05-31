# Testing

## Current State

The Unity Test Framework package is present in the generated Unity project.
Dedicated EditMode and PlayMode test assemblies exist. EditMode coverage
currently verifies the typed event bus, the initial game state machine
transitions, the health and damage domain model, inventory rules, and the
extraction objective flow.

## Strategy

- Use EditMode tests for plain C# domain rules, state transitions, and
  application services.
- Use PlayMode tests only for behavior that requires Unity runtime integration.
- Keep tests deterministic and independent of execution order.
- Prefer testing domain behavior without scene objects.

The health and damage logic is implemented as pure C# code. EditMode tests
verify health initialization, damage and healing clamps, one-time death event
publication, and the result returned by `DamageService` without loading a
Unity scene.

The inventory logic is also covered in EditMode without loading a scene. Tests
verify stacking, maximum stack boundaries, removal, count queries, full
inventory behavior, invalid counts, missing configuration, and reactive event
publication.

Objective EditMode tests drive typed domain events directly. They verify fuse
progress, generator activation, extraction completion, and the final
`Playing -> Won` game-state transition without loading a scene.

PlayMode coverage verifies the Unity-facing player health binder: a player
death event from its owned `HealthModel` transitions the game state to `Lost`.

The explicit enemy AI state machine is also covered in EditMode. Tests verify
idle and patrol transitions, player detection, entry into attack range, damage
application through `DamageService`, and stopping attacks after player death.
The placeholder scene is also suitable for a short Unity CLI smoke check of
movement, pickup interaction, and reactive HUD visibility.

## Commands

Run EditMode tests through Unity CLI:

```text
unity-cli tool call run_tests --json "{\"testMode\":\"EditMode\",\"filter\":\"ExtractionRoom.Tests.EditMode\"}"
```

Run project PlayMode tests through Unity CLI:

```text
unity-cli tool call run_tests --json "{\"testMode\":\"PlayMode\",\"filter\":\"ExtractionRoom.Tests.PlayMode\"}"
```
