using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Entities.Contracts.DTOs.ZAInstitution.WebSite
{
    public class SliderImagDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public string CreatedBy { get; set; }
        public string Image { get; set; }
        public string InsertDate { get; set; }
    }
}
