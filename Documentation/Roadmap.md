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
- [ ] Implement small, testable interaction and inventory domain models
- [ ] Add a minimal extraction objective flow
- [ ] Bind a small UGUI HUD through R3 Presenter or Binder classes
- [ ] Add one useful cancellable UniTask initialization flow
- [ ] Add presentation-only LitMotion feedback
- [ ] Add focused EditMode tests and minimal PlayMode coverage
- [ ] Add a small editor validation tool

Scope should remain compact. New systems should exist only when they strengthen
the engineering demonstration.
