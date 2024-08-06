using Parcel.Types;

namespace Parcel.Neo.Helpers
{
    public class MediaHelper
    {
        public static System.Windows.Media.Color ConvertColor(Color parcelColor)
        {
            return System.Windows.Media.Color.FromArgb(parcelColor.Alpha, parcelColor.Red, parcelColor.Green, parcelColor.Blue);
        }
    }
}
