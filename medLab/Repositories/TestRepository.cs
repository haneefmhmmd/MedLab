using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using medLab.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace medLab.Repositories
{

    public class TestRepository : ITestRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly AmazonDynamoDBClient _dynamoDbClient;

        public TestRepository(IDynamoDBContext context)
        {
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

        public async Task<LabTests> GetTestsByLabIdAsync(string labId)
        {
            return await _context.LoadAsync<LabTests>(labId);
        }

        public async Task<LabTest> GetTestByIdAsync(string labId, string testId)
        {
            var labTests = await GetTestsByLabIdAsync(labId);
            return labTests?.Tests?.Find(t => t.Id == testId);
        }

        public async Task SaveLabTestsAsync(LabTests labTests)
        {
            await _context.SaveAsync(labTests);
        }


        public async Task AddTestAsync(string labId, LabTest test)
        {
            var labTests = await GetTestsByLabIdAsync(labId) ?? new LabTests { LabId = labId };
            labTests.Tests.Add(test);
            await _context.SaveAsync(labTests);
        }

        public async Task UpdateTestAsync(string labId, string testId, LabTest test)
        {
            var labTests = await GetTestsByLabIdAsync(labId);
            var existingTest = labTests?.Tests?.Find(t => t.Id == testId);
            if (existingTest != null)
            {
                existingTest.Name = test.Name;
                existingTest.Unit = test.Unit;
                existingTest.ReferenceValue = test.ReferenceValue;
                await _context.SaveAsync(labTests);
            }
        }

        public async Task DeleteTestAsync(string labId, string testId)
        {
            var labTests = await GetTestsByLabIdAsync(labId);
            var testToRemove = labTests?.Tests?.Find(t => t.Id == testId);
            if (testToRemove != null)
            {
                labTests.Tests.Remove(testToRemove);
                await _context.SaveAsync(labTests);
            }
        }

        public async Task PatchTestAsync(string labId, string testId, LabTest test)
        {
            var labTests = await GetTestsByLabIdAsync(labId);
            var existingTest = labTests?.Tests?.Find(t => t.Id == testId);
            if (existingTest != null)
            {
                if (!string.IsNullOrEmpty(test.Name)) existingTest.Name = test.Name;
                if (!string.IsNullOrEmpty(test.Unit)) existingTest.Unit = test.Unit;
                if (!string.IsNullOrEmpty(test.ReferenceValue)) existingTest.ReferenceValue = test.ReferenceValue;
                await _context.SaveAsync(labTests);
            }
        }
    }
}
