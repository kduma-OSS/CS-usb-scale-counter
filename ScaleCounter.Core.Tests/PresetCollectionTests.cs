using Xunit;

namespace ScaleCounter.Core.Tests;

public class PresetCollectionTests
{
    [Fact]
    public void Merge_AddsNewPresets()
    {
        var target = new List<WeighedItemPreset> { new() { Id = "a", Name = "A" } };
        var imported = new List<WeighedItemPreset> { new() { Id = "b", Name = "B" } };

        var count = PresetCollection.Merge(target, imported);

        Assert.Equal(1, count);
        Assert.Equal(2, target.Count);
        Assert.Contains(target, p => p.Id == "b" && p.Name == "B");
    }

    [Fact]
    public void Merge_ReplacesExistingById()
    {
        var target = new List<WeighedItemPreset> { new() { Id = "a", Name = "Old" } };
        var imported = new List<WeighedItemPreset> { new() { Id = "a", Name = "New" } };

        PresetCollection.Merge(target, imported);

        Assert.Single(target);
        Assert.Equal("New", target[0].Name);
    }

    [Fact]
    public void Merge_AssignsIdWhenMissing()
    {
        var target = new List<WeighedItemPreset>();
        var imported = new List<WeighedItemPreset> { new() { Id = "", Name = "NoId" } };

        PresetCollection.Merge(target, imported);

        Assert.Single(target);
        Assert.False(string.IsNullOrEmpty(target[0].Id));
    }

    [Fact]
    public void Merge_ReimportedBackupDoesNotDuplicate()
    {
        var target = new List<WeighedItemPreset>
        {
            new() { Id = "a", Name = "A" },
            new() { Id = "b", Name = "B" }
        };
        // Re-importing the same two presets keeps the count at two.
        var imported = new List<WeighedItemPreset>
        {
            new() { Id = "a", Name = "A" },
            new() { Id = "b", Name = "B" }
        };

        PresetCollection.Merge(target, imported);

        Assert.Equal(2, target.Count);
    }

    [Fact]
    public void Merge_NullImported_ReturnsZero()
    {
        var target = new List<WeighedItemPreset>();
        Assert.Equal(0, PresetCollection.Merge(target, null));
    }
}
