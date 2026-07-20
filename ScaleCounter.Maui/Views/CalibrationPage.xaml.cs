using ScaleCounter.Core;
using ScaleCounter.Maui.ViewModels;

namespace ScaleCounter.Maui.Views;

public partial class CalibrationPage : ContentPage
{
	private readonly CalibrationViewModel _viewModel;

	public CalibrationPage(CalibrationViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.Finished += async (_, _) => await Navigation.PopAsync();
	}

	/// <summary>Loads the preset to edit/calibrate. Call before pushing the page.</summary>
	public void Load(WeighedItemPreset preset) => _viewModel.Load(preset);
}
