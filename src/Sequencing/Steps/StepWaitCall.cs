using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.Handustry.Sequencing.Steps;

/// <summary>
/// Waits until a provided action is called.
/// </summary>
public class StepWaitCall : IStep {
    private TaskCompletionSource _tcs = new();
    public Action GetTriggerAction() => () => _tcs.TrySetResult(); // Returns a simple lambda to trigger a task completion

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        await _tcs.Task; // Wait until the task is completed
    }

    public void Reset() {
        _tcs.TrySetResult();
        _tcs = new();
    }
}
