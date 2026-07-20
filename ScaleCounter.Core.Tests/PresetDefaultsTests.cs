using System.Text.Json;
using Xunit;

namespace ScaleCounter.Core.Tests;

public class PresetDefaultsTests
{
    [Fact]
    public void Json_LoadsEmbeddedResource()
    {
        var json = PresetDefaults.Json;
        Assert.False(string.IsNullOrWhiteSpace(json));
        Assert.Contains("Empty CD Sleeves", json);
    }

    [Fact]
    public void Json_ParsesToValidPresets()
    {
        var presets = JsonSerializer.Deserialize<List<WeighedItemPreset>>(PresetDefaults.Json);

        Assert.NotNull(presets);
        Assert.Equal(2, presets!.Count);
        Assert.All(presets, p =>
        {
            Assert.False(string.IsNullOrEmpty(p.Id));
            Assert.False(string.IsNullOrWhiteSpace(p.Name));
            Assert.True(p.PerItemWeightGrams > 0);
            Assert.NotEmpty(p.Samples);
        });
        Assert.Contains(presets!, p => p.Name == "Empty CD Sleeves");
        Assert.Contains(presets!, p => p.Name == "Sleeved CD's");
    }

    [Fact]
    public void Defaults_FitRoughlyMatchesStoredPerItemWeight()
    {
        var presets = JsonSerializer.Deserialize<List<WeighedItemPreset>>(PresetDefaults.Json)!;
        foreach (var preset in presets)
        {
            var fit = Calibration.Fit(preset.Samples);
            Assert.True(fit.IsValid);
            // The stored per-item weight should be consistent with the samples.
            Assert.Equal(preset.PerItemWeightGrams, fit.PerItemWeightGrams, 2);
        }
    }
}
