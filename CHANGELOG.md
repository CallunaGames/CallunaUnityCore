## [1.7.0-pre.4] - 2026-09-28

### Added
- `ObservableList<T>.Subscribe(added:, removed:, replaced:, swapped:, reset:)` (also on `ReadonlyObservableList<T>`) — one subscription with handlers for the kinds of changes passed by name. Without a `reset` handler, a reset (`Clear`, `OverrideWith`) is delivered as the removal of every former item followed by the addition of every current item, so leaving out a handler can't make a subscriber miss items. Returns an `IDisposable`.
- `ObservableList<T>.SubscribeAny(Action)` — once per change of any kind; replaces `ObservableListChangeDetector`.
- `ObservableList<T>.Subscribe(Action<ListChange<T>>)` — every change as a `ListChange<T>` value (`Kind`, `Index`, `Item`, `FormerItem`, `OtherIndex`, `OtherItem`); a reset arrives as one `ListChangeKind.Reset`.
- `ItemHandler<T>`, `ReplaceHandler<T>`, `SwapHandler<T>` — delegate types of the handlers.

### Changed
- `ReadonlyObservableList<T>` — new members `Subscribe(...)`, `Subscribe(Action<ListChange<T>>)` and `SubscribeAny`. Only affects classes implementing the interface outside this package.
- `ObservableListChangeDetector` — now built on `SubscribeAny`; behavior unchanged.

### Deprecated
- `ObservableList<T>` / `ReadonlyObservableList<T>` events `OnItemAdded`, `OnItemRemoved`, `OnItemReplaced`, `OnItemsSwapped`, `OnClean`, `OnContentsReplaced`, the delegate types `ItemChangeEvent`, `ItemReplaceEvent`, `ItemSwapEvent`, and `ObservableListChangeDetector` — a subscriber had to handle all six events to not miss changes. Use `Subscribe` / `SubscribeAny`.

## [1.7.0-pre.3] - 2026-09-28

### Added
- `AccumulatingValuePart<T>.Clear()` and `IsSet` — withdraw a part's contribution without disposing it; setting `Value` again contributes again. A cleared part counts as if it didn't exist, in every mode. Clearing an unset or disposed part does nothing.
- `AccumulatingValue<T>.AddPart()` — adds a part that doesn't contribute until its value is set.
- `KeyedAccumulatingParts<TKey, T>` — one part per key, for contributors identified by data (requester names, ids authored in assets, pooled views) rather than objects keeping their own part. `Set`, `Clear`, `ClearAll`, `TryGet`; disposing removes all parts.

### Changed
- `AccumulatingValue<T>.AddPart(T value = default)` is now `AddPart(T value)`, next to the new parameterless `AddPart()`. Before, `AddPart()` contributed `default` - which blocks an `AccumulatingBoolValue` in mode `All`.

## [1.7.0-pre.2] - 2026-09-28

Pre-release adding the new APIs of 1.7.0. The APIs they replace still work but are marked `[Obsolete]` and will be removed in 2.0.0, so projects can migrate step by step.

### Added
- `Subscription` and `SubscriptionBag` — an `IDisposable` ending a subscription, and a collection ending all added subscriptions (any `IDisposable`) on `Dispose`, in reverse order. A subscription throwing while being disposed is logged and doesn't stop the others.
- `Observable<T>.Subscribe(Action)` and `Subscribe(Action<T, T>)` (also on `ReadonlyObservable<T>`) — return a subscription that removes the callback when disposed.
- `Observable<T>(T value)` constructor.
- `CoroutineHelper.Run(IEnumerator)` returning a `CoroutineHandle` (`IsRunning`, `Stop()`, yieldable inside coroutines). The handle reports the routine as ended when it completes, throws (the exception is still logged), is stopped, or the helper is destroyed.
- `CoroutineSlot` — runs at most one routine at a time on a `CoroutineHelper`; running a new one stops the former. Replaces unique ids generated per class for `ReplaceWithID`.
- `AccumulatingValue<T>.AddPart(T)` returning an `AccumulatingValuePart<T>` — set its `Value` to change the contribution, dispose it to remove it.
- `UpdateScheduler.ScheduleOnce(Action, SchedulePhase)` and `Cancel(Action)` — deduplicate by the callback itself instead of a string id.
- `ValueTweener<T>.UseUnscaledTime` — advance tweens with `Time.unscaledDeltaTime` (default stays `Time.deltaTime`).
- `Id<TDefinition>` — a value-type key compared by its string (ordinal), typed by the definition it identifies; usable without loading assets, e.g. in tests or as dictionary keys.
- `ScriptableObjectId<TSelf>` — `ScriptableObjectId` exposing its id as `Key` (`Id<TSelf>`). Existing classes can switch to it without losing their serialized ids.
- `ScriptableObjectIdValidation.FindProblems` and, in the editor, `ScriptableObjectIdValidator.FindProblems()` plus the menu **Calluna > Diagnostics > Validate ScriptableObject Ids** — report empty ids and ids used several times within a type.

### Changed
- `IEventBus.Subscribe` / `EventBus.Subscribe` — return a subscription (`IDisposable`) that unsubscribes the listener when disposed; subscribing `null` throws `ArgumentNullException`. Source-compatible for callers; classes implementing `IEventBus` themselves have to return an `IDisposable`.
- `ReadonlyObservable<T>` — new members `Subscribe(Action)` and `Subscribe(Action<T, T>)`. Only affects classes implementing the interface outside this package.
- `Timer` and `ValueTweener<T>` — run on a `CoroutineSlot` instead of a generated string id.
- `ObservableDiagnostics` — listeners added via `Subscribe` are reported by the subscribed callback rather than an internal adapter.

### Deprecated
- The implicit conversion from `T` to `Observable<T>` — use `new Observable<T>(value)`. Assigning a value to an observable field that way replaced the observable and dropped its listeners. (The conversion from `Observable<T>` to `T` stays.)
- `CoroutineHelper.StartWithID`, `ReplaceWithID`, `StopWithID`, `HasRoutineWith` — use `Run` / `CoroutineHandle` / `CoroutineSlot`.
- `AccumulatingValue<T>.Add`, `Set`, `Remove`, `TryGetValuePart` and the string indexer — use `AddPart`.
- `UpdateScheduler.ScheduleOnce(string, Action, SchedulePhase)` and `Cancel(string)` — use the overloads taking the callback.
- `EnumerableUtility` — iterating an `IEnumerable<T>` allocates an enumerator for most collections, so it doesn't avoid allocations as intended.

## [1.7.0-pre.1] - 2026-09-28

Pre-release for testing in projects; the new APIs planned for 1.7.0 (IDisposable subscriptions, coroutine handles, accumulating value parts, Id<TDefinition>) follow in later pre-releases.

### Added
- `ObservableDiagnostics` (editor only) and the menu **Calluna > Diagnostics > Log Unchanged Observable Values** — logs each call site that sets an observable to its current value while listeners are subscribed, once per play session, with the listeners that are no longer called. Helps find code relying on the former notify-on-every-set behavior.

### Changed
- `Observable<T>` — **behavior change:** setting a value equal to the current one (per `EqualityComparer<T>.Default`) no longer notifies `OnChanged` / `OnChangedWithValues`. Unity objects are compared by reference, so a change from a destroyed object to `null` still notifies. Code that mutates a held array/list/object and sets the same instance again to trigger listeners must assign a new instance; code sending commands through an observable should use an event instead.
- `Calluna.Core.Editor` — now references `Calluna.Core`.
- `EventBus` — a listener throwing an exception no longer stops the remaining listeners of that event, nor leaves queued events waiting for the next `Publish`. The exception is logged via `Debug.LogException` instead of propagating to the publisher. Listeners are stored per event type in an array replaced on (un)subscribe, so `Publish` stays allocation-free; listeners subscribed or unsubscribed during dispatch still take effect from the next delivered event on.
- `AccumulatingValue<T>` — `Value` now starts at the mode's neutral value, the same value it returns to once all parts are removed: `AccumulatingBoolValue` `All` starts `true` (was `false`), `AccumulatingFloatValue` `Multiply` starts `1` (was `0`). Recalculation no longer allocates an enumerator.
- `package.json` — minimum Unity version corrected to `6000.0.33f1` (`"unity": "6000.0"`, `"unityRelease": "33f1"`); it read `6000.33`, a version that doesn't exist.

### Breaking Changes
- `AccumulatingValue<T>.CalculateValue` — the protected abstract method now takes `IReadOnlyList<T>` instead of `IEnumerable<T>`. Only affects classes deriving from `AccumulatingValue<T>` outside this package; subclasses call the new protected `Recalculate()` at the end of their constructor to set their initial value.

### Fixed
- `ValueTweener<T>.Perform` — with a duration of zero or less, a tween already running on the same tweener is now stopped. Before, it kept running and overwrote the value just set.
- `CoroutineHelper` — destroying it now also forgets its routines, so `HasRoutineWith` no longer reports stopped routines as running.

## [1.6.1] - 2026-06-02

### Added
- `ObservableList<T>.OverrideWithEvents(IEnumerable<T> items)` — new overload that performs a diff-and-patch update firing granular per-item events for each change: `OnItemsSwapped` when an item only changes position, `OnItemReplaced` when a slot's content changes, `OnItemAdded` for new items, and `OnItemRemoved` for departing items. `OnContentsReplaced` is not raised.

### Changed
- `ObservableList<T>.OverrideWith` — implementation simplified to `Clear` + `AddRange` internally; public contract is unchanged (`OnContentsReplaced` fires once, no per-item events).
- README — `OverrideWithEvents` documented under Observable Collections; `OnContentsReplaced` usage note updated.

## [1.6.0] - 2026-05-13

### Breaking Changes
- `ReadonlyObservableList<TValue>` — new interface member `event Action OnContentsReplaced` has been added without a default implementation. Any class that directly implements this interface must now declare this event.
- `ObservableList<TValue>.OverrideWith` — per-item events (`OnItemAdded`, `OnItemRemoved`, `OnItemReplaced`) no longer fire during a bulk replace. Only `OnContentsReplaced` fires once after all items have been replaced. Callers that subscribed to per-item events to react to `OverrideWith` must subscribe to `OnContentsReplaced` instead.

### Added
- `ObservableList<T>.OnContentsReplaced` — new event fired once when `OverrideWith` completes, replacing the previous per-item event sequence and enabling cheaper bulk-replace handling for subscribers.

### Changed
- `ObservableListChangeDetector<T>` — now subscribes to `OnContentsReplaced` so that its `OnChanged` event fires correctly after an `OverrideWith` call, without requiring any changes to call sites.
- README — `Insert` method documented, `OverrideWith` event behaviour clarified to match the new `OnContentsReplaced` semantics, `Timer.Progress` boundary description corrected, and the Observable type hierarchy diagram fixed.

## [1.5.2] - 2026-05-11

### Added
- `UpdateScheduler` — new `MonoBehaviour` that defers callbacks to a chosen Unity update phase within the current frame. Accepts a string ID; first registration wins (duplicate IDs in the same frame are discarded). Provides `ScheduleOnce(id, callback, SchedulePhase)`, `Cancel(id)`, and `CancelAll()`.
- `UpdateScheduler.SchedulePhase` — nested enum with `Update` and `LateUpdate` values, controlling which Unity loop phase the scheduled callback runs in.
- `TweenType.Linear` — new enum value for constant-speed (no easing) interpolation.
- `Tween.Linear(float)` — easing function that returns the input value unchanged, producing a straight linear interpolation across all `ValueTweener<T>` types.

## [1.5.1] - 2026-05-04

### Added
- `ValueTweener<TValue>.PerformAndWait(start, end, duration, TweenType, Action<TValue>)` — starts the tween and returns a `CustomYieldInstruction` that completes when the tween finishes, allowing callers to `yield return` inside a coroutine. Zero or negative duration resolves immediately.

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
