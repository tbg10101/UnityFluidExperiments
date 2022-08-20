using System.Diagnostics;

public static class StopwatchUtil {
    public static double ElapsedMillisecondsWithFraction(this Stopwatch stopwatch) {
        return stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000.0);
    }
}
