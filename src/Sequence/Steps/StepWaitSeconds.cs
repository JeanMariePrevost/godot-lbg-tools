using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.LBGTools.Sequencing.Steps;

/// <summary>
/// Waits for a specified number of seconds.
/// </summary>
public class StepWaitSeconds : IStep {
    public float Seconds { get; set; }

    public StepWaitSeconds(float seconds) {
        // Print($"StepWaitSeconds constructor called with {seconds} seconds");
        Seconds = seconds;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        // Print($"Waiting for {Seconds} seconds");
        var tree = Engine.GetMainLoop() as SceneTree ?? throw new Exception("Failed to get SceneTree");
        var timer = tree.CreateTimer(Seconds);
        await timer.ToSignal(timer, "timeout");
        // Print($"Finished waiting for {Seconds} seconds");
    }

    public void Reset() {
        // No state to reset
    }
}
