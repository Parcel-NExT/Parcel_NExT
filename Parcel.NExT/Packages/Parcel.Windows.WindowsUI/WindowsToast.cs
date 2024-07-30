using Microsoft.Toolkit.Uwp.Notifications;

namespace Parcel.Windows.WindowsUI
{
    public static class WindowsToast
    {
        public static void SendToastMessage(string message)
        {
            new ToastContentBuilder()
                .AddText(message)
                .Show();
        }
    }
}
