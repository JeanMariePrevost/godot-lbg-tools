using System;
using System.Threading.Tasks;

namespace LBG.GodotTools.Sequences.Steps;

/// <summary>
/// Repeats the previous step a specified number of times.
/// </summary>
public class StepRepeatPrevious : IStep {
    private readonly int Times;
    private int _repeatCount = 0;

    public StepRepeatPrevious(int times) {
        Times = times;
    }

    public Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        if (_repeatCount < Times) {
            sequenceControlCallback(SequenceCommand.RepeatPrevious);
            _repeatCount++;
        }
        return Task.CompletedTask;
    }

    public void Reset() {
        _repeatCount = 0;
    }
}
