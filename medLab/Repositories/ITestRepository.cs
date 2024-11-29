using medLab.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace medLab.Repositories
{
    public interface ITestRepository
    {
        Task<LabTests> GetTestsByLabIdAsync(string labId);
        Task<LabTest> GetTestByIdAsync(string labId, string testId);
        Task SaveLabTestsAsync(LabTests labTests);

        Task AddTestAsync(string labId, LabTest test);
        Task UpdateTestAsync(string labId, string testId, LabTest test);
        Task DeleteTestAsync(string labId, string testId);
        Task PatchTestAsync(string labId, string testId, LabTest test);
    }

}
