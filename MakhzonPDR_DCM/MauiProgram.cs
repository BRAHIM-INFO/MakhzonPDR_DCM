using Microsoft.Extensions.Logging;
using OneSignalSDK.DotNet;

namespace MakhzonPDR_DCM
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            OneSignal.Initialize("c8a48984-39ff-4048-b012-275acf55a313");
            // طلب إذن الإشعارات في الإصدار الجديد
            OneSignal.Notifications.RequestPermissionAsync(true);
             

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
