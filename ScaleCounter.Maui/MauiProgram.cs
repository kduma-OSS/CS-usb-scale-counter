using Microsoft.Extensions.Logging;
using ScaleCounter.Core;
using ScaleCounter.Maui.Services;
using ScaleCounter.Maui.ViewModels;
using ScaleCounter.Maui.Views;

namespace ScaleCounter.Maui;

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
			});

		// Domain services.
		builder.Services.AddSingleton<IPresetStore, JsonPresetStore>();
		builder.Services.AddSingleton<ItemCounter>();
#if ANDROID
		builder.Services.AddSingleton<IScale, ScaleCounter.Maui.AndroidScale>();
		builder.Services.AddSingleton<ISoundPlayer, ScaleCounter.Maui.AndroidSoundPlayer>();
#endif

		// Shell, pages and view-models.
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<PresetsViewModel>();
		builder.Services.AddTransient<PresetsPage>();
		builder.Services.AddTransient<CalibrationViewModel>();
		builder.Services.AddTransient<CalibrationPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
