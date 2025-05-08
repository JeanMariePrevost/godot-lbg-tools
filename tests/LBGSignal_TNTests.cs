using LBG.LBGTools.Signal;
using Xunit;

namespace LBG.LBGTools.Signal.Tests;
public class LBGSignal_TNTests {
    [Fact]
    public void LBGSignal_T2_Everything_ShouldWork() {
        // Arrange
        var signal = new LBGSignal<int, string>();

        var list = new List<string>();

        void normal(int i, string s) => list.Add($"N:{i}:{s}");
        void once(int i, string s) => list.Add($"O:{i}:{s}");
        void limited(int i, string s) => list.Add($"L:{i}:{s}");
        void toRemove(int i, string s) => list.Add("FAIL");

        signal.Add(normal).WithPriority(1);
        signal.Add(once).Once();
        signal.Add(limited).CallLimit(2);
        signal.Add(toRemove);
        signal.Remove(toRemove);

        // Act
        signal.Emit(1, "a");
        signal.Emit(2, "b");
        signal.Clear();
        signal.Emit(3, "c");

        // Assert
        var expected = new List<string>
        { "N:1:a", "O:1:a", "L:1:a", "N:2:b", "L:2:b" };

        Assert.Equal(expected, list);
    }

}
