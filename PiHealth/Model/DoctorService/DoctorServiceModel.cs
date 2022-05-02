using Abp.Application.Services.Dto;

namespace PiHealth.Web.Model.DoctorService
{
    public class DoctorServiceModel
    {
        public DoctorServiceModel()
        {
        }
        public long id { get; set; }
        public string serviceName { get; set; }
        public long fees { get; set; }
        public long userId { get; set; }
        public string doctorName { get; set; }
        public bool isDeleted { get; set; }
    }

    public class DoctorServiceInput : PagedAndSortedResultRequestDto
    {
        public long userId { get; set; }
        public string name { get; set;}
    }
}
