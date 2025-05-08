namespace LBG.LBGTools.Signal;

/// <summary>
/// A type-safe signal that takes two arguments.
/// </summary>
/// <typeparam name="T">The type of the argument passed to the signal's listeners.</typeparam>
public sealed class LBGSignal<T1, T2>
    : AbstractLBGSignal<Action<T1, T2>, ValueTuple<T1, T2>> {
    public LBGSignal() { }

    /// <summary>
    /// Converts a user-supplied Action<T1> into an Action<(T1)> used internally.
    /// </summary>
    protected override Action<ValueTuple<T1, T2>> Wrap(Action<T1, T2> callback)
        => t => callback(t.Item1, t.Item2);

    /// <summary>
    /// Emits the signal with one argument.
    /// </summary>
    public void Emit(T1 arg1, T2 arg2) => Emit(new ValueTuple<T1, T2>(arg1, arg2));
}

