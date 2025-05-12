using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.Handustry.Sequencing.Steps;

/// <summary>
/// Waits for a Godot signal to be emitted.
/// </summary>
public class StepWaitForSignal : IStep {
    private readonly GodotObject _source;
    private readonly string _signalName;

    public StepWaitForSignal(GodotObject source, string signalName) {
        _source = source;
        _signalName = signalName;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        await _source.ToSignal(_source, _signalName);
    }

    public void Reset() {
        // No state to reset
    }
}
