using medLab.Models;
using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace medLab.Repositories
{
    public interface ILabRepository
    {
        // Labs CRUD Operations
        Task<Labs> GetByIdAsync(string labId);
        Task<List<Labs>> GetAllAsync();
        Task<Labs> AddAsync(Labs lab);
        Task<Labs> UpdateAsync(Labs lab);
        Task<bool> DeleteAsync(string labId);
        Task<Labs> GetByEmailAsync(string email);

        // Reports CRUD Operations
        Task<Report?> GetReportAsync(string labId, string reportId);
        Task AddReportAsync(string labId, Report report);
        Task UpdateReportAsync(string labId, Report updatedReport);
        Task DeleteReportAsync(string labId, string reportId);

        // Utility method to create and save a sample lab with reports
        Task CreateAndSaveLabs();
    }
}
