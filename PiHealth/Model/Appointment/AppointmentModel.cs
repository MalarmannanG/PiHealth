﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.Web.Model.Patient;
using PiHealth.Web.Model.VitalsReport;

namespace PiHealth.Web.Model.Appointment
{
    public class AppointmentModel
    {
        public AppointmentModel()
        {
            patientFiles = new List<PatientFilesModel>();
        }
        
        public long id { get; set; }
        public long? patientId { get; set; }
        public long? consultingDoctorID { get; set; }
        public string consultingDoctorName { get; set; }
        public string patientName { get; set; }
        public string description { get; set; }
        public string visitType { get; set; }
        public string dayOrNight { get; set; }
        public string timeOfAppintment { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public DateTime appointmentDateTime { get; set; }
        public string appointmentISOString  { get; set; }
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public long? updatedBy { get; set; }
        public string referredBy { get; set; }
        public VitalsReportModel vitalsReportModel { get; set; }
        public PatientModel patient { get; set; }
        public List<PatientFilesModel> patientFiles { get; set; }
    }


    public class AppointmentQueryModel : BaseQueryModel
    {
        public string patientName { get; set; }
        public bool? isProcedure { get; set; }
        public string doctorName { get; set; }
        public string clinicName { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public bool todayPatients { get; set; }
    }

    public class DashboardQueryModel
    {
        public long patientId { get; set; }
        public long doctorId { get; set; }
        public long clinicId { get; set; }
        public string clinicName { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public bool todayPatients { get; set; }
        public bool? isProcedure { get; set; }
    }

    public class PatientDashboardQueryModel : BaseQueryModel
    {
        public long patientId { get; set; }
        public string searchText { get; set; }
    }
}
