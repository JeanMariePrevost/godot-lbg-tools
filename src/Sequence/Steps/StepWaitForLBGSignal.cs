using System;
using System.Threading.Tasks;
using LBG.LBGTools.Signal;

namespace LBG.LBGTools.Sequencing.Steps;

/// <summary>
/// Waits for a LBGSignal to be emitted.
/// </summary>
/// <remarks>
/// <b>NB:</b> Clearing the listeners on the signal will block this step.
/// </remarks>
public class StepWaitForLBGSignal : IStep {
    private readonly LBGSignal? _signal;
    private TaskCompletionSource _tcs = new();

    public StepWaitForLBGSignal(LBGSignal signal) {
        _signal = signal;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        if (_signal != null) {
            _signal.Add(OnSignalReceived);
        } else {
            throw new Exception("No signal provided to StepWaitForLBGSignal");
        }
        await _tcs.Task; // Wait until the task is completed
    }

    private void OnSignalReceived() {
        _tcs.TrySetResult();// complete the task
    }

    public void Reset() {
        _signal?.Remove(OnSignalReceived);
        _tcs.TrySetResult();
        _tcs = new();
    }
}

/// <summary>
/// Waits for a LBGSignal<T> to be emitted.
/// </summary>
/// <remarks>
/// <b>NB:</b> Clearing the listeners on the signal will block this step.
/// </remarks>
public class StepWaitForLBGSignal<T> : IStep {
    private readonly LBGSignal<T>? _signal;
    private TaskCompletionSource _tcs = new();

    public StepWaitForLBGSignal(LBGSignal<T> signal) {
        _signal = signal;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        if (_signal != null) {
            _signal.Add(OnSignalReceived);
        } else {
            throw new Exception("No signal provided to StepWaitForLBGSignal");
        }
        await _tcs.Task; // Wait until the task is completed
    }

    private void OnSignalReceived(T _) {
        _tcs.TrySetResult();// complete the task
    }

    public void Reset() {
        _signal?.Remove(OnSignalReceived);
        _tcs.TrySetResult();
        _tcs = new();
    }
}

