using ParkPal.Common.API.Models.Dtos;

namespace ParkPal.Common.Data.Interfaces;

public interface ICrowdSourceRepository
{
    Task SubmitAttractionStateAsync(string userId, AttractionSubmissionDto submission);
}