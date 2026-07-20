using ScaleCounter.Maui.Views;

namespace ScaleCounter.Maui;

public partial class AppShell : Shell
{
	public AppShell(MainPage mainPage)
	{
		InitializeComponent();

		// The main dashboard is the single root of the shell; the settings page is
		// pushed onto the navigation stack on demand (see MainViewModel.GoToSettings).
		Items.Add(new ShellContent { Content = mainPage, Route = "MainPage" });
	}
}
