# Testing

## Current State

The Unity Test Framework package is present in the generated Unity project.
Dedicated EditMode and PlayMode test assemblies exist. EditMode coverage
currently verifies the typed event bus and the initial game state machine
transitions.

## Strategy

- Use EditMode tests for plain C# domain rules, state transitions, and
  application services.
- Use PlayMode tests only for behavior that requires Unity runtime integration.
- Keep tests deterministic and independent of execution order.
- Prefer testing domain behavior without scene objects.

## Commands

Run EditMode tests through Unity CLI:

```text
unity-cli tool call run_tests --json "{\"testMode\":\"EditMode\",\"filter\":\"ExtractionRoom.Tests.EditMode\"}"
```
