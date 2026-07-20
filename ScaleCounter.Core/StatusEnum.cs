namespace ScaleCounter.Core;

/// <summary>
/// Scale status as reported in byte 1 of the HID data report.
/// Copied verbatim (order preserved) from the desktop ScaleLib.StatusEnum.
/// </summary>
public enum StatusEnum
{
    Unknown,
    Fault,
    Zero,
    InMotion,
    Stable,
    UnderZero,
    OverWeight,
    NeedCalibration,
    NeedZeroing
}
