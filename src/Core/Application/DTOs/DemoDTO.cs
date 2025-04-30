using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class DemoDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public bool NewDemo { get; set; } = false;

        public void ConvertToDTO (Demo demo)
        {
            if (demo == null)
                throw new NullReferenceException();

            Id = demo.Id;
            Name = demo.Name;
            Age = demo.Age;
            Address = demo.Address;
            Status = demo.Status;
            NewDemo = demo.NewDemo;
        }

        public void ConvertFromDTO(Demo demo)
        {
            if (demo == null)
            {
                throw new NullReferenceException("The Demo Object Cannot be null");
            }

            demo.Name = Name;
            demo.Age = Age;
            demo.Address = Address;
            demo.NewDemo = NewDemo;
        }
    }
}
