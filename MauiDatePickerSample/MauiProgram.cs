using Microsoft.Extensions.Logging;

namespace MauiDatePickerSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                            .ConfigureFonts(fonts =>
                            {
                                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                                fonts.AddFont("IRANSansX-Bold.ttf", "IRANSansX");
                                fonts.AddFont("IRANSansX-Regular.ttf", "IRANSans");
                                fonts.AddFont("Yekan.ttf", "Yekan");
                            });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
