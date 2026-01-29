using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using CommunityToolkit.Maui;
using assignment_2425.Database;

namespace assignment_2425
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBarcodeReader() // uses the barcode reader
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddTransient<BasketPage>();
            builder.Services.AddTransient<CheckoutPage>();
            builder.Services.AddTransient<OrderPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
