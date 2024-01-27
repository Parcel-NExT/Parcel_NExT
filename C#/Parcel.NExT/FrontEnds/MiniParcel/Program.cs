using Parcel.CoreEngine.MiniParcel;
using Parcel.CoreEngine.Service.CoreExtensions;

namespace MiniParcel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFile = args.First();
            var document = MiniParcelService.Parse(File.ReadAllText(inputFile));
            document.Execute();
        }
    }
}
