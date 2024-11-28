using AutoMapper;
using medLab.Models;
using medLab.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;  // Import IConfiguration
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace medLab.Controllers
{
    [ApiController]
    [Route("api/")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILabRepository _labRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IConfiguration _configuration; // Add IConfiguration dependency

        // Inject IConfiguration
        public RegistrationController(ILabRepository labRepository, IMapper mapper, ILogger<RegistrationController> logger, IConfiguration configuration)
        {
            _labRepository = labRepository;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration; // Initialize IConfiguration
        }

        [HttpPost]
        [Route("Registration")]
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login request received.");
                return BadRequest(ModelState);
            }

            try
            {
                // Fetch the lab by email
                var lab = await _labRepository.GetByEmailAsync(loginDto.LabEmail);
                if (lab == null)
                {
                    _logger.LogWarning($"Login failed for {loginDto.LabEmail}: User not found.");
                    return Unauthorized("Invalid email or password.");
                }

                // Verify the password
                if (!BCrypt.Net.BCrypt.EnhancedVerify(loginDto.PasswordHash, lab.PasswordHash))
                {
                    _logger.LogWarning($"Login failed for {loginDto.LabEmail}: Invalid password.");
                    return Unauthorized("Invalid email or password.");
                }

                _logger.LogInformation($"Login successful for {loginDto.LabEmail}.");

                // Get the secret key from appsettings.json
                var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");

                // Generate JWT Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey); // Use the key from the appsettings.json file
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim("LabId", lab.LabId),
                        new System.Security.Claims.Claim("LabEmail", lab.LabEmail)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration.GetValue<string>("JwtSettings:Issuer"),
                    Audience = _configuration.GetValue<string>("JwtSettings:Audience"),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Token = tokenString,
                    LabId = lab.LabId,
                    LabEmail = lab.LabEmail,
                    LabName = lab.LabName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
