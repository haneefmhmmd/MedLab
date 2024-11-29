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
        private readonly ITestRepository _testRepository;
        private readonly IMapper _mapper;  // Inject AutoMapper

        public TestsController(ITestRepository testRepository, IMapper mapper)
        {
            _testRepository = testRepository;
            _mapper = mapper;  // Initialize AutoMapper
        }

        [HttpGet("{labId}")]
        public async Task<IActionResult> GetTestsByLabId(string labId)
        {
            var labTests = await _testRepository.GetTestsByLabIdAsync(labId);
            if (labTests == null) return NotFound("Lab tests not found.");

            // Map LabTests to LabTestsDTO using AutoMapper
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
            // Map LabTestDTO to LabTest using AutoMapper
            var test = _mapper.Map<LabTest>(testDTO);
            test.Id = Guid.NewGuid().ToString();  // Generate new ID for the test

            await _testRepository.AddTestAsync(labId, test);
            return CreatedAtAction(nameof(GetTestById), new { labId, testId = test.Id }, testDTO);
        }

        [HttpPut("{labId}/{testId}")]
        public async Task<IActionResult> UpdateTest(string labId, string testId, [FromBody] LabTestDTO testDTO)
        {
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

            return NoContent();
        }


        [HttpDelete("{labId}/{testId}")]
        public async Task<IActionResult> DeleteTest(string labId, string testId)
        {
            await _testRepository.DeleteTestAsync(labId, testId);
            return NoContent();
        }

        [HttpPatch("{labId}/{testId}")]
        public async Task<IActionResult> PatchTest(string labId, string testId, [FromBody] LabTestDTO testDTO)
        {
            // Map LabTestDTO to LabTest using AutoMapper
            var test = _mapper.Map<LabTest>(testDTO);
            test.Id = testId;

            await _testRepository.PatchTestAsync(labId, testId, test);
            return NoContent();
        }
    }
}
