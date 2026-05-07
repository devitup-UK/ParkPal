namespace ParkPal.Common.Helpers;

public static class DataHelper
{
    public static string GenerateShareCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // No 0, O, 1, or I
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}