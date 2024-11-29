using AutoMapper;
using medLab.Models;
using medLab.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace medLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LabsController : ControllerBase
    {
        private readonly ILabRepository _repository;
        private readonly IMapper _mapper;

        public LabsController(ILabRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        private async Task InitializeLabsAsync()
        {
            var labs = await _repository.GetAllAsync();
            if (labs == null || !labs.Any())
            {
                await _repository.CreateAndSaveLabs();  // Ensure the labs are initialized here

            };

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabsDTO>>> GetAll()
        {
            await InitializeLabsAsync();
            var labs = await _repository.GetAllAsync();

            // Log or inspect the raw Labs data to ensure LabId is present
            if (labs == null || !labs.Any())
            {
                return NotFound("No labs found.");
            }

            //var labsDto = _mapper.Map<List<LabsDTO>>(labs);
            return Ok(labs);
        }

        [HttpGet("{labId}")]
        public async Task<ActionResult<LabsDTO>> GetById(string labId)
        {
            var lab = await _repository.GetByIdAsync(labId);
            if (lab == null)
                return NotFound();

            var labDto = _mapper.Map<LabsDTO>(lab);
            return Ok(labDto);
        }

        [HttpPost]
        public async Task<ActionResult<LabsDTO>> Post([FromBody] LabsDTO labDto)
        {
            var lab = _mapper.Map<Labs>(labDto);
            lab.LabId = Guid.NewGuid().ToString();

            var createdLab = await _repository.AddAsync(lab);

            var createdLabDto = _mapper.Map<LabsDTO>(createdLab);
            return CreatedAtAction(nameof(GetById), new { labId = createdLab.LabId }, createdLabDto);
        }

        [HttpPut("{labId}")]
        public async Task<ActionResult<LabsDTO>> Put(string labId, [FromBody] LabsDTO labDto)
        {
            var existingLab = await _repository.GetByIdAsync(labId);
            if (existingLab == null)
                return NotFound();

            var lab = _mapper.Map<Labs>(labDto);
            lab.LabId = labId; // Ensure the LabId is not changed
            var updatedLab = await _repository.UpdateAsync(lab);

            var updatedLabDto = _mapper.Map<LabsDTO>(updatedLab);
            return Ok(updatedLabDto);
        }

        [HttpDelete("{labId}")]
        public async Task<IActionResult> Delete(string labId)
        {
            var success = await _repository.DeleteAsync(labId);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{labId}")]
        public async Task<IActionResult> UpdateLabWithPatch(string labId, [FromBody] JsonPatchDocument<LabsDTO> patchDocument)
        {
            // Step 1: Fetch the lab by ID
            var lab = await _repository.GetByIdAsync(labId);
            if (lab == null)
            {
                return NotFound($"Lab with ID {labId} not found.");
            }

            // Step 2: Map the current lab details to a DTO (Data Transfer Object)
            var labDetailsDTO = _mapper.Map<LabsDTO>(lab);

            // Step 3: Apply the patch document to the DTO
            patchDocument.ApplyTo(labDetailsDTO);

            // Step 4: Validate the patched DTO
            if (!TryValidateModel(labDetailsDTO))
            {
                return BadRequest(ModelState);
            }

            // Step 5: Apply the changes back to the lab object
            if (!string.IsNullOrEmpty(labDetailsDTO.LabName))
            {
                lab.LabName = labDetailsDTO.LabName;
            }
            if (!string.IsNullOrEmpty(labDetailsDTO.LabEmail))
            {
                lab.LabEmail = labDetailsDTO.LabEmail;
            }
            if (!string.IsNullOrEmpty(labDetailsDTO.LabAddress))
            {
                lab.LabAddress = labDetailsDTO.LabAddress;
            }
            // Password should not be updated via patch for security reasons, unless handled separately (e.g., a password change endpoint)
            if (!string.IsNullOrEmpty(labDetailsDTO.PasswordHash))
            {
                // Encrypt the new password before saving
                lab.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(labDetailsDTO.PasswordHash);
            }

            // Step 6: Save the updated lab
            await _repository.UpdateAsync(lab);

            // Step 7: Return the updated lab data (optionally map back to DTO for response)
            var updatedLabDto = _mapper.Map<LabsDTO>(lab);
            return Ok(updatedLabDto);
        }

    }
}
