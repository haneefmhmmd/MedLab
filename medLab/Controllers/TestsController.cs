using medLab.Models;
using medLab.Models.DTOs;
using medLab.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using System;

namespace medLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ILabRepository _labRepository;

        private readonly ITestRepository _testRepository;
        private readonly IMapper _mapper;  // Inject AutoMapper

        public TestsController(ILabRepository labRepository, ITestRepository testRepository, IMapper mapper)
        {
            _labRepository = labRepository;
            _testRepository = testRepository;
            _mapper = mapper;  // Initialize AutoMapper
        }

        [HttpGet("{labId}")]
        public async Task<IActionResult> GetTestsByLabId(string labId)
        {
            var lab = await _labRepository.GetByIdAsync(labId);

            if (lab == null)
            {
                return NotFound($"Lab with ID {labId} not found.");
            }

            // Fetch the LabTests object (which will contain the tests)
            var labTests = await _testRepository.GetTestsByLabIdAsync(labId);

            // If no LabTests object exists for the lab, create one with an empty list and save it to DynamoDB
            if (labTests == null)
            {
                var newLabTests = new LabTests
                {
                    LabId = labId,
                    Tests = new List<LabTest>() // Empty list of tests
                };

                // Save the new LabTests object to DynamoDB
                await _testRepository.SaveLabTestsAsync(newLabTests);

                var emptyLabTestsDTO = new LabTestsDTO
                {
                    LabId = labId,
                    Tests = new List<LabTestDTO>() // Empty list of tests
                };

                return Ok(emptyLabTestsDTO);
            }

            // Ensure there's only one LabTests object for the lab (no need to check if the list is empty)
            var labTestsDTO = _mapper.Map<LabTestsDTO>(labTests);

            return Ok(labTestsDTO);
        }


        [HttpGet("{labId}/{testId}")]
        public async Task<IActionResult> GetTestById(string labId, string testId)
        {
            var test = await _testRepository.GetTestByIdAsync(labId, testId);
            if (test == null) return NotFound("Test not found.");

            // Map LabTest to LabTestDTO using AutoMapper
            var testDTO = _mapper.Map<LabTestDTO>(test);

            return Ok(testDTO);
        }

        [HttpPost("{labId}")]
        public async Task<IActionResult> CreateTest(string labId, [FromBody] LabTestDTO testDTO)
        {
            // Check if the test name is empty
            if (string.IsNullOrWhiteSpace(testDTO.Name))
            {
                return BadRequest("Test name cannot be empty.");
            }

            // Map LabTestDTO to LabTest using AutoMapper
            var test = _mapper.Map<LabTest>(testDTO);
            test.Id = Guid.NewGuid().ToString();  // Generate new ID for the test

            await _testRepository.AddTestAsync(labId, test);
            return CreatedAtAction(nameof(GetTestById), new { labId, testId = test.Id }, testDTO);
        }

        [HttpPut("{labId}/{testId}")]
        public async Task<IActionResult> UpdateTest(string labId, string testId, [FromBody] LabTestDTO testDTO)
        {
            // Check if the test name is empty
            if (string.IsNullOrWhiteSpace(testDTO.Name))
            {
                return BadRequest("Test name cannot be empty.");
            }

            // Check if the test exists before updating
            var existingTest = await _testRepository.GetTestByIdAsync(labId, testId);
            if (existingTest == null)
            {
                return NotFound("Test not found.");
            }

            // Map LabTestDTO to LabTest using AutoMapper
            var test = _mapper.Map<LabTest>(testDTO);
            test.Id = testId;  // Ensure the test ID matches the one in the URL

            // Update the test in the repository
            await _testRepository.UpdateTestAsync(labId, testId, test);

            return Ok(true);  // Return true for successful update
        }

        [HttpDelete("{labId}/{testId}")]
        public async Task<IActionResult> DeleteTest(string labId, string testId)
        {
            var test = await _testRepository.GetTestByIdAsync(labId, testId);
            if (test == null)
            {
                return NotFound("Test not found.");
            }

            // Delete the test from the repository
            await _testRepository.DeleteTestAsync(labId, testId);

            return Ok(true);  // Return true for successful delete
        }

        [HttpPatch("{labId}/{testId}")]
        public async Task<IActionResult> PatchTest(string labId, string testId, [FromBody] LabTestDTO testDTO)
        {
            // Check if the test name is empty
            if (string.IsNullOrWhiteSpace(testDTO.Name))
            {
                return BadRequest("Test name cannot be empty.");
            }

            // Map LabTestDTO to LabTest using AutoMapper
            var test = _mapper.Map<LabTest>(testDTO);
            test.Id = testId;

            await _testRepository.PatchTestAsync(labId, testId, test);
            return Ok(true);  // Return true for successful patch
        }
    }
}
