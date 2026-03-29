# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Package Overview

**CallunaUnityCore** (`com.calluna.core`) is a Unity UPM package providing common utility patterns for Calluna Games' Unity projects. It has no external dependencies.

## Development Commands

This is a Unity package — there are no standalone build or test commands. Development happens inside a Unity project that imports this package.

- **Toggle Tests/Samples visibility:** Use the Unity Editor menu `Calluna > Packages > Samples and Tests Toggler` to rename `Tests~` ↔ `Tests` and `Samples~` ↔ `Samples`, making them visible/hidden to the Package Manager.
- **Run tests:** Open Unity Test Runner (`Window > General > Test Runner`) and run `Calluna.Core.Tests` assembly.
- **Tests location:** `Tests~/` (must be toggled visible first via the editor tool above).

## Architecture

All runtime code lives under the `Calluna` namespace. The single runtime assembly is `Calluna.Core.asmdef`.

### Key Patterns

**Observable / ReadonlyObservable**
- `Observable<T>` wraps a value and fires `OnChanged` (no args) or `OnChangedWithValues(former, new)` on mutation.
- Expose as `ReadonlyObservable<T>` to prevent external writes.
- Implicit operators allow `Observable<int> x = 5;` and `int y = x;`.
- Use `SetValueWithoutNotify()` for silent updates.

**AccumulatingValue**
- Abstract base `AccumulatingValue<T>` maintains a dictionary of named partial values that combine into a result.
- Concrete types: `AccumulatingIntValue` (sum), `AccumulatingFloatValue` (sum or multiply mode), `AccumulatingBoolValue`.
- Exposes its result as `ReadonlyObservable<T>`.

**ObservableList**
- `ObservableList<T>` implements `IList<T>` and fires typed events for add, remove, replace, and swap operations.
- Expose as `ReadonlyObservableList<T>` (inherits `IReadOnlyList<T>`) to prevent external mutation.
- Custom delegates are defined in `ObservableCollectionDelegates.cs`.

**CoroutineHelper / Timer**
- `CoroutineHelper` (MonoBehaviour) wraps Unity coroutines with string IDs to prevent duplicates and allow replacement.
- `Timer` uses `CoroutineHelper` internally, implements `IDisposable`, tracks `Progress` (0–1) and `Elapsed` time, and fires `OnDone`.

**EnumerableUtility**
- Static utility class under `Runtime/Core/Utility/`.
- `Sum(IEnumerable<int>)`, `Sum(IEnumerable<float>)`, `Product(IEnumerable<float>)`, `Any(IEnumerable<bool>)`, `All(IEnumerable<bool>)`.

**Tween**
- Static utility class with easing functions (Sine, Cubic, Back × In/Out/InOut).
- Select via `TweenType` enum; all functions accept a `float` in [0, 1].

**ScriptableObjectId**
- Abstract base adding a serialized string `Id` field to ScriptableObjects, with equality by reference or ID string.

## Directory Layout

```
Runtime/Core/          # Production source, organized by feature
Editor/                # Editor-only tools (PackageSamplesTestsToggler)
Tests~/                # NUnit tests (hidden from Package Manager by default)
Samples~/              # Sample MonoBehaviours per feature (hidden by default)
package.json           # UPM metadata (v1.1.0, Unity 6000.33)
```

## Conventions

- Mutable types are always paired with a readonly interface (`Observable`/`ReadonlyObservable`, `ObservableList`/`ReadonlyObservableList`).
- Private fields are prefixed with `_`.
- Tests use NUnit with `[ValueSource]` for parameterized coverage across multiple generic types.

## Release Process

Run `/release-prep` to execute the full release pipeline. It runs automatically in five phases:

1. **Code Quality** — readability, performance, and complexity agents review and fix all Runtime `.cs` files in parallel.
2. **Verification** — breaking-change-detector (→ PATCH/MINOR/MAJOR verdict), test-coverage-gap-finder, dead-code-detector, and sample-sync-checker run in parallel.
3. **Documentation** — unit-test-writer fills coverage gaps; readme-updater syncs `README.md`.
4. **Release Artifacts** — changelog-writer adds a `CHANGELOG.md` entry; package-updater bumps `package.json` version using the semver verdict.
5. **Commit Staging** — files are staged by group (Runtime → Tests → artifacts) and a conventional commit message is composed for approval.

**After each release, update this file** to reflect any API changes (renamed members, new types, removed features) so future sessions start with accurate context.
