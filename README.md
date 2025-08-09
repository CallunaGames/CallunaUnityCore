# CallunaUnityCore
The core scripts of Calluna Games' Unity projects

## Observable Values
Observable values can be used to notify listeners when the value changes.
Use the *ReadonlyObservable* interface if you want to expose an *Observable* to other classes if the value shall not be changed from outside.

```c#
class Observable : ReadonlyObservable
interface ReadonlyObservable
```

### Usage
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
```

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

#### Expose Readonly Observable
```c#
private Observable<TValue> _observableValue = new TValue();
public ReadonlyObservable<TValue> ObservableValue => _observableValue;
```

#### Listen to Value Change
```c#
Observable<TValue> observableValue = new Observable<TValue>() { Value = new TValue() };
observableValue.OnChanged += () => { Debug.Log("On value changed") }
observableValue.Value = new TValue();

*On value changed*
```

#### Listen to Value Change With Values
```c#
Observable<int> observableValue = new Observable<int>() { Value = 2 };
observableValue.OnChangedWithValues += (TValue former, TValue newValue) => { Debug.Log($"On value changed ({former} | {newValue})") }
observableValue.Value = 5;

*On value changed (2 | 5)*
```