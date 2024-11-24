using Amazon.DynamoDBv2.DataModel;
using static System.Net.Mime.MediaTypeNames;

namespace medLab.Models
{
    [DynamoDBTable("Labs")]
    public class Labs
    {
        [DynamoDBHashKey("labId")] 
        public string LabId { get; set; } = "0";

        [DynamoDBProperty("labAddress")]
        public string LabAddress { get; set; } = string.Empty;

        [DynamoDBProperty("labName")]
        public string LabName { get; set; } = string.Empty;

        [DynamoDBProperty("reports")]
        public Report Reports { get; set; } = new Report();
    }

    public class Report
    {
        [DynamoDBProperty("age")]
        public int Age { get; set; } = 0;

        [DynamoDBProperty("dateOfTest")]
        public string DateOfTest { get; set; } = string.Empty;

        [DynamoDBProperty("gender")]
        public string Gender { get; set; } = string.Empty;

        [DynamoDBProperty("patientName")]
        public string PatientName { get; set; } = string.Empty;

        [DynamoDBProperty("tests")]
        public List<Test> Tests { get; set; } = new List<Test>();
    }

    public class Test
    {
        [DynamoDBProperty("testName")]
        public string TestName { get; set; } = string.Empty;

        [DynamoDBProperty("testValue")]
        public string TestValue { get; set; } = string.Empty;
    }


}
