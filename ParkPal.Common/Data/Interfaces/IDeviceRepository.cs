namespace ParkPal.Common.Data.Interfaces;

public interface IDeviceRepository
{
    Task UpsertDeviceAsync(string appUserId, string deviceToken);
}