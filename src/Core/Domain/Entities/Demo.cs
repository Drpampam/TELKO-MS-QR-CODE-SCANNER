using ConfigurationService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Demo : BaseEntity
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public bool NewDemo { get; set; } = false;
        public bool IsDeleted { get; set; }
    }
}
