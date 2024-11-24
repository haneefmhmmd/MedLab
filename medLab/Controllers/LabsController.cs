using AutoMapper;
using medLab.Models;
using medLab.Repositories;
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
    }
}
