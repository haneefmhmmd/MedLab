using AutoMapper;
using medLab.Models;
using medLab.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using BCrypt.Net;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Generators;

namespace medLab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILabRepository _labRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(ILabRepository labRepository, IMapper mapper, ILogger<RegistrationController> logger)
        {
            _labRepository = labRepository;
            _mapper = mapper;
            _logger = logger;
        }



        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO registrationDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration request received.");
                return BadRequest(ModelState);
            }

            var existingLab = await _labRepository.GetByEmailAsync(registrationDto.LabEmail);
            if (existingLab != null)
            {
                _logger.LogWarning($"Email {registrationDto.LabEmail} is already registered.");
                return Conflict("Email is already registered.");
            }

            var lab = _mapper.Map<Labs>(registrationDto);
            lab.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registrationDto.PasswordHash);

            lab.LabId = Guid.NewGuid().ToString();

            try
            {
                // Save the lab to the repository
                var createdLab = await _labRepository.AddAsync(lab);

                _logger.LogInformation($"New lab registered: {createdLab.LabEmail}");

                // Map the created lab back to a DTO for response
                var responseDto = new LabsDTO
                {
                    LabId = createdLab.LabId,
                    LabEmail = createdLab.LabEmail,
                    PasswordHash = createdLab.PasswordHash,
                    LabAddress = createdLab.LabAddress,
                    LabName = createdLab.LabName
                };

                return CreatedAtAction(nameof(Register), new { labId = responseDto.LabId }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetLabByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                var lab = await _labRepository.GetByEmailAsync(email);
                if (lab == null)
                {
                    _logger.LogWarning($"Lab with email {email} not found.");
                    return NotFound($"Lab with email {email} not found.");
                }

                var labDto = _mapper.Map<LabsDTO>(lab);
                return Ok(labDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the lab.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

    }
}
