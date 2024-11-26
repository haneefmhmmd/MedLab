namespace medLab.Models
{
    public class LabsDTO
    {
        public string LabId { get; set; }
        public string LabEmail { get; set; }
        public string PasswordHash {  get; set; }
        public string LabAddress { get; set; }
        public string LabName { get; set; }
        public List<ReportDTO> Reports { get; set; } = new List<ReportDTO>();
    }

    public class ReportDTO
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString(); // Unique identifier
        public int Age { get; set; }
        public string DateOfTest { get; set; }
        public string Gender { get; set; }
        public string PatientName { get; set; }
        public List<TestDTO> Tests { get; set; } = new List<TestDTO>();
    }

    public class TestDTO
    {
        public string TestName { get; set; }
        public string TestValue { get; set; }
    }

    public class RegistrationDTO
    {
        public string LabId { get; set; }
        public string LabEmail { get; set; }
        public string PasswordHash { get; set; }
        public string LabAddress { get; set; }
        public string LabName { get; set; }
    }
}
