using Xunit;
using LBG.GodotTools.Sequences;

namespace LBG.Tests.GodotTools.Sequences;
public class SequenceTests {
    // TODO: Move over to GdUnit4 because these need the Godot Engine
    // [Fact]
    // public async Task Test_DoAfterSeconds() {
    //     bool actionExecuted = false;
    //     Sequence.DoAfterSeconds(1.0f, () => actionExecuted = true);
    //     await Task.Delay(1100); // Wait for the action to execute
    //     Assert.True(actionExecuted, "Action should be executed after 1 second.");
    // }

    // [Fact]
    // public void Test_DoAfterFrames() {
    //     bool actionExecuted = false;
    //     Sequence.DoAfterFrames(1, () => actionExecuted = true);
    //     // Simulate frame delay
    //     Task.Delay(16).Wait(); // Assuming ~60 FPS, wait for ~1 frame
    //     Assert.True(actionExecuted, "Action should be executed after 1 frame.");
    // }

    [Fact]
    public void Test_SequenceDo() {
        var sequence = new Sequence();
        int counter = 0;

        sequence
            .Do(() => counter++)
            .Start();

        Assert.Equal(1, counter);
    }

    [Fact]
    public void Test_RepeatSequence() {
        var sequence = new Sequence();
        int counter = 0;

        sequence
            .Do(() => counter++)
            .RepeatSequence(2);

        sequence.Start();

        Assert.Equal(3, counter); // 1 initial + 2 repeats
    }

    [Fact]
    public void Test_BreakIf() {
        var sequence = new Sequence();
        int counter = 0;

        sequence
            .Do(() => counter++)
            .BreakIf(() => true) // This will break the sequence
            .Do(() => counter++);

        sequence.Start();

        Assert.Equal(1, counter); // Only the first action should execute
    }

    [Fact]
    public void Test_Clear() {
        var sequence = new Sequence();
        sequence
            .Do(() => { });

        Assert.NotEmpty(sequence.Steps); // Ensure steps are added

        sequence.Clear();
        Assert.Empty(sequence.Steps); // Ensure steps are cleared
    }
}

