using UnitsNet;

namespace ScaleCounter.Core;

/// <summary>
/// A single parsed HID input report from the scale: its status and measured weight.
/// Reproduces, byte-for-byte, the parsing done by the desktop ScaleLib.Scale.OnHidRead,
/// but as a pure, side-effect-free value so it can be unit tested on any platform.
///
/// Report byte layout (little-endian 16-bit weight):
///   [0] = report id (3 = data report; anything else is ignored)
///   [1] = status byte
///   [2] = unit code (2 = grams, 11 = ounces)
///   [3] = signed scaling exponent (sbyte)
///   [4] = weight LSB
///   [5] = weight MSB
/// </summary>
public readonly record struct ScaleReport(StatusEnum Status, Mass Weight)
{
    /// <summary>HID report id for a data (weight) report.</summary>
    public const byte DataReportId = 3;

    /// <summary>Unit code for grams in byte 2.</summary>
    public const byte UnitGrams = 2;

    /// <summary>Unit code for ounces in byte 2.</summary>
    public const byte UnitOunces = 11;

    /// <summary>
    /// Parses a raw HID input report. Returns <c>false</c> (and a default report)
    /// for buffers shorter than 6 bytes or that are not data reports (byte 0 != 3).
    /// </summary>
    public static bool TryParse(byte[] buffer, out ScaleReport report)
    {
        report = default;

        if (buffer == null || buffer.Length < 6 || buffer[0] != DataReportId)
            return false;

        var status = MapStatus(buffer[1]);

        int exponent = unchecked((sbyte)buffer[3]);          // signed scaling exponent
        double baseValue = buffer[4] + buffer[5] * 256;      // little-endian 16-bit raw weight
        double value = baseValue * Math.Pow(10, exponent);

        Mass weight = buffer[2] == UnitOunces
            ? Mass.FromOunces(value)
            : Mass.FromGrams(value);                          // grams for code 2 and as the default

        report = new ScaleReport(status, weight);
        return true;
    }

    /// <summary>Maps the raw status byte to <see cref="StatusEnum"/>.</summary>
    public static StatusEnum MapStatus(byte statusByte) => statusByte switch
    {
        1 => StatusEnum.Fault,
        2 => StatusEnum.Zero,
        3 => StatusEnum.InMotion,
        4 => StatusEnum.Stable,
        5 => StatusEnum.UnderZero,
        6 => StatusEnum.OverWeight,
        7 => StatusEnum.NeedCalibration,
        8 => StatusEnum.NeedZeroing,
        _ => StatusEnum.Unknown
    };
}
