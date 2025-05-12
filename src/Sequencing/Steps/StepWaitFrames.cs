using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.Handustry.Sequencing.Steps;

/// <summary>
/// Waits until a certain number of "Process Frames" have occurred.
/// </summary>
public class StepWaitFrames : IStep {
    public int Frames { get; set; }

    public StepWaitFrames(int frames) {
        // Print($"StepWaitFrames constructor called with {frames} frames");
        Frames = frames;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        var tree = Engine.GetMainLoop() as SceneTree ?? throw new Exception("Failed to get SceneTree");
        // Print($"Waiting for {Frames} frames");

        for (int i = 0; i < Frames; i++) {
            await tree.ToSignal(tree, "process_frame");
        }

        // Print($"Finished waiting for {Frames} frames");
    }

    public void Reset() {
        // No state to reset
    }
}
