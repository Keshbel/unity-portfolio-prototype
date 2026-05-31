# Tech Stack

## Current Project

- Unity `6000.3.11f1`
- C#
- Universal Render Pipeline
- UGUI
- Unity Test Framework
- `com.akiojin.unity-cli-bridge` for local Unity Editor automation

## Unity CLI Setup

The Unity-side bridge is installed through `Packages/manifest.json`. The
`unity-cli` binary is a developer-machine tool and is not committed to the
repository.

After opening the project in Unity, verify the local connection:

```text
unity-cli system ping
```

The default endpoint is `localhost:6400`.

## Planned Runtime Libraries

The vertical slice is intended to use:

- VContainer for dependency injection and composition roots
- R3 for reactive state observation and UI bindings
- UniTask for cancellable async workflows where async adds value
- LitMotion for presentation-layer animation

These third-party libraries are not installed in the foundation commit. Their
package sources and versions will be recorded when integration begins.

## Constraints

- Do not introduce global singletons.
- Do not store runtime state in `ScriptableObject` assets.
- Do not make gameplay state depend on presentation tween completion.
- Do not use async workflows where synchronous code is clearer.
