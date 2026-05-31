# Tech Stack

## Current Project

- Unity `6000.3.11f1`
- C#
- Universal Render Pipeline
- Unity Input System
- UGUI
- Unity Test Framework
- `com.akiojin.unity-cli-bridge` for local Unity Editor automation
- VContainer `1.18.0`
- R3 `1.3.1`
- LitMotion `2.0.1`
- NuGetForUnity `4.5.0` for restoring the R3 core package

## Unity CLI Setup

The Unity-side bridge is installed through `Packages/manifest.json`. The
`unity-cli` binary is a developer-machine tool and is not committed to the
repository.

After opening the project in Unity, verify the local connection:

```text
unity-cli system ping
```

The default endpoint is `localhost:6400`.

## Runtime Libraries

The vertical slice uses:

- VContainer for dependency injection and composition roots
- R3 for reactive state observation and UI bindings
- UGUI for the compact runtime HUD
- LitMotion for lightweight presentation-layer tweening

UGUI is sufficient for this prototype's small set of runtime labels and slots,
ships with Unity, and keeps the placeholder scene easy to inspect. A larger
production interface could justify a more elaborate UI approach, but it would
add unnecessary surface area here.

R3 is used where state observation is useful: domain services expose read-only
reactive properties and presenter classes subscribe to update passive Views.
Gameplay rules remain explicit C# logic rather than reactive pipelines.

LitMotion is limited to presentation feedback: UI fades, slot pulses, damage
feedback, and generator activation feedback. Tween completion is never used to
advance objectives, update inventory, apply damage, or transition game state.

R3 core binaries are restored from `Packages/nuget-packages/packages.config`.
Run `nugetforunity restore .` before opening Unity on a fresh checkout when the
CLI tool is installed.

## Planned Runtime Libraries

The vertical slice is intended to add:

- UniTask for cancellable async workflows where async adds value

Their package sources and versions will be recorded when integration begins.

## Constraints

- Do not introduce global singletons.
- Do not store runtime state in `ScriptableObject` assets.
- Do not make gameplay state depend on presentation tween completion.
- Do not use async workflows where synchronous code is clearer.
