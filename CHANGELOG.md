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
