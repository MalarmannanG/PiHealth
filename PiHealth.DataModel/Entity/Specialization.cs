using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class Specialization : BaseEntity
    {
     
        public string Name { get; set; }
    }
}
