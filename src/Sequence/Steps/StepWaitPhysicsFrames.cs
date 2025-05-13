using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.GodotTools.Sequences.Steps;

/// <summary>
/// Waits until a certain number of "Physics Frames" have occurred.
/// </summary>
public class StepWaitPhysicsFrames : IStep {
    public int Frames { get; set; }

    public StepWaitPhysicsFrames(int frames) {
        // Print($"StepWaitPhysicsFrames constructor called with {frames} frames");
        Frames = frames;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        var tree = Engine.GetMainLoop() as SceneTree ?? throw new Exception("Failed to get SceneTree");
        // Print($"Waiting for {Frames} physics frames");

        for (int i = 0; i < Frames; i++) {
            await tree.ToSignal(tree, "physics_frame");
        }

        // Print($"Finished waiting for {Frames} physics frames");
    }

    public void Reset() {
        // No state to reset
    }
}
