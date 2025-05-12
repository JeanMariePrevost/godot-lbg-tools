using System;
using System.Threading.Tasks;

namespace LBG.Handustry.Sequencing.Steps;

/// <summary>
/// Represents a single step in a sequence of operations.
/// </summary>
public interface IStep {
    /// <summary>
    /// Executes this step.
    /// </summary>
    /// <param name="sequenceControlCallback">Callback for sequence control. Most steps can ignore this.</param>
    Task Execute(Action<SequenceCommand> sequenceControlCallback);

    /// <summary> Reset the step to its initial state. </summary>
    /// <remarks> E.g. it is has to be repeated _and_ has internal state. </remarks>
    void Reset();
}
