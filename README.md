# Unity Portfolio Prototype

Repository name: `unity-portfolio-prototype`

Prototype name: `Extraction Room`

This repository is a small senior-level Unity engineering prototype. It is not
a complete game. The goal is to demonstrate clean architecture, testability,
dependency injection, reactive UI bindings, async workflows, and clean project
organization in a compact vertical slice.

The current version is a compact playable engineering slice. It defines the
project structure, assembly boundaries, dependency injection services, domain
models, a placeholder gameplay scene, a reactive UGUI HUD, and project
documentation.

## Implemented Features

- Health/Damage System: testable plain C# health state, typed domain events,
  and a damage application service
- Inventory System: fixed-capacity plain C# inventory state with stackable item
  rules and ScriptableObject-backed item configuration
- Objective System: event-driven extraction flow from fuse collection through
  generator activation to extraction completion
- Player and Interaction System: CharacterController movement, raycast
  interactions, item pickups, generator activation, extraction trigger, and a
  player health binder
- Reactive UGUI HUD: View + Presenter bindings for player health, objectives,
  inventory slots, interaction prompts, and win or lose state
- Enemy AI State Machine: one placeholder enemy with explicit idle, patrol,
  chase, and attack states

## Technical Highlights

- VContainer composition root with explicit service registration
- Testable plain C# game state machine with R3 observable state
- Typed event-driven architecture foundation
- Thin Unity-facing interaction adapters wired through dependency injection
- R3-driven UGUI presenters with passive Views and owned subscription cleanup
- LitMotion presentation feedback isolated from gameplay state transitions
- UniTask async initialization and scene loading with cancellation support

## Project Documentation

- [Architecture](Documentation/Architecture.md)
- [Testing](Documentation/Testing.md)
- [Tech Stack](Documentation/TechStack.md)
- [Roadmap](Documentation/Roadmap.md)
- [Asset Credits](Documentation/AssetCredits.md)

## Repository Layout

Project-owned Unity assets live under `Assets/_Project/`:

```text
Assets/_Project/
  Runtime/
  Editor/
  Tests/
  Scenes/
  Configs/
  Art/
  Audio/
```

Generated Unity folders such as `Library/`, `Temp/`, `Logs/`, and
`UserSettings/` are excluded from version control.

## Unity CLI

The project includes the `com.akiojin.unity-cli-bridge` Unity package for
Editor automation over a local TCP connection.

Install the `unity-cli` binary separately on each development machine, open the
project in Unity, and verify the connection:

```text
unity-cli system ping
```

The bridge listens on `localhost:6400` by default.
