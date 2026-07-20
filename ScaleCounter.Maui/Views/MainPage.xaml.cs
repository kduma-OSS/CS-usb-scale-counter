using ScaleCounter.Maui.ViewModels;

namespace ScaleCounter.Maui.Views;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Pick up any change to the active preset made on the presets page.
		_viewModel.ReloadPreset();
	}
}
