## [1.5.0] - 2026-05-04

### Breaking Changes
- `Timer.StopTimer()` renamed to `Timer.Stop()` — all call sites must be updated to the new name.
- `AccumulatingFloatValue.Mode.AddUp` renamed to `Mode.Sum` — all references to `Mode.AddUp` must be updated to `Mode.Sum`.

### Added
- `ValueTweener<TValue>` — abstract base class for animated value interpolation driven by `CoroutineHelper` coroutines. Exposes `IsTweening`, `Perform(start, end, duration, TweenType, Action<TValue>)`, `Stop()`, and `Dispose()`.
- `FloatValueTweener`, `IntValueTweener`, `Vector2ValueTweener`, `Vector3ValueTweener` — concrete `ValueTweener<T>` subclasses for the most common Unity value types, using `Mathf.LerpUnclamped` (float, Vector2, Vector3) and `Mathf.RoundToInt(LerpUnclamped(...))` (int).
- `Tween.GetEaseFunction(TweenType)` — returns a pre-allocated `Func<float,float>` delegate from a static lookup, avoiding a new allocation on every call.

### Changed
- `Tween.ValidateValue` is now compiled only under `UNITY_EDITOR || DEVELOPMENT_BUILD` — per-frame float comparisons are eliminated in release builds.
- `AccumulatingFloatValue` and `AccumulatingBoolValue` `_mode` fields are now `readonly`, preventing accidental mutation after construction.
- `EventBus.Subscribe` uses `TryAdd` instead of a `ContainsKey` + indexer pair, reducing dictionary lookups from two to one.
- `EnumerableUtility.Product` is now seeded with the multiplicative identity `1f`, replacing the previous sentinel pattern.

## [1.4.0] - 2026-04-26

### Added
- `IEventBus` — new public interface with `Subscribe<TEvent>`, `Unsubscribe<TEvent>`, and `Publish<TEvent>` methods for typed, decoupled publish/subscribe messaging.
- `EventBus` — concrete implementation of `IEventBus` with breadth-first, re-entrancy-safe dispatch: events published from inside a listener are queued and processed after the current dispatch batch completes. One dispatcher closure is allocated per event type at subscribe time; subsequent publishes of reference types allocate nothing.

## [1.3.0] - 2026-04-15

### Breaking Changes
- `ReadonlyObservableList<T>` interface — a new `event Action OnClean` member has been added. Any external type that implements `ReadonlyObservableList<T>` directly must add an `OnClean` event implementation; it cannot be left unimplemented.
- `ObservableList<T>.Clear()` — no longer fires `OnItemRemoved` once per element. It now fires the new `OnClean` event a single time after the backing list is cleared. Callers that subscribed to `OnItemRemoved` to react to `Clear()` must subscribe to `OnClean` instead.

### Added
- `ObservableList<T>.OnClean` — new event fired once when `Clear()` is called, replacing the previous per-element `OnItemRemoved` sequence and enabling cheaper clear handling for subscribers.
- `ObservableListChangeDetector<T>` — now subscribes to `OnClean` and routes it into the existing `OnChanged` event, so detectors correctly report `Clear()` calls without any changes to call sites.

### Fixed
- `PackageSamplesTestsToggler` — hiding a folder (renaming to `~`) now deletes the associated `.meta` file rather than leaving it as an orphaned `Foo~.meta`, which previously caused "meta file exists but folder can't be found" warnings in the Unity Editor.

## [1.2.0] - 2026-03-30

### Breaking Changes
- `ScriptableObjectId.GetHashCode()` now returns a hash derived from the `Id` string rather than the object's identity (reference). Callers that use `ScriptableObjectId` instances as dictionary keys or in `HashSet<T>` collections will observe different bucket assignments after this change — existing serialized or runtime dictionaries keyed on `ScriptableObjectId` must be rebuilt.

## [1.0.20] - 2026-03-29

### Breaking Changes
- `Timer.Percentage` renamed to `Timer.Progress` — callers must update all read sites.
- `Timer.Time` renamed to `Timer.Elapsed` — callers must update all read sites.
- `Timer.StartWith` first parameter renamed from `seconds` to `duration` — callers using named arguments (e.g. `StartWith(seconds: 2f)`) must update the argument name.
- `AccumulatingBoolValue` and `AccumulatingFloatValue` — the two-constructor overload pair (default + parameterised) has been collapsed into a single constructor with a default parameter. This is source-compatible in Unity but is a binary-breaking change; any assembly compiled against the previous version must be recompiled.
- `Tween.EaseInOutCubic` — the formula was previously a copy of the sine implementation; it now uses the correct cubic formula. Return values in the range (0, 1) differ from the previous release. Callers that relied on the old (incorrect) output must re-evaluate their animation curves.

### Added
- `EnumerableUtility` — new static utility class with allocation-free `Sum`, `Product`, `Any`, and `All` helpers for use without LINQ.
- Test suite for `AccumulatingIntValue`, `AccumulatingFloatValue`, and `AccumulatingBoolValue` covering add, remove, set, observable events, and error cases.
- Test suite for `ObservableList` covering add, remove, replace, swap, and clear with event verification across multiple value types.
- Test suite for `Tween` covering boundary values, input validation, and monotonicity of cubic ease functions across all `TweenType` values.

### Fixed
- `Tween.EaseInOutCubic` — replaced the incorrect sine formula with the standard cubic formula; the function now produces correct cubic easing values across the full [0, 1] range.
- `ObservableListChangeDetector` — fixed misleading parameter names in the `OnItemsSwapped` handler that swapped the meaning of "new item at index" arguments.

### Changed
- `ObservableList.OverrideWith` — internal variable naming clarified; behaviour is unchanged.
- `CoroutineHelper` — internal struct renamed from `Routines` to `CoroutinePair` and private method renamed from `StartRemove` to `WaitThenRemove`; no change to public API.

### Performance
- `AccumulatingBoolValue`, `AccumulatingFloatValue`, and `AccumulatingIntValue` — `CalculateValue` now uses `EnumerableUtility` instead of LINQ, eliminating delegate and iterator allocations on each recalculation.
- `Tween.GetEaseFunction(TweenType)` — now returns a pre-allocated delegate from a static array instead of allocating a new delegate object on each call.
