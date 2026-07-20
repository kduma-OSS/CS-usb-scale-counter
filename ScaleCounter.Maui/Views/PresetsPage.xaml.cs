using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using ScaleCounter.Core;
using ScaleCounter.Maui.ViewModels;

namespace ScaleCounter.Maui.Views;

public partial class PresetsPage : ContentPage
{
	private readonly PresetsViewModel _viewModel;

	public PresetsPage(PresetsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.EditRequested += OnEditRequested;
		_viewModel.ShareRequested += OnShareRequested;
	}

	private static string SafeFileName(string name)
	{
		foreach (var c in Path.GetInvalidFileNameChars())
			name = name.Replace(c, '_');
		return string.IsNullOrWhiteSpace(name) ? "preset" : name;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.Refresh();
	}

	private async void OnEditRequested(object? sender, WeighedItemPreset preset)
	{
		var page = IPlatformApplication.Current!.Services.GetRequiredService<CalibrationPage>();
		page.Load(preset);
		await Navigation.PushAsync(page);
	}

	private async void OnShareRequested(object? sender, WeighedItemPreset preset)
	{
		try
		{
			var path = Path.Combine(FileSystem.CacheDirectory, SafeFileName(preset.Name) + PresetFile.Extension);
			File.WriteAllText(path, _viewModel.ExportJson(new[] { preset }));

			await Share.Default.RequestAsync(new ShareFileRequest
			{
				Title = "Share preset",
				File = new ShareFile(path)
			});
		}
		catch (Exception ex)
		{
			await DisplayAlert("Export failed", ex.Message, "OK");
		}
	}

	private async void OnLoadDefaultsClicked(object? sender, EventArgs e)
	{
		bool ok = await DisplayAlert("Load default presets",
			"Add the built-in default presets? Existing presets with the same id are updated.",
			"Load", "Cancel");
		if (!ok)
			return;

		int count = _viewModel.LoadDefaults();
		await DisplayAlert("Defaults", $"Loaded {count} preset(s).", "OK");
	}

	private async void OnImportClicked(object? sender, EventArgs e)
	{
		try
		{
			var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Import preset(s)" });
			if (result == null)
				return;

			using var stream = await result.OpenReadAsync();
			using var reader = new StreamReader(stream);
			var json = await reader.ReadToEndAsync();

			int count = _viewModel.ImportJson(json);
			await DisplayAlert("Import", count > 0 ? $"Imported {count} preset(s)." : "No presets found in the file.", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Import failed", ex.Message, "OK");
		}
	}
}
