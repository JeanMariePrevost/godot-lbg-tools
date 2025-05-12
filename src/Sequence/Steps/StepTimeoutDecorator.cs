using System;
using System.Threading.Tasks;

namespace LBG.LBGTools.Sequencing.Steps;

/// <summary>
/// Wraps a step to add a timeout function to it.
/// </summary>
public class StepTimeoutDecorator : IStep {

    private IStep _wrappedStep;
    private float _timeout;

    public StepTimeoutDecorator(IStep step, float timeout) {
        _wrappedStep = step;
        _timeout = timeout;
    }

    /// <summary>Executes this step with a timeout.</summary>
    /// <remarks>
    /// <b>NB:</b> The Step will not be aborted by the timeout. Be wary of side effects if using this.
    /// It <i>should</i> be safe to use with passive steps like waiting and conditionals, but could still lead to side effects, orphaned references, etc.
    /// </remarks>
    /// <param name="sequenceControlCallback">Callback for sequence control. Most steps can ignore this.</param>
    public async Task Execute(Action<SequenceCommand> sequenceControlCallback) {
        // Run both the Step and a timeout task in parallel, first to complete is the result
        var task = _wrappedStep.Execute(sequenceControlCallback);
        var timeoutTask = Task.Delay((int)(_timeout * 1000));

        if (await Task.WhenAny(task, timeoutTask) == task) {
            await task; // Let exceptions bubble up
        } else {
            // If we get here, the timeout task completed first
            // Print($"Step timed out after {_timeout} seconds.");
        }
    }


    /// <summary> Reset the step to its initial state. </summary>
    public void Reset() {
        // Empty implementation
    }
}
