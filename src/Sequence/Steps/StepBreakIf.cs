using System;
using System.Threading.Tasks;

namespace LBG.GodotTools.Sequences.Steps;

/// <summary>
/// Interrupts the sequence if the predicate returns true.
/// </summary>
public class StepBreakIf : IStep {

    private readonly Func<bool> _predicate;

    public StepBreakIf(Func<bool> predicate) {
        _predicate = predicate;
    }

    public Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        if (_predicate()) {
            sequenceControlCallback(SequenceCommand.Break);
        }
        return Task.CompletedTask;
    }

    public void Reset() {
        // No state to reset
    }
}
