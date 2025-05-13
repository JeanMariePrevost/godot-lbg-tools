using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.GodotTools.Sequences.Steps;

/// <summary>
/// Waits until a predicate returns true.
/// </summary>
public class StepWaitUntil : IStep {
    private readonly Func<bool> _predicate;

    public StepWaitUntil(Func<bool> predicate) {
        _predicate = predicate;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        var tree = Engine.GetMainLoop() as SceneTree ?? throw new Exception("Failed to get SceneTree");

        while (!_predicate()) {
            // Wait for next process frame
            await tree.ToSignal(tree, "process_frame");
        }
    }

    public void Reset() {
        // No state to reset
    }
}
