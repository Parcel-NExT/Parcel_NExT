using System.Diagnostics;

namespace Parcel.CoreEngine.Helpers
{
    public sealed class Measure : IDisposable
    {
        private readonly Stopwatch Timer;
        private readonly Action<TimeSpan>? MeasureFinishCallback;
        public Measure(Action<TimeSpan>? callback)
        {
            Timer = new Stopwatch();
            Timer.Start();
            MeasureFinishCallback = callback;
        }
        public void Dispose()
        {
            Timer.Stop();
            MeasureFinishCallback?.Invoke(Timer.Elapsed);
        }
    }
}
