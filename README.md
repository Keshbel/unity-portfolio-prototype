# Unity Portfolio Prototype

Repository name: `unity-portfolio-prototype`

Prototype name: `Extraction Room`

This repository is a small senior-level Unity engineering prototype. It is not
a complete game. The goal is to demonstrate clean architecture, testability,
dependency injection, reactive UI bindings, async workflows, and clean project
organization in a compact vertical slice.

The current version is a foundation only. It defines the initial folder
structure, assembly boundaries, namespace roots, and project documentation.
Gameplay systems, package integration, scenes, UI, and tests have not been
implemented yet.

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
