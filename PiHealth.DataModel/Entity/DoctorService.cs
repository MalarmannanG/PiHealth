using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class DoctorService : BaseEntity
    {
        public DoctorService()
        {
        }
        public string ServiceName { get; set; }
        public long Fees { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public AppUser AppUser { get; set; }
    }
}
