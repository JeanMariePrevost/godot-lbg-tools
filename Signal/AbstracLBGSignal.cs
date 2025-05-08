namespace LBG.LBGTools.Signal;

/// <summary>
/// Base abstract LBGSignal class that handles the logic by wrapping every callback in a tuple.
/// It also keeps a map of callbacks to their wrapped version, allowing for "finding" the callback by its original signature.
/// This allows it to handle any arity of callbacks for a minimal performance cost, e.g. Action<T1>, Action<T1, T2>, Action<T1, T2, T3>, etc.
/// </summary>
/// <typeparam name="TExposed">
/// The type used by this adapter layer, exposed to the classes that use/extend it, e.g. Action<T1, T2, T3>.
/// It is the type that gets "exposed" though the public interface of this class, e.g. Add, Remove, etc.
/// Has to be a delegate type (e.g. Action, Func, etc.).
/// </typeparam>
/// <typeparam name="TWrapped">
/// The type that TExposed gets wrapped into, to be used by LBGSignal<T> under the hood, e.g. (T1, T2, T3).
/// This one is a tuple type, not a delegate.
/// </typeparam>
public abstract class AbstractLBGSignal<TExposed, TWrapped> where TExposed : Delegate // i.e. it "must be a function"
{
    /// <summary>
    /// A map of the callbacks to their wrapped versions, allowing for finding/removing the original delegate.
    /// Otherwise wrapping creates a new delegate each time, making it impossible to find the original one.
    /// </summary>
    private readonly Dictionary<TExposed, Action<TWrapped>> _map = [];

    /// <summary>
    /// Converts a user-supplied callback (e.g. Action<T1,T2>)
    /// into a callback that takes the internal wrapped payload (e.g. (T1,T2)).
    /// </summary>
    protected abstract Action<TWrapped> Wrap(TExposed calback);

    /// <summary> The list of callbacks to be called when the event is triggered. </summary>
	protected List<SignalListener<TWrapped>> Listeners { get; } = [];

    // ------------------------------------------
    // Public interface
    // ------------------------------------------

    /// <summary>
    /// Adds a callback to the signal.
    /// </summary>
    /// <param name="callback">The callback action to be invoked when the signal is triggered.</param>
    /// <param name="timesToTrigger">The number of times the callback should be triggered before being removed.</param>
    /// <param name="priority">
    /// The priority of the callback. Higher priority callbacks are executed first.
    /// Defaults to 0 if not specified.
    /// </param>
    /// <remarks>
    /// Use the fluent API to set the priority and times to trigger. E.g.:
    /// <code>signal.Add(callback).WithPriority(1).CallLimit(2);</code>
    /// <code>signal.Add(callback).Once();</code>
    /// </remarks>
    public SignalListener<TWrapped> Add(TExposed callback) {
        var wrappedCallback = GetOrCreate(callback);
        var entry = new SignalListener<TWrapped>(wrappedCallback);
        Listeners.Add(entry);
        return entry;
    }

    public bool Contains(TExposed cb) => _map.ContainsKey(cb);

    public void Remove(TExposed cb) {
        if (_map.TryGetValue(cb, out var wrapped)) {
            // We have a wrapped version of this callback, so remove it from the listeners.
            Listeners.RemoveAll(listener => listener.Callback == wrapped);
            // Remove the wrapped version from the map as well.
            _map.Remove(cb);
        }
    }

    /// <summary> Removes all registered callbacks. </summary>
    public void Clear() {
        Listeners.Clear();
        _map.Clear();
    }

    /// <summary> Returns the number of registered callbacks. </summary>
    public int Count => Listeners.Count;

    /// <summary> Triggers the signal, calling all registered callbacks. </summary>
    /// <param name="arg">The argument to pass to the callbacks.</param>
    public void Emit(TWrapped arg) {
        var toRun = Listeners
            .OrderByDescending(e => e.Priority) // Sort by priority, highest first.
            .ToList(); // Create a copy of the list to avoid modifying it while iterating.

        foreach (var entry in toRun) {
            entry.Call(arg);
        }

        // Remove any callbacks that have been marked for removal.
        Listeners.RemoveAll(cb => cb.IsExpired);
    }

    // ------------------------------------------
    // Helper functions
    // ------------------------------------------

    /// <summary>
    /// Helper function that checks if the callback is already wrapped and stored in the map.
    /// This allows us to use the wrapped version of callbacks just like the original ones.
    /// </summary>
    /// <param name="callbackDelegate">The target callback function </param>
    /// <returns> The wrapped version of the callback using only a single argument </returns>
    private Action<TWrapped> GetOrCreate(TExposed callbackDelegate) {
        if (_map.TryGetValue(callbackDelegate, out var wrapped)) {
            // We already have a wrapped version of this callback, so just return it.
            return wrapped;
        }

        // We don't have a wrapped version yet, so create one and store it in the map.
        wrapped = Wrap(callbackDelegate);
        _map[callbackDelegate] = wrapped;
        return wrapped;
    }
}
