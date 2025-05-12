using System;
using System.Threading.Tasks;

namespace LBG.LBGTools.Sequencing.Steps;

/// <summary>
/// Repeats the entire sequence from the beginning a specified number of times.
/// </summary>
public class StepRepeatSequence : IStep {
    private readonly int Times;
    private int _repeatCount = 0;

    public StepRepeatSequence(int times) {
        Times = times;
    }

    public Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        if (_repeatCount < Times) {
            sequenceControlCallback(SequenceCommand.RepeatFromBeginning);
            _repeatCount++;
        }
        return Task.CompletedTask;
    }

    public void Reset() {
        _repeatCount = 0;
    }
}
