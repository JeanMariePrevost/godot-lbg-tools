namespace LBG.LBGTools.Signal;

/// <summary>
/// Holds a callback and its associated properties, allowing for the storing of state related to the callback,
/// like priority and how many times it should be called.
/// 
/// Also enables the use of a "fluent" style API for modifying the callback's properties.
/// </summary>
/// <typeparam name="T">The type of the argument passed to the callback.</typeparam>
/// <remarks> This class is used internally by the LbgSignal class to manage the callbacks. </remarks>
public class SignalListener<T> {
    // --------------------------------
    //  Data section
    // -------------------------------

    /// <summary>The actual callback delegate.</summary>
    public Action<T>? Callback { get; private set; }

    /// <summary>
    /// Controls callback execution order.
    /// Higher values are called first.
    /// </summary>
    public int Priority { get; private set; } = 0;

    /// <summary>
    /// If set, the callback will be removed when this runs out
    /// Keep null to make it unlimited.
    /// </summary>
    public int? RemainingInvocations { get; private set; } = null;

    /// <summary>
    /// Returns true if this entry should be removed.
    /// Calling an expired entry will do nothing.
    /// </summary>
    public bool IsExpired => Callback == null || RemainingInvocations == 0;

    public SignalListener(Action<T> callback) {
        Callback = callback;
    }

    /// <summary>
    /// Calls the callback with the given argument and updates the internal trigger count.
    /// </summary>
    public void Call(T arg) {
        if (IsExpired) {
            return;
        }

        // Call the delegate
        Callback?.Invoke(arg);

        // Apply the trigger count logic
        if (RemainingInvocations is int remaining) {
            RemainingInvocations = remaining - 1;
            if (RemainingInvocations == 0) {
                Callback = null;
            }
        }
    }

    // --------------------------------
    //  Fluent Builder section
    // -------------------------------

    /// <summary>Sets the priority of this callback.</summary>
    public SignalListener<T> WithPriority(int priority) {
        Priority = priority;
        return this;
    }

    /// <summary>Sets how many times this listener can be triggered before it expires.</summary>
    public SignalListener<T> CallLimit(int times) {
        RemainingInvocations = times;
        return this;
    }

    /// <summary>Shorthand for ForTimes(1).</summary>
    public SignalListener<T> Once() {
        return CallLimit(1);
    }
}

