using medLab.Models;
using Amazon.DynamoDBv2.DataModel;

namespace medLab.Repositories
{
    public interface ILabRepository
    {
        Task<Labs> GetByIdAsync(string labId);
        Task<List<Labs>> GetAllAsync();
        Task<Labs> AddAsync(Labs lab);
        Task<Labs> UpdateAsync(Labs lab);
        Task<bool> DeleteAsync(string labId);
        Task CreateAndSaveLabs();
    }
}
