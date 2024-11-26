using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using medLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace medLab.Repositories
{
    public class LabRepository : ILabRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly ILogger<LabRepository> _logger;

        public LabRepository(ILogger<LabRepository> logger)
        {
            _logger = logger;

            var configuration = LoadConfiguration();
            var awsAccessKey = configuration["AWSCredentials:AccessKeyID"];
            var awsSecretKey = configuration["AWSCredentials:SecretAccessKey"];

            var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            _dynamoDbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);

            _context = new DynamoDBContext(_dynamoDbClient);
        }

        private IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }

        // Helper Method to Map Reports
        private Report MapReports(Dictionary<string, AttributeValue> reportsItem)
        {
            var reports = new Report
            {
                Age = int.Parse(reportsItem["age"].N),
                DateOfTest = reportsItem["dateOfTest"].S,
                Gender = reportsItem["gender"].S,
                PatientName = reportsItem["patientName"].S,
                Tests = reportsItem.ContainsKey("tests")
                        ? reportsItem["tests"].L.Select(t => new Test
                        {
                            TestName = t.M["testName"].S,  // Map to TestName property
                            TestValue = t.M["testValue"].S // Map to TestValue property
                        }).ToList()
                        : new List<Test>() // If no 'tests' key exists, return an empty list
            };
            return reports;
        }

        public async Task<Labs> GetByIdAsync(string labId)
        {
            try
            {
                return await _context.LoadAsync<Labs>(labId);
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception($"Failed to retrieve lab with ID: {labId}", ex);
            }
        }

        public async Task<List<Labs>> GetAllAsync()
        {
            var search = _context.ScanAsync<Labs>(new List<ScanCondition>());
            var labs = new List<Labs>();

            do
            {
                var page = await search.GetNextSetAsync();
                labs.AddRange(page);
            } while (!search.IsDone);

            return labs;
        }

        public async Task<Labs> AddAsync(Labs lab)
        {
            try
            {
                await _context.SaveAsync(lab);
                return lab;
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception("Failed to add lab", ex);
            }
        }

        public async Task<Labs> UpdateAsync(Labs lab)
        {
            try
            {
                await _context.SaveAsync(lab);
                return lab;
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception("Failed to update lab", ex);
            }
        }

        public async Task<bool> DeleteAsync(string labId)
        {
            try
            {
                var lab = await GetByIdAsync(labId);
                if (lab == null)
                {
                    return false;
                }
                await _context.DeleteAsync(lab);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here
                throw new Exception($"Failed to delete lab with ID: {labId}", ex);
            }
        }


        public async Task<Labs> GetByEmailAsync(string email)
        {
            var search = _context.ScanAsync<Labs>(new List<ScanCondition>
            {
                new ScanCondition(nameof(Labs.LabEmail), Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, email)
            });

            var result = await search.GetRemainingAsync();
            return result.FirstOrDefault(); // Return the first matching record or null
        }


        public async Task<Report?> GetReportAsync(string labId, string reportId)
        {
            var lab = await GetByIdAsync(labId);
            return lab?.Reports.FirstOrDefault(r => r.ReportId == reportId);
        }

        public async Task AddReportAsync(string labId, Report report)
        {
            var lab = await GetByIdAsync(labId);
            if (lab != null)
            {
                report.ReportId = Guid.NewGuid().ToString(); // Ensure a unique ID for the new report
                lab.Reports.Add(report);
                await UpdateAsync(lab);
            }
        }

        public async Task UpdateReportAsync(string labId, Report updatedReport)
        {
            var lab = await GetByIdAsync(labId);
            if (lab != null)
            {
                var index = lab.Reports.FindIndex(r => r.ReportId == updatedReport.ReportId);
                if (index != -1)
                {
                    lab.Reports[index] = updatedReport;
                    await UpdateAsync(lab);
                }
            }
        }

        public async Task DeleteReportAsync(string labId, string reportId)
        {
            var lab = await GetByIdAsync(labId);
            if (lab != null)
            {
                lab.Reports.RemoveAll(r => r.ReportId == reportId);
                await UpdateAsync(lab);
            }
        }

        public async Task CreateAndSaveLabs()
        {
            var lab = new Labs
            {
                LabId = Guid.NewGuid().ToString(), // Generate unique LabId
                LabEmail = "",
                PasswordHash = "",
                LabName = "Central Lab",
                LabAddress = "123 Main St, Cityville",
                Reports = new List<Report>
        {
            new Report
            {
                ReportId = Guid.NewGuid().ToString(), // Generate unique ReportId
                Age = 45,
                DateOfTest = DateTime.Now.ToString("yyyy-MM-dd"),
                Gender = "Female",
                PatientName = "Jane Doe",
                Tests = new List<Test>
                {
                    new Test { TestName = "Blood Test", TestValue = "Normal" },
                    new Test { TestName = "X-Ray", TestValue = "Clear" }
                }
            },
            new Report
            {
                ReportId = Guid.NewGuid().ToString(), // Another unique ReportId
                Age = 50,
                DateOfTest = DateTime.Now.ToString("yyyy-MM-dd"),
                Gender = "Male",
                PatientName = "John Doe",
                Tests = new List<Test>
                {
                    new Test { TestName = "MRI", TestValue = "Normal" }
                }
            }
        }
            };

            try
            {
                var addedLab = await AddAsync(lab);
                if (addedLab != null)
                {
                    _logger.LogInformation($"Successfully added lab: {addedLab.LabName}");
                }
                else
                {
                    _logger.LogError($"Failed to add lab: {lab.LabName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while adding lab {lab.LabName}: {ex.Message}");
            }
        }




    }
}
