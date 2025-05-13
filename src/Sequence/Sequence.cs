using System;
using System.Collections.Generic;
using LBG.GodotTools.Sequences.Steps;
using Godot;
using LBG.GodotTools.Signal;

namespace LBG.GodotTools.Sequences;

/// <summary>
/// A sequencing utility that allows scheduling of steps (delays, signals, conditions, etc.) to be executed in order.
/// Fire-and-forget by design; not thread-safe.
/// </summary>
/// <example>
/// new Sequence().WaitFrames(3).Do(myFunction).Start();
/// </example>
/// <example>
/// new Sequence()
///     .WaitForSignal(button, "pressed")
///     .WaitRealSeconds(1.0f)
///     .Do(() => GD.Print("Button was pressed. Delayed action triggered."))
///     .Start();
/// </example>
// Note: Possible better names:
// - Proc (procedure)
// - Chain
// - Cue
// - Flow
// - Job
// - Routine
// - Aside
// - Sequence
// - Schedule
// - Later
// - After
// - Then
// - Tasker / Tasklet
// - Workflow
// - Launcher
// - Runner
// - Invoke
// - Post
// - Delay
// -
// TODO:Consider reworking all Steps such that they accept a CancellationToken and can _all_ interrupt gracefully?
// TODO: Pause / resmume / abort
// TODO: Do<T> possible through closures or something? It's the state I wonder about.
// TODO: "While" loop witha predicate? Always from start or with a "BeginLoop()" anchor?
// TODO: Allow for composition of sequences?
// TODO: Is composition is added, consider adding conditional branching and/or parallel execution/propagation.
// TODO: Consider labels? E.g. start from there, loop back to here...?
// TODO: Add a sort of "mutex" to prevent modification while running? Or simply make each run use a copy? Might complicate things though with like repeats and such.
// TODO: Make thread-safe? Unlikely seeing the "fire-and-forget" nature of the class.
// TODO: Parallel and/or Race ? E.g. run these X steps at once?
// TODO: "WaitFor" other sequence? "WaitForAll" and/or "WaitForAny" ?
// TODO: Timescale?
// TODO: "ImmediatelyTrigerNext"? Manual sequences? (e.g. queue everything, but YOU handle the stepping)
// TODO: EmitSignal sugar needed or does a simple DO(()=>EmitSignal()) suffice?
// TODO: Debug printing of steps and state?
// TODO: Wait on promise / task?
// TODO: Sequence context? Sequence-wide state?
public class Sequence {
    public List<IStep> Steps { get; set; } = [];

    public bool IsRunning { get; private set; } = false;

    private int _currentStepIndex = 0; // Index of the current step in the sequence while executing
    private bool _breakRequested = false;

    // -------------------------------------
    // Static "Shortcuts" functions
    // -------------------------------------

    /// <summary> Invoke an action after a specified number of seconds (game time). </summary>
    public static void DoAfterSeconds(float seconds, Action action) {
        var sequence = new Sequence();
        sequence.WaitSeconds(seconds).Do(action).Start();
    }

    /// <summary> Invoke an action after a specified number of frames ("Process Frames"). </summary>
    public static void DoAfterFrames(int frames, Action action) {
        var sequence = new Sequence();
        sequence.WaitFrames(frames).Do(action).Start();
    }

    /// <summary> Invoke an action after a condition returns true. </summary>
    public static void DoAfterCondition(Func<bool> predicate, Action action) {
        var sequence = new Sequence();
        sequence.WaitUntil(predicate).Do(action).Start();
    }

    public Sequence() { }

    private Sequence AddStep(IStep step) {
        Steps.Add(step);
        return this;
    }

    /// <summary> Pause for a specified number of seconds (game time). </summary>
    public Sequence WaitSeconds(float seconds) {
        return AddStep(new StepWaitSeconds(seconds));
    }

    /// <summary> Pause for a specified number of seconds (real time, unaffected by timescale). </summary>
    public Sequence WaitRealSeconds(float seconds) {
        return AddStep(new StepWaitRealSeconds(seconds));
    }

    /// <summary> Pause for a specified number of frames ("Process Frames"). </summary>
    public Sequence WaitFrames(int frames) {
        return AddStep(new StepWaitFrames(frames));
    }

    /// <summary> Pause for a specified number of physics frames ("Physics Frames"). </summary>
    public Sequence WaitPhysicsFrames(int frames) {
        return AddStep(new StepWaitPhysicsFrames(frames));
    }

    /// <summary> Wait until a predicate returns true (evaluated every process frame) </summary>
    /// <remarks>
    /// <b>NB:</b> value types (e.g. <c>Vector2</c>, <c>float</c>) are <b>copied</b>
    /// when the closure is created and will not update. Use properties, methods or reference types instead.
    /// </remarks>
    /// <example>
    /// // This will work as expected.
    /// sequence.WaitUntil(() => MyObject.GetPosition().X > 100);
    ///
    /// // This will be "frozen in time" and never refresh.
    /// Vector2 pos = MyObject.GetPosition();
    /// sequence.WaitUntil(() => pos.X > 100);
    /// </example>
    public Sequence WaitUntil(Func<bool> predicate) {
        return AddStep(new StepWaitUntil(predicate));
    }

    /// <summary> Wait until a provided action is called. </summary>
    public Sequence WaitForCall(out Action triggerAction) {
        var step = new StepWaitCall();
        triggerAction = step.GetTriggerAction();
        return AddStep(step);
    }

    /// <summary> Wait for a Godot signal to be emitted. </summary>
    /// <example>
    /// sequence.WaitForSignal(myNode, "ready"); // Wait for node to be ready
    /// sequence.WaitForSignal(myButton, "pressed"); // Wait for button press
    /// </example>
    public Sequence WaitForSignal(GodotObject source, string signalName) {
        return AddStep(new StepWaitForSignal(source, signalName));
    }

    /// <summary> Wait for a LBGSignal to be emitted. </summary>
    public Sequence WaitForLBGSignal(LBGSignal signal) {
        return AddStep(new StepWaitForLBGSignal(signal));
    }

    /// <summary> Wait for a LBGSignal<T> to be emitted. </summary>
    /// <remarks> A Godot/mono bug means we cannot use straight-up overloading here, it resolves to the non-generic and chokes. Hence the "Typed" suffix. </remarks>
    public Sequence WaitForLBGSignalTyped<T>(LBGSignal<T> signal) {
        return AddStep(new StepWaitForLBGSignal<T>(signal));
    }

    /// <summary> Repeat the entire sequence up to this point from the beginning. </summary>
    public Sequence RepeatSequence(int times) {
        return AddStep(new StepRepeatSequence(times));
    }

    /// <summary> Repeat the previous step a specified number of times. </summary>
    public Sequence RepeatPrevious(int times) {
        return AddStep(new StepRepeatPrevious(times));
    }

    /// <summary> Break/interrupt the sequence if a predicate returns true. </summary>
    /// <remarks> The remaining steps in the sequence will not be executed. </remarks>
    public Sequence BreakIf(Func<bool> predicate) {
        return AddStep(new StepBreakIf(predicate));
    }

    /// <summary> Removes all steps from the sequence. </summary>
    public Sequence Clear() {
        Steps.Clear();
        return this;
    }

    /// <summary> Execute an action. </summary>
    public Sequence Do(Action action) {
        return AddStep(new StepCallAction(action));
    }

    /// <summary> Sets a timeout for the previous step. </summary>
    public Sequence Timeout(float timeout) {
        if (Steps.Count >= 1) {
            var wrapped = new StepTimeoutDecorator(Steps[^1], timeout);
            Steps[^1] = wrapped;
        } else {
            // PrintError("Cannot set timeout for an empty sequence.");
        }
        return this;
    }

    /// <summary>
    /// Starts executing the sequence asynchronously, one step at a time.
    /// </summary>
    /// <remarks>
    /// This method runs the sequence in order, awaiting each step sequentially.
    /// It is fire-and-forget: callers do not need to await it.
    /// </remarks>
    public async void Start() {
        // Reset the sequence state
        _currentStepIndex = 0;
        _breakRequested = false;
        IsRunning = true;

        // Execute all steps
        for (; _currentStepIndex < Steps.Count; _currentStepIndex++) {
            await Steps[_currentStepIndex].Execute(HandleStepCallback);

            if (_breakRequested) break;
        }

        IsRunning = false;
    }

    private void HandleStepCallback(SequenceCommand command) {
        if (command == SequenceCommand.RepeatFromBeginning) {
            // Reset the state of all steps that came before
            for (int i = 0; i < _currentStepIndex; i++) {
                Steps[i].Reset();
            }
            _currentStepIndex = -1; // To -1 because the for loop will increment it to 0
        } else if (command == SequenceCommand.RepeatPrevious) {
            if (_currentStepIndex >= 1) {
                // Reset the state of the previous step
                Steps[_currentStepIndex - 1].Reset();
                _currentStepIndex -= 2;
            } else {
                // PrintError("Cannot repeat previous step. Are you calling RepeatPrevious() on an empty sequence?");
            }
        } else if (command == SequenceCommand.Break) {
            _breakRequested = true;
        }
    }
}

/// <summary>
/// Basic control operations for sequence flow.
/// </summary>
public enum SequenceCommand {
    /// <summary> Go back to the start of the sequence. </summary>
    RepeatFromBeginning,

    /// <summary> Repeat the last executed step. </summary>
    RepeatPrevious,

    /// <summary> Break out of the sequence entirely. </summary>
    Break
}
