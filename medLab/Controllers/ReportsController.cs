using AutoMapper;
using medLab.Models;
using medLab.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace medLab.Controllers
{
    [ApiController]
    [Route("report")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ILabRepository _labRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ILabRepository labRepository, IMapper mapper, ILogger<ReportsController> logger)
        {
            _labRepository = labRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: /report/{labId}
        [HttpGet("{labId}")]
        public async Task<IActionResult> GetReportsForLab(string labId)
        {
            var lab = await _labRepository.GetByIdAsync(labId);
            if (lab == null)
            {
                _logger.LogWarning($"Lab with ID {labId} not found.");
                return NotFound($"Lab with ID {labId} not found.");
            }

            var reportDtos = _mapper.Map<List<ReportDTO>>(lab.Reports);
            return Ok(reportDtos);
        }

        // POST: /report/{labId}
        [HttpPost("{labId}")]
        public async Task<IActionResult> AddReportToLab(string labId, [FromBody] ReportDTO reportDto)
        {
            var lab = await _labRepository.GetByIdAsync(labId);
            if (lab == null)
            {
                _logger.LogWarning($"Lab with ID {labId} not found.");
                return NotFound($"Lab with ID {labId} not found.");
            }

            var report = _mapper.Map<Report>(reportDto);
            report.ReportId = Guid.NewGuid().ToString(); // Assign unique ReportId
            lab.Reports.Add(report);

            await _labRepository.UpdateAsync(lab);
            _logger.LogInformation($"Report added for Lab ID: {labId}, Report ID: {report.ReportId}.");

            var savedReportDto = _mapper.Map<ReportDTO>(report);
            return CreatedAtAction(nameof(GetReportsForLab), new { labId }, savedReportDto);
        }

        // PUT: /report/{labId}/{reportId}
        [HttpPut("{labId}/{reportId}")]
        public async Task<IActionResult> UpdateReportInLab(string labId, string reportId, [FromBody] ReportDTO reportDto)
        {
            var lab = await _labRepository.GetByIdAsync(labId);
            if (lab == null)
            {
                _logger.LogWarning($"Lab with ID {labId} not found.");
                return NotFound($"Lab with ID {labId} not found.");
            }

            var reportIndex = lab.Reports.FindIndex(r => r.ReportId == reportId);
            if (reportIndex == -1)
            {
                _logger.LogWarning($"Report with ID {reportId} not found in Lab ID {labId}.");
                return NotFound($"Report with ID {reportId} not found in Lab ID {labId}.");
            }

            lab.Reports[reportIndex] = _mapper.Map<Report>(reportDto);
            lab.Reports[reportIndex].ReportId = reportId; // Retain the original ReportId

            await _labRepository.UpdateAsync(lab);
            _logger.LogInformation($"Report updated for Lab ID: {labId}, Report ID: {reportId}.");

            var updatedReportDto = _mapper.Map<ReportDTO>(lab.Reports[reportIndex]);
            return Ok(updatedReportDto);
        }

        // DELETE: /report/{labId}/{reportId}
        [HttpDelete("{labId}/{reportId}")]
        public async Task<IActionResult> DeleteReportInLab(string labId, string reportId)
        {
            var lab = await _labRepository.GetByIdAsync(labId);
            if (lab == null)
            {
                _logger.LogWarning($"Lab with ID {labId} not found.");
                return NotFound($"Lab with ID {labId} not found.");
            }

            var report = lab.Reports.FirstOrDefault(r => r.ReportId == reportId);
            if (report == null)
            {
                _logger.LogWarning($"Report with ID {reportId} not found in Lab ID {labId}.");
                return NotFound($"Report with ID {reportId} not found in Lab ID {labId}.");
            }

            lab.Reports.Remove(report);
            await _labRepository.UpdateAsync(lab);
            _logger.LogInformation($"Report deleted for Lab ID: {labId}, Report ID: {reportId}.");

            return NoContent();
        }

        // DELETE: /report/{labId}
        [HttpDelete("{labId}")]
        public async Task<IActionResult> DeleteAllReportsInLab(string labId)
        {
            var lab = await _labRepository.GetByIdAsync(labId);
            if (lab == null)
            {
                _logger.LogWarning($"Lab with ID {labId} not found.");
                return NotFound($"Lab with ID {labId} not found.");
            }

            lab.Reports.Clear();
            await _labRepository.UpdateAsync(lab);
            _logger.LogInformation($"All reports deleted for Lab ID: {labId}.");

            return NoContent();
        }
    }
}
