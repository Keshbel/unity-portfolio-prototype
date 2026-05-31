# Testing

## Current State

The Unity Test Framework package is present in the generated Unity project.
Dedicated EditMode and PlayMode test assemblies now exist, but no tests have
been implemented yet.

## Strategy

- Use EditMode tests for plain C# domain rules, state transitions, and
  application services.
- Use PlayMode tests only for behavior that requires Unity runtime integration.
- Keep tests deterministic and independent of execution order.
- Prefer testing domain behavior without scene objects.

## Commands

Test execution commands will be documented when the first tests are added and
the project test workflow is established.
