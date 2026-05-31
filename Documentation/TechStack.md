# Tech Stack

## Current Project

- Unity `6000.3.11f1`
- C#
- Universal Render Pipeline
- UGUI
- Unity Test Framework

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
