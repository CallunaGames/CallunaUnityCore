# CallunaUnityCore
The core scripts of Calluna Games' Unity projects

## Observable Values
Observable values notify listeners when their value changes.
Use the `ReadonlyObservable<T>` interface to expose an `Observable<T>` to other classes without allowing external writes.

```c#
abstract class Observable
class Observable<T> : Observable, ReadonlyObservable<T>
interface ReadonlyObservable<T>
```

### Usage

#### Create
```c#
Observable<TValue> observableValue = new Observable<TValue>(new TValue());
Observable<TValue> empty = new Observable<TValue>(); // default(TValue)
```

#### Read Value
```c#
TValue value = observableValue.Value;
```

#### Read Value Implicitly
```c#
TValue value = observableValue;
```

#### Set Value
```c#
observableValue.Value = new TValue();
```

The implicit conversion from `TValue` to `Observable<TValue>` (`Observable<TValue> observableValue = new TValue();`) is obsolete and will be removed in 2.0.0 - assigning a value to an observable field that way silently replaced the observable and dropped its listeners. Use the constructor.

#### Check for Value
```c#
bool hasValue = observableValue.HasValue;
```

#### Set Without Notifying Listeners
```c#
observableValue.SetValueWithoutNotify(new TValue());
```

#### Expose Readonly Observable
```c#
private readonly Observable<TValue> _observableValue = new Observable<TValue>(new TValue());
public ReadonlyObservable<TValue> ObservableValue => _observableValue;
```

#### Listen to Value Change
`Subscribe` returns an `IDisposable` that ends the subscription - no need to keep the delegate around for unsubscribing.
```c#
IDisposable subscription = observableValue.Subscribe(() => Debug.Log("On value changed"));
observableValue.Value = new TValue();
subscription.Dispose();
```
**On value changed**

#### Listen to Value Change With Values
```c#
Observable<int> observableValue = new Observable<int>(2);
observableValue.Subscribe((int former, int newValue) => Debug.Log($"On value changed ({former} | {newValue})"));
observableValue.Value = 5;
```
**On value changed (2 | 5)**

The events `OnChanged` and `OnChangedWithValues` can still be used directly (`+=` / `-=`).

#### End Several Subscriptions Together
A `SubscriptionBag` collects subscriptions - from observables, the event bus or any other `IDisposable` - and ends them all on `Dispose`, in reverse order. The bag is empty afterwards and can be filled again, e.g. by a pooled object.
```c#
private readonly SubscriptionBag _subscriptions = new SubscriptionBag();

public void Initialize()
{
    _subscriptions.Add(_score.Subscribe(UpdateLabel));
    _subscriptions.Add(_eventBus.Subscribe<LevelLoadedEvent>(OnLevelLoaded));
}

public void Clean() => _subscriptions.Dispose();
```

#### Only Actual Changes Notify
Setting the current value again doesn't notify listeners. Values are compared with `EqualityComparer<T>.Default`; Unity objects are compared by reference, because a destroyed one equals `null` and a change from it to `null` would otherwise be lost.
```c#
Observable<int> observableValue = new Observable<int>(2);
observableValue.Subscribe(() => Debug.Log("On value changed"));
observableValue.Value = 2; // nothing logged
```
Values changed in place - arrays, lists, other mutable objects - count as unchanged when the same instance is set again. Assign a new instance instead:
```c#
int[] updated = (int[])observableArray.Value.Clone();
updated[0] = 5;
observableArray.Value = updated;
```
Commands ("show this popup again") aren't values and shouldn't be sent through an observable; use an event instead.

#### Find Code Relying on Unchanged-Value Notifications
In the editor, **Calluna > Diagnostics > Log Unchanged Observable Values** logs every place that sets an observable to its current value while listeners are subscribed - each call site once per play session, with the listeners no longer being called. Useful when migrating code written for the former behavior.

---

## Observable Collections
`ObservableList<TValue>` is a list that fires events when items are added, removed, replaced, or swapped.
Use `ReadonlyObservableList<TValue>` to expose the list without allowing external mutation.

```c#
class ObservableList<TValue> : IList<TValue>, ReadonlyObservableList<TValue>
interface ReadonlyObservableList<TValue> : IReadOnlyList<TValue>
class ObservableListChangeDetector<TValue> : IDisposable

delegate void ItemChangeEvent<TValue>(TValue item, int index)
delegate void ItemReplaceEvent<TValue>(TValue newItem, TValue formerItem, int index)
delegate void ItemSwapEvent<TValue>(TValue newIndex1Item, int index1, TValue newIndex2Item, int index2)
```

### Usage

#### Create and Expose as Readonly
```c#
private ObservableList<TValue> _list = new ObservableList<TValue>();          // empty
private ObservableList<TValue> _listWithCapacity = new ObservableList<TValue>(16);         // pre-allocated
private ObservableList<TValue> _listFromExisting = new ObservableList<TValue>(existingCollection); // copy
public ReadonlyObservableList<TValue> List => _list;
```

#### Add / Insert / Remove / Clear
```c#
_list.Add(item);
_list.Insert(index, item); // inserts at index; fires OnItemAdded
_list.Remove(item);        // returns bool
_list.RemoveAt(index);
_list.Clear();
```

#### Replace Item at Index
```c#
_list[index] = newItem;   // fires OnItemReplaced
```

#### Swap Two Items
```c#
_list.Swap(indexA, indexB);
```

#### Override Contents (bulk)
```c#
// Replaces all contents in-place to match the new sequence.
// No per-item events are fired; OnContentsReplaced is raised once when done.
_list.OverrideWith(newItems);
```

#### Override Contents (per-item events)
```c#
// Diff-and-patch update: fires OnItemsSwapped, OnItemReplaced, OnItemAdded, and
// OnItemRemoved for each individual change. OnContentsReplaced is NOT fired.
// Use this when listeners need to react to each slot change rather than a bulk reset.
_list.OverrideWithEvents(newItems);
```

#### Listen to Events
```c#
_list.OnItemAdded += (TValue item, int index) => { };
_list.OnItemRemoved += (TValue item, int index) => { };
_list.OnItemReplaced += (TValue newItem, TValue formerItem, int index) => { };
_list.OnItemsSwapped += (TValue newAt0, int index0, TValue newAt1, int index1) => { };
_list.OnClean += () => { };             // fires once when Clear() is called; OnItemRemoved is NOT fired per element
_list.OnContentsReplaced += () => { };  // fires once when OverrideWith() completes; NOT fired by OverrideWithEvents()
```

#### Detect Any Change with ObservableListChangeDetector
`ObservableListChangeDetector` routes all four item events, `OnClean`, and `OnContentsReplaced` into a single `OnChanged` event, so you can react to any mutation — including `Clear()`, `OverrideWith()`, and `OverrideWithEvents()` — from one subscription.
```c#
using var detector = new ObservableListChangeDetector<TValue>(_list);
detector.OnChanged += () => { Debug.Log("List changed"); };
// detector.Dispose() unsubscribes all listeners
```

---

## Accumulating Values
Accumulating values combine parts into a single result exposed as a `ReadonlyObservable<T>`. Use this to aggregate contributions from multiple independent sources (e.g. stat bonuses). Each source adds a part and keeps it to change or remove its contribution later.

```c#
abstract class AccumulatingValue<T>
class AccumulatingIntValue : AccumulatingValue<int>        // sums all parts
class AccumulatingFloatValue : AccumulatingValue<float>    // sum (default) or multiply
class AccumulatingBoolValue : AccumulatingValue<bool>      // Any (default) or All
```

### Usage

#### Add, Change, and Remove Parts
```c#
AccumulatingIntValue speed = new AccumulatingIntValue();
AccumulatingValuePart<int> baseSpeed = speed.AddPart(10);
AccumulatingValuePart<int> bonus = speed.AddPart(5);
bonus.Value = 8;  // recalculates
bonus.Dispose();  // removes the part; disposing again does nothing
```
The string-id methods (`Add`, `Set`, `Remove`, `TryGetValuePart`, the indexer) are obsolete and will be removed in 2.0.0. They avoided id collisions only by convention; a part belongs to whoever added it.

#### Read the Result
```c#
int totalSpeed = speed.Value.Value;
speed.Value.OnChanged += () => { Debug.Log("Speed changed"); };
```

#### Expose as Readonly
```c#
public ReadonlyObservable<int> Speed => speed.Value;
```

#### Read a Specific Part
```c#
int baseValue = baseSpeed.Value;
```

#### AccumulatingFloatValue Modes
```c#
AccumulatingFloatValue addUp   = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.Sum);
AccumulatingFloatValue product = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.Multiply);
```

#### AccumulatingBoolValue Modes
```c#
AccumulatingBoolValue anyTrue = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
AccumulatingBoolValue allTrue = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.All);
```

#### Value Without Parts
Without any parts, `Value` holds the mode's neutral value - the same value it returns to once all parts are removed again:

| Type / mode | Value without parts |
|---|---|
| `AccumulatingIntValue`, `AccumulatingFloatValue` `Sum` | `0` |
| `AccumulatingFloatValue` `Multiply` | `1` |
| `AccumulatingBoolValue` `Any` | `false` |
| `AccumulatingBoolValue` `All` | `true` |

---

## Tween
Static utility class with easing functions based on [easings.net](https://easings.net/). All functions accept a `float` in the range `[0, 1]`. Range validation is only performed in `UNITY_EDITOR` and `DEVELOPMENT_BUILD` builds; out-of-range values are silently accepted in release builds.

```c#
static class Tween
enum TweenType
```

| Method | TweenType |
|---|---|
| `EaseInSine` | `TweenType.EaseInSine` |
| `EaseOutSine` | `TweenType.EaseOutSine` |
| `EaseInOutSine` | `TweenType.EaseInOutSine` |
| `EaseInCubic` | `TweenType.EaseInCubic` |
| `EaseOutCubic` | `TweenType.EaseOutCubic` |
| `EaseInOutCubic` | `TweenType.EaseInOutCubic` |
| `EaseInBack` | `TweenType.EaseInBack` |
| `EaseOutBack` | `TweenType.EaseOutBack` |
| `EaseInOutBack` | `TweenType.EaseInOutBack` |
| `Linear` | `TweenType.Linear` |

### Usage

#### Direct Call
```c#
float eased = Tween.EaseInOutSine(t); // t in [0, 1]
```

#### Select at Runtime via TweenType
```c#
Func<float, float> easeFn = Tween.GetEaseFunction(TweenType.EaseOutCubic);
float eased = easeFn(t);
```

---

## Value Tweeners
`ValueTweener<TValue>` interpolates a typed value from a start to an end over a given duration using a `TweenType` easing function, driving the update via a `CoroutineHelper`. Use the concrete subclasses directly; implement your own subclass only when a new value type is needed.

```c#
abstract class ValueTweener<TValue> : IDisposable
class FloatValueTweener   : ValueTweener<float>
class IntValueTweener     : ValueTweener<int>      // steps are rounded via Mathf.RoundToInt
class Vector2ValueTweener : ValueTweener<Vector2>
class Vector3ValueTweener : ValueTweener<Vector3>
```

| Member | Description |
|---|---|
| `IsTweening` | `true` while a tween coroutine is running. |
| `Perform(start, end, duration, tweenType, updateAction)` | Starts (or replaces) a tween. When `duration <= 0`, a running tween is stopped, `updateAction` is called immediately with `end` and no coroutine is started. |
| `PerformAndWait(start, end, duration, tweenType, updateAction)` | Same as `Perform`, but returns a `CustomYieldInstruction` that completes when the tween finishes. Use with `yield return` to sequence work after the tween. |
| `UseUnscaledTime` | `false` (default): tweens advance with `Time.deltaTime`. `true`: with `Time.unscaledDeltaTime`, e.g. to keep UI animating while the game is paused via `Time.timeScale`. Also applies to a running tween. |
| `Stop()` | Cancels any in-progress tween and sets `IsTweening` to `false`. |
| `Dispose()` | Calls `Stop()`. |

### Usage

```c#
CoroutineHelper coroutineHelper = gameObject.AddComponent<CoroutineHelper>();
FloatValueTweener tweener = new FloatValueTweener(coroutineHelper);

// Tween a UI alpha from 0 to 1 over 0.5 seconds using an ease-out curve.
tweener.Perform(
    start: 0f,
    end: 1f,
    duration: 0.5f,
    tweenType: TweenType.EaseOutCubic,
    updateAction: value => canvasGroup.alpha = value
);

// Check whether a tween is in progress.
bool active = tweener.IsTweening;

// Cancel early.
tweener.Stop();

// Or dispose when the owning object is destroyed.
tweener.Dispose();
```

---

## Timer
`Timer` runs a countdown coroutine via a `CoroutineHelper` and fires `OnDone` when complete. It implements `IDisposable` — disposing stops the timer and resets state.

```c#
class Timer : IDisposable
```

### Usage

#### Create and Start
```c#
CoroutineHelper coroutineHelper = gameObject.AddComponent<CoroutineHelper>();
Timer timer = new Timer(coroutineHelper);
timer.OnDone += () => { Debug.Log("Done!"); };
timer.StartWith(duration: 3f);
timer.StartWith(duration: 3f, unscaled: true); // uses unscaled delta time
// StartWith returns the Timer instance for fluent chaining
timer.OnDone += () => { Debug.Log("Done!"); };
Timer t = new Timer(coroutineHelper).StartWith(duration: 5f);
```

#### Read Progress
```c#
float elapsed    = timer.Elapsed;  // seconds elapsed
float progress   = timer.Progress; // 0.0 – 1.0 while running; 1.0 when stopped
bool  isRunning  = timer.Running;
```

#### Stop and Reset
```c#
timer.Stop();    // stops and resets Elapsed/Running
timer.Dispose(); // same as Stop()
```

---

## CoroutineHelper
`CoroutineHelper` is a `MonoBehaviour` running coroutines for plain C# classes. `Run` returns a `CoroutineHandle` to stop the routine or check whether it's still running; a `CoroutineSlot` keeps at most one routine of a kind running. It is the backing component for `Timer` and the value tweeners.

```c#
class CoroutineHelper : MonoBehaviour
sealed class CoroutineHandle : CustomYieldInstruction
sealed class CoroutineSlot : IDisposable
```

### Usage

#### Run and Stop
```c#
CoroutineHelper helper = gameObject.AddComponent<CoroutineHelper>();

CoroutineHandle handle = helper.Run(MyEnumerator()); // runs up to the first yield right away
bool running = handle.IsRunning; // false once completed, thrown, stopped or the helper was destroyed
handle.Stop();                   // does nothing if already ended

yield return helper.Run(Other()); // inside a coroutine: waits until Other ended
```
A routine throwing an exception is logged and ends; its handle reports it as not running.

#### One Routine at a Time
```c#
private readonly CoroutineSlot _fadeSlot = new CoroutineSlot(helper);

_fadeSlot.Run(FadeIn());  // stops a running fade first
_fadeSlot.Run(FadeOut());
bool fading = _fadeSlot.IsRunning;
_fadeSlot.Dispose();      // stops the running fade, e.g. in Clean()
```

The string-id methods (`StartWithID`, `ReplaceWithID`, `StopWithID`, `HasRoutineWith`) are obsolete and will be removed in 2.0.0; ids only needed to be unique per routine, which a slot or handle is by construction.

---

## ScriptableObjectId
Abstract base class adding a serialized string `Id` field to `ScriptableObject`. Supports equality comparison by reference (against other assets) or by string ID.

```c#
abstract class ScriptableObjectId : ScriptableObject, IEquatable<ScriptableObjectId>, IEquatable<string>
```

### Usage

#### Define a Typed Id Asset
```c#
[CreateAssetMenu(fileName = "NewItemId", menuName = "Ids/Item Id")]
public class ItemId : ScriptableObjectId { }
```

#### Compare
```c#
ItemId idA; // assigned in Inspector
ItemId idB;
bool sameAsset  = idA == idB;            // reference equality
bool sameString = idA == "some-id";      // compares Id field
string idValue  = idA.ToString();        // returns Id field
```

#### Typed Keys with Id&lt;TDefinition&gt;
Deriving from `ScriptableObjectId<TSelf>` instead adds a `Key` of type `Id<TSelf>`: a value type compared by its string (ordinal). Logic, dictionaries and tests can use keys without loading the assets, and keys of different definition types can't be mixed up.
```c#
public class ItemId : ScriptableObjectId<ItemId> { }

Id<ItemId> key = itemAsset.Key;
Dictionary<Id<ItemId>, int> counts = new Dictionary<Id<ItemId>, int>();
counts[new Id<ItemId>("apple")] = 3;     // e.g. in a test - no asset needed
bool same = key == new Id<ItemId>("apple");
bool valid = key.IsValid;                // false for default or empty ids
```
Switching an existing class from `ScriptableObjectId` to `ScriptableObjectId<TSelf>` keeps its serialized ids - the field stays declared in `ScriptableObjectId`. Never rename the `Id` property: its backing field `<Id>k__BackingField` is what the assets store.

#### Validate Ids
Keys only work when every asset has an id that's unique within its type. **Calluna > Diagnostics > Validate ScriptableObject Ids** logs empty and duplicated ids. To guard a project, run the check in an edit mode test (the test assembly references `Calluna.Core.Editor`):
```c#
[Test]
public void ScriptableObjectIds_AreSetAndUnique() =>
    Assert.IsEmpty(Calluna.Core.Editor.ScriptableObjectIdValidator.FindProblems());
```
`ScriptableObjectIdValidation.FindProblems` runs the same checks on any given ids.

---

## EnumerableUtility
**Obsolete, will be removed in 2.0.0.** Iterating an `IEnumerable<T>` allocates an enumerator for most collections, so these helpers don't avoid allocations as intended; loop over the concrete collection instead. No longer used by the `AccumulatingValue` types.

```c#
static class EnumerableUtility
```

| Method | Description |
|---|---|
| `Sum(IEnumerable<int>)` | Returns the sum of all integers |
| `Sum(IEnumerable<float>)` | Returns the sum of all floats |
| `Product(IEnumerable<float>)` | Returns the product, seeded from `1f` (returns `1` for an empty sequence) |
| `Any(IEnumerable<bool>)` | Returns `true` if at least one value is `true` |
| `All(IEnumerable<bool>)` | Returns `true` if every value is `true` |

### Usage

```c#
int   total   = EnumerableUtility.Sum(new[] { 1, 2, 3 });        // 6
float product = EnumerableUtility.Product(new[] { 2f, 3f, 4f }); // 24f
bool  anyOn   = EnumerableUtility.Any(new[] { false, true });     // true
bool  allOn   = EnumerableUtility.All(new[] { true, true });      // true
```

---

## DontDestroyOnLoad
`DontDestroyOnLoad` is a `MonoBehaviour` that calls `DontDestroyOnLoad` on its `GameObject` during `Awake`, keeping the object alive across scene loads. Attach it to any root `GameObject` that should persist.

```c#
class DontDestroyOnLoad : MonoBehaviour
```

### Usage

```c#
// In the Inspector: add the DontDestroyOnLoad component to a root GameObject.
// No code required — the component handles persistence automatically on Awake.
```

---

## Event Bus

`IEventBus` is a typed publish/subscribe message bus. Use it to decouple publishers from subscribers — neither side needs a direct reference to the other. The concrete `EventBus` class uses breadth-first, re-entrancy-safe dispatch: events published from inside a listener are queued and processed after the current dispatch batch completes. A listener throwing an exception is logged via `Debug.LogException` and doesn't stop the other listeners or the delivery of queued events. Listeners subscribed or unsubscribed during dispatch take effect from the next delivered event on.

```c#
interface IEventBus
class EventBus : IEventBus
```

### Interface

| Method | Description |
|---|---|
| `Subscribe<TEvent>(Action<TEvent> listener)` | Register a listener for events of type `TEvent`. Returns an `IDisposable` that unsubscribes it. |
| `Unsubscribe<TEvent>(Action<TEvent> listener)` | Remove a previously registered listener. Safe to call if not subscribed. |
| `Publish<TEvent>(TEvent evt)` | Dispatch `evt` to all current listeners of `TEvent`. |

### Usage

#### Define an event

```c#
public struct PlayerDiedEvent { }

public struct ScoreChangedEvent
{
    public int NewScore;
}
```

#### Create and use directly

```c#
IEventBus bus = new EventBus();

IDisposable subscription = bus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
bus.Publish(new ScoreChangedEvent { NewScore = 42 });
subscription.Dispose(); // or: bus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);

void OnScoreChanged(ScoreChangedEvent evt) => Debug.Log(evt.NewScore);
```

#### Expose as IEventBus

Always inject or store the bus as `IEventBus`, not as `EventBus`, so the concrete type is not coupled to call sites.

```c#
private IEventBus _eventBus = new EventBus();
public IEventBus EventBus => _eventBus;
```

**Note:** When using `com.calluna.di`, a single `EventBus` is automatically bound as a singleton at `AppContext` scope — you do not need to create one manually.

---

## UpdateScheduler
`UpdateScheduler` is a `MonoBehaviour` that defers callbacks to a specific Unity update phase within the current frame, deduplicating multiple requests for the same callback. Scheduling a callback that is already pending is ignored, so several changes in one frame cause a single call. Callbacks count as the same when they are equal delegates - the same method on the same instance, e.g. a method group. A lambda capturing variables is a new delegate on every call and is never deduplicated; keep it in a field instead.

```c#
class UpdateScheduler : MonoBehaviour
enum UpdateScheduler.SchedulePhase { Update, LateUpdate }
```

### Usage

```c#
UpdateScheduler scheduler = gameObject.AddComponent<UpdateScheduler>();

// Schedule a callback for the end of this frame (LateUpdate is the default phase).
scheduler.ScheduleOnce(RefreshUI);
scheduler.ScheduleOnce(RefreshUI); // ignored - already pending

// Schedule a callback for Update instead.
scheduler.ScheduleOnce(ApplyMovement, UpdateScheduler.SchedulePhase.Update);

// Cancel a specific pending callback before it runs.
scheduler.Cancel(RefreshUI);

// Cancel all pending callbacks.
scheduler.CancelAll();
```
The string-id overloads `ScheduleOnce(string, Action, SchedulePhase)` and `Cancel(string)` are obsolete and will be removed in 2.0.0.

---

## Samples

The following samples are included and can be imported via **Window > Package Manager**:

| Sample | Description |
|---|---|
| Observable Test | Demonstrates Observable value usage |
| Tween Test | Demonstrates easing functions |
| Timer Test | Demonstrates the Timer class |
| Coroutine Helper Test | Demonstrates CoroutineHelper |
| Accumulating Value Test | Demonstrates AccumulatingIntValue, AccumulatingFloatValue, and AccumulatingBoolValue |
