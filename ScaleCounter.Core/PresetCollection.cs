namespace ScaleCounter.Core;

/// <summary>Helpers for combining preset collections (used by export/import).</summary>
public static class PresetCollection
{
    /// <summary>
    /// Merges <paramref name="imported"/> presets into <paramref name="target"/> by Id:
    /// a preset whose Id already exists replaces it, otherwise it is appended. Imported
    /// presets without an Id are given a fresh one. This makes import idempotent for
    /// re-imported backups while still accepting presets shared from another device.
    /// Returns the number of presets merged.
    /// </summary>
    public static int Merge(List<WeighedItemPreset> target, IEnumerable<WeighedItemPreset>? imported)
    {
        if (target == null || imported == null)
            return 0;

        int count = 0;
        foreach (var preset in imported)
        {
            if (preset == null)
                continue;

            if (string.IsNullOrEmpty(preset.Id))
                preset.Id = Guid.NewGuid().ToString("N");

            int index = target.FindIndex(p => p.Id == preset.Id);
            if (index >= 0)
                target[index] = preset;
            else
                target.Add(preset);

            count++;
        }

        return count;
    }
}
