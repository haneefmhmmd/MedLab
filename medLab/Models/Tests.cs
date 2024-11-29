using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace medLab.Models
{
    [DynamoDBTable("Tests")]
    public class LabTests
    {
        [DynamoDBHashKey("labId")]
        public string LabId { get; set; }

        [DynamoDBProperty]
        public List<LabTest> Tests { get; set; } = new List<LabTest>();
    }

    public class LabTest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string ReferenceValue { get; set; }
    }
}
