namespace medLab.Models.DTOs
{
    public class LabTestDTO
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string ReferenceValue { get; set; }
    }

    public class LabTestsDTO
    {
        public string LabId { get; set; }
        public List<LabTestDTO> Tests { get; set; } = new List<LabTestDTO>();
    }
}
