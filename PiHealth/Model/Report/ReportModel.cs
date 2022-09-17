using PiHealth.Web.Model;
using System;

namespace PiHealth.Model.Report
{
    public class ReportModel
    {
        public ReportModel()
        {

        }
    }

    public class ReportQueryModel : BaseQueryModel
    {
        public string name { get; set; }
        public long doctorId { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
    }
}
