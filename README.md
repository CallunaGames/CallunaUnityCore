# CallunaUnityCore
The core scripts of Calluna Games' Unity projects

## Observable Values
Observable values notify listeners when their value changes.
Use the `ReadonlyObservable<T>` interface to expose an `Observable<T>` to other classes without allowing external writes.

```c#
class Observable<T> : Observable, ReadonlyObservable<T>
interface ReadonlyObservable<T>
```

### Usage

#### Read Value
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
TValue value = observableValue.Value;
```

#### Read Value Implicitly
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
TValue value = observableValue;
```

#### Set Value
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
observableValue.Value = new TValue();
```

#### Set Implicitly
```c#
Observable<TValue> observableValue = new TValue();
```

#### Check for Value
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
bool hasValue = observableValue.HasValue;
```

#### Set Without Notifying Listeners
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
observableValue.SetValueWithoutNotify(new TValue());
```

#### Expose Readonly Observable
```c#
private Observable<TValue> _observableValue = new TValue();
public ReadonlyObservable<TValue> ObservableValue => _observableValue;
```

#### Listen to Value Change
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
observableValue.OnChanged += () => { Debug.Log("On value changed"); };
observableValue.Value = new TValue();
```
**On value changed**

#### Listen to Value Change With Values
```c#
Observable<int> observableValue = new Observable<int>() { Value = 2 };
observableValue.OnChangedWithValues += (int former, int newValue) => { Debug.Log($"On value changed ({former} | {newValue})"); };
observableValue.Value = 5;
```
**On value changed (2 | 5)**

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

#### Add / Remove / Clear
```c#
_list.Add(item);
_list.Remove(item);       // returns bool
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

#### Override Contents
```c#
_list.OverrideWith(newItems); // replaces, removes, or adds items to match the new sequence
```

#### Listen to Events
```c#
_list.OnItemAdded += (TValue item, int index) => { };
_list.OnItemRemoved += (TValue item, int index) => { };
_list.OnItemReplaced += (TValue newItem, TValue formerItem, int index) => { };
_list.OnItemsSwapped += (TValue newAt0, int index0, TValue newAt1, int index1) => { };
_list.OnClean += () => { };  // fires once when Clear() is called; OnItemRemoved is NOT fired per element
```

#### Detect Any Change with ObservableListChangeDetector
`ObservableListChangeDetector` routes all four item events and `OnClean` into a single `OnChanged` event, so you can react to any mutation — including `Clear()` — from one subscription.
```c#
using var detector = new ObservableListChangeDetector<TValue>(_list);
detector.OnChanged += () => { Debug.Log("List changed"); };
// detector.Dispose() unsubscribes all listeners
```

---

## Accumulating Values
Accumulating values maintain a named dictionary of partial values that combine into a single result exposed as a `ReadonlyObservable<T>`. Use this to aggregate contributions from multiple independent sources (e.g. stat bonuses).

```c#
abstract class AccumulatingValue<T>
class AccumulatingIntValue : AccumulatingValue<int>        // sums all parts
class AccumulatingFloatValue : AccumulatingValue<float>    // sum (default) or multiply
class AccumulatingBoolValue : AccumulatingValue<bool>      // Any (default) or All
```

### Usage

#### Add, Set, and Remove Parts
```c#
AccumulatingIntValue speed = new AccumulatingIntValue();
speed.Add("base", 10);       // throws if id already exists
speed.Add("bonus", 5);
speed.Set("bonus", 8);       // upserts — safe to call on new or existing id
speed.Remove("bonus");       // throws if id does not exist
```

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
int baseSpeed = speed["base"];
bool found = speed.TryGetValuePart("base", out int val);
```

#### AccumulatingFloatValue Modes
```c#
AccumulatingFloatValue addUp   = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.AddUp);
AccumulatingFloatValue product = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.Multiply);
```

#### AccumulatingBoolValue Modes
```c#
AccumulatingBoolValue anyTrue = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
AccumulatingBoolValue allTrue = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.All);
```

---

## Tween
Static utility class with easing functions based on [easings.net](https://easings.net/). All functions accept a `float` in the range `[0, 1]` and throw `ArgumentException` for out-of-range input.

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
float progress   = timer.Progress; // 0.0 – 1.0
bool  isRunning  = timer.Running;
```

#### Stop and Reset
```c#
timer.StopTimer(); // stops and resets Elapsed/Running
timer.Dispose();   // same as StopTimer()
```

---

## CoroutineHelper
`CoroutineHelper` is a `MonoBehaviour` that wraps Unity coroutines with string IDs, preventing duplicate coroutines and allowing targeted stop or replace operations. It is the backing component for `Timer`.

```c#
class CoroutineHelper : MonoBehaviour
```

### Usage

```c#
CoroutineHelper helper = gameObject.AddComponent<CoroutineHelper>();

helper.StartWithID(MyEnumerator(), "myId");    // throws if id already running
helper.ReplaceWithID(MyEnumerator(), "myId");  // stops existing, starts new
helper.StopWithID("myId");                     // returns bool — false if not found
bool running = helper.HasRoutineWith("myId");
```

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

---

## EnumerableUtility
Static utility class providing allocation-free aggregate operations over `IEnumerable<T>` sequences. Used internally by the `AccumulatingValue` types, but available for general use.

```c#
static class EnumerableUtility
```

| Method | Description |
|---|---|
| `Sum(IEnumerable<int>)` | Returns the sum of all integers |
| `Sum(IEnumerable<float>)` | Returns the sum of all floats |
| `Product(IEnumerable<float>)` | Returns the product, seeded from the first element |
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

## Samples

The following samples are included and can be imported via **Window > Package Manager**:

| Sample | Description |
|---|---|
| Observable Test | Demonstrates Observable value usage |
| Tween Test | Demonstrates easing functions |
| Timer Test | Demonstrates the Timer class |
| Coroutine Helper Test | Demonstrates CoroutineHelper |
| Accumulating Value Test | Demonstrates AccumulatingIntValue, AccumulatingFloatValue, and AccumulatingBoolValue |
