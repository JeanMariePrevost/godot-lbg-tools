using System;
using System.Threading.Tasks;
using Godot;

namespace LBG.Handustry.Sequencing.Steps;

/// <summary>
/// Waits for a specified number of seconds in real time (unaffected by timescale).
/// </summary>
public class StepWaitRealSeconds : IStep {
    public float Seconds { get; set; }

    public StepWaitRealSeconds(float seconds) {
        // Print($"StepWaitRealSeconds constructor called with {seconds} seconds");
        Seconds = seconds;
    }

    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        // Print($"Waiting for {Seconds} seconds in real time");
        var tree = Engine.GetMainLoop() as SceneTree ?? throw new Exception("Failed to get SceneTree");
        var timer = tree.CreateTimer(Seconds, ignoreTimeScale: true);
        await timer.ToSignal(timer, "timeout");
        // Print($"Finished waiting for {Seconds} seconds in real time");
    }

    public void Reset() {
        // No state to reset
    }
}
