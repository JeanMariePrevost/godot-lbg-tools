using System;
using System.Threading.Tasks;

namespace LBG.GodotTools.Sequences.Steps;

/// <summary>
/// Calls an action.
/// </summary>
public class StepCallAction : IStep {
    public Action Action { get; set; }

    public StepCallAction(Action action) {
        Action = action;
    }

    public Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        Action();
        return Task.CompletedTask;
    }

    public void Reset() {
        // No state to reset
    }
}
