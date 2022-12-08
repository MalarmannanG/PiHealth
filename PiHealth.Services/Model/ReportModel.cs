using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.Services.Model
{
    public class Report
    {
        public string data { get; set; }
        public int count { get; set; }
        public string date { get; set; }
    }

    public class AgeReport
    {
        public int data { get; set; }
        public int count { get; set; }
    }

    public class DataFormat
    {
        public string name { get; set; }
        public List<int> data { get; set; }
    }
    public class ResponseData
    {
        public List<string> dates { get; set; }
        public List<DataFormat> dataItems { get; set; }
    }
    public class PieResponseData
    {
        public List<string> labels { get; set; }
        public List<int> dataItems { get; set; }
    }
    public class FeesDataModel
    {
        public string date { get; set; }
        public string patientName { get; set; }
        public string serviceName { get; set; }
        public long fees { get; set; }
    }

    public class ImpressionDataModel
    {
        public DateTime date { get; set; }
        public string patientName { get; set; }
        public string impressionName { get; set; }
        public int patientAge { get; set; }
    }

}
