using Xunit;
using LBG.LBGTools.Sequencing;

namespace LBG.LBGTools.Tests {
    public class SequenceTests {
        [Fact]
        public async Task Test_DoAfterSeconds() {
            bool actionExecuted = false;
            Sequence.DoAfterSeconds(1.0f, () => actionExecuted = true);
            await Task.Delay(1100); // Wait for the action to execute
            Assert.True(actionExecuted, "Action should be executed after 1 second.");
        }

        [Fact]
        public void Test_DoAfterFrames() {
            bool actionExecuted = false;
            Sequence.DoAfterFrames(1, () => actionExecuted = true);
            // Simulate frame delay
            Task.Delay(16).Wait(); // Assuming ~60 FPS, wait for ~1 frame
            Assert.True(actionExecuted, "Action should be executed after 1 frame.");
        }

        [Fact]
        public void Test_SequenceExecution() {
            var sequence = new Sequence();
            int counter = 0;

            sequence
                .Do(() => counter++)
                .WaitSeconds(0.1f)
                .Do(() => counter++);

            sequence.Start();
            Task.Delay(200).Wait(); // Wait for the sequence to complete

            Assert.Equal(2, counter);
        }

        [Fact]
        public void Test_RepeatSequence() {
            var sequence = new Sequence();
            int counter = 0;

            sequence
                .Do(() => counter++)
                .RepeatSequence(2);

            sequence.Start();
            Task.Delay(100).Wait(); // Wait for the sequence to complete

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
            Task.Delay(100).Wait(); // Wait for the sequence to complete

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
}
