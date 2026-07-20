using Microsoft.Extensions.DependencyInjection;

namespace ScaleCounter.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var services = IPlatformApplication.Current!.Services;
		return new Window(services.GetRequiredService<AppShell>());
	}
}
