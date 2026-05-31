# Roadmap

This repository targets a compact one-day vertical slice. It is intentionally
not a full game.

## Foundation

- [x] Add project-owned folder structure
- [x] Add runtime, editor, EditMode test, and PlayMode test assemblies
- [x] Document architecture, testing strategy, tech stack, and asset policy

## Planned Vertical Slice

- [ ] Add the remaining approved third-party runtime libraries
- [x] Create the VContainer composition root
- [x] Implement a small, testable inventory domain model
- [x] Add a minimal extraction objective flow
- [x] Add a placeholder player and interaction loop
- [x] Bind a small UGUI HUD through R3 Presenter or Binder classes
- [ ] Add one useful cancellable UniTask initialization flow
- [x] Add presentation-only LitMotion feedback
- [x] Add focused EditMode tests and minimal PlayMode coverage
- [x] Add a small placeholder-scene bootstrap tool

Scope should remain compact. New systems should exist only when they strengthen
the engineering demonstration.
