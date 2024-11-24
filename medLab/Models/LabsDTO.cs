namespace medLab.Models
{

    // Lab DTO
    public class LabsDTO
    {
        public string LabId { get; set; }
        public string LabAddress { get; set; }
        public string LabName { get; set; }
        public ReportDTO Reports { get; set; } = new ReportDTO(); // Ensure it's initialized
       // public ReportDTO Reports { get; set; }
    }

    // Report DTO
    public class ReportDTO
    {
        public int Age { get; set; }
        public string DateOfTest { get; set; }
        public string Gender { get; set; }
        public string PatientName { get; set; }
        //public List<TestDTO> Tests { get; set; }
        public List<TestDTO> Tests { get; set; } = new List<TestDTO>(); // Ensure it's initialized
    }

    // Test DTO
    public class TestDTO
    {
        public string TestName { get; set; }
        public string TestValue { get; set; }
    }
}
