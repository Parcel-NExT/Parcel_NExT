using System.Diagnostics;

namespace Parcel.CoreEngine.Helpers
{
    public class Measure : IDisposable
    {
        private readonly Stopwatch Timer;
        public Measure()
        {
            Timer = new Stopwatch();
            Timer.Start();
        }
        public void Dispose()
        {
            Timer.Stop();
            TimeSpan timeTaken = Timer.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
        }
    }
}
