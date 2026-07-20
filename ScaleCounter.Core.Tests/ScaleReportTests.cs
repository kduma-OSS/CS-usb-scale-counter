using UnitsNet;
using Xunit;

namespace ScaleCounter.Core.Tests;

public class ScaleReportTests
{
    [Fact]
    public void TryParse_RejectsNonDataReportId()
    {
        var buffer = new byte[] { 0, 4, 2, 0, 0x44, 0x02 };
        Assert.False(ScaleReport.TryParse(buffer, out _));
    }

    [Fact]
    public void TryParse_RejectsTooShortBuffer()
    {
        var buffer = new byte[] { 3, 4, 2, 0, 0x44 };
        Assert.False(ScaleReport.TryParse(buffer, out _));
    }

    [Fact]
    public void TryParse_ParsesGrams()
    {
        // id 3, status Stable(4), unit grams(2), exp 0, raw = 0x44 + 0x02*256 = 580
        var buffer = new byte[] { 3, 4, 2, 0, 0x44, 0x02 };

        Assert.True(ScaleReport.TryParse(buffer, out var r));
        Assert.Equal(StatusEnum.Stable, r.Status);
        Assert.Equal(580d, r.Weight.Grams, 3);
    }

    [Fact]
    public void TryParse_ParsesOunces()
    {
        // unit ounces(11), raw 580
        var buffer = new byte[] { 3, 4, 11, 0, 0x44, 0x02 };

        Assert.True(ScaleReport.TryParse(buffer, out var r));
        Assert.Equal(580d, r.Weight.Ounces, 3);
    }

    [Fact]
    public void TryParse_AppliesNegativeExponent()
    {
        // exp = 0xFF = -1 (sbyte); raw = 0xA8 + 0x16*256 = 168 + 5632 = 5800; 5800 * 10^-1 = 580
        var buffer = new byte[] { 3, 4, 2, 0xFF, 0xA8, 0x16 };

        Assert.True(ScaleReport.TryParse(buffer, out var r));
        Assert.Equal(580d, r.Weight.Grams, 3);
    }

    [Fact]
    public void TryParse_AssemblesLittleEndian16Bit()
    {
        var buffer = new byte[] { 3, 4, 2, 0, 0xFF, 0xFF }; // raw 65535

        Assert.True(ScaleReport.TryParse(buffer, out var r));
        Assert.Equal(65535d, r.Weight.Grams, 3);
    }

    [Theory]
    [InlineData(1, StatusEnum.Fault)]
    [InlineData(2, StatusEnum.Zero)]
    [InlineData(3, StatusEnum.InMotion)]
    [InlineData(4, StatusEnum.Stable)]
    [InlineData(5, StatusEnum.UnderZero)]
    [InlineData(6, StatusEnum.OverWeight)]
    [InlineData(7, StatusEnum.NeedCalibration)]
    [InlineData(8, StatusEnum.NeedZeroing)]
    [InlineData(0, StatusEnum.Unknown)]
    [InlineData(9, StatusEnum.Unknown)]
    [InlineData(255, StatusEnum.Unknown)]
    public void MapStatus_MapsAllBytes(byte input, StatusEnum expected)
    {
        Assert.Equal(expected, ScaleReport.MapStatus(input));
    }
}
