using System.Reflection;

namespace ScaleCounter.Core;

/// <summary>
/// Access to the bundled default presets. The presets are stored as an embedded JSON
/// document (the same ".uscpreset" format) so they can be updated without touching code,
/// and each app parses them with its own JSON serializer.
/// </summary>
public static class PresetDefaults
{
    /// <summary>The bundled default presets as JSON (a list of <see cref="WeighedItemPreset"/>).</summary>
    public static string Json
    {
        get
        {
            var assembly = typeof(PresetDefaults).GetTypeInfo().Assembly;
            var name = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("DefaultPresets.json", StringComparison.Ordinal));

            using var stream = assembly.GetManifestResourceStream(name)!;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
