# Contribution Guide

## Project Context

- Repository: `unity-portfolio-prototype`
- Prototype: `Extraction Room`
- Engine: Unity 6 LTS
- Purpose: a compact senior-level engineering portfolio prototype, not a full game.

The project exists to demonstrate clean Unity architecture, maintainable C# code,
testable gameplay systems, dependency injection, reactive UI bindings, async
workflows, editor tooling, documentation, and professional repository hygiene.

## Core Principles

- Prefer clear, maintainable code over clever abstractions.
- Keep changes focused on the requested task.
- Do not overengineer speculative features.
- Preserve existing behavior unless a task explicitly changes it.
- Review surrounding code before editing so new work follows established patterns.
- Do not overwrite or revert unrelated local changes.

## Technology Stack

- Unity 6 LTS
- C#
- VContainer
- R3
- UniTask
- LitMotion
- UGUI
- Unity Test Framework

Do not introduce alternative frameworks for responsibilities already covered by
this stack without documenting the reason.

## Architecture Rules

- Do not use global singletons.
- Use VContainer as the composition root and dependency injection mechanism.
- Keep `MonoBehaviour` classes thin. Use them as Unity-facing adapters, views, or
  lifecycle entry points rather than containers for domain logic.
- Keep gameplay and domain logic testable without scene objects.
- Use plain C# classes for runtime state.
- Use `ScriptableObject` assets for configuration only. Do not store mutable
  runtime state in them.
- Keep UI Views free of gameplay logic.
- Use Presenter or Binder classes between services and UI Views.
- Use R3 to observe state and implement UI bindings.
- Do not use R3 as a substitute for explicit, readable domain logic.
- Use UniTask only for async workflows where it adds value.
- Pass `CancellationToken` to cancellable async operations and bind cancellation
  to the appropriate lifetime.
- Avoid `async void`. Use it only where an external Unity or framework callback
  strictly requires it, and handle exceptions explicitly.
- Use LitMotion only in the presentation layer.
- Do not make gameplay state depend on tween completion.

## Code Organization

Keep responsibilities separated:

- `Domain`: plain C# gameplay rules, models, and state transitions.
- `Application`: use cases, orchestration, and services.
- `Presentation`: UGUI Views, Presenters, Binders, and LitMotion animations.
- `Infrastructure`: Unity adapters, persistence adapters, and integration code.
- `Composition`: VContainer lifetime scopes and registrations.
- `Editor`: editor-only tooling.
- `Tests`: EditMode and PlayMode tests grouped by the behavior they verify.

Match existing folder and assembly-definition conventions as the repository
evolves. Add assembly definitions when they create a clear boundary or improve
testability; do not add them mechanically.

## C# Guidelines

- Use explicit names that describe intent.
- Keep public APIs small.
- Prefer constructor injection for plain C# classes.
- Prefer composition over inheritance.
- Keep methods short enough that their control flow remains obvious.
- Handle disposal for subscriptions and other owned resources.
- Keep reactive subscriptions scoped to the owner lifetime.
- Avoid hidden side effects in properties.
- Add comments only when they explain a non-obvious decision or constraint.

## Testing Rules

- Add or update tests when gameplay rules, services, state transitions, or
  integrations change.
- Prefer EditMode tests for plain C# domain and application logic.
- Use PlayMode tests for behavior that requires Unity runtime integration.
- Keep tests deterministic and independent of execution order.
- Run relevant tests before committing when the environment allows it.
- If tests cannot run, document the reason in the task summary and commit notes
  when appropriate.

## Documentation Rules

- Keep documentation accurate and current.
- Do not claim that a feature, workflow, test suite, or tool exists before it is
  implemented.
- Update `README.md` or files under `Documentation/` whenever architecture,
  testing, tooling, or gameplay flow changes.
- Record setup steps and external dependencies when adding them.

## Asset Rules

- Do not add copyrighted, pirated, or license-incompatible assets.
- Prefer placeholders, Unity primitives, or properly licensed free assets.
- Record the source and license for third-party assets in repository
  documentation.
- Suitable sources include:
  - Kenney
  - Quaternius
  - OpenGameArt, after checking the specific asset license
  - Unity Asset Store free assets, after checking the specific asset license

## Unity Repository Hygiene

Do not commit generated Unity folders:

- `Library/`
- `Temp/`
- `obj/`
- `Logs/`
- `Build/`
- `Builds/`
- `UserSettings/`

Keep `.meta` files paired with their Unity assets. When moving or deleting an
asset, include the corresponding `.meta` file change.

## Git Workflow

Use Conventional Commits v1.0.0:

```text
<type>[optional scope]: <description>
```

Common allowed types:

- `feat`
- `fix`
- `docs`
- `test`
- `refactor`
- `build`
- `ci`
- `chore`
- `perf`
- `style`

Examples:

```text
feat(inventory): add stackable item inventory system
test(core): add health model editmode tests
docs(portfolio): polish repository documentation
fix(ui): dispose reactive hud subscriptions
refactor(project): simplify bootstrap flow
chore(repo): update unity gitignore
ci(tests): add unity test workflow
```

Commit rules:

- Create one commit per requested task unless explicitly told otherwise.
- Keep commits focused and do not mix unrelated changes.
- Do not use vague descriptions such as `update`, `changes`, `work`, or `misc`.
- Review changed files before committing.
- Stage only files that belong to the current task.
- Run relevant tests when available.
- State clearly when tests could not be run and why.

## AI Agent Workflow

Before making changes:

1. Read this file and relevant documentation.
2. Inspect the repository status and preserve unrelated local changes.
3. Inspect the relevant code, scenes, assets, and tests before editing.
4. Identify the smallest maintainable change that satisfies the request.

While making changes:

1. Follow the architecture boundaries above.
2. Keep edits scoped to the task.
3. Add or update tests and documentation when required.
4. Do not claim unverified behavior.

Before finishing:

1. Review the diff.
2. Run relevant tests or explain why they could not run.
3. Confirm generated folders are not staged.
4. Commit only task-related files with a focused Conventional Commit message,
   unless the requester explicitly asks not to commit.
5. Summarize the implementation, verification performed, and any remaining
   limitations honestly.
